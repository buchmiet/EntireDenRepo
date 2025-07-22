using DataServicesNET80.Models;


namespace denMethods;

public class MockOrderGenerator
{
    public static Complete GenerateRandom()
    {
        var random = new Random();
        string[] currencies = { "$", "€", "£" };
        string[] productNames = { "Apple", "Pear", "Juice", "Banana", "Grapes", "Orange", "Peach", "Plum", "Pineapple", "Mango", "Melon", "Strawberry", "Raspberry", "Blueberry", "Kiwi" };
        string[] statuses = { "Pending", "Dispatched", "Delivered", "Cancelled" };

        string acurrency = currencies[random.Next(currencies.Length)];
        string scurrency = currencies[random.Next(currencies.Length)];

        var orderItems = new List<orderitem>();
        decimal total = 0;

        int numberOfProducts = random.Next(1, 6);

        for (int i = 0; i < numberOfProducts; i++)
        {
            string itemName = productNames[random.Next(productNames.Length)];
            decimal price =Convert.ToDecimal( Math.Round(random.NextDouble() * 15, 2));
            int quantity = random.Next(1, 11);
            total += price * quantity;
            orderItems.Add(new orderitem { itemName = itemName, price = price, quantity = quantity, itembodyID = i + 1, OrderItemTypeId = random.Next(1, 5), ItemWeight = random.Next(5, 15) });
        }

        var order = new order
        {
            orderID = random.Next(1000, 9999),
            quickbooked = random.Next(0, 2) == 1,
            customerID = random.Next(1000, 9999),
            paidOn = DateTime.Now,
            dispatchedOn = DateTime.Now.AddDays(2),
            tracking = "TRK" + random.Next(1000, 9999),
            market = random.Next(1, 5),
            locationID = random.Next(1, 10),
            salecurrency = scurrency,
            acquiredcurrency = acurrency,
            saletotal = total,
            VAT = random.Next(0, 2) == 1,
            order_notes = "This is a mock order",
            status = statuses[random.Next(statuses.Length)],
            postagePrice = Convert.ToDecimal(Math.Round(random.NextDouble() * 10, 2)),
            postageType = "Standard"
        };

        var customer = new customer
        {
            customerID = order.customerID,
            Title = "Mr.",
            GivenName = "John",
            MiddleName = "A",
            FamilyName = "Doe",
              
            CompanyName = "Mock Company",
            Email = "john.doe@mock.com",
            Phone = "+1234567890",
            DisplayName = "John Doe",
            currency = scurrency,
            customer_notes = "This is a mock customer"
        };

        var billAddr = new billaddr
        {
            billaddrID = random.Next(1000, 9999),
            Line1 = "123 Mock Street",
            Line2 = "Suite 1",
            City = "Mock City",
            CountryCode = "US",
            CountrySubDivisionCode = "CA",
            PostalCode = "90210",
            AddressAsAString = "123 Mock Street, Suite 1, Mock City, CA, 90210"
        };

        return new Complete
        {
            Order = order,
            Customer = customer,
            BillAddr = billAddr,
            OrderItems = orderItems
        };
    }
}