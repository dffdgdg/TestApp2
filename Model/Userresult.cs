using System;
using System.Collections.Generic;

namespace TestApp.Model;

public partial class Userresult
{
    public int Id { get; set; }

    public int User { get; set; }

    public int Test { get; set; }

    public double Score { get; set; }

    public virtual Test TestNavigation { get; set; } = null!;

    public virtual User UserNavigation { get; set; } = null!;
}
