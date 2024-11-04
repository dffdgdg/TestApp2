using System;
using System.Collections.Generic;

namespace TestApp.Model;

public partial class TestQuestionsAnswer
{
    public int? TestId { get; set; }

    public string? TestTitle { get; set; }

    public int? QuestionId { get; set; }

    public string? QuestionText { get; set; }

    public int? QuestionNumber { get; set; }

    public int? AnswerId { get; set; }

    public string? AnswerText { get; set; }

    public bool? IsCorrect { get; set; }
}
