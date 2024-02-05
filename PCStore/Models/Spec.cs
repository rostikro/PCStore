using System;
using System.Collections.Generic;

namespace PCStore.Models;

public partial class Spec
{
    public int Id { get; set; }

    public int CategoryId { get; set; }

    public string Name { get; set; } = null!;

    public virtual ProductCategory Category { get; set; } = null!;

    public virtual ICollection<SpecsOption> SpecsOptions { get; set; } = new List<SpecsOption>();
}
