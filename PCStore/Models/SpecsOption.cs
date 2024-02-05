using System;
using System.Collections.Generic;

namespace PCStore.Models;

public partial class SpecsOption
{
    public int Id { get; set; }

    public int SpecId { get; set; }

    public int ProductId { get; set; }

    public string Value { get; set; } = null!;

    public virtual Product Product { get; set; } = null!;

    public virtual Spec Spec { get; set; } = null!;
}
