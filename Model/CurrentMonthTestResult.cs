using System;
using System.Collections.Generic;

namespace TestApp.Model;

public partial class CurrentMonthTestResult
{
    public string? Фио { get; set; }

    public string? Тест { get; set; }

    public double? Баллы { get; set; }

    public DateOnly? Пройдено { get; set; }
}
