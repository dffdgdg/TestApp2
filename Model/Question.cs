using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace TestApp.Model;

public partial class Question
{
    public int Id { get; set; }

    public int? TestId { get; set; }

    public string QuestionText { get; set; } = null!;

    public string? ImageUrl { get; set; }

    public string QuestionType { get; set; } = null!;

    public int? QuestionNumber { get; set; }

    public virtual ICollection<Answer> Answers { get; set; } = new ObservableCollection<Answer>();

    public virtual Test? Test { get; set; }
}
