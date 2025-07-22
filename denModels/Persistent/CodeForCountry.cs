using System;
using System.Collections.Generic;

namespace denModels.Persistent;

public partial class CodeForCountry
{
    public string Code { get; set; } = null!;

    public string Name { get; set; } = null!;

    public virtual ICollection<ResidentialAddress> ResidentialAddresses { get; set; } = new List<ResidentialAddress>();

    public virtual ICollection<VatRateForCountry> VatRatesForCountries { get; set; } = new List<VatRateForCountry>();
}
