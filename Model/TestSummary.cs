using System;
using System.Collections.Generic;

namespace TestApp.Model;

public partial class TestSummary
{
    public int? TestId { get; set; }

    public string? TestTitle { get; set; }

    public string? TestDescription { get; set; }

    public string? DistrictName { get; set; }

    public long? QuestionCount { get; set; }
}
