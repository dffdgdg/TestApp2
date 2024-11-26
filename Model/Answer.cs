using System;
using System.Collections.Generic;

namespace TestApp.Model;

public partial class Answer
{
    public int Id { get; set; }

    public int Question { get; set; }

    public string Name { get; set; } = null!;

    public bool IsCorrect { get; set; }

    public virtual Question QuestionNavigation { get; set; } = null!;
}
