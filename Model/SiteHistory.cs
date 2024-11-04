using System;
using System.Collections.Generic;

namespace TestApp.Model;

public partial class SiteHistory
{
    public int Id { get; set; }

    public int Site { get; set; }

    public DateOnly EventDate { get; set; }

    public string? EventDescription { get; set; }

    public virtual HistoricalSite SiteNavigation { get; set; } = null!;
}
