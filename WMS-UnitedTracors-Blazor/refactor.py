import os

tracking_path = r'C:\laragon\www\WMS-UnitedTractors-Blazor\WMS-UnitedTracors-Blazor\Components\Pages\Tracking.razor'
detail_path = r'C:\laragon\www\WMS-UnitedTractors-Blazor\WMS-UnitedTracors-Blazor\Components\Pages\TrackingDetail.razor'

with open(tracking_path, 'r', encoding='utf-8') as f:
    t_lines = f.readlines()

with open(detail_path, 'r', encoding='utf-8') as f:
    d_lines = f.readlines()

######################################################################
# 1. Modify Tracking.razor
######################################################################
t_new = []
in_expanded = False
in_modals = False
for i, line in enumerate(t_lines):
    if '@if(isExpanded)' in line.replace(' ', ''):
        in_expanded = True
        continue
    
    if in_expanded:
        if '                        }' in line and 1150 <= i <= 1160:
            in_expanded = False
        continue

    if '<div @onclick="() => ToggleGroup(groupId)"' in line:
        line = line.replace('ToggleGroup(groupId)', 'Navigation.NavigateTo($"/tracking-detail/{groupId}")')
    
    if 'rotate-180' in line and 'isExpanded' in line:
        line = '                                    <svg class="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 5l7 7-7 7"/></svg>\n'

    # Modals begin around line 1160
    if '<Modal Show="IsHandoverModalOpen"' in line:
        in_modals = True
    
    if in_modals:
        if '</Modal>' in line and i > 1400: # The last modal closes around line 1425
             # Let's be safe and check if it's the Revision modal
             # We just strip ALL modals from the end of HTML
             pass
        # Wait, the code block is at line 1427. So any Modal between 1160 and 1427
        if i >= 1426: # This is where @code starts
            in_modals = False
            t_new.append(line)
        continue

    # Also we should remove methods from @code that are no longer needed, 
    # but for now leaving them is harmless. We'll strip what we can.

    t_new.append(line)

with open(tracking_path, 'w', encoding='utf-8') as f:
    f.writelines(t_new)

print('Done Tracking.razor')

######################################################################
# 2. Modify TrackingDetail.razor
######################################################################
d_new = []
in_filter = False
in_loop = False
in_list_render = False
skip_header = False

for i, line in enumerate(d_lines):
    # Change route
    if '@page "/Tracking"' in line:
        line = '@page "/tracking-detail/{GroupId}"\n'

    # Remove filters (from <div class="relative bg-white rounded-xl... mb-4 ...> down to the </div> of filters)
    if '<div class="relative bg-white rounded-xl border border-[#e5e3dc] mb-4' in line:
        in_filter = True
    if in_filter:
        if '        <div class="mb-4 bg-white rounded-xl border border-[#e5e3dc] overflow-hidden shadow-sm">' in line:
            in_filter = False # Stopped being in filter, now in tabs
            pass # Keep it removed as well? Yes, we don't need tabs on detail page.
        elif '@if(GroupedUsages == null)' in line:
            in_filter = False # End of all filters and tabs
    if in_filter:
        continue
    
    # We want to remove the tabs as well:
    if '<div class="mb-4 bg-white rounded-xl border border-[#e5e3dc] overflow-hidden shadow-sm">' in line:
        in_filter = True
        continue

    if '            var visibleGroups = GetVisibleTrackingGroups().ToList();' in line:
        # Instead of visibleGroups, just use the matching group
        d_new.append('            var groupItems = GroupedUsages.ContainsKey(GroupId) ? GroupedUsages[GroupId] : new List<Transaction>();\n')
        d_new.append('            if (!groupItems.Any())\n')
        continue

    if '            @if (!visibleGroups.Any())' in line:
        # Replaced logic above
        continue
    
    if '@foreach(var group in visibleGroups)' in line:
        in_loop = True
        continue
    
    if in_loop:
        if '                    var groupId = group.Key;' in line:
            d_new.append('                    var groupId = GroupId;\n')
            continue
        if '                    var groupItems = group.Value;' in line:
            # We already have groupItems
            continue
        
        # Remove the accordion wrapper and header
        if '<div class="bg-white rounded-xl border overflow-hidden shadow-sm transition-all duration-200' in line:
            skip_header = True
            # Add a back button and title instead
            d_new.append('        <button @onclick=\'() => Navigation.NavigateTo("/Tracking")\' class="mb-4 flex items-center gap-2 text-gray-500 hover:text-[#1a1a1a] transition font-bold text-sm">\n')
            d_new.append('            <svg class="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M10 19l-7-7m0 0l7-7m-7 7h18"/></svg>\n')
            d_new.append('            Kembali ke Tracking\n')
            d_new.append('        </button>\n')
            d_new.append('\n')
            d_new.append('        <div class="bg-white rounded-xl border border-[#e5e3dc] overflow-hidden shadow-sm">\n')
            continue
        
        if skip_header:
            if '@if(isExpanded)' in line.replace(' ', ''):
                skip_header = False
                continue
            continue
        
        if '                            <div class="border-t border-[#e5e3dc] bg-[#faf9f5] p-5">' in line:
            line = line.replace('border-t ', '')
        
        # Remove the closing bracket of if(isExpanded)
        if '                        }' in line and 1150 <= i <= 1160:
            continue
        
        # Remove the closing bracket of foreach
        if '                }' in line and 1152 <= i <= 1162:
            in_loop = False
            continue

    # Code modifications
    if '@code {' in line:
        d_new.append(line)
        d_new.append('    [Parameter]\n    public string GroupId { get; set; } = "";\n')
        continue

    d_new.append(line)

with open(detail_path, 'w', encoding='utf-8') as f:
    f.writelines(d_new)

print('Done TrackingDetail.razor')
