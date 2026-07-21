import os

detail_path = r'C:\laragon\www\WMS-UnitedTractors-Blazor\WMS-UnitedTracors-Blazor\Components\Pages\TrackingDetail.razor'

with open(detail_path, 'r', encoding='utf-8') as f:
    lines = f.readlines()

# 1. We will extract all these variables to class level fields:
fields = """
    private List<Transaction> groupItems = new();
    private Transaction? firstItem;
    private List<Transaction> borrowItemsOnly = new();
    private List<Transaction> giveawayItemsOnly = new();
    private bool hasBorrow;
    private int totalItems;
    private int totalReturned;
    private bool allReturned;
    private bool isGiveaway;
    private string status = "";
    private bool isHandoverPending;
    private bool isWaitingAdminHandover;
    private bool handoverDone;
    private bool canHandover;
    private bool isWaitingUserConfirm;
    private bool hasHandoverDraftPhoto;
    private bool canGiveawayHandover;
    private bool hasGiveawayHandoverDraftPhoto;
    private bool canUploadDocumentation;
    private DateTime? borrowStartDate;
    private DateTime? expectedReturnDate;
    private DateTime? pickupDate;
    private bool isBorrowOverdue;
"""

# 2. We will add a method CalculateVariables() that populates these fields.
calc_method = """
    private void CalculateVariables()
    {
        groupItems = GroupedUsages.ContainsKey(GroupId) ? GroupedUsages[GroupId] : new List<Transaction>();
        if (!groupItems.Any()) return;

        firstItem = groupItems.First();
        borrowItemsOnly = groupItems.Where(i => i.request_type == "BORROW").ToList();
        giveawayItemsOnly = groupItems.Where(i => i.request_type == "GIVEAWAY").ToList();
        hasBorrow = borrowItemsOnly.Any();
        totalItems = hasBorrow ? borrowItemsOnly.Sum(i => i.quantity ?? 0) : groupItems.Sum(i => i.quantity ?? 0);
        totalReturned = hasBorrow ? borrowItemsOnly.Sum(i => i.returned_quantity ?? 0) : groupItems.Sum(i => i.returned_quantity ?? 0);
        allReturned = totalItems > 0 && totalReturned >= totalItems;
        isGiveaway = !hasBorrow;
        status = firstItem.status ?? "";

        isHandoverPending = hasBorrow && borrowItemsOnly.Any(i => i.status == "WAITING_HANDOVER" || i.status == "WAITING_ADMIN_HANDOVER" || i.status == "WAITING_HANDOVER_CONFIRM");
        isWaitingAdminHandover = hasBorrow && borrowItemsOnly.Any(i => i.status == "WAITING_ADMIN_HANDOVER");
        handoverDone = hasBorrow && borrowItemsOnly.Any(i => i.status == "APPROVED" && !string.IsNullOrEmpty(i.handover_photo));
        canHandover = false; 
        isWaitingUserConfirm = hasBorrow && borrowItemsOnly.Any(i =>
            (i.status == "WAITING_HANDOVER_CONFIRM" ||
             (i.status == "APPROVED" && i.handover_timestamp.HasValue && (WibHelper.Now - i.handover_timestamp.Value).TotalHours < 24)) &&
            (i.handover_uploaded_by == "SI" || string.IsNullOrEmpty(i.handover_uploaded_by)));
        hasHandoverDraftPhoto = hasBorrow && borrowItemsOnly.Any(i => i.status == "WAITING_HANDOVER" && !string.IsNullOrEmpty(i.handover_photo));
        canGiveawayHandover = false;
        hasGiveawayHandoverDraftPhoto = isGiveaway && giveawayItemsOnly.Any(i => i.status == "WAITING_HANDOVER" && !string.IsNullOrEmpty(i.handover_photo));
        canUploadDocumentation = isGiveaway && (status == "WAITING_DOCUMENTATION" || status == "DOCUMENTATION_OVERDUE") && (firstItem.requester_id == CurrentUserId || userPrincipal.HasPermission(WMS_UnitedTracors_Blazor.Helpers.Permissions.ProductsManage));
        borrowStartDate = hasBorrow
            ? borrowItemsOnly.Where(i => i.borrow_start_date.HasValue).Select(i => i.borrow_start_date).OrderBy(i => i).FirstOrDefault()
            : null;
        expectedReturnDate = hasBorrow
            ? borrowItemsOnly.Where(i => i.expected_return_date.HasValue).Select(i => i.expected_return_date).OrderByDescending(i => i).FirstOrDefault()
            : null;
        pickupDate = isGiveaway
            ? giveawayItemsOnly.Where(i => i.pickup_date.HasValue).Select(i => i.pickup_date).OrderBy(i => i).FirstOrDefault()
            : null;
        isBorrowOverdue = hasBorrow && expectedReturnDate.HasValue && (expectedReturnDate.Value.Date < DateTime.Today) && !allReturned;
    }
"""

new_lines = []
in_old_vars = False
for i, line in enumerate(lines):
    if '@{' in line and 'var firstItem = groupItems.First();' in lines[i+1]:
        in_old_vars = True
        continue
    if in_old_vars:
        if '}' in line and 'isBorrowOverdue' in lines[i-1]:
            in_old_vars = False
        continue

    # Also remove `var groupItems = GroupedUsages...` from the top of the file since we do it in CalculateVariables
    if 'var groupItems = GroupedUsages.ContainsKey(GroupId) ? GroupedUsages[GroupId] : new List<Transaction>();' in line:
        continue
    if 'if (!groupItems.Any())' in line and 'var groupItems =' in lines[i-1]:
        continue

    if line.strip() == '@code {':
        new_lines.append(line)
        new_lines.append(fields)
        new_lines.append(calc_method)
        continue

    if 'await LoadData();' in line:
        new_lines.append(line)
        if 'protected override async Task OnInitializedAsync()' in ''.join(lines[i-20:i]) or 'private async Task ResetFilters()' in ''.join(lines[i-10:i]) or 'private async void DashboardUpdated()' in ''.join(lines[i-10:i]):
            new_lines.append('        CalculateVariables();\n')
        continue

    new_lines.append(line)

with open(detail_path, 'w', encoding='utf-8') as f:
    f.writelines(new_lines)
