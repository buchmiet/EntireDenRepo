using System.Text;

namespace denMethods;

public class AddressGenerator
{
    private static readonly string[] streets =
    {
        "High Street", "Station Road", "Church Road", "Main Street", "Park Road", "London Road",
        "Elm Street", "Baker Street", "Oak Street", "Pine Street", "Cedar Street", "Maple Street",
        "Spruce Street", "Ash Street", "Birch Street", "Cherry Street", "Holly Street", "Walnut Street",
        "Willow Street", "Redwood Street", "Fir Street", "Cypress Street", "Hawthorn Street", "Hemlock Street",
        "Juniper Street", "Linden Street", "Magnolia Street", "Poplar Street", "Sycamore Street",
        "Alder Street", "Beech Street"
    };

    private static readonly string[] cities =
    {
        "London", "Manchester", "Birmingham", "Edinburgh", "Glasgow", "Liverpool",
        "Leeds", "Sheffield", "Bristol", "Newcastle", "Sunderland", "Brighton",
        "Nottingham", "Southampton", "Derby", "Plymouth", "Reading", "Norwich",
        "Aberdeen", "Preston", "St Albans", "Belfast", "Stoke-on-Trent", "Coventry",
        "York", "Exeter", "Oxford", "Cambridge", "Swansea", "Wolverhampton"
    };

    private static readonly string[] postcodes =
    {
        "W1A 1AA", "M1 1AA", "B1 1AA", "EH1 1AA", "G1 1AA", "L1 1AA",
        "LS1 1AA", "S1 1AA", "BS1 1AA", "NE1 1AA", "SR1 1AA", "BN1 1AA",
        "NG1 1AA", "SO1 1AA", "DE1 1AA", "PL1 1AA", "RG1 1AA", "NR1 1AA",
        "AB1 1AA", "PR1 1AA", "AL1 1AA", "BT1 1AA", "ST1 1AA", "CV1 1AA",
        "YO1 1AA", "EX1 1AA", "OX1 1AA", "CB1 1AA", "SA1 1AA", "WV1 1AA"
    };

    private static readonly string[] businesses =
    {
        "Pizzeria Bella", "Smith's Auto Repair", "Green Thumb Nursery", "Baker's Delight", "Elegant Tailors",
        "Techie Gadgets", "Bookworm Library", "Sunshine Cafe", "Pet Paradise", "Fitness Freak Gym",
        "Candlelight Inn", "Oceanview Restaurant", "Mountain Peak Outfitters", "Riverside Bakery", "Sunrise Electronics",
        "Moonlight Cinema", "Rainbow Florist", "Harmony Music Store", "Twinkle Star Daycare", "Evergreen Landscaping",
        "Majestic Jewelers", "Pampered Pooches", "Creative Crafts", "Treasure Trove Antiques", "Speedy Courier Service",
        "Hearty Home Cooking", "Frosty Treats Ice Cream", "Sparkle Clean Laundry", "Dreamscape Designers", "Cozy Corner Cafe",
        "Adventure Travel Agency", "Vibrant Visages Salon", "Wholesome Health Foods", "Flashy Fashion Boutique", "Old Towne Tavern",
        "Seaside Seafood Market", "Royal Roast Coffee Shop", "Bright Future Optometry", "Crispy Crunch Candy Store", "Smooth Sailing Boat Rentals",
        "Urban Oasis Spa", "Gourmet Grains Bakery", "Fresh Start Fitness Center", "Snuggle Inn Bed & Breakfast", "Peaceful Path Yoga Studio"
    };

    private static readonly string[] firstNames =
    {
        "John", "Mary", "Michael", "Sarah", "James", "Emily",
        "Robert", "Jessica", "William", "Elizabeth", "David", "Jennifer",
        "Charles", "Linda", "Joseph"
    };

    private static readonly string[] lastNames =
    {
        "Smith", "Johnson", "Taylor", "Brown", "Wilson", "Evans",
        "Thomas", "Roberts", "Walker", "Wright", "Turner", "Cooper",
        "Martin", "Edwards", "Clark"
    };

    private static Random random = new Random();

    public static string GenerateRandomUKAddress()
    {
        StringBuilder addressBuilder = new StringBuilder();
        if (random.Next(2) == 0)
        {
            // Generate random business name
            addressBuilder.Append(businesses[random.Next(businesses.Length)]);
        }
        else
        {
            // Generate random personal name
            addressBuilder.Append(firstNames[random.Next(firstNames.Length)] + " " + lastNames[random.Next(lastNames.Length)]);
        }

        // Generate random house number
        addressBuilder.AppendLine(random.Next(1, 200).ToString());

        // Generate random street name
        addressBuilder.AppendLine(streets[random.Next(streets.Length)]);

        // Generate random city name
        addressBuilder.AppendLine(cities[random.Next(cities.Length)]);

        // Generate random postcode
        addressBuilder.AppendLine(postcodes[random.Next(postcodes.Length)]);

        // Randomly decide whether to generate a business or personal name for the address
           

        return addressBuilder.ToString();
    }
}