using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using UT_WMSDotnet.Data;
using UT_WMSDotnet.Models;
using UT_WMSDotnet.ViewModels;
using WMS_UnitedTracors_Blazor.Helpers;

namespace WMS_UnitedTracors_Blazor.Services;

public class TransactionService
{
    private readonly IDbContextFactory<ApplicationDbContext> _factory;
    private readonly IWebHostEnvironment _env;
    private readonly IEmailService _emailService;

    public TransactionService(IDbContextFactory<ApplicationDbContext> factory, IWebHostEnvironment env, IEmailService emailService)
    {
        _factory = factory;
        _env = env;
        _emailService = emailService;
    }

    // Shared upload constraints for proof photos/PDFs (handover & return).
    public const long MaxUploadBytes = 4 * 1024 * 1024; // 4 MB
    private static readonly string[] AllowedUploadTypes =
    {
        "image/jpg", "image/jpeg", "image/png", "image/webp", "application/pdf"
    };

    private static string? ValidateUploadFile(Microsoft.AspNetCore.Components.Forms.IBrowserFile file)
    {
        if (!AllowedUploadTypes.Contains(file.ContentType))
            return "Format file tidak didukung. Gunakan JPG, PNG, WEBP, atau PDF.";
        if (file.Size > MaxUploadBytes)
            return $"Ukuran file terlalu besar ({file.Size / 1024.0 / 1024.0:0.#} MB). Maksimal 4 MB.";
        return null;
    }

    public static List<string> ParseStoredFiles(string? storedValue)
    {
        if (string.IsNullOrWhiteSpace(storedValue)) return new List<string>();

        var trimmed = storedValue.Trim();
        if (!trimmed.StartsWith("["))
        {
            return new List<string> { trimmed };
        }

        try
        {
            return JsonSerializer.Deserialize<List<string>>(trimmed)?
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .ToList() ?? new List<string>();
        }
        catch
        {
            return new List<string> { trimmed };
        }
    }

    public static bool HasStoredFiles(string? storedValue) => ParseStoredFiles(storedValue).Any();

    public async Task<string?> StoreTransactionAsync(TransactionRequestViewModel model, int currentUserId)
    {
        using var _context = _factory.CreateDbContext();
        var items = new List<TransactionItemViewModel>();
        if (model.items != null && model.items.Any())
        {
            items = model.items;
        }
        else if (!string.IsNullOrEmpty(model.sku) && model.quantity.HasValue)
        {
            items.Add(new TransactionItemViewModel
            {
                sku = model.sku,
                quantity = model.quantity.Value,
                request_type = model.request_type ?? "BORROW"
            });
        }
        else
        {
            return "Item transaksi harus diisi.";
        }

        bool hasBorrowItems = items.Any(i => (i.request_type ?? "BORROW") == "BORROW");
        bool hasGiveawayItems = items.Any(i => (i.request_type ?? "BORROW") == "GIVEAWAY");

        if (model.type == "OUT")
        {
            if (string.IsNullOrEmpty(model.event_name) || !model.event_date.HasValue ||
                string.IsNullOrEmpty(model.applicant_name) || !model.division_id.HasValue || model.division_id.Value <= 0)
            {
                return "Detail event, pemohon, dan divisi wajib diisi untuk transaksi OUT.";
            }

            if (model.event_date.Value.Date < WibHelper.Today)
            {
                return "Tanggal event tidak boleh sebelum hari ini.";
            }

            if (model.event_end_date.HasValue && model.event_end_date.Value.Date < model.event_date.Value.Date)
            {
                return "Tanggal akhir event tidak boleh sebelum tanggal mulai event.";
            }

            if (hasBorrowItems)
            {
                if (!model.borrow_start_date.HasValue || !model.borrow_end_date.HasValue)
                {
                    return "Tanggal pinjam dan tanggal kembali wajib diisi untuk item pinjaman.";
                }
                if (model.borrow_start_date.Value.Date < WibHelper.Today)
                {
                    return "Tanggal pinjam tidak boleh sebelum hari ini.";
                }
                if (model.borrow_end_date.Value.Date <= model.borrow_start_date.Value.Date)
                {
                    return "Tanggal kembali harus lebih besar dari tanggal pinjam.";
                }

                // Durasi peminjaman maksimal 10 hari
                var borrowDays = (model.borrow_end_date.Value.Date - model.borrow_start_date.Value.Date).Days;
                if (borrowDays > 10)
                {
                    return "Durasi peminjaman maksimal adalah 10 hari.";
                }

                // Tanggal event harus berada di dalam rentang tanggal peminjaman
                if (model.event_date.HasValue)
                {
                    if (model.borrow_start_date.Value.Date > model.event_date.Value.Date ||
                        (model.event_end_date.HasValue 
                            ? model.event_end_date.Value.Date > model.borrow_end_date.Value.Date 
                            : model.event_date.Value.Date > model.borrow_end_date.Value.Date))
                    {
                        return "Tanggal event (mulai dan selesai) harus berada di dalam rentang tanggal peminjaman.";
                    }
                }
            }

            if (hasGiveawayItems && model.giveaway_pickup_date.HasValue)
            {
                if (model.giveaway_pickup_date.Value.Date < WibHelper.Today)
                {
                    return "Tanggal pengambilan tidak boleh sebelum hari ini.";
                }
            }
        }

        foreach (var item in items)
        {
            if (string.IsNullOrEmpty(item.sku))
            {
                return "SKU item tidak boleh kosong.";
            }

            if (item.quantity <= 0)
            {
                return $"Kuantitas untuk SKU {item.sku} harus lebih besar dari 0.";
            }

            var reqType = item.request_type ?? "BORROW";
            if (reqType == "BORROW")
            {
                var borrowStart = item.borrow_start_date ?? model.borrow_start_date;
                var borrowEnd = item.expected_return_date ?? model.borrow_end_date;

                if (!borrowStart.HasValue || !borrowEnd.HasValue)
                {
                    return $"Tanggal pinjam dan tanggal kembali wajib diisi untuk SKU: {item.sku}.";
                }

                var duration = (int)(borrowEnd.Value.Date - borrowStart.Value.Date).TotalDays;
                if (duration <= 0) duration = 1;
                item.borrow_duration_days = duration;
                item.borrow_start_date = borrowStart;
                item.expected_return_date = borrowEnd;
            }
            else if (reqType == "GIVEAWAY")
            {
                var pickupDate = item.pickup_date ?? model.giveaway_pickup_date;
                item.pickup_date = pickupDate;
            }
        }

        var user = await _context.Users.FindAsync(currentUserId);
        if (user == null) return "Unauthorized.";

        var strategy = _context.Database.CreateExecutionStrategy();
        var result = await strategy.ExecuteAsync(async () =>
        {
            using var dbTransaction = await _context.Database.BeginTransactionAsync();
            try
            {
                if (model.type == "OUT")
                {
                    int totalPointsRequired = 0;
                    foreach (var item in items)
                    {
                        var product = await _context.Products.FirstOrDefaultAsync(p => p.sku == item.sku);
                        if (product == null) return $"Produk dengan SKU {item.sku} tidak ditemukan.";

                        var stockToCheck = product.current_stock;
                        if (item.product_variant_id.HasValue)
                        {
                            var variant = await _context.ProductVariants.FindAsync(item.product_variant_id.Value);
                            if (variant != null) stockToCheck = variant.stock;
                        }

                        if (stockToCheck < item.quantity)
                        {
                            return $"Stok tidak mencukupi untuk produk: {product.name}.";
                        }

                        // Hanya barang non-returnable (GIVEAWAY) yang memakai poin.
                        // Barang returnable (BORROW) dipinjam tanpa biaya poin.
                        var itemReqType = item.request_type ?? "BORROW";
                        if (itemReqType == "GIVEAWAY")
                        {
                            totalPointsRequired += (product.value * item.quantity);
                        }
                    }

                    if (totalPointsRequired > 0)
                    {
                        if (user.poin < totalPointsRequired)
                        {
                            return $"Poin Anda tidak mencukupi untuk request ini. Diperlukan {totalPointsRequired} poin, namun Anda hanya memiliki {user.poin} poin.";
                        }

                        user.poin -= totalPointsRequired;
                        _context.Users.Update(user);
                    }
                }

                var createdTransactions = new List<(Transaction t, Product p, int qty, int? variantId, int stockBefore)>();

                foreach (var item in items)
                {
                    var product = await _context.Products.FirstOrDefaultAsync(p => p.sku == item.sku);
                    if (product != null)
                    {
                        var reqType = item.request_type ?? "BORROW";

                        // Deduct stock immediately if type is OUT
                        int stockBefore = product.current_stock;
                        if (model.type == "OUT")
                        {
                            product.current_stock -= item.quantity;
                            _context.Products.Update(product);

                            if (item.product_variant_id.HasValue)
                            {
                                var variant = await _context.ProductVariants.FindAsync(item.product_variant_id.Value);
                                if (variant != null)
                                {
                                    variant.stock -= item.quantity;
                                    _context.ProductVariants.Update(variant);
                                }
                            }
                        }

                        var transaction = new Transaction
                        {
                            product_id = product.id,
                            product_variant_id = item.product_variant_id,
                            type = model.type,
                            request_type = model.type == "OUT" ? reqType : "BORROW",
                            quantity = item.quantity,
                            // Borrow & giveaway (OUT) sama-sama mulai dari tahap Staff Inventoris.
                            status = model.type == "OUT"
                                ? WorkflowStatuses.PendingStaffInventory
                                : WorkflowStatuses.Pending,
                            requester_id = currentUserId,
                            notes = model.notes,
                            applicant_name = model.applicant_name,
                            applicant_nrp = model.applicant_nrp,
                            event_name = model.event_name,
                            event_date = model.event_date,
                            event_end_date = model.event_end_date,
                            documentation_link = model.documentation_link,
                            borrow_duration_days = (reqType == "BORROW") ? (item.borrow_duration_days ?? 0) : 0,
                            borrow_start_date = item.borrow_start_date,
                            expected_return_date = item.expected_return_date,
                            pickup_date = item.pickup_date,
                            used_by = model.applicant_name,
                            division_id = model.division_id,
                            created_at = DateTime.UtcNow,
                            updated_at = DateTime.UtcNow
                        };

                        _context.Transactions.Add(transaction);
                        createdTransactions.Add((transaction, product, item.quantity, item.product_variant_id, stockBefore));
                    }
                }

                await _context.SaveChangesAsync();

                // Create stock logs for OUT transactions
                if (model.type == "OUT")
                {
                    foreach (var ct in createdTransactions)
                    {
                        var stockLog = new StockLog
                        {
                            transaction_id = ct.t.id,
                            product_id = ct.p.id,
                            stock_before = ct.stockBefore,
                            stock_after = ct.p.current_stock,
                            created_at = DateTime.UtcNow,
                            updated_at = DateTime.UtcNow
                        };
                        _context.StockLogs.Add(stockLog);
                    }
                    await _context.SaveChangesAsync();
                }

                await dbTransaction.CommitAsync();
                return null;
            }
            catch (Exception ex)
            {
                await dbTransaction.RollbackAsync();
                return "Checkout failed: " + ex.Message;
            }
        });

        if (result != null)
        {
            return result;
        }

        if (model.type == "OUT")
        {
            var requester = await _context.Users.FindAsync(currentUserId);
            if (requester != null && !string.IsNullOrWhiteSpace(requester.email))
            {
                string requesterMessage = "Pesanan Anda berhasil dibuat dan saat ini sedang menunggu persetujuan dari pihak terkait. Kami akan menginformasikan kembali jika status pesanan Anda telah diperbarui.";
                string requesterHtml = GetEmailTemplate("Request Berhasil Dibuat", requesterMessage);
                _ = _emailService.SendEmailAsync(requester.email, "Request Berhasil Dibuat", requesterHtml);
            }

            var adminManagerEmails = await _context.Users
                .Where(u => u.role == "Admin" || u.role == "Manager")
                .Select(u => u.email)
                .ToListAsync();

            if (adminManagerEmails.Any())
            {
                string adminMessage = $"Terdapat request peminjaman/pengambilan barang baru dari <strong>{model.applicant_name ?? requester?.name}</strong>. Silakan login ke sistem WMS untuk meninjau dan melakukan persetujuan.";
                string adminHtml = GetEmailTemplate("Request Baru Menunggu Persetujuan", adminMessage);
                _ = _emailService.SendEmailToMultipleAsync(adminManagerEmails, "Request Baru (WMS UT)", adminHtml);
            }
        }

        return null;
    }

    private string GetEmailTemplate(string title, string message)
    {
        return $@"
        <div style='font-family: ""Segoe UI"", Arial, sans-serif; background-color: #f4f4f5; padding: 40px 20px;'>
            <div style='max-width: 600px; margin: 0 auto; background-color: #ffffff; border-radius: 12px; overflow: hidden; box-shadow: 0 4px 15px rgba(0,0,0,0.05); border: 1px solid #e4e4e7;'>
                <div style='background-color: #fdc300; padding: 25px; text-align: center; border-bottom: 4px solid #e8a000;'>
                    <h1 style='color: #18181b; margin: 0; font-size: 24px; font-weight: 800; letter-spacing: 0.5px;'>UT WMS</h1>
                </div>
                <div style='padding: 35px 30px; color: #3f3f46; line-height: 1.6;'>
                    <h2 style='color: #18181b; margin-top: 0; font-size: 20px; font-weight: 600;'>{title}</h2>
                    <p style='font-size: 16px; margin-bottom: 0;'>{message}</p>
                </div>
                <div style='background-color: #fafafa; padding: 20px; text-align: center; font-size: 13px; color: #71717a; border-top: 1px solid #e4e4e7;'>
                    <p style='margin: 0;'>Pesan ini dikirim secara otomatis oleh Sistem Manajemen Inventaris United Tractors.</p>
                    <p style='margin: 5px 0 0 0;'>Mohon tidak membalas email ini.</p>
                </div>
            </div>
        </div>";
    }

    public async Task<(List<Transaction> Transactions, int TotalItems, int TotalPages)> GetHistoryAsync(
        int currentUserId,
        string? userRole,
        string? type,
        DateTime? start_date,
        DateTime? end_date,
        int page,
        int pageSize = 15,
        string? requestType = null,
        bool? isReturned = null)
    {
        using var _context = _factory.CreateDbContext();
        var query = _context.Transactions
            .Include(t => t.Product)
            .Include(t => t.ProductVariant)
            .Include(t => t.Requester)
            .Include(t => t.Approver)
            .Include(t => t.Division)
            .Where(t =>
                t.status == WorkflowStatuses.Rejected ||
                t.status == WorkflowStatuses.Approved ||
                t.status == WorkflowStatuses.Completed ||
                t.status == WorkflowStatuses.Revision ||
                t.status == WorkflowStatuses.RevisionByStaffInventory ||
                t.status == WorkflowStatuses.RevisionByAdmin ||
                t.status == WorkflowStatuses.RevisionByManager)
            .AsQueryable();

        // Pemohon biasa (tanpa izin approval/kelola) hanya melihat riwayatnya sendiri.
        var actor = await _context.Users.FindAsync(currentUserId);
        var actorRole = actor != null ? await _context.AdminRoles.FirstOrDefaultAsync(r => r.RoleName == actor.role && r.IsActive) : null;
        var perms = WMS_UnitedTracors_Blazor.Helpers.Permissions.Resolve(actor?.role, actorRole?.Permissions);
        bool isApproverOrAdmin =
            perms.Contains(WMS_UnitedTracors_Blazor.Helpers.Permissions.ApprovalStage1) ||
            perms.Contains(WMS_UnitedTracors_Blazor.Helpers.Permissions.ApprovalStage2) ||
            perms.Contains(WMS_UnitedTracors_Blazor.Helpers.Permissions.ApprovalManager) ||
            perms.Contains(WMS_UnitedTracors_Blazor.Helpers.Permissions.ApprovalHandover) ||
            perms.Contains(WMS_UnitedTracors_Blazor.Helpers.Permissions.ProductsManage);

        if (!isApproverOrAdmin)
        {
            query = query.Where(t => t.requester_id == currentUserId);
        }

        if (string.Equals(userRole, "staff", StringComparison.OrdinalIgnoreCase) || string.Equals(userRole, "user", StringComparison.OrdinalIgnoreCase))
        {
            query = query.Where(t => t.type == "OUT");
        }
        else if (!string.IsNullOrEmpty(type))
        {
            query = query.Where(t => t.type == type);
        }

        if (start_date.HasValue)
        {
            query = query.Where(t => t.created_at >= start_date.Value);
        }

        if (end_date.HasValue)
        {
            var endDateInclusive = end_date.Value.AddDays(1);
            query = query.Where(t => t.created_at < endDateInclusive);
        }

        if (!string.IsNullOrEmpty(requestType))
        {
            query = query.Where(t => t.request_type == requestType);
        }

        if (isReturned.HasValue)
        {
            if (isReturned.Value)
                query = query.Where(t => t.request_type == "BORROW" && t.returned_quantity >= t.quantity);
            else
                query = query.Where(t => t.request_type == "BORROW" && (t.returned_quantity == null || t.returned_quantity < t.quantity));
        }

        int totalItems = await query.CountAsync();
        int totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

        var transactions = await query
            .OrderByDescending(t => t.created_at)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (transactions, totalItems, totalPages);
    }

    public async Task<List<Transaction>> GetProductRequestHistoryAsync(int productId)
    {
        using var _context = _factory.CreateDbContext();
        return await _context.Transactions
            .Include(t => t.Requester)
            .Include(t => t.Division)
            .Where(t => t.product_id == productId && t.type == "OUT")
            .OrderByDescending(t => t.created_at)
            .ToListAsync();
    }

    public async Task<(List<Transaction> Items, int TotalItems, int TotalPages)> GetProductRequestHistoryPagedAsync(
        int productId, int page = 1, int pageSize = 8)
    {
        using var _context = _factory.CreateDbContext();
        var query = _context.Transactions
            .Include(t => t.Requester)
            .Include(t => t.Division)
            .Where(t => t.product_id == productId && t.type == "OUT")
            .OrderByDescending(t => t.created_at);

        int totalItems = await query.CountAsync();
        int totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);
        var items = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
        return (items, totalItems, totalPages);
    }

    public async Task<string?> ReturnItemAsync(ReturnItemViewModel model, bool asDraft = false)
    {
        using var _context = _factory.CreateDbContext();
        var transaction = await _context.Transactions
            .Include(t => t.Product)
            .FirstOrDefaultAsync(t => t.id == model.transaction_id);

        if (transaction == null) return "Transaction not found.";

        if (model.return_status == "rusak" || model.return_status == "hilang")
        {
            if (string.IsNullOrWhiteSpace(model.return_reason))
                return "Alasan kerusakan atau kehilangan wajib diisi.";
        }
        else
        {
            model.return_reason = null;
        }

        if (transaction.type != "OUT" || transaction.status != "APPROVED")
            return "This transaction is not eligible for return.";

        if (transaction.request_type == "GIVEAWAY")
            return "This item was given away and cannot be returned.";

        // Boleh menimpa draft sendiri (is_return_draft == 1) untuk re-upload,
        // tapi blokir jika sudah disubmit & menunggu persetujuan admin.
        if ((transaction.pending_return_quantity ?? 0) > 0 && transaction.is_return_draft == 0)
            return "Pengembalian sudah diajukan dan menunggu persetujuan admin.";

        var remainingToReturn = transaction.quantity - transaction.returned_quantity;
        if (model.return_quantity > remainingToReturn)
            return $"You can only return up to {remainingToReturn} items.";

        string photoPath = transaction.return_photo ?? "";
        if (model.return_photo != null)
        {
            var uploads = Path.Combine(_env.WebRootPath, "storage", "returns");
            if (!Directory.Exists(uploads)) Directory.CreateDirectory(uploads);
            var fileName = Guid.NewGuid().ToString() + Path.GetExtension(model.return_photo.FileName);
            var filePath = Path.Combine(uploads, fileName);
            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await model.return_photo.CopyToAsync(fileStream);
            }
            photoPath = "storage/returns/" + fileName;
        }
        else if (model.return_photo_browser != null)
        {
            var uploadError = ValidateUploadFile(model.return_photo_browser);
            if (uploadError != null) return uploadError;

            var uploads = Path.Combine(_env.WebRootPath, "storage", "returns");
            if (!Directory.Exists(uploads)) Directory.CreateDirectory(uploads);
            var fileName = Guid.NewGuid().ToString() + Path.GetExtension(model.return_photo_browser.Name);
            var filePath = Path.Combine(uploads, fileName);
            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await model.return_photo_browser.OpenReadStream(MaxUploadBytes).CopyToAsync(fileStream);
            }
            photoPath = "storage/returns/" + fileName;
        }
        else if (string.IsNullOrEmpty(photoPath))
        {
            // If dummy photo is set by force return, allow it
            if (photoPath != "forced-by-admin")
            {
                return "Foto pengembalian wajib diunggah.";
            }
        }

        // Pengembalian tidak langsung selesai: dibuat sebagai pending dan menunggu
        // persetujuan admin (ApproveReturnAsync) sebelum stok ditambahkan kembali.
        transaction.return_photo = photoPath;
        transaction.return_status = model.return_status;
        transaction.return_reason = model.return_reason;
        transaction.pending_return_quantity = model.return_quantity;
        // asDraft=true  -> draft (bisa dibatalkan / diajukan batch dari halaman Returns)
        // asDraft=false -> langsung diajukan, menunggu ACC admin (alur Tracking)
        transaction.is_return_draft = asDraft ? 1 : 0;
        transaction.updated_at = DateTime.UtcNow;

        _context.Transactions.Update(transaction);
        await _context.SaveChangesAsync();
        return null;
    }

    public async Task<string?> ForceReturnTransactionAsync(int transactionId, int returnQty, string status, string? reason, int adminId)
    {
        using var _context = _factory.CreateDbContext();
        // Force return adalah override admin: langsung selesai tanpa approval & tanpa foto.
        var transaction = await _context.Transactions.Include(t => t.Product).FirstOrDefaultAsync(t => t.id == transactionId);
        if (transaction == null) return "Transaction not found.";
        if (transaction.Product == null) return "Associated product not found.";

        if (transaction.type != "OUT" || transaction.status != "APPROVED")
            return "This transaction is not eligible for return.";
        if (transaction.request_type == "GIVEAWAY")
            return "This item was given away and cannot be returned.";

        if (status == "rusak" || status == "hilang")
        {
            if (string.IsNullOrWhiteSpace(reason))
                return "Alasan kerusakan atau kehilangan wajib diisi.";
        }
        else
        {
            reason = null;
        }

        var remainingToReturn = (transaction.quantity ?? 0) - (transaction.returned_quantity ?? 0) - (transaction.pending_return_quantity ?? 0);
        if (returnQty > remainingToReturn)
            return $"You can only return up to {remainingToReturn} items.";
        if (returnQty <= 0)
            return "Jumlah pengembalian tidak valid.";

        transaction.return_photo = "forced-by-admin";
        transaction.return_status = status;
        transaction.return_reason = reason;

        var strategy = _context.Database.CreateExecutionStrategy();
        return await strategy.ExecuteAsync(async () =>
        {
            using var dbTransaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var stockBefore = transaction.Product!.current_stock;
                transaction.Product.current_stock += returnQty;
                _context.Products.Update(transaction.Product);

                if (transaction.product_variant_id.HasValue)
                {
                    var variant = await _context.ProductVariants.FindAsync(transaction.product_variant_id.Value);
                    if (variant != null)
                    {
                        variant.stock += returnQty;
                        _context.ProductVariants.Update(variant);
                    }
                }

                _context.StockLogs.Add(new StockLog
                {
                    transaction_id = transaction.id,
                    product_id = transaction.Product.id,
                    stock_before = stockBefore,
                    stock_after = transaction.Product.current_stock,
                    created_at = DateTime.UtcNow,
                    updated_at = DateTime.UtcNow
                });

                transaction.returned_quantity = (transaction.returned_quantity ?? 0) + returnQty;
                transaction.approver_id = adminId;
                if (transaction.returned_quantity >= transaction.quantity)
                {
                    transaction.returned_at = DateTime.UtcNow;
                }
                transaction.updated_at = DateTime.UtcNow;
                _context.Transactions.Update(transaction);

                await _context.SaveChangesAsync();
                await dbTransaction.CommitAsync();
                return null;
            }
            catch (Exception ex)
            {
                await dbTransaction.RollbackAsync();
                return "Return failed: " + ex.Message;
            }
        });
    }

    public async Task<string?> ConfirmHandoverAsync(
        List<int> transactionIds,
        Microsoft.AspNetCore.Components.Forms.IBrowserFile? photo,
        string? notes,
        string? uploadedBy = null,
        string? recipientName = null,
        DateTime? handoverTimestamp = null)
    {
        var photos = photo == null
            ? new List<Microsoft.AspNetCore.Components.Forms.IBrowserFile>()
            : new List<Microsoft.AspNetCore.Components.Forms.IBrowserFile> { photo };
        return await ConfirmHandoverAsync(transactionIds, photos, notes, uploadedBy, recipientName, handoverTimestamp);
    }

    public async Task<string?> ConfirmHandoverAsync(
        List<int> transactionIds,
        IReadOnlyList<Microsoft.AspNetCore.Components.Forms.IBrowserFile> photos,
        string? notes,
        string? uploadedBy = null,
        string? recipientName = null,
        DateTime? handoverTimestamp = null,
        List<string>? existingPhotos = null)
    {
        using var _context = _factory.CreateDbContext();
        if (transactionIds == null || !transactionIds.Any())
            return "Tidak ada item yang menunggu serah terima.";

        var transactions = await _context.Transactions
            .Where(t => transactionIds.Contains(t.id))
            .ToListAsync();

        // Hanya item yang masih menunggu bukti (draft) yang bisa diunggah/diganti.
        var pending = transactions
            .Where(t =>
                t.status == WorkflowStatuses.WaitingHandover &&
                (t.request_type == "BORROW" || t.request_type == "GIVEAWAY"))
            .ToList();

        if (!pending.Any())
            return "Item ini tidak sedang menunggu bukti serah terima.";

        if ((photos == null || photos.Count == 0) && (existingPhotos == null || existingPhotos.Count == 0))
            return "Minimal satu foto bukti serah terima wajib diunggah.";

        if ((photos?.Count ?? 0) + (existingPhotos?.Count ?? 0) > 5)
            return "Maksimal 5 file bukti serah terima per upload.";

        if (photos != null && photos.Count > 0)
        {
            foreach (var photo in photos)
            {
                var uploadError = ValidateUploadFile(photo);
                if (uploadError != null) return uploadError;
            }
        }

        List<string> photoPaths = existingPhotos != null ? new List<string>(existingPhotos) : new List<string>();
        try
        {
            var uploads = Path.Combine(_env.WebRootPath, "storage", "handovers");
            if (!Directory.Exists(uploads)) Directory.CreateDirectory(uploads);
            if (photos != null && photos.Count > 0)
            {
                foreach (var photo in photos)
                {
                    var fileName = Guid.NewGuid().ToString() + Path.GetExtension(photo.Name);
                    var filePath = Path.Combine(uploads, fileName);
                    using var fileStream = new FileStream(filePath, FileMode.Create);
                    await photo.OpenReadStream(MaxUploadBytes).CopyToAsync(fileStream);
                    photoPaths.Add("storage/handovers/" + fileName);
                }
            }
        }
        catch (Exception ex)
        {
            return "Gagal mengunggah foto: " + ex.Message;
        }

        foreach (var item in pending)
        {
            item.handover_photo = JsonSerializer.Serialize(photoPaths);
            item.handover_notes = notes;
            item.handover_recipient_name = string.IsNullOrWhiteSpace(recipientName) ? (item.applicant_name ?? item.Requester?.name) : recipientName;
            item.handover_timestamp = handoverTimestamp ?? DateTime.Now;
            item.handover_uploaded_by = uploadedBy;
            item.rejection_reason = null; // bersihkan alasan penolakan sebelumnya (jika re-upload)
            item.updated_at = DateTime.UtcNow;
            _context.Transactions.Update(item);
        }

        await _context.SaveChangesAsync();
        return null;
    }

    public async Task<string?> SubmitHandoverAsync(List<int> transactionIds)
    {
        using var _context = _factory.CreateDbContext();
        if (transactionIds == null || !transactionIds.Any())
            return "Tidak ada item serah terima.";

        var pending = await _context.Transactions
            .Include(t => t.Product)
            .Where(t => transactionIds.Contains(t.id) && t.status == WorkflowStatuses.WaitingHandover)
            .ToListAsync();

        if (!pending.Any())
            return "Tidak ada draft serah terima yang bisa disubmit.";

        if (pending.Any(t => !HasStoredFiles(t.handover_photo)))
            return "Unggah bukti serah terima terlebih dahulu sebelum submit.";

        var strategy = _context.Database.CreateExecutionStrategy();
        return await strategy.ExecuteAsync(async () =>
        {
            using var dbTransaction = await _context.Database.BeginTransactionAsync();
            try
            {
                foreach (var item in pending)
                {
                    if (item.request_type == "BORROW")
                    {
                        item.status = WorkflowStatuses.WaitingHandoverConfirm;
                    }
                    else if (item.request_type == "GIVEAWAY")
                    {
                        item.status = WorkflowStatuses.WaitingDocumentation;
                    }

                    item.updated_at = DateTime.UtcNow;
                    _context.Transactions.Update(item);
                }

                await _context.SaveChangesAsync();
                await dbTransaction.CommitAsync();
                return (string?)null;
            }
            catch (Exception ex)
            {
                await dbTransaction.RollbackAsync();
                return "Gagal submit serah terima: " + ex.Message;
            }
        });
    }

    public async Task<string?> ConfirmHandoverReceiptByUserAsync(string groupId, int currentUserId, bool isApproved, string? rejectionReason)
    {
        using var _context = _factory.CreateDbContext();
        if (!isApproved && string.IsNullOrWhiteSpace(rejectionReason)) return "Alasan penolakan wajib diisi.";

        var query = await _context.Transactions
            .Where(t => t.status == WorkflowStatuses.WaitingHandoverConfirm &&
                        (t.handover_uploaded_by == "SI" || t.handover_uploaded_by == null) &&
                        t.type == "OUT" && t.request_type == "BORROW")
            .ToListAsync();

        var matched = query.Where(t => t.group_id == groupId).ToList();

        if (matched.Count == 0) return "Tidak ada transaksi serah terima yang menunggu konfirmasi Anda pada event ini.";

        if (matched.Any(t => t.requester_id != currentUserId))
            return "Unauthorized action.";

        foreach (var item in matched)
        {
            if (isApproved)
            {
                item.status = WorkflowStatuses.Approved;
                item.rejection_reason = null;
            }
            else
            {
                item.status = WorkflowStatuses.WaitingHandover;
                item.rejection_reason = rejectionReason;
            }
            item.updated_at = DateTime.UtcNow;
            _context.Transactions.Update(item);
        }

        await _context.SaveChangesAsync();
        return null;
    }

    public async Task CheckAndAutoConfirmHandoversAsync()
    {
        using var _context = _factory.CreateDbContext();
        var cutoff = WibHelper.Now.AddHours(-24);
        var expiredHandovers = await _context.Transactions
            .Where(t =>
                t.status == WorkflowStatuses.WaitingHandoverConfirm &&
                t.request_type == "BORROW" &&
                t.updated_at <= cutoff)
            .ToListAsync();

        if (expiredHandovers.Any())
        {
            foreach (var item in expiredHandovers)
            {
                item.status = WorkflowStatuses.Approved;
                item.updated_at = DateTime.UtcNow;
                _context.Transactions.Update(item);
            }
            await _context.SaveChangesAsync();
        }
    }

    public async Task<string?> RejectDisputedHandoverReportAsync(int transactionId, string reason, int currentUserId)
    {
        using var _context = _factory.CreateDbContext();
        var transaction = await _context.Transactions
            .FirstOrDefaultAsync(t => t.id == transactionId);
            
        if (transaction == null) return "Transaksi tidak ditemukan.";
        
        transaction.status = WorkflowStatuses.WaitingHandoverConfirm;
        transaction.rejection_reason = null; // Clear the dispute reason
        transaction.updated_at = DateTime.UtcNow; // Reset the 24-hour auto-confirm timer!
        
        _context.Transactions.Update(transaction);
        await _context.SaveChangesAsync();
        return null;
    }

    public async Task<string?> SubmitGiveawayDocumentationAsync(
        List<int> transactionIds,
        Microsoft.AspNetCore.Components.Forms.IBrowserFile? photo,
        string? notes)
        => await SubmitGiveawayDocumentationAsync(
            transactionIds,
            photo == null ? Array.Empty<Microsoft.AspNetCore.Components.Forms.IBrowserFile>() : new[] { photo },
            notes);

    public async Task<string?> SubmitGiveawayDocumentationAsync(
        List<int> transactionIds,
        IReadOnlyList<Microsoft.AspNetCore.Components.Forms.IBrowserFile> photos,
        string? notes)
    {
        using var _context = _factory.CreateDbContext();
        if (transactionIds == null || !transactionIds.Any())
            return "Tidak ada item dokumentasi.";

        var transactions = await _context.Transactions
            .Where(t => transactionIds.Contains(t.id) && t.request_type == "GIVEAWAY" &&
                        (t.status == WorkflowStatuses.WaitingDocumentation || t.status == WorkflowStatuses.DocumentationOverdue))
            .ToListAsync();

        if (!transactions.Any())
            return "Tidak ada giveaway yang menunggu dokumentasi.";

        if (photos == null || photos.Count == 0)
            return "Foto dokumentasi wajib diunggah.";

        if (photos.Count > 5)
            return "Maksimal 5 file dokumentasi.";

        var photoPaths = new List<string>();
        try
        {
            var uploads = Path.Combine(_env.WebRootPath, "storage", "documentation");
            if (!Directory.Exists(uploads)) Directory.CreateDirectory(uploads);

            foreach (var photo in photos)
            {
                var uploadError = ValidateUploadFile(photo);
                if (uploadError != null) return uploadError;

                var fileName = Guid.NewGuid() + Path.GetExtension(photo.Name);
                var filePath = Path.Combine(uploads, fileName);
                using var fileStream = new FileStream(filePath, FileMode.Create);
                await photo.OpenReadStream(MaxUploadBytes).CopyToAsync(fileStream);
                photoPaths.Add("storage/documentation/" + fileName);
            }
        }
        catch (Exception ex)
        {
            return "Gagal mengunggah dokumentasi: " + ex.Message;
        }

        foreach (var item in transactions)
        {
            item.documentation_photo = JsonSerializer.Serialize(photoPaths);
            item.documentation_notes = notes;
            item.documentation_uploaded_at = DateTime.UtcNow;
            item.status = WorkflowStatuses.Completed; // Langsung COMPLETED tanpa perlu verifikasi admin lagi
            item.updated_at = DateTime.UtcNow;
            _context.Transactions.Update(item);
        }

        await _context.SaveChangesAsync();
        return null;
    }

    /// <summary>Submit satu draft pengembalian ke admin: is_return_draft 1 -> 0 (menunggu ACC).</summary>
    public async Task<string?> SubmitReturnDraftAsync(int id)
    {
        using var _context = _factory.CreateDbContext();
        var transaction = await _context.Transactions.FindAsync(id);
        if (transaction == null) return "Transaction not found.";
        if (transaction.is_return_draft != 1 || (transaction.pending_return_quantity ?? 0) <= 0)
            return "Item ini bukan draft pengembalian.";

        if (string.IsNullOrEmpty(transaction.return_photo) || transaction.return_photo == "forced-by-admin")
            return "Unggah bukti pengembalian terlebih dahulu sebelum submit.";

        transaction.is_return_draft = 0; // diajukan, menunggu persetujuan admin
        transaction.updated_at = DateTime.UtcNow;
        _context.Transactions.Update(transaction);
        await _context.SaveChangesAsync();
        return null;
    }

    public async Task<string?> CancelReturnDraftAsync(int id)
    {
        using var _context = _factory.CreateDbContext();
        var transaction = await _context.Transactions.FindAsync(id);
        if (transaction != null && transaction.is_return_draft == 1)
        {
            transaction.pending_return_quantity = 0;
            transaction.return_photo = null;
            transaction.return_status = null;
            transaction.return_reason = null;
            transaction.is_return_draft = 0;
            transaction.updated_at = DateTime.UtcNow;

            _context.Update(transaction);
            await _context.SaveChangesAsync();
            return null;
        }
        return "Item is not in draft state.";
    }

    public async Task<string?> SubmitReturnBatchAsync(string groupId)
    {
        using var _context = _factory.CreateDbContext();
        var transactions = await _context.Transactions
            .Where(t => t.status == WorkflowStatuses.Approved && t.type == "OUT" && t.request_type == "BORROW" &&
                        t.pending_return_quantity > 0 && t.is_return_draft == 1)
            .ToListAsync();

        var grouped = transactions.GroupBy(t => t.group_id);
        var group = grouped.FirstOrDefault(g => g.Key == groupId);

        if (group == null)
            return "No draft returns found for this event.";

        foreach (var item in group)
        {
            item.is_return_draft = 0;
            item.updated_at = DateTime.UtcNow;
            _context.Update(item);
        }

        await _context.SaveChangesAsync();
        return null;
    }

    public async Task<string?> UpdateRevisionAsync(int id, TransactionRequestViewModel model)
    {
        using var _context = _factory.CreateDbContext();
        var transaction = await _context.Transactions.Include(t => t.Product).Include(t => t.Requester).FirstOrDefaultAsync(t => t.id == id);
        if (transaction == null || !WorkflowStatuses.IsRevision(transaction.status)) return "Request is not in revision state.";

        int newQty = model.quantity ?? transaction.quantity ?? 0;
        string newRequestType = model.request_type ?? transaction.request_type ?? "BORROW";
        var product = transaction.Product;

        if (transaction.type == "OUT" && product != null)
        {
            var stockToCheck = product.current_stock;
            if (transaction.product_variant_id.HasValue)
            {
                var variant = await _context.ProductVariants.FindAsync(transaction.product_variant_id.Value);
                if (variant != null) stockToCheck = variant.stock;
            }

            if (stockToCheck < newQty) return "Insufficient stock for this product.";

            // Deduct the new revised quantity from inventory
            int stockBefore = product.current_stock;
            product.current_stock -= newQty;
            _context.Products.Update(product);

            if (transaction.product_variant_id.HasValue)
            {
                var variant = await _context.ProductVariants.FindAsync(transaction.product_variant_id.Value);
                if (variant != null)
                {
                    variant.stock -= newQty;
                    _context.ProductVariants.Update(variant);
                }
            }

            _context.StockLogs.Add(new StockLog
            {
                transaction_id = transaction.id,
                product_id = product.id,
                stock_before = stockBefore,
                stock_after = product.current_stock,
                created_at = DateTime.UtcNow,
                updated_at = DateTime.UtcNow
            });
        }

        var requester = transaction.Requester;
        if (requester == null) return "User not found.";

        int oldPoints = transaction.request_type == "GIVEAWAY" ? (product?.value ?? 0) * (transaction.quantity ?? 0) : 0;
        int newPoints = newRequestType == "GIVEAWAY" ? (product?.value ?? 0) * newQty : 0;
        int pointsDifference = newPoints - oldPoints;

        if (pointsDifference > 0)
        {
            if (requester.poin < pointsDifference) return $"Insufficient credit points. You need {pointsDifference} more points.";
            requester.poin -= pointsDifference;
        }
        else if (pointsDifference < 0)
        {
            requester.poin += Math.Abs(pointsDifference);
        }

        _context.Users.Update(requester);

        transaction.quantity = newQty;
        transaction.request_type = newRequestType;
        transaction.status = GetResubmittedStatus(transaction);
        transaction.updated_at = DateTime.UtcNow;
        transaction.rejection_reason = null;
        transaction.last_revision_stage = null;

        if (newRequestType == "BORROW")
        {
            transaction.borrow_start_date = model.items?.FirstOrDefault()?.borrow_start_date ?? transaction.borrow_start_date;
            transaction.expected_return_date = model.items?.FirstOrDefault()?.expected_return_date ?? transaction.expected_return_date;

            if (transaction.borrow_start_date.HasValue && transaction.expected_return_date.HasValue)
            {
                int duration = (int)(transaction.expected_return_date.Value.Date - transaction.borrow_start_date.Value.Date).TotalDays;
                transaction.borrow_duration_days = duration > 0 ? duration : 1;
            }
        }

        _context.Transactions.Update(transaction);

        // Update shared fields for all items in the same group to prevent splitting
        string groupId = transaction.group_id;
        var groupItems = await _context.Transactions
            .Where(t => t.created_at == transaction.created_at && t.requester_id == transaction.requester_id && t.id != transaction.id)
            .ToListAsync();

        foreach (var item in groupItems.Where(t => t.group_id == groupId))
        {
            item.applicant_name = model.applicant_name;
            item.applicant_nrp = model.applicant_nrp;
            item.event_name = model.event_name;
            item.event_date = model.event_date;
            item.division_id = model.division_id ?? item.division_id;
            item.documentation_link = model.documentation_link;
            // Notes are NOT updated here to preserve original item notes
            _context.Transactions.Update(item);
        }

        // Apply shared fields to the main transaction too
        transaction.applicant_name = model.applicant_name;
        transaction.applicant_nrp = model.applicant_nrp;
        transaction.event_name = model.event_name;
        transaction.event_date = model.event_date;
        transaction.division_id = model.division_id ?? transaction.division_id;
        transaction.documentation_link = model.documentation_link;
        transaction.notes = model.notes;

        _context.Transactions.Update(transaction);
        await _context.SaveChangesAsync();
        return null;
    }

    public async Task<string?> CancelRevisionAsync(int id)
    {
        using var _context = _factory.CreateDbContext();
        var transaction = await _context.Transactions.Include(t => t.Product).Include(t => t.Requester).FirstOrDefaultAsync(t => t.id == id);
        if (transaction == null || !WorkflowStatuses.IsRevision(transaction.status)) return "Request is not in revision state.";

        var product = transaction.Product;
        var requester = transaction.Requester;

        if (transaction.request_type == "GIVEAWAY" && requester != null)
        {
            int pointsToRefund = (product?.value ?? 0) * (transaction.quantity ?? 0);
            if (pointsToRefund > 0)
            {
                requester.poin += pointsToRefund;
                _context.Users.Update(requester);
            }
        }

        transaction.status = WorkflowStatuses.Rejected;
        transaction.rejection_reason = "Cancelled by requester";
        transaction.updated_at = DateTime.UtcNow;

        _context.Transactions.Update(transaction);
        await _context.SaveChangesAsync();
        return null;
    }

    public async Task<int> MarkGiveawayDocumentationOverdueAsync()
    {
        using var _context = _factory.CreateDbContext();
        var now = WibHelper.Today;
        var transactions = await _context.Transactions
            .Where(t =>
                t.request_type == "GIVEAWAY" &&
                t.status == WorkflowStatuses.WaitingDocumentation &&
                t.event_date.HasValue &&
                t.event_date.Value.Date.AddDays(3) < now)
            .ToListAsync();

        if (!transactions.Any()) return 0;

        foreach (var item in transactions)
        {
            item.status = WorkflowStatuses.DocumentationOverdue;
            item.updated_at = DateTime.UtcNow;
        }

        await _context.SaveChangesAsync();
        return transactions.Count;
    }

    private static string GetResubmittedStatus(Transaction transaction)
    {
        if (transaction.request_type == "GIVEAWAY")
        {
            return transaction.last_revision_stage switch
            {
                "STAFF_INVENTORY" => WorkflowStatuses.PendingStaffInventory,
                "ADMIN" => WorkflowStatuses.PendingAdmin,
                "MANAGER" => WorkflowStatuses.PendingManager,
                _ => WorkflowStatuses.PendingStaffInventory
            };
        }

        return WorkflowStatuses.Pending;
    }

    public async Task<string?> DeleteTransactionAsync(int id)
    {
        using var _context = _factory.CreateDbContext();
        var transaction = await _context.Transactions.Include(t => t.Product).Include(t => t.Requester).FirstOrDefaultAsync(t => t.id == id);
        if (transaction == null) return "Transaction not found.";

        var product = transaction.Product;
        var requester = transaction.Requester;

        if (transaction.request_type == "GIVEAWAY" && requester != null)
        {
            int pointsToRefund = (product?.value ?? 0) * (transaction.quantity ?? 0);
            if (pointsToRefund > 0)
            {
                requester.poin += pointsToRefund;
                _context.Users.Update(requester);
            }
        }

        _context.Transactions.Remove(transaction);
        await _context.SaveChangesAsync();
        return null;
    }

    public async Task<List<Transaction>> GetTransactionsByGroupIdAsync(string groupId)
    {
        using var _context = _factory.CreateDbContext();
        var allTransactions = await _context.Transactions
            .Include(t => t.Product)
            .ThenInclude(p => p!.Unit)
            .Include(t => t.Requester)
            .Include(t => t.Approver)
            .Include(t => t.Division)
            .ToListAsync();

        return allTransactions.Where(t => t.group_id == groupId).ToList();
    }

    public async Task<string?> SubmitHandoverBatchAsync(string groupId, string? photoPath, string? notes)
    {
        using var _context = _factory.CreateDbContext();
        var allTransactions = await _context.Transactions
            .Include(t => t.Product)
            .ThenInclude(p => p!.Unit)
            .Include(t => t.Requester)
            .Include(t => t.Approver)
            .Include(t => t.Division)
            .ToListAsync();
        var transactions = allTransactions.Where(t => t.group_id == groupId).ToList();
        var matched = transactions.Where(t => t.status == "WAITING_HANDOVER" && t.type == "OUT" && (t.request_type == "GIVEAWAY" || t.request_type == "BORROW")).ToList();

        if (matched.Count == 0) return "Tidak ada transaksi serah terima yang menunggu bukti pada event ini.";

        foreach (var item in matched)
        {
            item.handover_photo = photoPath;
            item.handover_notes = notes;
            item.status = "WAITING_ADMIN_HANDOVER";
            _context.Transactions.Update(item);
        }

        await _context.SaveChangesAsync();
        return null;
    }

    public async Task ProcessScannerTransactionAsync(UT_WMSDotnet.ViewModels.ScannerTransactionViewModel model, int userId)
    {
        using var _context = _factory.CreateDbContext();
        if (model.Type == "OUT" && model.EventDate.HasValue && model.EventDate.Value.Date < WibHelper.Today)
            throw new Exception("Tanggal event tidak boleh sebelum hari ini.");

        foreach (var item in model.Items)
        {
            var product = await _context.Products.FirstOrDefaultAsync(p => p.sku == item.Sku);
            if (product == null)
                throw new Exception($"Produk dengan SKU {item.Sku} tidak ditemukan.");

            if (model.Type == "OUT")
            {
                if (product.current_stock < item.Quantity)
                    throw new Exception($"Stok produk {product.name} ({item.Sku}) tidak mencukupi. Stok: {product.current_stock}");

                product.current_stock -= item.Quantity;
            }
            else
            {
                product.current_stock += item.Quantity;
            }

            var transaction = new Transaction
            {
                product_id = product.id,
                type = model.Type,
                quantity = item.Quantity,
                request_type = model.Type == "OUT" ? "BORROW" : null, // Default behavior
                status = "APPROVED",
                requester_id = userId,
                approver_id = userId,
                notes = model.Notes,
                created_at = DateTime.UtcNow,
                updated_at = DateTime.UtcNow
            };

            if (model.Type == "OUT")
            {
                transaction.applicant_name = model.ApplicantName;
                transaction.applicant_nrp = model.ApplicantNrp;
                transaction.event_name = model.EventName;
                transaction.event_date = model.EventDate;
                transaction.division_id = model.DivisionId;
                transaction.documentation_link = model.DocumentationLink;

                if (product.is_returnable != 1)
                {
                    transaction.request_type = "GIVEAWAY";
                }
            }

            _context.Transactions.Add(transaction);

            // Add stock log
            int stockBefore = model.Type == "OUT" ? product.current_stock + item.Quantity : product.current_stock - item.Quantity;
            _context.StockLogs.Add(new StockLog
            {
                product_id = product.id,
                transaction_id = transaction.id,
                stock_before = stockBefore,
                stock_after = product.current_stock,
                created_at = DateTime.UtcNow,
                updated_at = DateTime.UtcNow
            });
        }

        await _context.SaveChangesAsync();
    }
    public async Task<(List<Transaction> items, int totalItems, int totalPages)> GetDamagedGoodsAsync(string? search, string? filterCategory = null, string? sortBy = "newest", int page = 1, int pageSize = 10)
    {
        using var _context = _factory.CreateDbContext();
        var query = _context.Transactions
            .Include(t => t.Product)
            .Include(t => t.Requester)
            .AsQueryable();

        if (filterCategory == "rusak")
        {
            query = query.Where(t => t.return_status == "rusak" || t.return_condition == "rusak");
        }
        else if (filterCategory == "hilang")
        {
            query = query.Where(t => t.return_status == "hilang" || t.return_condition == "hilang");
        }
        else
        {
            query = query.Where(t => t.return_status == "rusak" || t.return_status == "hilang" || t.return_condition == "rusak" || t.return_condition == "hilang");
        }

        if (!string.IsNullOrWhiteSpace(search))
        {
            var s = search.ToLower();
            query = query.Where(t => 
                (t.Product != null && t.Product.name.ToLower().Contains(s)) ||
                (t.applicant_name != null && t.applicant_name.ToLower().Contains(s)) ||
                (t.return_reason != null && t.return_reason.ToLower().Contains(s)));
        }

        switch (sortBy)
        {
            case "oldest":
                query = query.OrderBy(t => t.returned_at ?? t.updated_at);
                break;
            case "name_asc":
                query = query.OrderBy(t => t.Product.name);
                break;
            case "name_desc":
                query = query.OrderByDescending(t => t.Product.name);
                break;
            case "newest":
            default:
                query = query.OrderByDescending(t => t.returned_at ?? t.updated_at);
                break;
        }

        var totalItems = await query.CountAsync();
        var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (items, totalItems, totalPages);
    }
}
