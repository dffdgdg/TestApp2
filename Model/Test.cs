using System;
using System.Collections.Generic;

namespace TestApp.Model;

public partial class Test
{
    public int Id { get; set; }

    public string Title { get; set; } = null!;

    public string Description { get; set; } = null!;

    public int District { get; set; }

    public virtual District DistrictNavigation { get; set; } = null!;

    public virtual ICollection<Question> Questions { get; set; } = new List<Question>();

    public virtual ICollection<Userresult> Userresults { get; set; } = new List<Userresult>();
}
