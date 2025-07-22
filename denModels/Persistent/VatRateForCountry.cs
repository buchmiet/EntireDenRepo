using System;
using System.Collections.Generic;

namespace denModels.Persistent;

public partial class VatRateForCountry
{
    public int VatRateForCountryId { get; set; }

    public string Code { get; set; } = null!;

    public decimal Rate { get; set; }

    public virtual CodeForCountry CodeNavigation { get; set; } = null!;
}
