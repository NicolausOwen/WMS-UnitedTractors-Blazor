import os

path = r'C:\laragon\www\WMS-UnitedTractors-Blazor\WMS-UnitedTracors-Blazor\Components\Pages\TrackingDetail.razor'
with open(path, 'r', encoding='utf-8') as f:
    lines = f.readlines()

new_lines = []
for i, line in enumerate(lines):
    if i == 41:
        new_lines.append('                <svg class="w-5 h-5 shrink-0" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 12l2 2 4-4m6 2a9 9 0 11-18 0 9 9 0 0118 0z"/></svg>\n')
        new_lines.append('                <p class="text-sm font-semibold">@SuccessMessage</p>\n')
        new_lines.append('            </div>\n')
        new_lines.append('        }\n\n')
        new_lines.append('        @if (!groupItems.Any())\n')
        new_lines.append('        {\n')
        new_lines.append('            <div class="text-center py-8 text-[#8a8880]">Data tracking tidak ditemukan.</div>\n')
        new_lines.append('        }\n')
        new_lines.append('        else\n')
        new_lines.append('        {\n')
        new_lines.append('        <button @onclick=\'() => Navigation.NavigateTo("/Tracking")\' class="mb-4 flex items-center gap-2 text-gray-500 hover:text-[#1a1a1a] transition font-bold text-sm">\n')
        new_lines.append('            <svg class="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M10 19l-7-7m0 0l7-7m-7 7h18"/></svg>\n')
        new_lines.append('            Kembali ke Tracking\n')
        new_lines.append('        </button>\n')
        new_lines.append('        <div class="bg-white rounded-xl border border-[#e5e3dc] overflow-hidden shadow-sm">\n')
        new_lines.append('                            <div class="bg-[#faf9f5] p-5">\n')
        new_lines.append('                                <div class="grid grid-cols-1 lg:grid-cols-4 gap-6">\n')

    if i >= 41 and i < 43:
        pass # Skip these 2 lines which were the original ones we replaced
    elif i >= 43:
        new_lines.append(line)
    elif i < 41:
        new_lines.append(line)

with open(path, 'w', encoding='utf-8') as f:
    f.writelines(new_lines)
