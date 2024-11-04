using System;
using System.Collections.Generic;

namespace TestApp.Model;

public partial class Userresult
{
    public int Id { get; set; }

    public int Userid { get; set; }

    public int Testid { get; set; }

    public double Score { get; set; }

    public virtual Test Test { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
