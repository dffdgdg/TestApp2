namespace TestApp.Model;

public partial class Useranswer
{
    public int Id { get; set; }

    public int Questionid { get; set; }

    public string Answertext { get; set; } = null!;

    public int Userid { get; set; }

    public DateTime? Createdat { get; set; }

    public virtual Question Question { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
