using System;
using System.IO;
using System.Linq;

class Program
{
    static void Main()
    {
        string approvalDetailPath = @""C:\laragon\www\WMS-UnitedTractors-Blazor\WMS-UnitedTracors-Blazor\Components\Pages\ApprovalDetail.razor"";
        string trackingPath = @""C:\laragon\www\WMS-UnitedTractors-Blazor\WMS-UnitedTracors-Blazor\Components\Pages\Tracking.razor"";

        string trackingContent = File.ReadAllText(trackingPath);
        string approvalContent = File.ReadAllText(approvalDetailPath);

        // 1. Get TrackingStep and GetGroupSteps from Tracking.razor
        string trackingHelperStart = ""public class TrackingStep"";
        int startIdx = trackingContent.IndexOf(trackingHelperStart);
        int endIdx = trackingContent.IndexOf(""private async Task SubmitHandoverToAdmin"", startIdx);
        string newHelperCode = trackingContent.Substring(startIdx, endIdx - startIdx).Trim();
        newHelperCode = ""    // Timeline Helper\r\n    "" + newHelperCode;

        // Replace old helper code in ApprovalDetail.razor
        string oldHelperStart = ""public class TimelineStep"";
        int oldStartIdx = approvalContent.IndexOf(oldHelperStart);
        int oldEndIdx = approvalContent.LastIndexOf(""}"");
        
        string oldHelperFull = @""    public class TimelineStep
    {
        public string Title { get; set; } = """""";
        public string Description { get; set; } = """""";
        public string? Date { get; set; }
        public bool IsCompleted { get; set; }
        public bool IsCurrent { get; set; }
    }

    private List<TimelineStep> GetGroupSteps(List<Transaction> group)
    {
        var steps = new List<TimelineStep>();
        var firstItem = group.FirstOrDefault();
        if (firstItem == null) return steps;

        steps.Add(new TimelineStep
        {
            Title = """"Submitted"""",
            Description = $""""{firstItem.applicant_name ?? """"-""""} mengajukan permintaan"""",
            Date = firstItem.created_at.ToString(""""dd MMM yyyy""""),
            IsCompleted = true
        });

        steps.Add(new TimelineStep
        {
            Title = """"Department Approval"""",
            Description = """"Menunggu persetujuan departemen"""",
            IsCompleted = firstItem.status != """"PENDING"""",
            IsCurrent = firstItem.status == """"PENDING""""
        });

        steps.Add(new TimelineStep
        {
            Title = """"Final Processing"""",
            Description = """"Menunggu serah terima barang"""",
            IsCompleted = firstItem.status == """"APPROVED"""",
            IsCurrent = firstItem.status == """"STAGE_1_APPROVED""""
        });

        return steps;
    }"";
        approvalContent = approvalContent.Replace(oldHelperFull, newHelperCode);

        // 2. Now for the UI block
        string newUI = @""                    <div class=""""relative flex flex-col space-y-6 my-2 pl-2"""">
                        <!-- Vertical connecting line -->
                        <div class=""""absolute top-2 bottom-2 bg-gray-300 z-0"""" style=""""width: 2px; left: 15px;""""></div>

                        @{ var steps = GetGroupSteps(GroupItems); }
                        @for (int i = 0; i < steps.Count; i++)
                        {
                            var step = steps[i];
                            <div class=""""flex items-start gap-4 relative z-10"""">
                                <!-- Circle indicator wrapper -->
                                <div class=""""w-4 h-4 flex items-center justify-center shrink-0 mt-0.5 bg-white border rounded-full z-10
                                    @(step.Status == """"DONE"""" ? """"border-emerald-600"""" :
                                      step.Status == """"ACTIVE"""" ? """"border-blue-600 animate-pulse"""" :
                                      step.Status == """"WARNING"""" ? """"border-rose-600"""" :
                                      """"border-gray-300"""")"""">
                                    @if (step.Status == """"DONE"""") {
                                        <div class=""""w-full h-full rounded-full bg-[#1a7a30] flex items-center justify-center shrink-0"""">
                                            <svg class=""""w-2.5 h-2.5 text-white"""" fill=""""none"""" stroke=""""currentColor"""" viewBox=""""0 0 24 24""""><path stroke-linecap=""""round"""" stroke-linejoin=""""round"""" stroke-width=""""3"""" d=""""M5 13l4 4L19 7""""/></svg>
                                        </div>
                                    } else if (step.Status == """"ACTIVE"""") {
                                        <span class=""""relative flex h-2 w-2 items-center justify-center"""">
                                            <span class=""""animate-ping absolute inline-flex h-full w-full rounded-full bg-blue-400 opacity-75""""></span>
                                            <span class=""""relative inline-flex rounded-full h-2 w-2 bg-blue-600""""></span>
                                        </span>
                                    } else if (step.Status == """"WARNING"""") {
                                        <div class=""""w-full h-full rounded-full bg-[#d94040] flex items-center justify-center shrink-0"""">
                                            <svg class=""""w-2.5 h-2.5 text-white"""" fill=""""none"""" stroke=""""currentColor"""" viewBox=""""0 0 24 24""""><path stroke-linecap=""""round"""" stroke-linejoin=""""round"""" stroke-width=""""3"""" d=""""M12 9v2m0 4h.01m-6.938 4h13.856c1.54 0 2.502-1.667 1.732-3L13.732 4c-.77-1.333-2.694-1.333-3.464 0L3.34 16c-.77 1.333.192 3 1.732 3z""""/></svg>
                                        </div>
                                    } else {
                                        <div class=""""w-1.5 h-1.5 rounded-full bg-gray-300""""></div>
                                    }
                                </div>
                                <!-- Text Info -->
                                <div class=""""flex flex-col min-w-0"""">
                                    <span class=""""text-xs font-bold @(step.Status == """"DONE"""" ? """"text-[#1a7a30]"""" : step.Status == """"ACTIVE"""" ? """"text-blue-600"""" : step.Status == """"WARNING"""" ? """"text-[#d94040]"""" : """"text-gray-500"""")"""">@step.Name</span>
                                    <span class=""""text-[10px] text-gray-400 leading-tight mt-0.5"""">
                                        @if (step.Status == """"DONE"""" && step.CompletedAt.HasValue) {
                                            <span>Selesai: @step.CompletedAt.Value.ToString(""""dd MMM HH:mm"""")</span>
                                        } else if (step.Status == """"DONE"""") {
                                            <span>Selesai dilakukan</span>
                                        } else if (step.Status == """"ACTIVE"""") {
                                            <span>Sedang diproses</span>
                                        } else if (step.Status == """"WARNING"""") {
                                            <span>Butuh perhatian / ditolak</span>
                                        } else {
                                            <span>Menunggu giliran</span>
                                        }
                                    </span>
                                </div>
                            </div>
                        }
                    </div>"";

        string oldUIStart = @""                    <div class=""""space-y-4"""">
                        @{
                            var steps = GetGroupSteps(GroupItems);"";
                            
        int uiReplaceStartIdx = approvalContent.IndexOf(oldUIStart);
        int uiReplaceEndIdx = approvalContent.IndexOf(""</div>\r\n                </div>\r\n\r\n                <!-- Items List -->"");
        
        if(uiReplaceStartIdx == -1) { Console.WriteLine(""Couldn't find UI start.""); return; }
        if(uiReplaceEndIdx == -1) { Console.WriteLine(""Couldn't find UI end.""); return; }
        
        string oldUIFull = approvalContent.Substring(uiReplaceStartIdx, uiReplaceEndIdx - uiReplaceStartIdx);
        
        approvalContent = approvalContent.Replace(oldUIFull, newUI);

        File.WriteAllText(approvalDetailPath, approvalContent);
        Console.WriteLine(""Replaced timeline successfully!"");
    }
}
