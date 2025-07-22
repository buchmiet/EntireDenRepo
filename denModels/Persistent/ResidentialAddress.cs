using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace denModels.Persistent
{
    public class ResidentialAddress
    {
        public int ResidentialAddressId { get; set; }

        public string? Line1 { get; set; }

        public string? Line2 { get; set; }
        public string? Line3 { get; set; }

        public string? City { get; set; }

        public string CountryCode { get; set; } = null!;

        public string? CountrySubDivisionCode { get; set; }

        public string PostalCode { get; set; } = null!;

        public string? AddressAsAString { get; set; }

        public virtual countrycode CountryCodeNavigation { get; set; } = null!;

        public virtual ICollection<customer> customers { get; set; } = new List<customer>();
    }
}
