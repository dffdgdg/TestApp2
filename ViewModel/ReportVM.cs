using System.IO;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using Microsoft.EntityFrameworkCore;
using Microsoft.Win32;
using OfficeOpenXml;
using TestApp.Model;

namespace TestApp.ViewModel
{
    public class ReportVM : BaseViewModel
    {
        private readonly TestDbContext _dbContext;

        public ICommand GenerateReportCommand { get; private set; }

        public ReportVM()
        {
            _dbContext = new TestDbContext();
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            GenerateReportCommand = new RelayCommand(async () => await GenerateReportAsync());
        }

        public async Task GenerateReportAsync()
        {
            var saveFileDialog = new SaveFileDialog
            {
                Filter = "Excel (*.xlsx)|*.xlsx",
                Title = "Сохранить отчет"
            };

            if (saveFileDialog.ShowDialog() == true)
            {
                string filePath = saveFileDialog.FileName;

                var testResults = await GetTestResultsAsync();
                GenerateExcelReport(testResults, filePath);

                System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                {
                    FileName = filePath,
                    UseShellExecute = true
                });
            }
        }

        private async Task<List<Tuple<string, int, double, double>>> GetTestResultsAsync()
        {
            var results = await _dbContext.Users
                .Include(u => u.Userresults)
                .Select(u => new
                {
                    AgeGroup = GetAgeGroup(u.Birthdate),
                    TotalTests = u.Userresults.Count(),
                    TotalScore = u.Userresults.Sum(ur => ur.Score) // Максимально 100 за тест
                })
                .ToListAsync();

            // Группируем по возрастной группе и исключаем группы с нулевыми тестами
            var groupedResults = results
                .GroupBy(r => r.AgeGroup)
                .Where(g => g.Sum(x => x.TotalTests) > 0) // Исключаем группы с 0 тестами
                .Select(g =>
                {
                    var totalTests = g.Sum(x => x.TotalTests);
                    var totalScore = g.Sum(x => x.TotalScore);

                    // Рассчитываем процент правильных ответов
                    double percentCorrect = totalTests > 0 ? (totalScore / (double)(totalTests * 100)) * 100 : 0;

                    return new Tuple<string, int, double, double>(
                        g.Key,
                        totalTests,
                        totalScore,
                        percentCorrect // Добавляем процент правильных ответов
                    );
                })
                .ToList();

            return groupedResults;
        }


        private void GenerateExcelReport(List<Tuple<string, int, double, double>> results, string filePath)
        {
            using var package = new ExcelPackage();
            var worksheet = package.Workbook.Worksheets.Add("Отчет");

            worksheet.Cells[1, 1].Value = "Возрастная группа";
            worksheet.Cells[1, 2].Value = "Всего тестов";
            worksheet.Cells[1, 3].Value = "Всего очков";
            worksheet.Cells[1, 4].Value = "Процент правильных ответов"; // Заголовок для нового столбца

            int row = 2;
            foreach (var result in results)
            {
                worksheet.Cells[row, 1].Value = result.Item1; // Возрастная группа
                worksheet.Cells[row, 2].Value = result.Item2; // Всего тестов
                worksheet.Cells[row, 3].Value = result.Item3; // Всего очков
                worksheet.Cells[row, 4].Value = result.Item4; // Процент правильных ответов

                row++;
            }

            // Установка ширины столбцов
            worksheet.Column(1).AutoFit(); // Возрастная группа
            worksheet.Column(2).AutoFit(); // Всего тестов
            worksheet.Column(3).AutoFit(); // Всего очков
            worksheet.Column(4).AutoFit(); // Процент правильных ответов

            // Добавление границ к ячейкам
            using (var range = worksheet.Cells[1, 1, row - 1, 4]) // Задаем диапазон с данными
            {
                range.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin); // Границы вокруг всех ячеек
                range.Style.Border.Top.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                range.Style.Border.Bottom.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                range.Style.Border.Left.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                range.Style.Border.Right.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
            }

            FileInfo excelFile = new(filePath);
            package.SaveAs(excelFile);
        }

        private static string GetAgeGroup(DateOnly dateOfBirth)
        {
            int age = DateTime.Now.Year - dateOfBirth.Year;
            if (dateOfBirth > DateOnly.FromDateTime(DateTime.Now.AddYears(-age))) age--;

            if (age <= 15) return "0-15";
            else if (age <= 25) return "16-25";
            else if (age <= 35) return "26-35";
            else if (age <= 45) return "36-45";
            else if (age <= 60) return "46-60";
            else return "61+";
        }
    }
}