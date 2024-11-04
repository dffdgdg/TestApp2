using System;
using System.Collections.Generic;

namespace TestApp.Model;

public partial class HistoricalSitesInfo
{
    public int? Id { get; set; }

    public string? SiteName { get; set; }

    public string? SiteDescription { get; set; }

    public DateOnly? ConstructionDate { get; set; }

    public string? DistrictName { get; set; }

    public string? DistrictDescription { get; set; }
}
