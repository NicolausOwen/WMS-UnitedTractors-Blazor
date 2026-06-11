using Microsoft.EntityFrameworkCore;
using UT_WMSDotnet.Data;
using UT_WMSDotnet.Models;
using WMS_UnitedTracors_Blazor.Helpers;

namespace WMS_UnitedTracors_Blazor.Services;

public class DashboardService
{
    private readonly IDbContextFactory<ApplicationDbContext> _factory;

    public DashboardService(IDbContextFactory<ApplicationDbContext> factory)
    {
        _factory = factory;
    }

    public class DashboardData
    {
        public int WidgetTotalItems { get; set; }
        public int PendingApprovals { get; set; }
        public int TotalStock { get; set; }
        public List<Category> Categories { get; set; } = new();
        public List<Division> Divisions { get; set; } = new();
        public List<Product> CatalogProducts { get; set; } = new();
        public int TotalItems { get; set; }
        public int TotalPages { get; set; }

        public List<Product> LowStockProducts { get; set; } = new();
        public int DonutApproved { get; set; }
        public int DonutRejected { get; set; }
        public int DonutPending { get; set; }
        public int MerchAvailable { get; set; }
        public int MerchLowStock { get; set; }
        public int MerchOutOfStock { get; set; }
        public int BorrowOverdue { get; set; }
        public int BorrowAvailable { get; set; }
        public int BorrowBorrowed { get; set; }
    }

    public async Task<DashboardData> GetDashboardDataAsync(int? category, string? search = null, int page = 1, int pageSize = 15)
    {
        using var _context = _factory.CreateDbContext();
        var widgetTotalItems = await _context.Products.CountAsync();
        var pendingApprovals = await _context.Transactions.CountAsync(t =>
            t.status == WorkflowStatuses.Pending ||
            t.status == WorkflowStatuses.PendingStaffInventory ||
            t.status == WorkflowStatuses.PendingAdmin ||
            t.status == WorkflowStatuses.PendingManager);
        var totalStock = await _context.Products.SumAsync(p => (int?)p.current_stock) ?? 0;
        
        var categories = await _context.Categories.OrderBy(c => c.name).ToListAsync();
        var divisions = await _context.Divisions.OrderBy(d => d.name).ToListAsync();

        var catalogQuery = _context.Products
            .Include(p => p.Category)
            .Include(p => p.Location)
            .Include(p => p.Unit)
            .Include(p => p.ProductVariants)
            .OrderBy(p => p.name)
            .AsQueryable();

        if (category.HasValue)
        {
            catalogQuery = catalogQuery.Where(p => p.category_id == category.Value);
        }

        if (!string.IsNullOrWhiteSpace(search))
        {
            catalogQuery = catalogQuery.Where(p => p.name.Contains(search) || p.sku.Contains(search));
        }

        int catalogTotalItems = await catalogQuery.CountAsync();
        int totalPages = (int)Math.Ceiling(catalogTotalItems / (double)pageSize);

        var catalogProducts = await catalogQuery
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        var data = new DashboardData
        {
            WidgetTotalItems = widgetTotalItems,
            PendingApprovals = pendingApprovals,
            TotalStock = totalStock,
            Categories = categories,
            Divisions = divisions,
            CatalogProducts = catalogProducts,
            TotalItems = catalogTotalItems,
            TotalPages = totalPages,
            LowStockProducts = await _context.Products.Include(p => p.Unit).Where(p => p.current_stock <= 2 && p.current_stock >= 0).OrderBy(p => p.current_stock).Take(10).ToListAsync(),
            DonutApproved = await _context.Transactions.CountAsync(t => t.status == WorkflowStatuses.Approved || t.status == WorkflowStatuses.Completed),
            DonutRejected = await _context.Transactions.CountAsync(t => t.status == WorkflowStatuses.Rejected),
            DonutPending = await _context.Transactions.CountAsync(t =>
                t.status == WorkflowStatuses.Pending ||
                t.status == WorkflowStatuses.PendingStaffInventory ||
                t.status == WorkflowStatuses.PendingAdmin ||
                t.status == WorkflowStatuses.PendingManager),
            MerchAvailable = await _context.Products.Where(p => p.is_returnable != 1 && p.current_stock > 2).CountAsync(),
            MerchLowStock = await _context.Products.Where(p => p.is_returnable != 1 && p.current_stock > 0 && p.current_stock <= 2).CountAsync(),
            MerchOutOfStock = await _context.Products.Where(p => p.is_returnable != 1 && p.current_stock <= 0).CountAsync(),
            BorrowOverdue = await _context.Transactions.Where(t => t.request_type == "BORROW" && t.status == WorkflowStatuses.Approved && t.returned_quantity < t.quantity && t.expected_return_date < DateTime.Today).CountAsync(),
            BorrowAvailable = await _context.Products.Where(p => p.is_returnable == 1 && p.current_stock > 0).CountAsync(),
            BorrowBorrowed = await _context.Transactions.Where(t => t.request_type == "BORROW" && t.status == WorkflowStatuses.Approved && t.returned_quantity < t.quantity).CountAsync()
        };

        return data;
    }

    public async Task<List<Transaction>> GetActiveBorrowsAsync(int currentUserId, string? userRole)
    {
        using var _context = _factory.CreateDbContext();
        var activeBorrowsQuery = _context.Transactions
            .Include(t => t.Product)
            .Include(t => t.Requester)
            .Where(t => t.type == "OUT" && t.status == WorkflowStatuses.Approved && 
                        t.Product != null && t.Product.is_returnable == 1 &&
                        (t.quantity - (t.returned_quantity) - t.pending_return_quantity) > 0)
            .AsQueryable();

        // Pengelola/approver melihat semua peminjaman aktif; pemohon biasa hanya miliknya.
        var actor = await _context.Users.FindAsync(currentUserId);
        var actorRole = actor != null ? await _context.AdminRoles.FirstOrDefaultAsync(r => r.RoleName == actor.role && r.IsActive) : null;
        var perms = WMS_UnitedTracors_Blazor.Helpers.Permissions.Resolve(actor?.role, actorRole?.Permissions);
        bool seesAll = perms.Contains(WMS_UnitedTracors_Blazor.Helpers.Permissions.ProductsManage) ||
                       perms.Contains(WMS_UnitedTracors_Blazor.Helpers.Permissions.ApprovalReturn);

        if (!seesAll)
        {
            activeBorrowsQuery = activeBorrowsQuery.Where(t => t.requester_id == currentUserId);
        }

        return await activeBorrowsQuery.ToListAsync();
    }

    public class ActionItemModel
    {
        public string Type { get; set; } = "";
        public string Title { get; set; } = "";
        public string BadgeClass { get; set; } = "";
        public string Badge { get; set; } = "";
        public string Description { get; set; } = "";
        public DateTime Date { get; set; }
        public string BtnUrl { get; set; } = "";
        public string BtnText { get; set; } = "";
    }

    public async Task<List<ActionItemModel>> GetUserActionItemsAsync(int userId)
    {
        using var _context = _factory.CreateDbContext();
        var actionItems = new List<ActionItemModel>();

        var pendingTransactions = await _context.Transactions
            .Include(t => t.Product)
            .Where(t => t.requester_id == userId && 
                        (t.status == WorkflowStatuses.WaitingHandover || 
                         t.status == WorkflowStatuses.WaitingAdminHandover ||
                         t.status == WorkflowStatuses.Revision ||
                         t.status == WorkflowStatuses.RevisionByStaffInventory ||
                         t.status == WorkflowStatuses.RevisionByAdmin ||
                         t.status == WorkflowStatuses.RevisionByManager ||
                         (t.request_type == "BORROW" && t.status == WorkflowStatuses.Approved && t.returned_quantity < t.quantity)))
            .OrderByDescending(t => t.updated_at)
            .ToListAsync();

        foreach (var t in pendingTransactions)
        {
            if (t.status == WorkflowStatuses.WaitingHandover || t.status == WorkflowStatuses.WaitingAdminHandover)
            {
                actionItems.Add(new ActionItemModel
                {
                    Type = "handover",
                    Title = t.event_name ?? "Request Barang",
                    BadgeClass = "bg-[#fef3c7] text-[#d97706]",
                    Badge = "Menunggu Serah Terima",
                    Description = $"Upload bukti serah terima untuk {t.quantity}x {(t.Product != null ? t.Product.name : "")}",
                    Date = t.created_at,
                    BtnUrl = "Tracking",
                    BtnText = "Upload Bukti"
                });
            }
            else if (t.status == WorkflowStatuses.Revision || t.status == WorkflowStatuses.RevisionByStaffInventory || t.status == WorkflowStatuses.RevisionByAdmin || t.status == WorkflowStatuses.RevisionByManager)
            {
                actionItems.Add(new ActionItemModel
                {
                    Type = "revision",
                    Title = t.event_name ?? "Request Barang",
                    BadgeClass = "bg-[#fef0f0] text-[#d94040]",
                    Badge = "Butuh Revisi",
                    Description = $"Revisi request {(t.Product != null ? t.Product.name : "")}. Alasan: {t.rejection_reason}",
                    Date = t.created_at,
                    BtnUrl = "Tracking",
                    BtnText = "Revisi"
                });
            }
            else if (t.request_type == "BORROW" && t.status == WorkflowStatuses.Approved && t.returned_quantity < t.quantity)
            {
                bool isOverdue = t.expected_return_date < DateTime.Today;
                actionItems.Add(new ActionItemModel
                {
                    Type = "return",
                    Title = t.event_name ?? "Peminjaman Barang",
                    BadgeClass = isOverdue ? "bg-[#fef0f0] text-[#d94040]" : "bg-[#f0f9ff] text-[#0284c7]",
                    Badge = isOverdue ? "Overdue" : "Dipinjam",
                    Description = $"Kembalikan {t.quantity - (t.returned_quantity ?? 0)}x {(t.Product != null ? t.Product.name : "")}. {(t.expected_return_date.HasValue ? "Tenggat: " + t.expected_return_date.Value.ToString("dd MMM yyyy") : "")}",
                    Date = t.created_at,
                    BtnUrl = "Tracking",
                    BtnText = "Kembalikan"
                });
            }
        }
        
        return actionItems;
    }
}
