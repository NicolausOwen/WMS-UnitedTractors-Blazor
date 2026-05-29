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
                return "Detail event dan pemohon wajib diisi untuk transaksi OUT.";
            }
        }

        foreach (var item in items)
        {
            var reqType = item.request_type ?? "BORROW";
            if (reqType == "BORROW")
            {
                if (item.borrow_start_date.HasValue && item.expected_return_date.HasValue)
                {
                    item.borrow_duration_days = (int)(item.expected_return_date.Value.Date - item.borrow_start_date.Value.Date).TotalDays;
                    if (item.borrow_duration_days <= 0) item.borrow_duration_days = 1;
                }

                if (!item.borrow_duration_days.HasValue || item.borrow_duration_days <= 0)
                {
                    return $"Durasi peminjaman atau tanggal harus diisi dengan benar untuk SKU: {item.sku}.";
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
                    used_by = model.applicant_name,
                    division_id = model.division_id ?? 0,
                    created_at = DateTime.UtcNow,
                    updated_at = DateTime.UtcNow
                };

                _context.Transactions.Add(transaction);
            }
        }

        await _context.SaveChangesAsync();
        return null;
    }

    public async Task<(List<Transaction> Transactions, int TotalItems, int TotalPages)> GetHistoryAsync(int currentUserId, string? userRole, string? type, DateTime? start_date, DateTime? end_date, int page = 1, int pageSize = 10)
    {
        var query = _context.Transactions
            .Include(t => t.Product)
            .Include(t => t.Requester)
            .Include(t => t.Approver)
            .Include(t => t.Division)
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

        string? photoPath = transaction.return_photo;
        if (model.return_photo != null && model.return_photo.Length > 0)
        {
            var fileName = Guid.NewGuid().ToString() + Path.GetExtension(model.return_photo.FileName);
            var path = Path.Combine(_env.ContentRootPath, "Storage", "returns");
            if (!Directory.Exists(path)) Directory.CreateDirectory(path);

            using (var stream = new FileStream(Path.Combine(path, fileName), FileMode.Create))
            {
                await model.return_photo.CopyToAsync(stream);
            }
            photoPath = "returns/" + fileName;
        }
        else if (string.IsNullOrEmpty(photoPath))
        {
            if (transaction.is_return_draft == 0 || string.IsNullOrEmpty(transaction.return_photo))
                return "Foto pengembalian wajib diunggah.";
        }

        transaction.pending_return_quantity = model.return_quantity;
        transaction.return_photo = photoPath;
        transaction.return_status = model.return_status;
        transaction.return_reason = model.return_reason;
        transaction.is_return_draft = 1;
        transaction.updated_at = DateTime.UtcNow;

        _context.Update(transaction);
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
}
