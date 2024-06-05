using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using PCStore.Models;

namespace PCStore.Services;

public class OrderReceiptService
{
    public void WriteToStreamAsync(Stream stream, Order order)
    {
        if (!stream.CanWrite)
        {
            throw new ArgumentException("Input stream is not writable");
        }

        using var fileStream = new FileStream(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Templates", "receipt_template.docx"), FileMode.Open, FileAccess.Read);
        fileStream.CopyTo(stream);

        using var receipt =
            WordprocessingDocument.Open(stream, true);

        var productsTable = receipt.MainDocumentPart.Document.Body.Elements<Table>().First();
        var paragraphs = receipt.MainDocumentPart.Document.Body.Elements<Paragraph>().First();

        var header = paragraphs.Elements<Run>().First().Elements<Text>().First();
        header.Text = header.Text.Replace("чек № ", $"чек № {order.Id}");

        int i = 1;
        foreach (var product in order.OrderProducts)
        {
            TableRow tr = new TableRow();

            TableCell tc1 = new TableCell();
            TableCell tc2 = new TableCell();
            TableCell tc3 = new TableCell();
            TableCell tc4 = new TableCell();
            TableCell tc5 = new TableCell();
            TableCell tc6 = new TableCell();

            tc1.Append(new Paragraph(new Run(new Text(i.ToString()))));
            tc2.Append(new Paragraph(new Run(new Text(product.Product.Name))));
            tc3.Append(new Paragraph(new Run(new Text("шт."))));
            tc4.Append(new Paragraph(new Run(new Text(product.Quantity.ToString()))));
            tc5.Append(new Paragraph(new Run(new Text(product.Product.Price.ToString()))));
            tc6.Append(new Paragraph(new Run(new Text((product.Quantity * product.Product.Price).ToString()))));

            tr.Append(tc1);
            tr.Append(tc2);
            tr.Append(tc3);
            tr.Append(tc4);
            tr.Append(tc5);
            tr.Append(tc6);

            productsTable.Append(tr);
            
            i++;
        }

        var r = new TableRow();
        var c5 = new TableCell();
        var c6 = new TableCell();

        c5.Append(new Paragraph(new Run(new Text("Разом: "))));
        c6.Append(new Paragraph(new Run(new Text(order.Total.ToString()))));

        for (int j=0; j<4; j++)
        {
            var c = new TableCell();
            c.Append(new Paragraph(new Run(new Text(""))));
            r.Append(c);
        }

        r.Append(c5);
        r.Append(c6);

        productsTable.Append(r);
        
        receipt.MainDocumentPart.Document.Save();
    }
}