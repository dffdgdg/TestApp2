using System;
using System.Collections.Generic;

namespace TestApp.Model;

public partial class SiteHistory
{
    public int Id { get; set; }

    public int Site { get; set; }

    public DateOnly Date { get; set; }

    public string Description { get; set; } = null!;

    public virtual HistoricalSite SiteNavigation { get; set; } = null!;
}
