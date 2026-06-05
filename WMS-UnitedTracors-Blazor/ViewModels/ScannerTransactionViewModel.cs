using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace UT_WMSDotnet.ViewModels
{
    public class ScannerTransactionViewModel
    {
        [Required]
        public string Type { get; set; } = "IN";

        [Required]
        [MinLength(1, ErrorMessage = "At least one item is required.")]
        public List<ScannerItemViewModel> Items { get; set; } = new();

        public string? Notes { get; set; }

        public string? ApplicantName { get; set; }
        public string? ApplicantNrp { get; set; }
        public string? EventName { get; set; }
        public DateTime? EventDate { get; set; }
        public int? DivisionId { get; set; }
        public string? DocumentationLink { get; set; }
    }

    public class ScannerItemViewModel
    {
        [Required]
        public string Sku { get; set; } = string.Empty;

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be at least 1.")]
        public int Quantity { get; set; } = 1;
    }
}