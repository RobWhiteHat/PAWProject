using System;
using System.Collections.Generic;

namespace PAWProject.Data.Models;

public partial class SourceItem
{
    public int Id { get; set; }

    public int? SourceId { get; set; }

    public string? Json { get; set; }

    public DateTime? CreatedAt { get; set; }
}
