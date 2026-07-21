import os

path = r'C:\laragon\www\WMS-UnitedTractors-Blazor\WMS-UnitedTracors-Blazor\Components\Pages\Tracking.razor'
with open(path, 'r', encoding='utf-8') as f:
    text = f.read()

# Replace the single quoted string with proper Blazor Razor syntax
old_str = """<div @onclick='() => Navigation.NavigateTo($"/tracking-detail/{groupId}")'"""
new_str = """<div @onclick='@(() => Navigation.NavigateTo($"/tracking-detail/{groupId}"))'"""
text = text.replace(old_str, new_str)

with open(path, 'w', encoding='utf-8') as f:
    f.write(text)
