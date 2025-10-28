using System;
using System.Linq;
using System.IO;
using ApplShopAPI.Model;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace ApplShopAPI.Services
{
    public class ReceiptService
    {
        public byte[] Generate(Order order)
        {
            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(36);
                    page.DefaultTextStyle(x => x.FontSize(12));

                    page.Header().Row(row =>
                    {
                        row.RelativeItem().Column(col =>
                        {
                            col.Item().Text("AppleShop").Bold().FontSize(20);
                            col.Item().Text($"Receipt # {order.Id}");
                            col.Item().Text($"Date: {DateTime.Now:dd.MM.yyyy HH:mm}");
                        });

                        var logoPath = @"C:\Users\rakhm\source\repos\AppleStore_Project\AppleStore_Project\Assets\Images\LogoProject.png";
                        if (File.Exists(logoPath))
                            row.ConstantItem(80).Image(logoPath).FitArea();
                        else
                            row.ConstantItem(80).Image(Placeholders.Image(80, 80));
                    });

                    page.Content().Column(col =>
                    {
                        col.Spacing(8);
                        if (order.User != null)
                        {
                            col.Item().Text($"Recipient: {order.User.Email}");
                            col.Item().Text($"Delivery address: {order.DeliveryAddress}");
                        }

                        col.Item().Table(table =>
                        {
                            table.ColumnsDefinition(cols =>
                            {
                                cols.RelativeColumn(6);
                                cols.RelativeColumn(2);
                                cols.RelativeColumn(3);
                                cols.RelativeColumn(3);
                            });

                            table.Header(header =>
                            {
                                header.Cell().Text("Item").SemiBold();
                                header.Cell().Text("Qty").SemiBold();
                                header.Cell().Text("Price").SemiBold();
                                header.Cell().Text("Amount").SemiBold();
                            });

                            foreach (var item in order.OrderItems)
                            {
                                var name = item.Product?.Name ?? $"ID {item.ProductId}";
                                var qtyText = item.Quantity == 1 ? "1 pc." : $"{item.Quantity} pcs.";
                                table.Cell().Text(name);
                                table.Cell().Text(qtyText);
                                table.Cell().Text($"{item.Price:C}");
                                table.Cell().Text($"{(item.Price * item.Quantity):C}");
                            }
                        });

                        var total = order.OrderItems.Sum(i => i.Price * i.Quantity);
                        col.Item().PaddingTop(12).Text($"Total: {total:C}").Bold().FontSize(14);
                        col.Item().Text($"Payment method: {order.PaymentMethod}");
                    });

                    page.Footer().AlignRight().Text("Thank you for your purchase!").Italic();
                });
            });

            return document.GeneratePdf();
        }
    }
}