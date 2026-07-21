using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System.IO;
using System.Linq;
using UT_WMSDotnet.Models;
using System;
using System.Collections.Generic;
using System.Text.Json;

namespace WMS_UnitedTracors_Blazor.Services
{
    public class PdfReceiptService
    {
        public PdfReceiptService()
        {
            QuestPDF.Settings.License = LicenseType.Community;
        }

        public byte[] GenerateReceiptRequest(List<Transaction> items, string wwwrootPath)
        {
            var firstItem = items.First();
            string title = "Form Permintaan Pengadaan / Pengeluaran Material";
            return GenerateDocument(items, firstItem, title, wwwrootPath,
                sig1Title: "Pemohon",
                sig1Name: !string.IsNullOrEmpty(firstItem.applicant_name) ? firstItem.applicant_name : (firstItem.Requester?.name ?? "-"),
                sig1Role: "Associate CorpU",
                sig2Title: "Diketahui",
                sig2Name: "Ilona Kirana Saradella",
                sig2Role: "Team Leader KM & Infrastructure",
                sig3Title: "Disetujui",
                sig3Name: "Ahmad Anwari",
                sig3Role: "CorpU Dept Head");
        }

        public byte[] GenerateReceiptApproval(List<Transaction> items, string wwwrootPath)
        {
            var firstItem = items.First();
            string title = "Form Persetujuan Pengeluaran Material";
            return GenerateDocument(items, firstItem, title, wwwrootPath,
                sig1Title: "Diketahui",
                sig1Name: "Ilona Kirana Saradella",
                sig1Role: "Team Leader KM & Infrastructure",
                sig2Title: "Penyetuju",
                sig2Name: firstItem.Approver?.name ?? "Ahmad Anwari",
                sig2Role: "CorpU Dept Head",
                sig3Title: "Diterima (Peminjam)",
                sig3Name: !string.IsNullOrEmpty(firstItem.applicant_name) ? firstItem.applicant_name : (firstItem.Requester?.name ?? "-"),
                sig3Role: "Associate CorpU");
        }

        public byte[] GenerateReceiptHandover(List<Transaction> items, string wwwrootPath)
        {
            var firstItem = items.First();
            string title = firstItem.request_type == "GIVEAWAY" ? "Form Penyerahan Souvenir CorpU" : "Form Penyerahan Peminjaman Barang CorpU";
            return GenerateDocument(items, firstItem, title, wwwrootPath,
                sig1Title: "Yang Menyerahkan",
                sig1Name: "Ilona Kirana Saradella",
                sig1Role: "Admin CorpU",
                sig2Title: "Mengetahui",
                sig2Name: "Ahmad Anwari",
                sig2Role: "CorpU Dept Head",
                sig3Title: "Penerima",
                sig3Name: !string.IsNullOrEmpty(firstItem.applicant_name) ? firstItem.applicant_name : (firstItem.Requester?.name ?? "-"),
                sig3Role: "Associate CorpU",
                proofImagePath: ResolveProofImagePath(firstItem.handover_photo, wwwrootPath),
                proofLabel: "Bukti Serah Terima");
        }

        public byte[] GenerateReceiptDocumentation(List<Transaction> items, string wwwrootPath)
        {
            var firstItem = items.First();
            string title = "Form Dokumentasi Giveaway CorpU";
            return GenerateDocument(items, firstItem, title, wwwrootPath,
                sig1Title: "Pemohon",
                sig1Name: !string.IsNullOrEmpty(firstItem.applicant_name) ? firstItem.applicant_name : (firstItem.Requester?.name ?? "-"),
                sig1Role: "Associate CorpU",
                sig2Title: "Mengetahui",
                sig2Name: "Ilona Kirana Saradella",
                sig2Role: "Team Leader KM & Infrastructure",
                sig3Title: "Arsip CorpU",
                sig3Name: "Ahmad Anwari",
                sig3Role: "CorpU Dept Head",
                proofImagePath: ResolveProofImagePath(firstItem.documentation_photo, wwwrootPath),
                proofLabel: "Bukti Dokumentasi");
        }

        public byte[] GenerateReceipt(List<Transaction> items, string wwwrootPath)
        {
            var firstItem = items.First();
            string title = "Tanda Terima Peminjaman Barang";
            return GenerateDocument(items, firstItem, title, wwwrootPath,
                sig1Title: "Yang Menyerahkan",
                sig1Name: "Admin CorpU",
                sig1Role: "",
                sig2Title: "Mengetahui",
                sig2Name: "Ahmad Anwari",
                sig2Role: "CorpU Dept Head",
                sig3Title: "Penerima",
                sig3Name: !string.IsNullOrEmpty(firstItem.applicant_name) ? firstItem.applicant_name : (firstItem.Requester?.name ?? "-"),
                sig3Role: "Associate CorpU");
        }

        public byte[] GenerateReturnReceipt(List<Transaction> items, string wwwrootPath)
        {
            var firstItem = items.First();
            string title = "Berita Acara Pengembalian Barang";
            return GenerateDocument(items, firstItem, title, wwwrootPath,
                sig1Title: "Yang Mengembalikan",
                sig1Name: !string.IsNullOrEmpty(firstItem.applicant_name) ? firstItem.applicant_name : (firstItem.Requester?.name ?? "-"),
                sig1Role: "Associate CorpU",
                sig2Title: "Mengetahui",
                sig2Name: "Ahmad Anwari",
                sig2Role: "CorpU Dept Head",
                sig3Title: "Yang Menerima",
                sig3Name: "Ilona Kirana Saradella",
                sig3Role: "Admin CorpU",
                proofImagePath: ResolveProofImagePath(firstItem.return_photo, wwwrootPath),
                proofLabel: "Bukti Pengembalian");
        }

        // Resolve path foto bukti (handover/return) ke path fisik; null jika tidak ada / bukan gambar.
        private static string? ResolveProofImagePath(string? stored, string wwwrootPath)
        {
            if (string.IsNullOrWhiteSpace(stored)) return null;
            if (stored == "forced-by-admin") return null;

            var resolved = stored.Trim();
            if (resolved.StartsWith("["))
            {
                try
                {
                    var parsed = JsonSerializer.Deserialize<List<string>>(resolved);
                    resolved = parsed?.FirstOrDefault(x => !string.IsNullOrWhiteSpace(x)) ?? string.Empty;
                }
                catch
                {
                    resolved = string.Empty;
                }
            }

            if (string.IsNullOrWhiteSpace(resolved)) return null;

            var rel = resolved.TrimStart('/').Replace('/', Path.DirectorySeparatorChar);
            string full;
            if (rel.StartsWith("storage" + Path.DirectorySeparatorChar, StringComparison.OrdinalIgnoreCase) || 
                rel.Equals("storage", StringComparison.OrdinalIgnoreCase))
            {
                var appRoot = Directory.GetParent(wwwrootPath.TrimEnd(Path.DirectorySeparatorChar))?.FullName ?? string.Empty;
                var subPath = rel.Substring("storage".Length).TrimStart(Path.DirectorySeparatorChar);
                full = Path.Combine(appRoot, "Storage", subPath);
            }
            else
            {
                full = Path.Combine(wwwrootPath, rel);
            }

            var ext = Path.GetExtension(full).ToLowerInvariant();
            if (ext != ".jpg" && ext != ".jpeg" && ext != ".png" && ext != ".webp") return null;
            return File.Exists(full) ? full : null;
        }

        private byte[] GenerateDocument(List<Transaction> items, Transaction firstItem, string title, string wwwrootPath,
            string sig1Title, string sig1Name, string sig1Role,
            string sig2Title, string sig2Name, string sig2Role,
            string sig3Title, string sig3Name, string sig3Role,
            string? proofImagePath = null, string? proofLabel = null)
        {
            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4.Landscape());
                    page.Margin(20, QuestPDF.Infrastructure.Unit.Point);
                    page.PageColor(Colors.White);
                    page.DefaultTextStyle(x => x.FontSize(9).FontFamily(Fonts.Arial).FontColor(Colors.Black));

                    page.Content().Border(1.5f).BorderColor(Colors.Black).Column(column =>
                    {
                        // Header row (Logos and Title)
                        column.Item().Row(row =>
                        {
                            row.AutoItem().Width(120).Padding(10).AlignCenter().AlignMiddle()
                               .Image(Path.Combine(wwwrootPath, "img", "ut.png"));
                            
                            row.RelativeItem().BorderLeft(1.5f).BorderRight(1.5f).BorderColor(Colors.Black)
                               .Padding(10).AlignCenter().AlignMiddle()
                               .Text(title).FontSize(13).Bold();

                            row.AutoItem().Width(100).Padding(10).AlignCenter().AlignMiddle()
                               .Image(Path.Combine(wwwrootPath, "img", "corpu.png"));
                        });

                        // Top border before metadata
                        column.Item().LineHorizontal(1.5f).LineColor(Colors.Black);

                        // Metadata Block
                        column.Item().Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.ConstantColumn(120);
                                columns.RelativeColumn();
                            });

                            void AddMetaRow(string label, string val)
                            {
                                table.Cell().BorderBottom(1).BorderColor(Colors.Black).Padding(4).Text(label).Bold();
                                table.Cell().BorderBottom(1).BorderLeft(1).BorderColor(Colors.Black).Padding(4).Text(": " + val);
                            }

                            string docNumber = $"UT/WMS/SND/{firstItem.id}/{firstItem.created_at.ToString("MM/yyyy")}";
                            AddMetaRow("Nomor", docNumber);

                            string dateLabel = title.Contains("Persetujuan") ? "Tanggal Persetujuan" :
                                               title.Contains("Penyerahan") ? "Tanggal Serah" :
                                               title.Contains("Pengembalian") ? "Tanggal Kembali" : "Tanggal Permintaan";
                            
                            AddMetaRow(dateLabel, firstItem.updated_at.ToString("dd MMMM yyyy", new System.Globalization.CultureInfo("id-ID")));
                            AddMetaRow("Divisi", firstItem.Division?.name ?? "-");
                            AddMetaRow("Nama", !string.IsNullOrEmpty(firstItem.applicant_name) ? firstItem.applicant_name : (firstItem.Requester?.name ?? "-"));
                            
                            table.Cell().Padding(4).Text("NRP").Bold();
                            table.Cell().BorderLeft(1).BorderColor(Colors.Black).Padding(4).Text(": " + (!string.IsNullOrEmpty(firstItem.applicant_nrp) ? firstItem.applicant_nrp : "-"));
                        });

                        // Mid border before Items Table
                        column.Item().LineHorizontal(1.5f).LineColor(Colors.Black);

                        // Items Table
                        column.Item().Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.ConstantColumn(40); // No
                                columns.RelativeColumn(); // Nama
                                columns.ConstantColumn(100); // Jumlah
                                columns.RelativeColumn(); // Kegiatan
                                columns.ConstantColumn(120); // Tgl Kegiatan
                            });

                            table.Header(header =>
                            {
                                header.Cell().Background(Colors.Grey.Lighten4).BorderBottom(1.5f).BorderColor(Colors.Black).Padding(4).AlignCenter().Text("No").Bold();
                                header.Cell().Background(Colors.Grey.Lighten4).BorderBottom(1.5f).BorderLeft(1.5f).BorderColor(Colors.Black).Padding(4).AlignCenter().Text($"Nama {(firstItem.request_type == "GIVEAWAY" ? "Souvenir" : "Barang")}").Bold();
                                header.Cell().Background(Colors.Grey.Lighten4).BorderBottom(1.5f).BorderLeft(1.5f).BorderColor(Colors.Black).Padding(4).AlignCenter().Text("Jumlah").Bold();
                                header.Cell().Background(Colors.Grey.Lighten4).BorderBottom(1.5f).BorderLeft(1.5f).BorderColor(Colors.Black).Padding(4).AlignCenter().Text("Kegiatan").Bold();
                                header.Cell().Background(Colors.Grey.Lighten4).BorderBottom(1.5f).BorderLeft(1.5f).BorderColor(Colors.Black).Padding(4).AlignCenter().Text("Tanggal Kegiatan").Bold();
                            });

                            int idx = 1;
                            foreach(var item in items)
                            {
                                table.Cell().BorderBottom(1).BorderColor(Colors.Black).Padding(4).AlignCenter().Text(idx.ToString());
                                table.Cell().BorderBottom(1).BorderLeft(1.5f).BorderColor(Colors.Black).Padding(4).Text(item.Product?.name ?? "-");
                                table.Cell().BorderBottom(1).BorderLeft(1.5f).BorderColor(Colors.Black).Padding(4).AlignCenter().Text($"{item.quantity} {(item.Product?.Unit?.name ?? "Pcs")}").Bold();
                                table.Cell().BorderBottom(1).BorderLeft(1.5f).BorderColor(Colors.Black).Padding(4).Text(!string.IsNullOrEmpty(item.event_name) ? item.event_name : "-");
                                table.Cell().BorderBottom(1).BorderLeft(1.5f).BorderColor(Colors.Black).Padding(4).AlignCenter().Text(item.event_date?.ToString("dd/MM/yyyy") ?? "-");
                                idx++;
                            }

                            // Empty rows to fill space (at least 10 rows like old_laravel)
                            for(int i = items.Count; i < 10; i++)
                            {
                                table.Cell().BorderBottom(1).BorderColor(Colors.Black).Padding(4).Text("");
                                table.Cell().BorderBottom(1).BorderLeft(1.5f).BorderColor(Colors.Black).Padding(4).Text("");
                                table.Cell().BorderBottom(1).BorderLeft(1.5f).BorderColor(Colors.Black).Padding(4).Text("");
                                table.Cell().BorderBottom(1).BorderLeft(1.5f).BorderColor(Colors.Black).Padding(4).Text("");
                                table.Cell().BorderBottom(1).BorderLeft(1.5f).BorderColor(Colors.Black).Padding(4).Text("");
                            }
                        });

                        // Proof photo (Bukti Serah Terima / Pengembalian), jika ada
                        if (!string.IsNullOrEmpty(proofImagePath))
                        {
                            column.Item().LineHorizontal(1.5f).LineColor(Colors.Black);
                            column.Item().Padding(8).Row(r =>
                            {
                                r.AutoItem().Width(180).Column(c =>
                                {
                                    c.Item().Text(proofLabel ?? "Bukti Foto").Bold().FontSize(9);
                                    c.Item().PaddingTop(4).Width(180).MaxHeight(180).Image(proofImagePath).FitArea();
                                });
                                r.RelativeItem();
                            });
                        }

                        // Foot border before signatures
                        column.Item().LineHorizontal(1.5f).LineColor(Colors.Black);

                        // Signatures
                        column.Item().Row(row =>
                        {
                            row.RelativeItem(4).Padding(10).Column(col => 
                            {
                                col.Item().Text("Catatan:").Bold().FontSize(10);
                                col.Item().PaddingTop(2).Text(!string.IsNullOrEmpty(firstItem.notes) ? firstItem.notes : "-").FontSize(9).FontColor(Colors.Grey.Darken2);
                            });

                            row.RelativeItem(6).BorderLeft(1.5f).BorderColor(Colors.Black).Column(col =>
                            {
                                col.Item().BorderBottom(1).BorderColor(Colors.Black).PaddingVertical(5).PaddingHorizontal(8).AlignRight()
                                   .Text($"Jakarta, {firstItem.created_at.ToString("dd MMMM yyyy", new System.Globalization.CultureInfo("id-ID"))}").Bold().FontSize(9);

                                col.Item().Table(t =>
                                {
                                    t.ColumnsDefinition(c =>
                                    {
                                        c.RelativeColumn();
                                        c.RelativeColumn();
                                        c.RelativeColumn();
                                    });

                                    t.Cell().BorderBottom(1).BorderColor(Colors.Black).PaddingVertical(5).AlignCenter().Text(sig1Title).Bold();
                                    t.Cell().BorderBottom(1).BorderLeft(1).BorderColor(Colors.Black).PaddingVertical(5).AlignCenter().Text(sig2Title).Bold();
                                    t.Cell().BorderBottom(1).BorderLeft(1).BorderColor(Colors.Black).PaddingVertical(5).AlignCenter().Text(sig3Title).Bold();

                                    t.Cell().Height(80).PaddingBottom(6).AlignBottom().AlignCenter().Column(c => {
                                        c.Item().Text(sig1Name).Bold().Underline();
                                        c.Item().Text(sig1Role).FontSize(8).FontColor(Colors.Grey.Darken2);
                                    });
                                    t.Cell().BorderLeft(1).BorderColor(Colors.Black).Height(80).PaddingBottom(6).AlignBottom().AlignCenter().Column(c => {
                                        c.Item().Text(sig2Name).Bold().Underline();
                                        c.Item().Text(sig2Role).FontSize(8).FontColor(Colors.Grey.Darken2);
                                    });
                                    t.Cell().BorderLeft(1).BorderColor(Colors.Black).Height(80).PaddingBottom(6).AlignBottom().AlignCenter().Column(c => {
                                        c.Item().Text(sig3Name).Bold().Underline();
                                        c.Item().Text(sig3Role).FontSize(8).FontColor(Colors.Grey.Darken2);
                                    });
                                });
                            });
                        });

                    });
                });
            });

            return document.GeneratePdf();
        }
    }
}
