using System;
using System.Collections.Generic;

namespace TestApp.Model;

public partial class HistoricalSite
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public int District { get; set; }

    public string? Description { get; set; }

    public DateOnly? ConstructionDate { get; set; }

    public DateTime? Timestamp { get; set; }

    public byte[]? Image { get; set; }

    public virtual District DistrictNavigation { get; set; } = null!;

    public virtual ICollection<SiteHistory> SiteHistories { get; set; } = new List<SiteHistory>();
}
