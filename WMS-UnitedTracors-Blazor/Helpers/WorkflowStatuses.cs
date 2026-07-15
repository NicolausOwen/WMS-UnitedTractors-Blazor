namespace WMS_UnitedTracors_Blazor.Helpers;

public static class WorkflowStatuses
{
    public const string Pending = "PENDING";
    public const string PendingStaffInventory = "PENDING_STAFF_INVENTORY";
    public const string PendingAdmin = "PENDING_ADMIN";
    public const string PendingManager = "PENDING_MANAGER";
    public const string WaitingHandover = "WAITING_HANDOVER";
    public const string WaitingHandoverConfirm = "WAITING_HANDOVER_CONFIRM";
    public const string WaitingAdminHandover = "WAITING_ADMIN_HANDOVER";
    public const string WaitingDocumentation = "WAITING_DOCUMENTATION";
    public const string WaitingAdminDocumentation = "WAITING_ADMIN_DOCUMENTATION";
    public const string DocumentationOverdue = "DOCUMENTATION_OVERDUE";
    public const string Approved = "APPROVED";
    public const string Completed = "COMPLETED";
    public const string Rejected = "REJECTED";
    public const string Revision = "REVISION";
    public const string RevisionByStaffInventory = "REVISION_BY_STAFF_INVENTORY";
    public const string RevisionByAdmin = "REVISION_BY_ADMIN";
    public const string RevisionByManager = "REVISION_BY_MANAGER";

    public static bool IsLegacyPending(string? status) =>
        status == Pending || status == PendingAdmin || status == PendingManager || status == PendingStaffInventory;

    public static bool IsRevision(string? status) =>
        status == Revision ||
        status == RevisionByStaffInventory ||
        status == RevisionByAdmin ||
        status == RevisionByManager;

    public static bool IsGiveawayApprovalPending(string? status) =>
        status == PendingStaffInventory || status == PendingAdmin || status == PendingManager;

    public static bool IsGiveawayActive(string? status) =>
        IsGiveawayApprovalPending(status) ||
        IsRevision(status) ||
        status == WaitingHandover ||
        status == WaitingHandoverConfirm ||
        status == WaitingAdminHandover ||
        status == WaitingDocumentation ||
        status == WaitingAdminDocumentation ||
        status == DocumentationOverdue;

    public static bool IsBorrowActive(string? status) =>
        status == Pending ||
        status == PendingStaffInventory ||
        status == PendingAdmin ||
        IsRevision(status) ||
        status == WaitingHandover ||
        status == WaitingAdminHandover ||
        status == WaitingHandoverConfirm ||
        status == Approved;

    public static string GetRevisionStage(string? status) =>
        status switch
        {
            RevisionByStaffInventory => "STAFF_INVENTORY",
            RevisionByAdmin => "ADMIN",
            RevisionByManager => "MANAGER",
            _ => "GENERAL"
        };
}
