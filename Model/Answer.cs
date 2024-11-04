using System;
using System.Collections.Generic;

namespace TestApp.Model;

public partial class Answer
{
    public int Id { get; set; }

    public int? QuestionId { get; set; }

    public string AnswerText { get; set; } = null!;

    public bool? IsCorrect { get; set; }

    public virtual Question? Question { get; set; }
}
