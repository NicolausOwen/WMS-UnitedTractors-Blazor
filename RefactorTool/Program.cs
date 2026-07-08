using System;
using System.IO;

namespace RefactorTool
{
    class Program
    {
        static void Main(string[] args)
        {
            string path = @"C:\laragon\www\WMS-UnitedTractors-Blazor\WMS-UnitedTracors-Blazor\Components\Pages\ApprovalDashboard.razor";
            string content = File.ReadAllText(path);

            // 1. Remove split-container classes
            content = content.Replace("<div class=\"split-container mb-12\">", "<div class=\"mb-12\">");
            content = content.Replace("<div class=\"split-left bg-white", "<div class=\"bg-white");

            // 2. Change onclick to Navigation.NavigateTo
            string oldOnclick = "<div @onclick=\"() => SelectedGroupId = group.Key\"";
            string newOnclick = "<div @onclick=\"@(() => Navigation.NavigateTo($\\\"/approval-detail/{group.Key}\\\"))\"";
            content = content.Replace(oldOnclick, newOnclick);
            
            // 3. Remove border logic (borderClass) because we are no longer tracking selection.
            // Replace the @borderClass part with our new class
            // var borderClass = isBorrow ... (about 3 lines) -> remove this
            int borderClassStart = content.IndexOf("var borderClass = isBorrow");
            if (borderClassStart != -1)
            {
                int nextDivStart = content.IndexOf("<div @onclick", borderClassStart);
                if (nextDivStart != -1)
                {
                    content = content.Remove(borderClassStart, nextDivStart - borderClassStart);
                }
            }

            // Also replace @borderClass in the div class string
            content = content.Replace("relative @borderClass", "relative border-[#e5e3dc] hover:border-[#1a6b8a]/50 hover:bg-[#eef6fd]/20");

            // 4. Remove the Right Pane and Batch Action Bar
            int rightPaneStart = content.IndexOf("<!-- Right Pane: Request Details -->");
            if (rightPaneStart != -1)
            {
                // We want to delete up to the closing brackets of the if-else block
                // Let's find "else if (ActiveTab == \"handovers\")"
                int endTarget = content.IndexOf("else if (ActiveTab == \"handovers\")");
                if (endTarget != -1)
                {
                    // Now find the `}` that precedes this `else if`
                    int lastBraceBeforeElseIf = content.LastIndexOf("}", endTarget);
                    int secondToLastBrace = content.LastIndexOf("}", lastBraceBeforeElseIf - 1);
                    int thirdToLastBrace = content.LastIndexOf("}", secondToLastBrace - 1);
                    
                    // The original structure is:
                    //                 }
                    //             }
                    //         }
                    //         else if (ActiveTab == "handovers")
                    
                    // If we delete from RightPaneStart up to just before thirdToLastBrace, we leave the closing braces intact!
                    content = content.Remove(rightPaneStart, thirdToLastBrace - rightPaneStart);
                }
            }

            File.WriteAllText(path, content);
            Console.WriteLine("Refactored ApprovalDashboard.razor successfully!");
        }
    }
}
