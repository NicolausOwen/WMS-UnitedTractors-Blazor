using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;
using UT_WMSDotnet.Data;
using UT_WMSDotnet.Models;
using UT_WMSDotnet.ViewModels;

namespace WMS_UnitedTracors_Blazor.Services;

public class TransactionService
{
    private readonly ApplicationDbContext _context;
    private readonly IWebHostEnvironment _env;

    public TransactionService(ApplicationDbContext context, IWebHostEnvironment env)
    {
        _context = context;
        _env = env;
    }

    public async Task<string?> StoreTransactionAsync(TransactionRequestViewModel model, int currentUserId)
    {
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

        if (model.type == "OUT")
        {
            if (string.IsNullOrEmpty(model.event_name) || !model.event_date.HasValue || 
                string.IsNullOrEmpty(model.applicant_name) || !model.division_id.HasValue)
            {
                return "Detail event, pemohon, dan divisi wajib diisi untuk transaksi OUT.";
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
                if (!item.borrow_start_date.HasValue || !item.expected_return_date.HasValue)
                {
                    return $"Tanggal pinjam dan tanggal kembali wajib diisi untuk SKU: {item.sku}.";
                }

                if (item.expected_return_date <= item.borrow_start_date)
                {
                    return $"Tanggal kembali harus lebih besar dari tanggal pinjam untuk SKU: {item.sku}.";
                }

                var duration = (int)(item.expected_return_date.Value.Date - item.borrow_start_date.Value.Date).TotalDays;
                if (duration <= 0) duration = 1;
                item.borrow_duration_days = duration;

                if (item.borrow_duration_days <= 0)
                {
                    return $"Durasi peminjaman untuk SKU {item.sku} tidak valid.";
                }
            }
        }

        var user = await _context.Users.FindAsync(currentUserId);
        if (user == null) return "Unauthorized.";

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

                totalPointsRequired += (product.value * item.quantity);
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

        foreach (var item in items)
        {
            var product = await _context.Products.FirstOrDefaultAsync(p => p.sku == item.sku);
            if (product != null)
            {
                var reqType = item.request_type ?? "BORROW";
                var transaction = new Transaction
                {
                    product_id = product.id,
                    product_variant_id = item.product_variant_id,
                    type = model.type,
                    request_type = model.type == "OUT" ? reqType : "BORROW",
                    quantity = item.quantity,
                    status = "PENDING",
                    requester_id = currentUserId,
                    notes = model.notes,
                    applicant_name = model.applicant_name,
                    applicant_nrp = model.applicant_nrp,
                    event_name = model.event_name,
                    event_date = model.event_date,
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
            }
        }

        await _context.SaveChangesAsync();
        return null;
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
        var query = _context.Transactions
            .Include(t => t.Product)
            .Include(t => t.ProductVariant)
            .Include(t => t.Requester)
            .Include(t => t.Approver)
            .Include(t => t.Division)
            .Where(t => t.status == "REJECTED" || t.status == "APPROVED")
            .AsQueryable();

        if (userRole == "staff")
        {
            query = query.Where(t => t.requester_id == currentUserId);
        }

        if (!string.IsNullOrEmpty(type))
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

    public async Task<string?> ReturnItemAsync(ReturnItemViewModel model)
    {
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
            var uploads = Path.Combine(_env.WebRootPath, "storage", "returns");
            if (!Directory.Exists(uploads)) Directory.CreateDirectory(uploads);
            var fileName = Guid.NewGuid().ToString() + Path.GetExtension(model.return_photo_browser.Name);
            var filePath = Path.Combine(uploads, fileName);
            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await model.return_photo_browser.OpenReadStream(10 * 1024 * 1024).CopyToAsync(fileStream);
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

        transaction.return_photo = photoPath;
        transaction.return_status = model.return_status;
        transaction.return_reason = model.return_reason;
        
        // Process the return immediately
        var strategy = _context.Database.CreateExecutionStrategy();
        return await strategy.ExecuteAsync(async () =>
        {
            using var dbTransaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var stockBefore = transaction.Product.current_stock;
                transaction.Product.current_stock += model.return_quantity;
                _context.Products.Update(transaction.Product);

                if (transaction.product_variant_id.HasValue)
                {
                    var variant = await _context.ProductVariants.FindAsync(transaction.product_variant_id.Value);
                    if (variant != null)
                    {
                        variant.stock += model.return_quantity;
                        _context.ProductVariants.Update(variant);
                    }
                }

                var stockLog = new StockLog
                {
                    transaction_id = transaction.id,
                    product_id = transaction.Product.id,
                    stock_before = stockBefore,
                    stock_after = transaction.Product.current_stock,
                    created_at = DateTime.UtcNow,
                    updated_at = DateTime.UtcNow
                };
                _context.StockLogs.Add(stockLog);

                transaction.returned_quantity = (transaction.returned_quantity ?? 0) + model.return_quantity;
                transaction.pending_return_quantity = 0;
                transaction.is_return_draft = 0;
                
                if (transaction.returned_quantity >= transaction.quantity)
                {
                    transaction.returned_at = DateTime.UtcNow; // Mark fully returned
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

    public async Task<string?> ForceReturnTransactionAsync(int transactionId, int returnQty, string status, string? reason, int adminId)
    {
        var model = new ReturnItemViewModel
        {
            transaction_id = transactionId,
            return_quantity = returnQty,
            return_status = status,
            return_reason = reason
        };
        // We can reuse ReturnItemAsync logic but bypass photo requirements by setting a dummy photo or bypassing it
        var transaction = await _context.Transactions.Include(t => t.Product).FirstOrDefaultAsync(t => t.id == transactionId);
        if (transaction == null) return "Transaction not found.";
        
        transaction.return_photo = "forced-by-admin"; // dummy
        
        return await ReturnItemAsync(model);
    }

    public async Task<string?> ConfirmHandoverAsync(
        List<int> transactionIds,
        Microsoft.AspNetCore.Components.Forms.IBrowserFile? photo,
        string? notes)
    {
        if (transactionIds == null || !transactionIds.Any())
            return "Tidak ada item yang menunggu serah terima.";

        var transactions = await _context.Transactions
            .Where(t => transactionIds.Contains(t.id))
            .ToListAsync();

        var pending = transactions
            .Where(t => t.request_type == "BORROW" &&
                        (t.status == "WAITING_HANDOVER" || t.status == "WAITING_ADMIN_HANDOVER"))
            .ToList();

        if (!pending.Any())
            return "Item ini tidak sedang menunggu bukti serah terima.";

        if (photo == null)
            return "Foto bukti serah terima wajib diunggah.";

        string photoPath;
        try
        {
            var uploads = Path.Combine(_env.WebRootPath, "storage", "handovers");
            if (!Directory.Exists(uploads)) Directory.CreateDirectory(uploads);
            var fileName = Guid.NewGuid().ToString() + Path.GetExtension(photo.Name);
            var filePath = Path.Combine(uploads, fileName);
            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await photo.OpenReadStream(10 * 1024 * 1024).CopyToAsync(fileStream);
            }
            photoPath = "storage/handovers/" + fileName;
        }
        catch (Exception ex)
        {
            return "Gagal mengunggah foto: " + ex.Message;
        }

        foreach (var item in pending)
        {
            item.handover_photo = photoPath;
            item.handover_notes = notes;
            item.status = "APPROVED";
            item.updated_at = DateTime.UtcNow;
            _context.Transactions.Update(item);
        }

        await _context.SaveChangesAsync();
        return null;
    }

    public async Task<string?> CancelReturnDraftAsync(int id)
    {
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
        var transactions = await _context.Transactions
            .Where(t => t.status == "APPROVED" && t.type == "OUT" && t.request_type == "BORROW" &&
                        t.pending_return_quantity > 0 && t.is_return_draft == 1)
            .ToListAsync();

        var grouped = transactions.GroupBy(t => CreateGroupId(t.created_at, t.requester_id, t.applicant_name ?? ""));
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
        var transaction = await _context.Transactions.Include(t => t.Product).Include(t => t.Requester).FirstOrDefaultAsync(t => t.id == id);
        if (transaction == null || transaction.status != "REVISION") return "Request is not in revision state.";

        int newQty = model.quantity ?? transaction.quantity ?? 0;
        string newRequestType = model.request_type ?? transaction.request_type ?? "BORROW";
        var product = transaction.Product;

        if (product?.current_stock < newQty) return "Insufficient stock for this product.";

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
        transaction.applicant_name = model.applicant_name;
        transaction.applicant_nrp = model.applicant_nrp;
        transaction.event_name = model.event_name;
        transaction.event_date = model.event_date;
        transaction.division_id = model.division_id ?? transaction.division_id;
        transaction.documentation_link = model.documentation_link;
        transaction.notes = model.notes;
        transaction.status = "PENDING";
        transaction.updated_at = DateTime.UtcNow;

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
        await _context.SaveChangesAsync();
        return null;
    }

    public async Task<string?> CancelRevisionAsync(int id)
    {
        var transaction = await _context.Transactions.Include(t => t.Product).Include(t => t.Requester).FirstOrDefaultAsync(t => t.id == id);
        if (transaction == null || transaction.status != "REVISION") return "Request is not in revision state.";

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

        transaction.status = "REJECTED";
        transaction.rejection_reason = "Cancelled by requester";
        transaction.updated_at = DateTime.UtcNow;

        _context.Transactions.Update(transaction);
        await _context.SaveChangesAsync();
        return null;
    }

    public string CreateGroupId(DateTime createdAt, int requesterId, string applicantName)
    {
        var raw = $"{createdAt:yyyy-MM-dd HH:mm}_{requesterId}_{applicantName}";
        using var md5 = MD5.Create();
        var hashBytes = md5.ComputeHash(Encoding.UTF8.GetBytes(raw));
        return BitConverter.ToString(hashBytes).Replace("-", "").ToLowerInvariant();
    }

    public async Task<string?> DeleteTransactionAsync(int id)
    {
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
        var allTransactions = await _context.Transactions
            .Include(t => t.Product)
            .ThenInclude(p => p.Unit)
            .Include(t => t.Requester)
            .Include(t => t.Approver)
            .Include(t => t.Division)
            .ToListAsync();

        return allTransactions.Where(t => t.group_id == groupId).ToList();
    }

    public async Task<string?> SubmitHandoverBatchAsync(string groupId, string? photoPath, string? notes)
    {
        var transactions = await GetTransactionsByGroupIdAsync(groupId);
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
    }
