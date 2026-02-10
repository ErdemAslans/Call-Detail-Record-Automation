using System.Drawing;
using Cdr.Api.Models;
using Cdr.Api.Models.Response;
using Cdr.Api.Models.Response.Dashboard;
using Cdr.Api.Models.Response.UserStatistics;
using OfficeOpenXml;
using OfficeOpenXml.Style;

namespace Cdr.Api.Extensions;

public static class DepartmentCallStatisticsExtensions
{
    public static byte[] ToExcelFile(this IEnumerable<DepartmentCallStatistics> statistics)
    {
        ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

        using var package = new ExcelPackage();
        var worksheet = package.Workbook.Worksheets.Add("Departman İstatistikleri");

        // Dikey başlıkları ayarla
        worksheet.Cells[1, 1].Value = "Departman Adı";
        worksheet.Cells[2, 1].Value = "Toplam Çağrı";
        worksheet.Cells[3, 1].Value = "Cevaplanan Çağrı";
        worksheet.Cells[4, 1].Value = "Cevapsız Çağrı";
        worksheet.Cells[5, 1].Value = "Molada Gelen Çağrı";
        worksheet.Cells[6, 1].Value = "Cevaplama Oranı (%)";

        // Başlık stilini ayarla
        var headerRange = worksheet.Cells[1, 1, 6, 1];
        headerRange.Style.Font.Bold = true;
        headerRange.Style.Fill.PatternType = ExcelFillStyle.Solid;
        headerRange.Style.Fill.BackgroundColor.SetColor(Color.LightGray);
        headerRange.Style.Border.BorderAround(ExcelBorderStyle.Thin);

        // Verileri doldur
        var column = 2;
        foreach (var stat in statistics)
        {
            worksheet.Cells[1, column].Value = stat.DepartmentName;
            worksheet.Cells[2, column].Value = stat.TotalCalls;
            worksheet.Cells[3, column].Value = stat.AnsweredCalls;
            worksheet.Cells[4, column].Value = stat.MissedCalls;
            worksheet.Cells[5, column].Value = stat.OnBreakCalls;
            worksheet.Cells[6, column].Value = stat.AnsweredCallRate / 100;

            // Yüzde formatı
            worksheet.Cells[6, column].Style.Numberformat.Format = "0.00%";

            column++;
        }

        // Sütun genişliklerini otomatik ayarla
        worksheet.Cells.AutoFitColumns();

        // Tüm hücrelere border ekle
        var dataRange = worksheet.Cells[1, 1, 6, column - 1];
        dataRange.Style.Border.Top.Style = ExcelBorderStyle.Thin;
        dataRange.Style.Border.Left.Style = ExcelBorderStyle.Thin;
        dataRange.Style.Border.Right.Style = ExcelBorderStyle.Thin;
        dataRange.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;

        return package.GetAsByteArray();
    }

    public static byte[] ToExcelFile(this DepartmentCallStatisticsByCallDirection statisticsByCallDirection)
    {
        ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

        using var package = new ExcelPackage();

        void AddSheet(string sheetName, IEnumerable<DepartmentCallStatistics> statistics)
        {
            var worksheet = package.Workbook.Worksheets.Add(sheetName);

            // Dikey başlıkları ayarla
            worksheet.Cells[1, 1].Value = "Departman Adı";
            worksheet.Cells[2, 1].Value = "Toplam Çağrı";
            worksheet.Cells[3, 1].Value = "Cevaplanan Çağrı";
            worksheet.Cells[4, 1].Value = "Cevapsız Çağrı";
            worksheet.Cells[5, 1].Value = "Molada Gelen Çağrı";
            worksheet.Cells[6, 1].Value = "Cevaplama Oranı (%)";

            // Başlık stilini ayarla
            var headerRange = worksheet.Cells[1, 1, 6, 1];
            headerRange.Style.Font.Bold = true;
            headerRange.Style.Fill.PatternType = ExcelFillStyle.Solid;
            headerRange.Style.Fill.BackgroundColor.SetColor(Color.LightGray);
            headerRange.Style.Border.BorderAround(ExcelBorderStyle.Thin);

            // Verileri doldur
            var column = 2;
            foreach (var stat in statistics)
            {
                worksheet.Cells[1, column].Value = stat.DepartmentName;
                worksheet.Cells[2, column].Value = stat.TotalCalls;
                worksheet.Cells[3, column].Value = stat.AnsweredCalls;
                worksheet.Cells[4, column].Value = stat.MissedCalls;
                worksheet.Cells[5, column].Value = stat.OnBreakCalls;
                worksheet.Cells[6, column].Value = stat.AnsweredCallRate / 100;

                // Yüzde formatı
                worksheet.Cells[6, column].Style.Numberformat.Format = "0.00%";

                column++;
            }

            // Sütun genişliklerini otomatik ayarla
            worksheet.Cells.AutoFitColumns();

            // Tüm hücrelere border ekle
            var dataRange = worksheet.Cells[1, 1, 6, column - 1];
            dataRange.Style.Border.Top.Style = ExcelBorderStyle.Thin;
            dataRange.Style.Border.Left.Style = ExcelBorderStyle.Thin;
            dataRange.Style.Border.Right.Style = ExcelBorderStyle.Thin;
            dataRange.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
        }

        AddSheet("Incoming", statisticsByCallDirection.Incoming);
        AddSheet("Outgoing", statisticsByCallDirection.Outgoing);
        AddSheet("Internal", statisticsByCallDirection.Internal);

        return package.GetAsByteArray();
    }

    public static byte[] ToExcelFile(this DepartmentCallStatisticsByCallDirection statisticsByCallDirection, List<OperatorBreakSummary> breakSummaries)
    {
        ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

        using var package = new ExcelPackage();

        void AddSheet(string sheetName, IEnumerable<DepartmentCallStatistics> statistics)
        {
            var worksheet = package.Workbook.Worksheets.Add(sheetName);

            worksheet.Cells[1, 1].Value = "Departman Adı";
            worksheet.Cells[2, 1].Value = "Toplam Çağrı";
            worksheet.Cells[3, 1].Value = "Cevaplanan Çağrı";
            worksheet.Cells[4, 1].Value = "Cevapsız Çağrı";
            worksheet.Cells[5, 1].Value = "Molada Gelen Çağrı";
            worksheet.Cells[6, 1].Value = "Cevaplama Oranı (%)";

            var headerRange = worksheet.Cells[1, 1, 6, 1];
            headerRange.Style.Font.Bold = true;
            headerRange.Style.Fill.PatternType = ExcelFillStyle.Solid;
            headerRange.Style.Fill.BackgroundColor.SetColor(Color.LightGray);

            var column = 2;
            foreach (var stat in statistics)
            {
                worksheet.Cells[1, column].Value = stat.DepartmentName;
                worksheet.Cells[2, column].Value = stat.TotalCalls;
                worksheet.Cells[3, column].Value = stat.AnsweredCalls;
                worksheet.Cells[4, column].Value = stat.MissedCalls;
                worksheet.Cells[5, column].Value = stat.OnBreakCalls;
                worksheet.Cells[6, column].Value = stat.AnsweredCallRate / 100;
                worksheet.Cells[6, column].Style.Numberformat.Format = "0.00%";
                column++;
            }

            worksheet.Cells.AutoFitColumns();
            var dataRange = worksheet.Cells[1, 1, 6, column - 1];
            dataRange.Style.Border.Top.Style = ExcelBorderStyle.Thin;
            dataRange.Style.Border.Left.Style = ExcelBorderStyle.Thin;
            dataRange.Style.Border.Right.Style = ExcelBorderStyle.Thin;
            dataRange.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
        }

        AddSheet("Incoming", statisticsByCallDirection.Incoming);
        AddSheet("Outgoing", statisticsByCallDirection.Outgoing);
        AddSheet("Internal", statisticsByCallDirection.Internal);

        // Break summary sheet
        if (breakSummaries.Count > 0)
        {
            var ws = package.Workbook.Worksheets.Add("Mola Özeti");

            // Headers
            ws.Cells[1, 1].Value = "Operatör";
            ws.Cells[1, 2].Value = "Telefon";
            ws.Cells[1, 3].Value = "Mola Sayısı";
            ws.Cells[1, 4].Value = "Toplam Süre (dk)";
            ws.Cells[1, 5].Value = "Mola Başlangıç";
            ws.Cells[1, 6].Value = "Mola Bitiş";
            ws.Cells[1, 7].Value = "Süre (dk)";
            ws.Cells[1, 8].Value = "Sebep";

            var bHeaderRange = ws.Cells[1, 1, 1, 8];
            bHeaderRange.Style.Font.Bold = true;
            bHeaderRange.Style.Fill.PatternType = ExcelFillStyle.Solid;
            bHeaderRange.Style.Fill.BackgroundColor.SetColor(Color.LightGray);

            var row = 2;
            foreach (var opBreak in breakSummaries)
            {
                // Write operator summary row
                ws.Cells[row, 1].Value = opBreak.OperatorName;
                ws.Cells[row, 2].Value = opBreak.PhoneNumber;
                ws.Cells[row, 3].Value = opBreak.BreakCount;
                ws.Cells[row, 4].Value = Math.Round(opBreak.TotalDurationMinutes, 1);

                // Write each break detail
                foreach (var b in opBreak.Breaks)
                {
                    ws.Cells[row, 5].Value = b.StartTime;
                    ws.Cells[row, 5].Style.Numberformat.Format = "yyyy-MM-dd HH:mm";
                    ws.Cells[row, 6].Value = b.EndTime;
                    ws.Cells[row, 6].Style.Numberformat.Format = "yyyy-MM-dd HH:mm";
                    ws.Cells[row, 7].Value = Math.Round(b.DurationMinutes, 1);
                    ws.Cells[row, 8].Value = b.Reason ?? "";
                    row++;
                }

                if (opBreak.Breaks.Count == 0)
                    row++;
            }

            // Border for all data
            if (row > 2)
            {
                var allData = ws.Cells[1, 1, row - 1, 8];
                allData.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                allData.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                allData.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                allData.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
            }

            ws.Cells.AutoFitColumns();
        }

        return package.GetAsByteArray();
    }

    public static byte[] ToExcelFile(this UserSpecificReport report)
    {
        ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

        using var package = new ExcelPackage();

        void AddSheet<T>(string sheetName, IEnumerable<T> data, Action<ExcelWorksheet, IEnumerable<T>> fillSheet)
        {
            var worksheet = package.Workbook.Worksheets.Add(sheetName);
            fillSheet(worksheet, data);
            worksheet.Cells.AutoFitColumns();
        }

        void FillCallDetailsSheet(ExcelWorksheet worksheet, IEnumerable<UserCallListItem> callDetails)
        {
            worksheet.Cells[1, 1].Value = "Süre";
            worksheet.Cells[1, 2].Value = "Çağrı Türü";
            worksheet.Cells[1, 3].Value = "Başlangıç Tarihi";
            worksheet.Cells[1, 4].Value = "Bitiş Tarihi";
            worksheet.Cells[1, 5].Value = "Arayan Numara";
            worksheet.Cells[1, 6].Value = "İlk Aranan Numara";
            worksheet.Cells[1, 7].Value = "Son Aranan Numara";
            worksheet.Cells[1, 8].Value = "Arayan Kullanıcı Adı";
            worksheet.Cells[1, 9].Value = "Arayan Departman Adı";
            worksheet.Cells[1, 10].Value = "İlk Aranan Kullanıcı Adı";
            worksheet.Cells[1, 11].Value = "İlk Aranan Departman Adı";
            worksheet.Cells[1, 12].Value = "Son Aranan Kullanıcı Adı";
            worksheet.Cells[1, 13].Value = "Son Aranan Departman Adı";
            worksheet.Cells[1, 14].Value = "Çağrı Yönü";

            var row = 2;
            foreach (var call in callDetails)
            {
                worksheet.Cells[row, 1].Value = FormatDuration(call.Duration);
                worksheet.Cells[row, 2].Value = call.CallType.ToString();
                worksheet.Cells[row, 3].Value = call.DateTimeOrigination;
                worksheet.Cells[row, 3].Style.Numberformat.Format = "yyyy-mm-dd hh:mm:ss";
                worksheet.Cells[row, 4].Value = call.DateTimeDisconnect;
                worksheet.Cells[row, 4].Style.Numberformat.Format = "yyyy-mm-dd hh:mm:ss";
                worksheet.Cells[row, 5].Value = call.CallingPartyNumber;
                worksheet.Cells[row, 6].Value = call.OriginalCalledPartyNumber;
                worksheet.Cells[row, 7].Value = call.FinalCalledPartyNumber;
                worksheet.Cells[row, 8].Value = call.CallingPartyUserName;
                worksheet.Cells[row, 9].Value = call.CallingPartyDepartmentName;
                worksheet.Cells[row, 10].Value = call.OriginalCalledPartyUserName;
                worksheet.Cells[row, 11].Value = call.OriginalCalledPartyDepartmentName;
                worksheet.Cells[row, 12].Value = call.FinalCalledPartyUserName;
                worksheet.Cells[row, 13].Value = call.FinalCalledPartyDepartmentName;
                worksheet.Cells[row, 14].Value = call.CallDirection.ToString();
                row++;
            }
        }

        void FillStatisticsSheet(ExcelWorksheet worksheet, CallStatistics statistics)
        {
            worksheet.Cells[1, 1].Value = "Toplam Çağrı";
            worksheet.Cells[1, 2].Value = statistics.TotalCalls;

            worksheet.Cells[2, 1].Value = "Gelen Çağrı";
            worksheet.Cells[2, 2].Value = statistics.IncomingCalls;

            worksheet.Cells[3, 1].Value = "Cevaplanan Çağrı";
            worksheet.Cells[3, 2].Value = statistics.AnsweredCalls;

            worksheet.Cells[4, 1].Value = "Cevapsız Çağrı";
            worksheet.Cells[4, 2].Value = statistics.MissedCalls;

            worksheet.Cells[5, 1].Value = "Molada Gelen Çağrı";
            worksheet.Cells[5, 2].Value = statistics.OnBreakCalls;

            worksheet.Cells[6, 1].Value = "Yönlendirilen Çağrı";
            worksheet.Cells[6, 2].Value = statistics.RedirectedCalls;

            worksheet.Cells[7, 1].Value = "Toplam Süre";
            worksheet.Cells[7, 2].Value = statistics.TotalDuration.HasValue ? FormatDuration(statistics.TotalDuration.Value) : "N/A";
        }

        void FillBreakTimesSheet(ExcelWorksheet worksheet, IEnumerable<BreakTime> breakTimes)
        {
            worksheet.Cells[1, 1].Value = "Mola Başlangıcı";
            worksheet.Cells[1, 2].Value = "Mola Bitişi";

            var row = 2;
            foreach (var breakTime in breakTimes)
            {
                worksheet.Cells[row, 1].Value = breakTime.BreakStart;
                worksheet.Cells[row, 1].Style.Numberformat.Format = "yyyy-mm-dd hh:mm:ss";
                worksheet.Cells[row, 2].Value = breakTime.BreakEnd;
                worksheet.Cells[row, 2].Style.Numberformat.Format = "yyyy-mm-dd hh:mm:ss";
                row++;
            }
        }

        AddSheet("Çağrı Detayları", report.CallDetails, FillCallDetailsSheet);
        AddSheet("Mesai Saati İstatistikleri", new[] { report.WorkHoursStatistics }, (ws, data) => FillStatisticsSheet(ws, data.First()));
        AddSheet("Mesai Saati Çağrıları", report.WorkHours, FillCallDetailsSheet);
        AddSheet("Mesai Dışı İstatistikleri", new[] { report.NonWorkHoursStatistics }, (ws, data) => FillStatisticsSheet(ws, data.First()));
        AddSheet("Mesai Dışı Çağrıları", report.NonWorkHours, FillCallDetailsSheet);
        AddSheet("Mola Zamanları", report.BreakTimes, FillBreakTimesSheet);

        return package.GetAsByteArray();
    }

    private static string FormatDuration(int? durationInSeconds)
    {
        if (!durationInSeconds.HasValue)
        {
            return "N/A";
        }

        var timeSpan = TimeSpan.FromSeconds(durationInSeconds.Value);
        var formattedDuration = string.Empty;

        if (timeSpan.TotalHours >= 1)
        {
            formattedDuration += $"{(int)timeSpan.TotalHours}h ";
        }

        if (timeSpan.Minutes > 0 || timeSpan.TotalHours >= 1)
        {
            formattedDuration += $"{timeSpan.Minutes}m ";
        }

        formattedDuration += $"{timeSpan.Seconds}s";

        return formattedDuration.Trim();
    }
}