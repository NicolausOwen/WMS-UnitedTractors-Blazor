using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
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

        // Passive Auto-approve WAITING_HANDOVER_CONFIRM after 24 hours
        var cutoff = DateTime.UtcNow.AddHours(-24);
        var expiredHandovers = await _context.Transactions
            .Where(t =>
                t.request_type == "BORROW" &&
                t.status == WorkflowStatuses.WaitingHandoverConfirm &&
                t.updated_at <= cutoff)
            .ToListAsync();

        if (expiredHandovers.Any())
        {
            foreach (var item in expiredHandovers)
            {
                item.status = WorkflowStatuses.Approved;
                item.updated_at = DateTime.UtcNow;
            }
            await _context.SaveChangesAsync();
        }

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
            .Include(p => p.ProductVariants.Where(v => v.is_hidden == 0))
            .Where(p => p.is_hidden == 0)
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
            BorrowOverdue = await _context.Transactions.Where(t => t.request_type == "BORROW" && t.status == WorkflowStatuses.Approved && t.returned_quantity < t.quantity && t.expected_return_date < WibHelper.Today).CountAsync(),
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
                       perms.Contains(WMS_UnitedTracors_Blazor.Helpers.Permissions.ApprovalHandover);

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

    public async Task<List<ActionItemModel>> GetUserActionItemsAsync(int userId, ClaimsPrincipal? user = null)
    {
        using var _context = _factory.CreateDbContext();
        var actionItems = new List<ActionItemModel>();

        // Determine category-level restriction for admins / approvers
        var userAdminRoles = await _context.UserAdminRoles.Where(uar => uar.UserId == userId).ToListAsync();
        bool isSuperAdmin = user != null && Permissions.All.All(p => user.HasPermission(p));
        bool isGlobalAdmin = isSuperAdmin || userAdminRoles.Any(uar => uar.CategoryId == null);
        List<int>? allowedCategoryIds = null;
        if (!isGlobalAdmin)
        {
            allowedCategoryIds = userAdminRoles.Where(uar => uar.CategoryId != null).Select(uar => uar.CategoryId!.Value).ToList();
        }

        // 1. General User Tasks (Requester role / their own requests)
        var pendingTransactions = await _context.Transactions
            .Include(t => t.Product)
            .Where(t => t.requester_id == userId && 
                        (t.status == WorkflowStatuses.WaitingHandover || 
                         t.status == WorkflowStatuses.WaitingAdminHandover ||
                         t.status == WorkflowStatuses.Revision ||
                         t.status == WorkflowStatuses.RevisionByStaffInventory ||
                         t.status == WorkflowStatuses.RevisionByAdmin ||
                         t.status == WorkflowStatuses.RevisionByManager ||
                         t.status == WorkflowStatuses.WaitingHandoverConfirm ||
                         t.status == WorkflowStatuses.WaitingDocumentation ||
                         t.status == WorkflowStatuses.DocumentationOverdue ||
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
                    BadgeClass = "bg-[#fef3c7] text-[#d97706] border border-[#fde68a]",
                    Badge = "Menunggu Serah Terima",
                    Description = $"Menunggu Staff Inventoris mengunggah bukti serah terima untuk {t.quantity}x {(t.Product != null ? t.Product.name : "")}",
                    Date = t.created_at,
                    BtnUrl = "Tracking",
                    BtnText = "Cek Tracking"
                });
            }
            else if (t.status == WorkflowStatuses.WaitingHandoverConfirm)
            {
                actionItems.Add(new ActionItemModel
                {
                    Type = "handover",
                    Title = t.event_name ?? "Berita Acara Serah Terima",
                    BadgeClass = "bg-[#e0f2fe] text-[#0369a1] border border-[#bae6fd]",
                    Badge = "Berita Acara Serah Terima",
                    Description = $"Barang telah diserahkan. Harap cek kecocokan fisik untuk {t.quantity}x {(t.Product != null ? t.Product.name : "")}.",
                    Date = t.created_at,
                    BtnUrl = "Tracking",
                    BtnText = "Cek / Return"
                });
            }
            else if (t.status == WorkflowStatuses.WaitingDocumentation || t.status == WorkflowStatuses.DocumentationOverdue)
            {
                bool isOverdue = t.status == WorkflowStatuses.DocumentationOverdue;
                actionItems.Add(new ActionItemModel
                {
                    Type = "handover",
                    Title = t.event_name ?? "Dokumentasi Giveaway",
                    BadgeClass = isOverdue ? "bg-[#fef0f0] text-[#d94040] border border-[#f8c0c0]" : "bg-[#fff8e6] text-[#b37a00] border border-[#ffe099]",
                    Badge = isOverdue ? "Dokumentasi Overdue" : "Menunggu Dokumentasi",
                    Description = $"Unggah bukti dokumentasi foto untuk {t.quantity}x {(t.Product != null ? t.Product.name : "")}",
                    Date = t.created_at,
                    BtnUrl = "Tracking",
                    BtnText = "Upload Foto"
                });
            }
            else if (t.status == WorkflowStatuses.Revision || t.status == WorkflowStatuses.RevisionByStaffInventory || t.status == WorkflowStatuses.RevisionByAdmin || t.status == WorkflowStatuses.RevisionByManager)
            {
                actionItems.Add(new ActionItemModel
                {
                    Type = "revision",
                    Title = t.event_name ?? "Request Barang",
                    BadgeClass = "bg-[#fef0f0] text-[#d94040] border border-[#f8c0c0]",
                    Badge = "Butuh Revisi",
                    Description = $"Revisi request {(t.Product != null ? t.Product.name : "")}. Alasan: {t.rejection_reason}",
                    Date = t.created_at,
                    BtnUrl = "Tracking",
                    BtnText = "Revisi"
                });
            }
            else if (t.request_type == "BORROW" && t.status == WorkflowStatuses.Approved && t.returned_quantity < t.quantity)
            {
                bool isOverdue = t.expected_return_date < WibHelper.Today;
                actionItems.Add(new ActionItemModel
                {
                    Type = "return",
                    Title = t.event_name ?? "Peminjaman Barang",
                    BadgeClass = isOverdue ? "bg-[#fef0f0] text-[#d94040] border border-[#f8c0c0]" : "bg-[#f0f9ff] text-[#0284c7] border border-[#b3e5fc]",
                    Badge = isOverdue ? "Overdue" : "Dipinjam",
                    Description = $"Kembalikan {t.quantity - (t.returned_quantity ?? 0)}x {(t.Product != null ? t.Product.name : "")}. {(t.expected_return_date.HasValue ? "Tenggat: " + t.expected_return_date.Value.ToString("dd MMM yyyy") : "")}",
                    Date = t.created_at,
                    BtnUrl = "Tracking",
                    BtnText = "Kembalikan"
                });
            }
        }

        // 2. Staff Inventory (SI) / Admin Tasks
        if (user != null && (user.HasPermission(Permissions.ApprovalStage1) || user.HasPermission(Permissions.ApprovalHandover)))
        {
            // A. Menunggu Approval Tahap 1
            var qStage1 = _context.Transactions
                .Include(t => t.Product)
                .Include(t => t.Requester)
                .Where(t => t.status == WorkflowStatuses.PendingStaffInventory || t.status == WorkflowStatuses.Pending);
            if (!isGlobalAdmin && allowedCategoryIds != null)
            {
                qStage1 = qStage1.Where(t => t.Product != null && t.Product.category_id != null && allowedCategoryIds.Contains(t.Product.category_id.Value));
            }
            var toApproveStage1 = await qStage1.OrderByDescending(t => t.updated_at).ToListAsync();

            foreach (var t in toApproveStage1)
            {
                actionItems.Add(new ActionItemModel
                {
                    Type = "handover",
                    Title = t.event_name ?? "Approval Tahap 1",
                    BadgeClass = "bg-[#fff8e6] text-[#b37a00] border border-[#ffe099]",
                    Badge = "Menunggu Approval",
                    Description = $"Persetujuan pengajuan {t.request_type} dari {t.Requester?.name ?? "User"} untuk {t.quantity}x {(t.Product != null ? t.Product.name : "")}",
                    Date = t.created_at,
                    BtnUrl = "Approvals",
                    BtnText = "Proses"
                });
            }

            // B. Menunggu Serah Terima (SI harus isi bukti serah terima)
            var qHandover = _context.Transactions
                .Include(t => t.Product)
                .Include(t => t.Requester)
                .Where(t => t.status == WorkflowStatuses.WaitingHandover);
            if (!isGlobalAdmin && allowedCategoryIds != null)
            {
                qHandover = qHandover.Where(t => t.Product != null && t.Product.category_id != null && allowedCategoryIds.Contains(t.Product.category_id.Value));
            }
            var toHandover = await qHandover.OrderByDescending(t => t.updated_at).ToListAsync();

            foreach (var t in toHandover)
            {
                actionItems.Add(new ActionItemModel
                {
                    Type = "handover",
                    Title = t.event_name ?? "Berita Acara Serah Terima",
                    BadgeClass = "bg-[#fef3c7] text-[#d97706] border border-[#fde68a]",
                    Badge = "Butuh Serah Terima",
                    Description = $"Kirim bukti berita acara serah terima untuk {t.quantity}x {(t.Product != null ? t.Product.name : "")} (Penerima: {t.Requester?.name ?? "User"})",
                    Date = t.created_at,
                    BtnUrl = "Approvals",
                    BtnText = "Isi Bukti"
                });
            }

            // C. Dispute Serah Terima (User dispute "Saya Belum Menerima Barang")
            var qDisputes = _context.Transactions
                .Include(t => t.Product)
                .Include(t => t.Requester)
                .Where(t => t.status == WorkflowStatuses.WaitingHandoverConfirm && !string.IsNullOrEmpty(t.rejection_reason));
            if (!isGlobalAdmin && allowedCategoryIds != null)
            {
                qDisputes = qDisputes.Where(t => t.Product != null && t.Product.category_id != null && allowedCategoryIds.Contains(t.Product.category_id.Value));
            }
            var disputes = await qDisputes.OrderByDescending(t => t.updated_at).ToListAsync();

            foreach (var t in disputes)
            {
                actionItems.Add(new ActionItemModel
                {
                    Type = "revision",
                    Title = t.event_name ?? "Dispute Penerimaan",
                    BadgeClass = "bg-[#fef0f0] text-[#d94040] border border-[#f8c0c0]",
                    Badge = "Disputed",
                    Description = $"User {t.Requester?.name} mengklaim belum menerima {t.quantity}x {(t.Product != null ? t.Product.name : "")}. Alasan: {t.rejection_reason}",
                    Date = t.created_at,
                    BtnUrl = "Approvals",
                    BtnText = "Verifikasi"
                });
            }

            // D. Approval Return (Returns waiting for ACC)
            var qReturns = _context.Transactions
                .Include(t => t.Product)
                .Include(t => t.Requester)
                .Where(t => t.status == WorkflowStatuses.Approved && t.pending_return_quantity > 0 && t.is_return_draft == 0);
            if (!isGlobalAdmin && allowedCategoryIds != null)
            {
                qReturns = qReturns.Where(t => t.Product != null && t.Product.category_id != null && allowedCategoryIds.Contains(t.Product.category_id.Value));
            }
            var pendingReturns = await qReturns.OrderByDescending(t => t.updated_at).ToListAsync();

            foreach (var t in pendingReturns)
            {
                actionItems.Add(new ActionItemModel
                {
                    Type = "return",
                    Title = t.event_name ?? "Persetujuan Return",
                    BadgeClass = "bg-[#e0f2fe] text-[#0369a1] border border-[#bae6fd]",
                    Badge = "Return Pending",
                    Description = $"Persetujuan pengembalian {t.pending_return_quantity}x {(t.Product != null ? t.Product.name : "")} dari {t.Requester?.name}",
                    Date = t.created_at,
                    BtnUrl = "Approvals",
                    BtnText = "Verifikasi Return"
                });
            }
        }

        // 3. TL / PIC Studio / Admin Stage 2 Tasks
        if (user != null && (user.HasPermission(Permissions.ApprovalStage2) || user.HasPermission(Permissions.ApprovalHandoverFinal)))
        {
            // A. Approval Tahap 2
            var qStage2 = _context.Transactions
                .Include(t => t.Product)
                .Include(t => t.Requester)
                .Where(t => t.status == WorkflowStatuses.PendingAdmin);
            if (!isGlobalAdmin && allowedCategoryIds != null)
            {
                qStage2 = qStage2.Where(t => t.Product != null && t.Product.category_id != null && allowedCategoryIds.Contains(t.Product.category_id.Value));
            }
            var toApproveStage2 = await qStage2.OrderByDescending(t => t.updated_at).ToListAsync();

            foreach (var t in toApproveStage2)
            {
                actionItems.Add(new ActionItemModel
                {
                    Type = "handover",
                    Title = t.event_name ?? "Approval Tahap 2",
                    BadgeClass = "bg-[#fff8e6] text-[#b37a00] border border-[#ffe099]",
                    Badge = "Menunggu Approval TL",
                    Description = $"Persetujuan pengajuan {t.request_type} dari {t.Requester?.name ?? "User"} untuk {t.quantity}x {(t.Product != null ? t.Product.name : "")}",
                    Date = t.created_at,
                    BtnUrl = "Approvals",
                    BtnText = "Proses"
                });
            }

            // B. Verifikasi Bukti Serah Terima Final
            var qVerifyHandovers = _context.Transactions
                .Include(t => t.Product)
                .Include(t => t.Requester)
                .Where(t => t.status == WorkflowStatuses.WaitingAdminHandover);
            if (!isGlobalAdmin && allowedCategoryIds != null)
            {
                qVerifyHandovers = qVerifyHandovers.Where(t => t.Product != null && t.Product.category_id != null && allowedCategoryIds.Contains(t.Product.category_id.Value));
            }
            var verifyHandovers = await qVerifyHandovers.OrderByDescending(t => t.updated_at).ToListAsync();

            foreach (var t in verifyHandovers)
            {
                actionItems.Add(new ActionItemModel
                {
                    Type = "handover",
                    Title = t.event_name ?? "Verifikasi Serah Terima",
                    BadgeClass = "bg-[#f5f3ff] text-[#6d28d9] border border-[#ddd6fe]",
                    Badge = "Verifikasi Bukti",
                    Description = $"Verifikasi bukti serah terima untuk {t.quantity}x {(t.Product != null ? t.Product.name : "")} (Penerima: {t.Requester?.name ?? "User"})",
                    Date = t.created_at,
                    BtnUrl = "Approvals",
                    BtnText = "Verifikasi"
                });
            }
        }

        // 4. Manager Tasks
        if (user != null && user.HasPermission(Permissions.ApprovalManager))
        {
            var qManager = _context.Transactions
                .Include(t => t.Product)
                .Include(t => t.Requester)
                .Where(t => t.status == WorkflowStatuses.PendingManager);
            if (!isGlobalAdmin && allowedCategoryIds != null)
            {
                qManager = qManager.Where(t => t.Product != null && t.Product.category_id != null && allowedCategoryIds.Contains(t.Product.category_id.Value));
            }
            var toApproveManager = await qManager.OrderByDescending(t => t.updated_at).ToListAsync();

            foreach (var t in toApproveManager)
            {
                actionItems.Add(new ActionItemModel
                {
                    Type = "handover",
                    Title = t.event_name ?? "Approval Manager",
                    BadgeClass = "bg-[#fff8e6] text-[#b37a00] border border-[#ffe099]",
                    Badge = "Menunggu Approval Manager",
                    Description = $"Persetujuan pengajuan {t.request_type} dari {t.Requester?.name ?? "User"} untuk {t.quantity}x {(t.Product != null ? t.Product.name : "")}",
                    Date = t.created_at,
                    BtnUrl = "Approvals",
                    BtnText = "Proses"
                });
            }
        }

        return actionItems.OrderByDescending(x => x.Date).ToList();
    }
}
