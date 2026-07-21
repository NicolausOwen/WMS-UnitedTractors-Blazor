import os

tracking_path = r'C:\laragon\www\WMS-UnitedTractors-Blazor\WMS-UnitedTracors-Blazor\Components\Pages\Tracking.razor'
detail_path = r'C:\laragon\www\WMS-UnitedTractors-Blazor\WMS-UnitedTracors-Blazor\Components\Pages\TrackingDetail.razor'

# Fix Tracking.razor quotes
with open(tracking_path, 'r', encoding='utf-8') as f:
    t_lines = f.readlines()

for i, line in enumerate(t_lines):
    if '<div @onclick="() => Navigation.NavigateTo($"/tracking-detail/{groupId}")"' in line:
        t_lines[i] = line.replace('<div @onclick="() => Navigation.NavigateTo($"/tracking-detail/{groupId}")"', '<div @onclick=\'() => Navigation.NavigateTo($"/tracking-detail/{groupId}")\'')

with open(tracking_path, 'w', encoding='utf-8') as f:
    f.writelines(t_lines)

# Fix TrackingDetail.razor DateTime.Today and groupId
with open(detail_path, 'r', encoding='utf-8') as f:
    d_lines = f.readlines()

for i, line in enumerate(d_lines):
    if '< DateTime.Today' in line:
        d_lines[i] = line.replace('expectedReturnDate.Value.Date < DateTime.Today', '(expectedReturnDate.Value.Date < DateTime.Today)')
    # Also fix groupId to GroupId in the file where missing
    if 'ExpandedGroupIds.Contains(groupId)' in line:
        d_lines[i] = line.replace('groupId', 'GroupId')
    if 'ToggleShowAllProducts(groupId)' in line:
        d_lines[i] = line.replace('groupId', 'GroupId')

with open(detail_path, 'w', encoding='utf-8') as f:
    f.writelines(d_lines)
