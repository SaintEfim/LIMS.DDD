using System.Data;
using FastReport;
using FastReport.Export.Pdf;
using Service.Reports.Client.Models;

namespace Service.Reports;

public sealed class OrderReportGenerator
{
    public byte[] Generate(
        OrderReportRequest request)
    {
        var order = request.Order!;
        var samples = new DataTable("Samples");
        samples.Columns.Add("Code", typeof(string));
        samples.Columns.Add("Name", typeof(string));
        samples.Columns.Add("Status", typeof(string));
        samples.Columns.Add("Volume", typeof(string));
        foreach (var sample in request.Samples ?? [])
            samples.Rows.Add(sample.Code ?? "", sample.Name, sample.Status,
                sample.VolumeValue is null ? "" : $"{sample.VolumeValue} {sample.VolumeUnit}".Trim());

        using var report = new Report();
        var page = new ReportPage { Name = "OrderPage" };
        report.Pages.Add(page);

        var title = new ReportTitleBand { Height = 135 };
        page.ReportTitle = title;
        AddText(title, $"Laboratory order {order.Code ?? order.Id.ToString()}", 0, 0, 700, 28, 16);
        AddText(title, $"Name: {order.Name}", 0, 35, 700, 22);
        AddText(title, $"Contractor: {order.Contractor}    Status: {order.Status}", 0, 60, 700, 22);
        AddText(title, $"Description: {order.Description}", 0, 85, 700, 40);

        report.RegisterData(samples, "Samples");
        var band = new DataBand
        {
            Height = 90,
            CanGrow = true,
            DataSource = report.GetDataSource("Samples")
        };
        page.Bands.Add(band);
        band.Objects.Add(new TextObject
        {
            Text = "Code: [Samples.Code]\nName: [Samples.Name]\nStatus: [Samples.Status]\nVolume: [Samples.Volume]",
            Left = 0,
            Top = 0,
            Width = 700,
            Height = 85,
            Font = new System.Drawing.Font("Arial", 10),
            WordWrap = true,
            CanGrow = true,
            AllowExpressions = true
        });

        report.Prepare();
        using var output = new MemoryStream();
        using var export = new PDFExport();
        report.Export(export, output);
        return output.ToArray();
    }

    private static void AddText(
        BandBase band,
        string value,
        float left,
        float top,
        float width,
        float height,
        float fontSize = 10,
        bool expressions = false)
    {
        band.Objects.Add(new TextObject
        {
            Text = value,
            Left = left,
            Top = top,
            Width = width,
            Height = height,
            Font = new System.Drawing.Font("Arial", fontSize),
            AllowExpressions = expressions
        });
    }
}
