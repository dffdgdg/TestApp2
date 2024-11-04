using System;
using System.Collections.Generic;

namespace TestApp.Model;

public partial class District
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string Description { get; set; } = null!;

    public DateOnly FoundingDate { get; set; }

    public byte[]? Image { get; set; }

    public virtual ICollection<HistoricalSite> HistoricalSites { get; set; } = new List<HistoricalSite>();

    public virtual ICollection<Test> Tests { get; set; } = new List<Test>();
}
