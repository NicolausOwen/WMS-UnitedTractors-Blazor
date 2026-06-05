using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System.IO;
using System.Linq;
using UT_WMSDotnet.Models;
using System;
using System.Collections.Generic;

namespace WMS_UnitedTracors_Blazor.Services
{
    public class PdfReportService
    {
        public PdfReportService()
        {
            QuestPDF.Settings.License = LicenseType.Community;
        }

        public byte[] GeneratePeminjamanReport(List<Transaction> transactions, DateTime startDate, DateTime endDate, string wwwrootPath)
        {
            var statuses = new[] { "APPROVED", "PENDING", "REJECTED" };
            
            var document = Document.Create(container =>
            {
                var totalTransactions = transactions.Count;
                if (totalTransactions == 0)
                {
                    GenerateEmptyReport(container, "Laporan Peminjaman Barang", startDate, endDate, wwwrootPath);
                    return;
                }

                foreach (var status in statuses)
                {
                    var statusTransactions = transactions.Where(t => t.status == status).ToList();
                    if (statusTransactions.Count == 0) continue;

                    container.Page(page =>
                    {
                        page.Size(PageSizes.A4.Landscape());
                        page.Margin(1.5f, QuestPDF.Infrastructure.Unit.Centimetre);
                        page.PageColor(Colors.White);
                        page.DefaultTextStyle(x => x.FontSize(9).FontFamily(Fonts.Arial).LineHeight(1.3f));

                        page.Header().Element(c => ComposeHeader(c, "Laporan Peminjaman Barang", wwwrootPath));
                        
                        page.Content().PaddingVertical(10).Column(column =>
                        {
                            column.Item().AlignCenter().Text($"Periode: {startDate:dd MMM yyyy} - {endDate:dd MMM yyyy}").FontSize(8).FontColor(Colors.Grey.Darken1);
                            column.Item().PaddingBottom(8);

                            column.Item().Background(Colors.Grey.Lighten4).BorderLeft(3).BorderColor(Colors.Blue.Medium).Padding(4).Text($"Status: {status} | Total Transaksi: {statusTransactions.Count} | Total Qty: {statusTransactions.Sum(t => t.quantity)}").FontSize(8).FontColor(Colors.Grey.Darken3).SemiBold();
                            
                            column.Item().PaddingTop(10).PaddingBottom(4).BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Text($"Daftar Transaksi - {status}").FontSize(10).SemiBold().FontColor(Colors.Grey.Darken4);

                            column.Item().Table(table =>
                            {
                                table.ColumnsDefinition(columns =>
                                {
                                    columns.ConstantColumn(30); // No
                                    columns.RelativeColumn(2); // Tanggal
                                    columns.RelativeColumn(4); // Produk
                                    columns.RelativeColumn(3); // SKU
                                    columns.ConstantColumn(30); // Qty
                                    columns.RelativeColumn(4); // Info Pengembalian
                                    columns.RelativeColumn(2); // Kondisi
                                    columns.RelativeColumn(3); // Pemohon
                                    columns.RelativeColumn(2); // Divisi
                                    columns.RelativeColumn(3); // Event
                                    columns.RelativeColumn(2); // Status
                                });

                                table.Header(header =>
                                {
                                    header.Cell().Element(CellStyle).AlignCenter().Text("No");
                                    header.Cell().Element(CellStyle).Text("Tanggal");
                                    header.Cell().Element(CellStyle).Text("Produk");
                                    header.Cell().Element(CellStyle).Text("SKU");
                                    header.Cell().Element(CellStyle).AlignCenter().Text("Qty");
                                    header.Cell().Element(CellStyle).Text("Keterangan Pengembalian");
                                    header.Cell().Element(CellStyle).Text("Kondisi");
                                    header.Cell().Element(CellStyle).Text("Pemohon");
                                    header.Cell().Element(CellStyle).Text("Divisi");
                                    header.Cell().Element(CellStyle).Text("Event");
                                    header.Cell().Element(CellStyle).Text("Status");

                                    static IContainer CellStyle(IContainer container)
                                    {
                                        return container.DefaultTextStyle(x => x.SemiBold().FontColor(Colors.White)).PaddingVertical(6).PaddingHorizontal(4).Border(1).BorderColor(Colors.Grey.Darken3).Background(Colors.Grey.Darken3);
                                    }
                                });

                                int i = 1;
                                foreach (var item in statusTransactions)
                                {
                                    var isEven = i % 2 == 0;
                                    var bgColor = isEven ? Colors.Grey.Lighten5 : Colors.White;

                                    table.Cell().Element(c => CellStyle(c, bgColor)).AlignCenter().Text(i.ToString());
                                    table.Cell().Element(c => CellStyle(c, bgColor)).Text(item.created_at.ToString("dd/MM/yyyy"));
                                    table.Cell().Element(c => CellStyle(c, bgColor)).Text(item.Product?.name ?? "-");
                                    table.Cell().Element(c => CellStyle(c, bgColor)).Text(item.Product?.sku ?? "-");
                                    table.Cell().Element(c => CellStyle(c, bgColor)).AlignCenter().Text(item.quantity?.ToString() ?? "-");
                                    
                                    var returnInfo = "-";
                                    if (item.status != "REJECTED")
                                    {
                                        if (item.returned_quantity >= item.quantity) returnInfo = "Sudah Dikembalikan Semuanya";
                                        else if (item.returned_quantity > 0) returnInfo = $"Dikembalikan Sebagian ({item.returned_quantity} dari {item.quantity})";
                                        else returnInfo = "Belum Dikembalikan";
                                    }
                                    
                                    table.Cell().Element(c => CellStyle(c, bgColor)).Text(returnInfo);
                                    table.Cell().Element(c => CellStyle(c, bgColor)).Text(item.status == "REJECTED" ? "-" : (item.return_condition ?? "-"));
                                    table.Cell().Element(c => CellStyle(c, bgColor)).Text(item.applicant_name ?? "-");
                                    table.Cell().Element(c => CellStyle(c, bgColor)).Text(item.Division?.name ?? "-");
                                    table.Cell().Element(c => CellStyle(c, bgColor)).Text(item.event_name ?? "-");
                                    table.Cell().Element(c => CellStyle(c, bgColor)).Text(item.status ?? "-");

                                    i++;
                                }
                                
                                static IContainer CellStyle(IContainer container, string bgColor)
                                {
                                    return container.Border(1).BorderColor(Colors.Grey.Lighten2).Background(bgColor).PaddingVertical(5).PaddingHorizontal(4);
                                }
                            });
                        });

                        page.Footer().Element(c => ComposeFooter(c, wwwrootPath));
                    });
                }
            });

            return document.GeneratePdf();
        }

        public byte[] GenerateRequestReport(List<Transaction> transactions, DateTime startDate, DateTime endDate, string wwwrootPath)
        {
            var statuses = new[] { "APPROVED", "PENDING", "REJECTED" };
            
            var document = Document.Create(container =>
            {
                var totalTransactions = transactions.Count;
                if (totalTransactions == 0)
                {
                    GenerateEmptyReport(container, "Laporan Request Barang (Giveaway)", startDate, endDate, wwwrootPath);
                    return;
                }

                foreach (var status in statuses)
                {
                    var statusTransactions = transactions.Where(t => status == "PENDING" ? (t.status == "PENDING" || t.status == "PENDING_MANAGER") : t.status == status).ToList();
                    if (statusTransactions.Count == 0) continue;

                    container.Page(page =>
                    {
                        page.Size(PageSizes.A4.Landscape());
                        page.Margin(1.5f, QuestPDF.Infrastructure.Unit.Centimetre);
                        page.PageColor(Colors.White);
                        page.DefaultTextStyle(x => x.FontSize(9).FontFamily(Fonts.Arial).LineHeight(1.3f));

                        page.Header().Element(c => ComposeHeader(c, "Laporan Request Barang (Giveaway)", wwwrootPath));
                        
                        page.Content().PaddingVertical(10).Column(column =>
                        {
                            column.Item().AlignCenter().Text($"Periode: {startDate:dd MMM yyyy} - {endDate:dd MMM yyyy}").FontSize(8).FontColor(Colors.Grey.Darken1);
                            column.Item().PaddingBottom(8);

                            column.Item().Background(Colors.Grey.Lighten4).BorderLeft(3).BorderColor(Colors.Blue.Medium).Padding(4).Text($"Status: {status} | Total Transaksi: {statusTransactions.Count} | Total Qty: {statusTransactions.Sum(t => t.quantity)}").FontSize(8).FontColor(Colors.Grey.Darken3).SemiBold();
                            
                            column.Item().PaddingTop(10).PaddingBottom(4).BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Text($"Daftar Transaksi - {status}").FontSize(10).SemiBold().FontColor(Colors.Grey.Darken4);

                            column.Item().Table(table =>
                            {
                                table.ColumnsDefinition(columns =>
                                {
                                    columns.ConstantColumn(30); // No
                                    columns.RelativeColumn(3); // Tanggal
                                    columns.RelativeColumn(6); // Produk
                                    columns.RelativeColumn(4); // SKU
                                    columns.ConstantColumn(30); // Qty
                                    columns.RelativeColumn(4); // Pemohon
                                    columns.RelativeColumn(3); // Divisi
                                    columns.RelativeColumn(3); // Persetujuan
                                });

                                table.Header(header =>
                                {
                                    header.Cell().Element(CellStyle).AlignCenter().Text("No");
                                    header.Cell().Element(CellStyle).Text("Tanggal");
                                    header.Cell().Element(CellStyle).Text("Produk");
                                    header.Cell().Element(CellStyle).Text("SKU");
                                    header.Cell().Element(CellStyle).AlignCenter().Text("Qty");
                                    header.Cell().Element(CellStyle).Text("Pemohon");
                                    header.Cell().Element(CellStyle).Text("Divisi");
                                    header.Cell().Element(CellStyle).Text("Persetujuan");

                                    static IContainer CellStyle(IContainer container)
                                    {
                                        return container.DefaultTextStyle(x => x.SemiBold().FontColor(Colors.White)).PaddingVertical(6).PaddingHorizontal(4).Border(1).BorderColor(Colors.Grey.Darken3).Background(Colors.Grey.Darken3);
                                    }
                                });

                                int i = 1;
                                foreach (var item in statusTransactions)
                                {
                                    var isEven = i % 2 == 0;
                                    var bgColor = isEven ? Colors.Grey.Lighten5 : Colors.White;

                                    table.Cell().Element(c => CellStyle(c, bgColor)).AlignCenter().Text(i.ToString());
                                    table.Cell().Element(c => CellStyle(c, bgColor)).Text(item.created_at.ToString("dd/MM/yyyy"));
                                    table.Cell().Element(c => CellStyle(c, bgColor)).Text(item.Product?.name ?? "-");
                                    table.Cell().Element(c => CellStyle(c, bgColor)).Text(item.Product?.sku ?? "-");
                                    table.Cell().Element(c => CellStyle(c, bgColor)).AlignCenter().Text(item.quantity?.ToString() ?? "-");
                                    table.Cell().Element(c => CellStyle(c, bgColor)).Text(item.applicant_name ?? "-");
                                    table.Cell().Element(c => CellStyle(c, bgColor)).Text(item.Division?.name ?? "-");
                                    
                                    var approvalText = "";
                                    if(item.status == "APPROVED") approvalText = $"Disetujui oleh: {item.Approver?.name ?? "Admin"}";
                                    else if(item.status == "PENDING" || item.status == "PENDING_MANAGER") approvalText = "Menunggu Persetujuan";
                                    else approvalText = $"Ditolak. Alasan: {item.rejection_reason}";
                                    
                                    table.Cell().Element(c => CellStyle(c, bgColor)).Text(approvalText);

                                    i++;
                                }
                                
                                static IContainer CellStyle(IContainer container, string bgColor)
                                {
                                    return container.Border(1).BorderColor(Colors.Grey.Lighten2).Background(bgColor).PaddingVertical(5).PaddingHorizontal(4);
                                }
                            });
                        });

                        page.Footer().Element(c => ComposeFooter(c, wwwrootPath));
                    });
                }
            });

            return document.GeneratePdf();
        }

        public byte[] GenerateAdjustReport(List<Transaction> transactions, DateTime startDate, DateTime endDate, string wwwrootPath)
        {
            var types = new[] { "IN", "OUT" };
            
            var document = Document.Create(container =>
            {
                var totalTransactions = transactions.Count;
                if (totalTransactions == 0)
                {
                    GenerateEmptyReport(container, "Laporan Update Stok", startDate, endDate, wwwrootPath);
                    return;
                }

                foreach (var type in types)
                {
                    var typeTransactions = transactions.Where(t => t.type == type).ToList();
                    if (typeTransactions.Count == 0) continue;

                    container.Page(page =>
                    {
                        page.Size(PageSizes.A4.Landscape());
                        page.Margin(1.5f, QuestPDF.Infrastructure.Unit.Centimetre);
                        page.PageColor(Colors.White);
                        page.DefaultTextStyle(x => x.FontSize(9).FontFamily(Fonts.Arial).LineHeight(1.3f));

                        page.Header().Element(c => ComposeHeader(c, "Laporan Update Stok", wwwrootPath));
                        
                        page.Content().PaddingVertical(10).Column(column =>
                        {
                            column.Item().AlignCenter().Text($"Periode: {startDate:dd MMM yyyy} - {endDate:dd MMM yyyy}").FontSize(8).FontColor(Colors.Grey.Darken1);
                            column.Item().PaddingBottom(8);

                            column.Item().Background(Colors.Grey.Lighten4).BorderLeft(3).BorderColor(Colors.Blue.Medium).Padding(4).Text($"Tipe Update: {(type == "IN" ? "STOCK IN" : "STOCK OUT")} | Total Transaksi: {typeTransactions.Count} | Total Qty: {typeTransactions.Sum(t => t.quantity)}").FontSize(8).FontColor(Colors.Grey.Darken3).SemiBold();
                            
                            column.Item().PaddingTop(10).PaddingBottom(4).BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Text($"Daftar Penyesuaian - {type}").FontSize(10).SemiBold().FontColor(Colors.Grey.Darken4);

                            column.Item().Table(table =>
                            {
                                table.ColumnsDefinition(columns =>
                                {
                                    columns.ConstantColumn(30); // No
                                    columns.RelativeColumn(3); // Tanggal
                                    columns.RelativeColumn(6); // Produk
                                    columns.RelativeColumn(4); // SKU
                                    columns.RelativeColumn(3); // Tipe
                                    columns.ConstantColumn(40); // Qty
                                    columns.RelativeColumn(5); // Catatan
                                });

                                table.Header(header =>
                                {
                                    header.Cell().Element(CellStyle).AlignCenter().Text("No");
                                    header.Cell().Element(CellStyle).Text("Tanggal");
                                    header.Cell().Element(CellStyle).Text("Produk");
                                    header.Cell().Element(CellStyle).Text("SKU");
                                    header.Cell().Element(CellStyle).Text("Tipe");
                                    header.Cell().Element(CellStyle).AlignCenter().Text("Qty");
                                    header.Cell().Element(CellStyle).Text("Catatan");

                                    static IContainer CellStyle(IContainer container)
                                    {
                                        return container.DefaultTextStyle(x => x.SemiBold().FontColor(Colors.White)).PaddingVertical(6).PaddingHorizontal(4).Border(1).BorderColor(Colors.Grey.Darken3).Background(Colors.Grey.Darken3);
                                    }
                                });

                                int i = 1;
                                foreach (var item in typeTransactions)
                                {
                                    var isEven = i % 2 == 0;
                                    var bgColor = isEven ? Colors.Grey.Lighten5 : Colors.White;

                                    table.Cell().Element(c => CellStyle(c, bgColor)).AlignCenter().Text(i.ToString());
                                    table.Cell().Element(c => CellStyle(c, bgColor)).Text(item.created_at.ToString("dd/MM/yyyy HH:mm"));
                                    table.Cell().Element(c => CellStyle(c, bgColor)).Text(item.Product?.name ?? "-");
                                    table.Cell().Element(c => CellStyle(c, bgColor)).Text(item.Product?.sku ?? "-");
                                    table.Cell().Element(c => CellStyle(c, bgColor)).Text(item.type == "IN" ? "STOCK IN" : "STOCK OUT");
                                    table.Cell().Element(c => CellStyle(c, bgColor)).AlignCenter().Text((item.type == "IN" ? "+" : "-") + (item.quantity?.ToString() ?? "-"));
                                    table.Cell().Element(c => CellStyle(c, bgColor)).Text(item.notes ?? "-");

                                    i++;
                                }
                                
                                static IContainer CellStyle(IContainer container, string bgColor)
                                {
                                    return container.Border(1).BorderColor(Colors.Grey.Lighten2).Background(bgColor).PaddingVertical(5).PaddingHorizontal(4);
                                }
                            });
                        });

                        page.Footer().Element(c => ComposeFooter(c, wwwrootPath));
                    });
                }
            });

            return document.GeneratePdf();
        }

        public byte[] GenerateRequestReceipt(List<Transaction> transactions, string wwwrootPath)
        {
            return GenerateGenericReceipt(transactions, "Bukti Request Barang", wwwrootPath, Colors.Purple.Medium);
        }

        public byte[] GenerateApprovalReceipt(List<Transaction> transactions, string wwwrootPath)
        {
            return GenerateGenericReceipt(transactions, "Bukti Approval Barang", wwwrootPath, Colors.Green.Medium);
        }

        public byte[] GenerateHandoverReceipt(List<Transaction> transactions, string wwwrootPath)
        {
            return GenerateGenericReceipt(transactions, "Bukti Serah Terima Barang", wwwrootPath, Colors.Orange.Medium);
        }

        public byte[] GenerateReturnReceipt(List<Transaction> transactions, string wwwrootPath)
        {
            var firstItem = transactions.FirstOrDefault();
            if (firstItem == null) return Array.Empty<byte>();

            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4.Landscape());
                    page.Margin(20);
                    page.PageColor(Colors.White);
                    page.DefaultTextStyle(x => x.FontSize(11).FontFamily(Fonts.Arial).LineHeight(1.3f).FontColor(Colors.Grey.Darken4));

                    page.Header().Element(c => 
                    {
                        var utLogo = Path.Combine(wwwrootPath, "img", "ut.png");
                        var corpuLogo = Path.Combine(wwwrootPath, "img", "corpu.png");

                        c.BorderBottom(2).BorderColor(Colors.Green.Medium).PaddingBottom(8).Row(row =>
                        {
                            row.ConstantItem(120).AlignLeft().Height(35).Image(File.Exists(utLogo) ? utLogo : Path.Combine(wwwrootPath, "img", "logo.svg"));
                            row.RelativeItem().AlignCenter().Column(col => 
                            {
                                col.Item().Text("Bukti Pengembalian Barang").FontSize(20).SemiBold().FontColor(Colors.Green.Medium);
                                col.Item().Text("Warehouse Management System - United Tractors").FontSize(10).FontColor(Colors.Grey.Medium);
                            });
                            row.ConstantItem(120).AlignRight().Height(35).Image(File.Exists(corpuLogo) ? corpuLogo : Path.Combine(wwwrootPath, "img", "logo.svg"));
                        });
                    });

                    page.Content().PaddingVertical(15).Column(column =>
                    {
                        column.Item().Table(table => 
                        {
                            table.ColumnsDefinition(cols => 
                            {
                                cols.ConstantColumn(140);
                                cols.RelativeColumn();
                                cols.ConstantColumn(140);
                                cols.RelativeColumn();
                            });
                            
                            table.Cell().Text("Nama Pemohon").SemiBold();
                            table.Cell().Text($": {firstItem.applicant_name ?? firstItem.Requester?.name}");
                            
                            table.Cell().Text("Tanggal Pinjam").SemiBold();
                            table.Cell().Text($": {firstItem.created_at:dd MMM yyyy}");
                            
                            table.Cell().Text("NRP Pemohon").SemiBold();
                            table.Cell().Text($": {firstItem.applicant_nrp ?? "-"}");
                            
                            table.Cell().Text("Divisi").SemiBold();
                            table.Cell().Text($": {firstItem.Division?.name ?? "-"}");
                        });

                        column.Item().PaddingTop(15).Table(table =>
                        {
                            table.ColumnsDefinition(cols =>
                            {
                                cols.ConstantColumn(30); // No
                                cols.RelativeColumn(3); // Nama Barang
                                cols.ConstantColumn(60); // Qty Pinjam
                                cols.ConstantColumn(60); // Qty Kembali
                                cols.RelativeColumn(2); // Kondisi
                                cols.RelativeColumn(3); // Tanggal Kembali
                            });

                            table.Header(header =>
                            {
                                header.Cell().Element(CellStyle).AlignCenter().Text("No");
                                header.Cell().Element(CellStyle).Text("Nama Barang");
                                header.Cell().Element(CellStyle).AlignCenter().Text("Qty Pinjam");
                                header.Cell().Element(CellStyle).AlignCenter().Text("Qty Kembali");
                                header.Cell().Element(CellStyle).AlignCenter().Text("Kondisi");
                                header.Cell().Element(CellStyle).AlignCenter().Text("Tanggal Kembali");

                                static IContainer CellStyle(IContainer container)
                                {
                                    return container.Background(Colors.Grey.Lighten4).Border(1).BorderColor(Colors.Grey.Lighten2).Padding(5).DefaultTextStyle(x => x.FontSize(10).SemiBold());
                                }
                            });

                            int index = 1;
                            foreach (var item in transactions)
                            {
                                table.Cell().Element(CellStyle).AlignCenter().Text(index.ToString());
                                table.Cell().Element(CellStyle).Text(item.Product?.name ?? "-");
                                table.Cell().Element(CellStyle).AlignCenter().Text(item.quantity?.ToString());
                                table.Cell().Element(CellStyle).AlignCenter().Text(item.returned_quantity?.ToString()).SemiBold().FontColor(Colors.Green.Medium);
                                table.Cell().Element(CellStyle).AlignCenter().Text(item.return_condition ?? "Baik");
                                table.Cell().Element(CellStyle).AlignCenter().Text(item.returned_at?.ToString("dd/MM/yyyy HH:mm") ?? "-");

                                static IContainer CellStyle(IContainer container)
                                {
                                    return container.Border(1).BorderColor(Colors.Grey.Lighten2).Padding(5).DefaultTextStyle(x => x.FontSize(10));
                                }
                                index++;
                            }
                        });

                        column.Item().PaddingTop(15).Text("* Pengembalian barang telah diverifikasi oleh Admin gudang.").FontSize(10).FontColor(Colors.Grey.Medium).Italic();

                        column.Item().PaddingTop(40).Table(table => 
                        {
                            table.ColumnsDefinition(cols => 
                            {
                                cols.RelativeColumn();
                                cols.ConstantColumn(100);
                                cols.RelativeColumn();
                            });

                            table.Cell().Column(1).AlignCenter().Column(col => {
                                col.Item().PaddingTop(40).BorderBottom(1).Text("");
                                col.Item().AlignCenter().Text(firstItem.applicant_name ?? firstItem.Requester?.name).SemiBold();
                                col.Item().AlignCenter().Text("Pemohon (Peminjam)").FontSize(10);
                            });
                            
                            table.Cell().Column(3).AlignCenter().Column(col => {
                                col.Item().PaddingTop(40).BorderBottom(1).Text("");
                                col.Item().AlignCenter().Text(firstItem.Approver?.name ?? "Admin Gudang").SemiBold();
                                col.Item().AlignCenter().Text("Admin (Penerima)").FontSize(10);
                            });
                        });
                    });
                });
            });

            return document.GeneratePdf();
        }

        private byte[] GenerateGenericReceipt(List<Transaction> transactions, string title, string wwwrootPath, string accentColor)
        {
            var firstItem = transactions.FirstOrDefault();
            if (firstItem == null) return Array.Empty<byte>();

            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4.Landscape());
                    page.Margin(20);
                    page.PageColor(Colors.White);
                    page.DefaultTextStyle(x => x.FontSize(12).FontFamily(Fonts.Arial).LineHeight(1.4f).FontColor(Colors.Grey.Darken4));

                    page.Header().Element(c => 
                    {
                        var utLogo = Path.Combine(wwwrootPath, "img", "ut.png");
                        var corpuLogo = Path.Combine(wwwrootPath, "img", "corpu.png");

                        c.BorderBottom(2).BorderColor(accentColor).PaddingBottom(8).Row(row =>
                        {
                            row.ConstantItem(120).AlignLeft().Height(35).Image(File.Exists(utLogo) ? utLogo : Path.Combine(wwwrootPath, "img", "logo.svg"));
                            row.RelativeItem().AlignCenter().Column(col => 
                            {
                                col.Item().Text(title).FontSize(20).SemiBold().FontColor(accentColor);
                                col.Item().Text("Warehouse Management System - United Tractors").FontSize(10).FontColor(Colors.Grey.Medium);
                            });
                            row.ConstantItem(120).AlignRight().Height(35).Image(File.Exists(corpuLogo) ? corpuLogo : Path.Combine(wwwrootPath, "img", "logo.svg"));
                        });
                    });

                    page.Content().PaddingVertical(15).Column(column =>
                    {
                        column.Item().Table(table => 
                        {
                            table.ColumnsDefinition(cols => 
                            {
                                cols.ConstantColumn(150);
                                cols.RelativeColumn();
                            });
                            
                            table.Cell().Text("Tanggal Request").SemiBold();
                            table.Cell().Text($": {firstItem.created_at:dd MMM yyyy HH:mm:ss}");
                            
                            table.Cell().Text("Nama Pemohon").SemiBold();
                            table.Cell().Text($": {firstItem.applicant_name ?? firstItem.Requester?.name}");
                            
                            table.Cell().Text("Divisi").SemiBold();
                            table.Cell().Text($": {firstItem.Division?.name ?? "-"}");
                            
                            if (!string.IsNullOrEmpty(firstItem.event_name))
                            {
                                table.Cell().Text("Event / Program").SemiBold();
                                table.Cell().Text($": {firstItem.event_name}");
                            }
                        });

                        column.Item().PaddingTop(15).Table(table =>
                        {
                            table.ColumnsDefinition(cols =>
                            {
                                cols.ConstantColumn(40); // No
                                cols.RelativeColumn(2); // SKU
                                cols.RelativeColumn(4); // Nama Barang
                                cols.ConstantColumn(80); // Qty
                                cols.RelativeColumn(3); // Keterangan
                            });

                            table.Header(header =>
                            {
                                header.Cell().Element(CellStyle).AlignCenter().Text("No");
                                header.Cell().Element(CellStyle).Text("SKU");
                                header.Cell().Element(CellStyle).Text("Nama Barang");
                                header.Cell().Element(CellStyle).AlignCenter().Text("Qty");
                                header.Cell().Element(CellStyle).Text("Keterangan");

                                static IContainer CellStyle(IContainer container)
                                {
                                    return container.Background(Colors.Grey.Lighten4).Border(1).BorderColor(Colors.Grey.Lighten2).Padding(6).DefaultTextStyle(x => x.SemiBold());
                                }
                            });

                            int index = 1;
                            foreach (var item in transactions)
                            {
                                table.Cell().Element(CellStyle).AlignCenter().Text(index.ToString());
                                table.Cell().Element(CellStyle).Text(item.Product?.sku ?? "-");
                                table.Cell().Element(CellStyle).Text(item.Product?.name ?? "-");
                                table.Cell().Element(CellStyle).AlignCenter().Text($"{item.quantity} {item.Product?.Unit?.name ?? "--"}");
                                table.Cell().Element(CellStyle).Text(item.request_type == "GIVEAWAY" ? "Request (Giveaway)" : "Peminjaman");

                                static IContainer CellStyle(IContainer container)
                                {
                                    return container.Border(1).BorderColor(Colors.Grey.Lighten2).Padding(6);
                                }
                                index++;
                            }
                        });
                    });
                });
            });

            return document.GeneratePdf();
        }

        public byte[] GenerateReceipt(List<Transaction> transactions, string wwwrootPath)
        {
            var firstItem = transactions.FirstOrDefault();
            if (firstItem == null) return Array.Empty<byte>();

            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4.Landscape());
                    page.Margin(20);
                    page.PageColor(Colors.White);
                    page.DefaultTextStyle(x => x.FontSize(12).FontFamily(Fonts.Arial).LineHeight(1.4f).FontColor(Colors.Grey.Darken4));

                    page.Header().Element(c => 
                    {
                        var utLogo = Path.Combine(wwwrootPath, "img", "ut.png");
                        var corpuLogo = Path.Combine(wwwrootPath, "img", "corpu.png");

                        c.BorderBottom(2).BorderColor(Colors.Black).PaddingBottom(8).Row(row =>
                        {
                            row.ConstantItem(120).AlignLeft().Height(35).Image(File.Exists(utLogo) ? utLogo : Path.Combine(wwwrootPath, "img", "logo.svg"));
                            row.RelativeItem().AlignCenter().Column(col => 
                            {
                                col.Item().Text("Bukti Transaksi Barang").FontSize(20).SemiBold().FontColor(Colors.Black);
                                col.Item().Text("Warehouse Management System - United Tractors").FontSize(10).FontColor(Colors.Grey.Medium);
                            });
                            row.ConstantItem(120).AlignRight().Height(35).Image(File.Exists(corpuLogo) ? corpuLogo : Path.Combine(wwwrootPath, "img", "logo.svg"));
                        });
                    });

                    page.Content().PaddingVertical(15).Column(column =>
                    {
                        column.Item().PaddingBottom(15).Table(table => 
                        {
                            table.ColumnsDefinition(cols => 
                            {
                                cols.ConstantColumn(150);
                                cols.RelativeColumn();
                            });
                            
                            table.Cell().Text("Tanggal Request").SemiBold();
                            table.Cell().Text($": {firstItem.created_at:dd MMM yyyy HH:mm:ss}");
                            
                            table.Cell().Text("Nama Pemohon").SemiBold();
                            table.Cell().Text($": {firstItem.applicant_name ?? firstItem.Requester?.name}");
                            
                            table.Cell().Text("NRP").SemiBold();
                            table.Cell().Text($": {firstItem.applicant_nrp ?? "-"}");
                            
                            table.Cell().Text("Divisi").SemiBold();
                            table.Cell().Text($": {firstItem.Division?.name ?? "-"}");
                            
                            if (!string.IsNullOrEmpty(firstItem.event_name))
                            {
                                table.Cell().Text("Keperluan / Event").SemiBold();
                                table.Cell().Text($": {firstItem.event_name} ({(firstItem.event_date.HasValue ? firstItem.event_date.Value.ToString("dd MMM yyyy") : "-")})");
                            }
                            
                            if (firstItem.request_type != "GIVEAWAY")
                            {
                                table.Cell().Text("Tanggal Pengembalian").SemiBold();
                                table.Cell().Text($": {(firstItem.returned_at.HasValue ? firstItem.returned_at.Value.ToString("dd MMM yyyy HH:mm:ss") : "Belum Dikembalikan")}");
                            }

                            if (!string.IsNullOrEmpty(firstItem.notes))
                            {
                                table.Cell().Text("Catatan").SemiBold();
                                table.Cell().Text($": {firstItem.notes}");
                            }
                        });

                        column.Item().Table(table =>
                        {
                            table.ColumnsDefinition(cols =>
                            {
                                cols.ConstantColumn(40); // No
                                cols.RelativeColumn(2); // SKU
                                cols.RelativeColumn(3); // Nama Barang
                                cols.ConstantColumn(80); // Qty
                                cols.RelativeColumn(2); // Tipe Barang
                                cols.RelativeColumn(3); // Info Pengembalian
                            });

                            table.Header(header =>
                            {
                                header.Cell().Element(CellStyle).AlignCenter().Text("No");
                                header.Cell().Element(CellStyle).Text("SKU");
                                header.Cell().Element(CellStyle).Text("Nama Barang");
                                header.Cell().Element(CellStyle).AlignCenter().Text("Qty");
                                header.Cell().Element(CellStyle).AlignCenter().Text("Tipe Barang");
                                header.Cell().Element(CellStyle).AlignCenter().Text("Info Pengembalian");

                                static IContainer CellStyle(IContainer container)
                                {
                                    return container.Background(Colors.Grey.Lighten4).Border(1).BorderColor(Colors.Grey.Lighten2).Padding(6).DefaultTextStyle(x => x.SemiBold());
                                }
                            });

                            int index = 1;
                            foreach (var item in transactions)
                            {
                                table.Cell().Element(CellStyle).AlignCenter().Text(index.ToString());
                                table.Cell().Element(CellStyle).Text(item.Product?.sku ?? "-");
                                table.Cell().Element(CellStyle).Text(item.Product?.name ?? "-");
                                table.Cell().Element(CellStyle).AlignCenter().Text($"{item.quantity} {item.Product?.Unit?.name ?? "--"}");
                                
                                table.Cell().Element(CellStyle).AlignCenter().Element(c => 
                                {
                                    if (item.request_type == "GIVEAWAY")
                                    {
                                        c.Background(Colors.Purple.Lighten5).Border(1).BorderColor(Colors.Purple.Lighten4).PaddingVertical(2).PaddingHorizontal(6).Text("Request (Giveaway)").FontSize(11).SemiBold().FontColor(Colors.Purple.Darken3);
                                    }
                                    else
                                    {
                                        c.Background(Colors.LightBlue.Lighten5).Border(1).BorderColor(Colors.LightBlue.Lighten4).PaddingVertical(2).PaddingHorizontal(6).Text("Peminjaman (Bisa Return)").FontSize(11).SemiBold().FontColor(Colors.LightBlue.Darken3);
                                    }
                                });

                                table.Cell().Element(CellStyle).AlignCenter().Element(c => 
                                {
                                    if (item.request_type == "GIVEAWAY")
                                    {
                                        c.Text("-");
                                    }
                                    else
                                    {
                                        if (item.returned_quantity > 0)
                                        {
                                            c.Column(col => 
                                            {
                                                col.Item().Text($"{item.returned_quantity} / {item.quantity} Kembali").FontColor(Colors.Green.Medium).SemiBold();
                                                if (item.returned_at.HasValue)
                                                {
                                                    col.Item().Text($"({item.returned_at.Value:dd MMM yyyy HH:mm})").FontSize(9).FontColor(Colors.Grey.Medium);
                                                }
                                            });
                                        }
                                        else
                                        {
                                            c.Text("Belum Kembali").FontColor(Colors.Red.Medium).SemiBold();
                                        }
                                    }
                                });

                                static IContainer CellStyle(IContainer container)
                                {
                                    return container.Border(1).BorderColor(Colors.Grey.Lighten2).Padding(6);
                                }
                                index++;
                            }
                        });

                        column.Item().PaddingTop(15).Text("* Barang peminjaman (Wajib Kembali) harus dikembalikan dalam kondisi baik sesuai dengan jumlah yang dipinjam.\n* Bukti ini sah dan dicetak secara otomatis oleh sistem.").FontSize(12).FontColor(Colors.Grey.Medium).Italic();

                        column.Item().PaddingTop(30).Table(table => 
                        {
                            table.ColumnsDefinition(cols => 
                            {
                                cols.RelativeColumn();
                                cols.ConstantColumn(50); // Spacer
                                cols.RelativeColumn();
                            });

                            table.Cell().Row(1).Column(1).PaddingTop(50).BorderBottom(1).BorderColor(Colors.Grey.Darken4).Text("");
                            table.Cell().Row(2).Column(1).AlignCenter().PaddingTop(5).Text(firstItem.applicant_name ?? firstItem.Requester?.name).SemiBold();
                            table.Cell().Row(3).Column(1).AlignCenter().Text("Pemohon (Penerima Barang)").FontSize(12).FontColor(Colors.Grey.Medium);

                            table.Cell().Row(1).Column(3).PaddingTop(50).BorderBottom(1).BorderColor(Colors.Grey.Darken4).Text("");
                            table.Cell().Row(2).Column(3).AlignCenter().PaddingTop(5).Text(firstItem.Approver?.name ?? "Admin / Manager").SemiBold();
                            table.Cell().Row(3).Column(3).AlignCenter().Text("Menyetujui (Pemberi Barang)").FontSize(12).FontColor(Colors.Grey.Medium);
                        });
                    });
                });
            });

            return document.GeneratePdf();
        }

        private void GenerateEmptyReport(IDocumentContainer container, string title, DateTime startDate, DateTime endDate, string wwwrootPath)
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4.Landscape());
                page.Margin(1.5f, QuestPDF.Infrastructure.Unit.Centimetre);
                page.PageColor(Colors.White);
                page.DefaultTextStyle(x => x.FontSize(9).FontFamily(Fonts.Arial).LineHeight(1.3f));

                page.Header().Element(c => ComposeHeader(c, title, wwwrootPath));
                page.Content().PaddingVertical(10).Column(column =>
                {
                    column.Item().AlignCenter().Text($"Periode: {startDate:dd MMM yyyy} - {endDate:dd MMM yyyy}").FontSize(8).FontColor(Colors.Grey.Darken1);
                    column.Item().PaddingBottom(8);
                    column.Item().Background(Colors.Grey.Lighten4).BorderLeft(3).BorderColor(Colors.Red.Medium).Padding(4).Text($"Tidak ada transaksi pada periode ini.").FontSize(8).FontColor(Colors.Grey.Darken3).SemiBold();
                });
                page.Footer().Element(c => ComposeFooter(c, wwwrootPath));
            });
        }

        private void ComposeHeader(IContainer container, string title, string wwwrootPath)
        {
            var utLogo = Path.Combine(wwwrootPath, "img", "ut.png");
            var corpuLogo = Path.Combine(wwwrootPath, "img", "corpu.png");

            container.BorderBottom(2).BorderColor(Colors.Grey.Darken4).PaddingBottom(8).Row(row =>
            {
                row.ConstantItem(120).AlignLeft().Height(35).Image(File.Exists(utLogo) ? utLogo : Path.Combine(wwwrootPath, "img", "logo.svg"));
                row.RelativeItem().Column(column =>
                {
                    column.Item().AlignCenter().Text(title).FontSize(14).SemiBold().FontColor(Colors.Grey.Darken4);
                    column.Item().AlignCenter().Text("Warehouse Management System - United Tractors").FontSize(8).FontColor(Colors.Grey.Darken1);
                });
                row.ConstantItem(120).AlignRight().Height(35).Image(File.Exists(corpuLogo) ? corpuLogo : Path.Combine(wwwrootPath, "img", "logo.svg"));
            });
        }

        private void ComposeFooter(IContainer container, string wwwrootPath)
        {
            var movingLogo = Path.Combine(wwwrootPath, "img", "moving.png");
            
            container.BorderTop(1).BorderColor(Colors.Grey.Lighten2).PaddingTop(6).Row(row =>
            {
                row.RelativeItem().Text($"Dicetak pada {DateTime.Now:dd MMM yyyy HH:mm:ss} | WMS United Tractors").FontSize(7.5f).FontColor(Colors.Grey.Medium);
                if (File.Exists(movingLogo))
                {
                    row.ConstantItem(100).AlignRight().Height(20).Image(movingLogo);
                }
            });
        }
    }
}