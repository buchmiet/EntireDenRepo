using DataServicesNET80.Models;
using Microsoft.EntityFrameworkCore;


namespace DataServicesNET80;

public partial class Time4PartsContext : DbContext
{

    public Time4PartsContext(DbContextOptions<Time4PartsContext> options)
        : base(options)
    {
    }

    public Time4PartsContext()     
    {
    }

    public virtual DbSet<amazonmarketplace> amazonmarketplaces { get; set; }

    public virtual DbSet<asinsku> asinskus { get; set; }

    public virtual DbSet<billaddr> billaddrs { get; set; }

    public virtual DbSet<bodiesgrouped> bodiesgroupeds { get; set; }

    public virtual DbSet<bodyinthebox> bodyintheboxes { get; set; }

    public virtual DbSet<brand> brands { get; set; }

    public virtual DbSet<casioinvoice> casioinvoices { get; set; }

    public virtual DbSet<casioukbackorder> casioukbackorders { get; set; }

    public virtual DbSet<casioukcurrentorder> casioukcurrentorders { get; set; }

    public virtual DbSet<colourtranslation> colourtranslations { get; set; }

    public virtual DbSet<country2rmass> country2rmasses { get; set; }

    public virtual DbSet<countrycode> countrycodes { get; set; }

    public virtual DbSet<currency> currencies { get; set; }

    public virtual DbSet<customer> customers { get; set; }

    public virtual DbSet<deliveryprice> deliveryprices { get; set; }

    public virtual DbSet<group4body> group4bodies { get; set; }

    public virtual DbSet<group4watch> group4watches { get; set; }

    public virtual DbSet<invoicetxn> invoicetxns { get; set; }

    public virtual DbSet<itembody> itembodies { get; set; }

    public virtual DbSet<itemheader> itemheaders { get; set; }

    public virtual DbSet<itmitmassociation> itmitmassociations { get; set; }

    public virtual DbSet<itmmarketassoc> itmmarketassocs { get; set; }

    public virtual DbSet<itmparameter> itmparameters { get; set; }

    public virtual DbSet<keyvalue> keyvalues { get; set; }

    public virtual DbSet<location> locations { get; set; }

    public virtual DbSet<locmarassociation> locmarassociations { get; set; }

    public virtual DbSet<logentry> logentries { get; set; }

    public virtual DbSet<logevent> logevents { get; set; }

    public virtual DbSet<market> markets { get; set; }

    public virtual DbSet<marketplatformassociation> marketplatformassociations { get; set; }

    public virtual DbSet<mayalsofit> mayalsofits { get; set; }

    public virtual DbSet<multidrawer> multidrawers { get; set; }

    public virtual DbSet<order> orders { get; set; }

    public virtual DbSet<orderitem> orderitems { get; set; }

    public virtual DbSet<orderitemtype> orderitemtypes { get; set; }

    public virtual DbSet<orderstatus> orderstatuses { get; set; }

    public virtual DbSet<parameter> parameters { get; set; }

    public virtual DbSet<parametervalue> parametervalues { get; set; }

    public virtual DbSet<part2itemass> part2itemasses { get; set; }

    public virtual DbSet<photo> photos { get; set; }

    public virtual DbSet<platform> platforms { get; set; }

    public virtual DbSet<postagetype> postagetypes { get; set; }

    public virtual DbSet<rmzone> rmzones { get; set; }

    public virtual DbSet<searchentry> searchentries { get; set; }

    public virtual DbSet<shopitem> shopitems { get; set; }

    public virtual DbSet<stockshot> stockshots { get; set; }

    public virtual DbSet<supplier> suppliers { get; set; }

    public virtual DbSet<token> tokens { get; set; }

    public virtual DbSet<type> types { get; set; }

    public virtual DbSet<typeparassociation> typeparassociations { get; set; }

    public virtual DbSet<vatrate> vatrates { get; set; }


    public virtual DbSet<watch> watches { get; set; }

    public virtual DbSet<watchesgrouped> watchesgroupeds { get; set; }

    public virtual DbSet<xrate> xrates { get; set; }

    public virtual DbSet<zibiinvoice> zibiinvoices { get; set; }

    public virtual DbSet<OauthToken> OauthTokens { get; set; }
    public virtual DbSet<OauthService> OauthServices { get; set; }
    public virtual DbSet<OauthTokenWithService> OauthTokensWithService { get; set; }

    public Dictionary<System.Type, List<string>> CreateDictionary()
    {
        // Tworzenie słownika
        Dictionary<System.Type, List<string>> dictionary = new Dictionary<System.Type, List<string>>
        {
            // Dodawanie DbSet i odpowiadających im stringów
            { typeof(DbSet<countrycode>),new List<string>{ @"CREATE TABLE IF NOT EXISTS `countrycode` (
  `code` varchar(2) NOT NULL,
  `name` text NOT NULL,
  PRIMARY KEY (`code`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4  COLLATE=utf8mb4_unicode_ci;
" } },
            { typeof(DbSet<rmzone>),new List<string>{ @"CREATE TABLE IF NOT EXISTS `rmzones` (
  `RMZoneId` int(11) NOT NULL AUTO_INCREMENT,
  `Zone` varchar(10) NOT NULL,
  PRIMARY KEY (`RMZoneId`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci
" }},
            { typeof(DbSet<country2rmass>),new List<string>{ @"CREATE TABLE IF NOT EXISTS `country2rmass` (
  `Country2RMAssId` int(11) NOT NULL AUTO_INCREMENT,
  `code` varchar(2) NOT NULL,
  `RMZoneID` int(11) NOT NULL,
  PRIMARY KEY (`Country2RMAssId`),
  UNIQUE KEY `uq_code` (`code`),
  CONSTRAINT `FK_country2rmass_code` FOREIGN KEY (`code`) REFERENCES `countrycode` (`code`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
" } },


            { typeof(DbSet<brand>),new List<string>{ @"CREATE TABLE IF NOT EXISTS `brands` (
  `brandID` int NOT NULL AUTO_INCREMENT,
  `name` varchar(50) DEFAULT NULL,
  PRIMARY KEY (`brandID`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;
" } },
            { typeof(DbSet<currency>),new List<string>{ @"CREATE TABLE IF NOT EXISTS `Currency` (
  `code` varchar(3) NOT NULL,
  `name` text NOT NULL,
 `symbol` text NOT NULL,
  PRIMARY KEY (`code`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

" } },

            { typeof(DbSet<deliveryprice>),new List<string>{ @"CREATE TABLE IF NOT EXISTS DeliveryPrice (
  DeliveryPriceId INT NOT NULL AUTO_INCREMENT,
  code CHAR(4) NOT NULL,
  name VARCHAR(50) NOT NULL,
  price DECIMAL(18, 2) NOT NULL,
  PRIMARY KEY (DeliveryPriceId)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

" } },

            { typeof(DbSet<market>),new List<string>{ @"CREATE TABLE IF NOT EXISTS market (
  marketID INT NOT NULL AUTO_INCREMENT,
  name TEXT,
  IsInUse TINYINT(1) NOT NULL,
 `currency` CHAR(3) NOT NULL,
  PRIMARY KEY (marketID),
 CONSTRAINT `FK_market_Currency` FOREIGN KEY (`currency`) REFERENCES `Currency` (`code`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

" } },
            { typeof(DbSet<platform>),new List<string>{ @"
CREATE TABLE IF NOT EXISTS `platforms` (
  `platformID` INT NOT NULL AUTO_INCREMENT,
  `name` VARCHAR(50) NULL,
  `Description` TEXT NULL,
  PRIMARY KEY (`platformID`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
" } },
            { typeof(DbSet<xrate>), new List<string>{ @"
CREATE TABLE IF NOT EXISTS `Xrate` (
  `XrateId` INT NOT NULL AUTO_INCREMENT,
  `date` DATETIME NOT NULL,
  `rate` DECIMAL(18,0) NOT NULL,
  `code` CHAR(3) NOT NULL,
  `SourceCurrencyCode` CHAR(3) NOT NULL,
  PRIMARY KEY (`XrateId`),
 CONSTRAINT `FK_Xrate_Currency` FOREIGN KEY (`code`) REFERENCES `Currency` (`code`),
CONSTRAINT `FK_Xrate_Currency2` FOREIGN KEY (`SourceCurrencyCode`) REFERENCES `Currency` (`code`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
" } },



            { typeof(DbSet<location>), new List<string>{ @"
CREATE TABLE IF NOT EXISTS `Location` (
  `locationID` INT NOT NULL AUTO_INCREMENT,
  `name` TEXT NOT NULL,
  `currency` VARCHAR(3),
  `active` TINYINT(1),
  PRIMARY KEY (`locationID`),
  CONSTRAINT `FK_Location_Currency` FOREIGN KEY (`currency`) REFERENCES `Currency` (`code`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

" } },
            { typeof(DbSet<locmarassociation>), new List<string>{@"
CREATE TABLE IF NOT EXISTS `locMarAssociation` (
  `loc` INT NOT NULL,
  `reference` INT NOT NULL,
  `pos` INT NOT NULL,
  PRIMARY KEY (`loc`, `reference`),
  CONSTRAINT `fk_locMarAssociation_market` FOREIGN KEY (`reference`) REFERENCES `market` (`marketID`),
  CONSTRAINT `fk_locMarAssociation_location` FOREIGN KEY (`loc`) REFERENCES `Location` (`locationID`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
" } },
            { typeof(DbSet<postagetype>), new List<string>{@"
CREATE TABLE IF NOT EXISTS `PostageTypes` (
  `code` VARCHAR(5) NOT NULL,
  `name` VARCHAR(50) NOT NULL,
  PRIMARY KEY (`code`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
" } },
            { typeof(DbSet<countryvatrrate>), new List<string>{ @"
CREATE TABLE IF NOT EXISTS `CountryVatRrate` (
  `Countryvatrateid` INT NOT NULL AUTO_INCREMENT,
  `code` VARCHAR(2) NOT NULL,
  `rate` DECIMAL(18,0) NOT NULL, 
  PRIMARY KEY (`Countryvatrateid`),
  CONSTRAINT `FK_VatRrate_CountryCode` FOREIGN KEY (`code`) REFERENCES `CountryCode` (`code`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
" } },

            { typeof(DbSet<orderstatus>), new List<string>{@"
CREATE TABLE IF NOT EXISTS `OrderStatus` (
  `code` VARCHAR(4) NOT NULL,
  `name` TEXT NOT NULL,
  PRIMARY KEY (`code`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
" } },
            { typeof(DbSet<supplier>), new List<string>{ @"
CREATE TABLE IF NOT EXISTS `suppliers` (
  `supplierID` INT NOT NULL AUTO_INCREMENT,
  `name` TEXT,
 `currency` CHAR(3) NOT NULL,
  PRIMARY KEY (`supplierID`),
 CONSTRAINT `FK_Supplier_Currency` FOREIGN KEY (`currency`) REFERENCES `Currency` (`code`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
" } },
            { typeof(DbSet<orderitemtype>), new List<string>{ @"
CREATE TABLE IF NOT EXISTS `OrderItemType` (
  `OrderItemTypeId` INT NOT NULL AUTO_INCREMENT,
  `name` TEXT NOT NULL,
  PRIMARY KEY (`OrderItemTypeId`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
" } },
            { typeof(DbSet<marketplatformassociation>), new List<string>{@"
CREATE TABLE IF NOT EXISTS `MarketPlatformAssociation` (
  `marketID` INT NOT NULL,
  `platformID` INT NOT NULL,
  `MarketPlatformAssociationID` INT NOT NULL AUTO_INCREMENT,
  PRIMARY KEY (`MarketPlatformAssociationID`),
  INDEX `idx_marketID` (`marketID`),
  INDEX `idx_platformID` (`platformID`),
  CONSTRAINT `FK_MarketPlatformAssociation_market` FOREIGN KEY (`marketID`) REFERENCES `market` (`marketID`),
  CONSTRAINT `FK_MarketPlatformAssociation_platforms` FOREIGN KEY (`platformID`) REFERENCES `platforms` (`platformID`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
" } },
            { typeof(DbSet<amazonmarketplace>), new List<string>{@"
CREATE TABLE IF NOT EXISTS `AmazonMarketplace` (
  `Id` INT NOT NULL AUTO_INCREMENT,
  `locationID` INT NOT NULL,
  `marketID` INT NOT NULL,
  `code` VARCHAR(50) NOT NULL,
  PRIMARY KEY (`Id`),
  INDEX `idx_locationID` (`locationID`),
  INDEX `idx_marketID` (`marketID`),
  CONSTRAINT `FK_AmazonMarketplace_Location` FOREIGN KEY (`locationID`) REFERENCES `Location` (`locationID`),
  CONSTRAINT `FK_AmazonMarketplace_market` FOREIGN KEY (`marketID`) REFERENCES `market` (`marketID`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
" } },
            { typeof(DbSet<asinsku>), new List<string>{@"
CREATE TABLE IF NOT EXISTS `AsinSku` (
  `asinskuID` INT NOT NULL AUTO_INCREMENT,
  `asin` VARCHAR(50) NOT NULL,
  `sku` VARCHAR(50) NOT NULL,
  `locationID` INT NOT NULL,
  PRIMARY KEY (`asinskuID`),
  UNIQUE INDEX `UQ_AsinSku_asin_sku` (`asin`, `sku`),
  INDEX `idx_locationID` (`locationID`),
  CONSTRAINT `FK_AsinSku_Location` FOREIGN KEY (`locationID`) REFERENCES `Location` (`locationID`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
" } },
            { typeof(DbSet<billaddr>), new List<string>{ @"
CREATE TABLE IF NOT EXISTS `BillAddr` (
  `billaddrID` INT NOT NULL AUTO_INCREMENT,
  `Line1` TEXT NULL,
  `Line2` TEXT NULL,
  `City` TEXT NULL,
  `CountryCode` VARCHAR(2) NOT NULL,
  `CountrySubDivisionCode` TEXT NULL,
  `PostalCode` TEXT NOT NULL,
  `AddressAsAString` TEXT NULL,
  PRIMARY KEY (`billaddrID`),
  INDEX `IX_BillAddr_CountryCode` (`CountryCode`),
  CONSTRAINT `FK_BillAddr_CountryCode` FOREIGN KEY (`CountryCode`) REFERENCES `CountryCode` (`code`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
" } },
            { typeof(DbSet<parameter>), new List<string>{@"
CREATE TABLE IF NOT EXISTS `parameters` (
  `parameterID` INT NOT NULL AUTO_INCREMENT,
  `name` TEXT NOT NULL,
  PRIMARY KEY (`parameterID`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
" } },
            { typeof(DbSet<customer>), new List<string>{ @"
CREATE TABLE IF NOT EXISTS `Customer` (
  `customerID` int NOT NULL AUTO_INCREMENT,
  `Title` varchar(50),
  `GivenName` varchar(255),
  `MiddleName` varchar(255),
  `FamilyName` varchar(255),
  `CompanyName` varchar(255),
  `Email` varchar(255),
  `Phone` varchar(255),
  `DisplayName` varchar(255),
  `currency` varchar(3),
  `customer_notes` text,
  `billaddrID` int,
  PRIMARY KEY (`customerID`),
  INDEX `IX_Customer_BillAddrID` (`billaddrID`),
  CONSTRAINT `FK_Customer_BillAddr` FOREIGN KEY (`billaddrID`) REFERENCES `BillAddr` (`billaddrID`),
  CONSTRAINT `FK_Customer_Currency` FOREIGN KEY (`currency`) REFERENCES `Currency` (`code`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
" } },
            { typeof(DbSet<keyvalue>), new List<string>{@"
CREATE TABLE IF NOT EXISTS `keyvalue` (
  `keyvalueID` INT NOT NULL AUTO_INCREMENT,
  `key` TEXT NOT NULL,
  `value` TEXT NOT NULL,
  `timestamp` DATETIME NOT NULL,
  PRIMARY KEY (`keyvalueID`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
" } },
            { typeof(DbSet<type>),new List<string>{ @"
CREATE TABLE IF NOT EXISTS `Types` (
  `typeID` INT NOT NULL AUTO_INCREMENT,
  `name` VARCHAR(50) DEFAULT NULL,
  PRIMARY KEY (`typeID`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
" } },

            { typeof(DbSet<vatrate>), new List<string>{@"
CREATE TABLE IF NOT EXISTS `VATRates` (
  `VATRateID` INT NOT NULL AUTO_INCREMENT,
  `Rate` DECIMAL(5, 2) NOT NULL,
  `VATDescription` VARCHAR(255) NOT NULL,
  PRIMARY KEY (`VATRateID`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
" } },
            { typeof(DbSet<searchentry>), new List<string>{@"
CREATE TABLE IF NOT EXISTS `searchentry` (
  `searchentryId` INT NOT NULL AUTO_INCREMENT,
  `searchPhrase` TEXT NOT NULL,
  `searchTimeStamp` DATETIME NOT NULL,
  PRIMARY KEY (`searchentryId`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
" } },
            { typeof(DbSet<itembody>), new List<string>{ @"
CREATE TABLE IF NOT EXISTS `itemBody` (
  `itembodyID` INT NOT NULL AUTO_INCREMENT,
  `brandID` INT NOT NULL,
  `name` TEXT NOT NULL,
  `myname` TEXT NOT NULL,
  `mpn` TEXT NOT NULL,
  `visible` TINYINT(1) NOT NULL,
  `description` TEXT NULL,
  `readyTotrack` TINYINT(1) NOT NULL,
  `typeId` INT NOT NULL,
  `logoPic` TEXT NULL,
  `packagePic` TEXT NULL,
  `fullsearchterm` TEXT NULL,
  `weight` INT NOT NULL,
  PRIMARY KEY (`itembodyID`),
  CONSTRAINT `FK_itemBody_brandID` FOREIGN KEY (`brandID`) REFERENCES `brands` (`brandID`),
  CONSTRAINT `FK_itemBody_type` FOREIGN KEY (`typeId`) REFERENCES `Types` (`typeID`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
" } },
            { typeof(DbSet<itmitmassociation>), new List<string>{@"
CREATE TABLE IF NOT EXISTS `itmitmAssociation` (
  `itmitmassID` INT NOT NULL AUTO_INCREMENT,
  `sourceBody` INT NOT NULL,
  `targetBody` INT NOT NULL,
  PRIMARY KEY (`itmitmassID`),
  CONSTRAINT `FK_itmitmAssociation_sourceBody` FOREIGN KEY (`sourceBody`) REFERENCES `itemBody` (`itembodyID`),
  CONSTRAINT `FK_itmitmAssociation_targetBody` FOREIGN KEY (`targetBody`) REFERENCES `itemBody` (`itembodyID`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
" } },
            { typeof(DbSet<itmmarketassoc>), new List<string>{@"
CREATE TABLE IF NOT EXISTS `itmMarketAssoc` (
  `itmmarketassID` INT NOT NULL AUTO_INCREMENT,
  `itembodyID` INT NOT NULL,
  `marketID` INT NOT NULL,
  `quantitySold` INT NOT NULL,
  `soldWith` INT DEFAULT NULL,
  `locationID` INT NOT NULL,
  `itemNumber` VARCHAR(255) NOT NULL,
  `SEName` VARCHAR(255) DEFAULT NULL,
  PRIMARY KEY (`itmmarketassID`),
  CONSTRAINT `FK_itmMarketAssoc_itembodyID` FOREIGN KEY (`itembodyID`) REFERENCES `itemBody` (`itembodyID`) ON DELETE CASCADE,
  CONSTRAINT `FK_itmMarketAssoc_locationID` FOREIGN KEY (`locationID`) REFERENCES `Location` (`locationID`),
  CONSTRAINT `FK_itmMarketAssoc_marketID` FOREIGN KEY (`marketID`) REFERENCES `market` (`marketID`),
  CONSTRAINT `FK_itmMarketAssoc_soldWith` FOREIGN KEY (`soldWith`) REFERENCES `itemBody` (`itembodyID`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
" } },
            { typeof(DbSet<order>), new List<string>{ @"
CREATE TABLE IF NOT EXISTS `Order` (
  `orderID` INT NOT NULL AUTO_INCREMENT,
  `quickbooked` TINYINT(1) NOT NULL,
  `customerID` INT NOT NULL,
  `paidOn` DATETIME NOT NULL,
  `dispatchedOn` DATETIME NOT NULL,
  `tracking` TEXT NOT NULL,
  `market` INT NOT NULL,
  `locationID` INT NOT NULL,
  `salecurrency` VARCHAR(3),
  `acquiredcurrency` VARCHAR(3),
  `saletotal` DECIMAL(18, 0) NOT NULL,
  `xchgrate` DECIMAL(18, 0) NOT NULL,
  `VAT` TINYINT(1) NOT NULL,
  `order_notes` TEXT NULL,
  `status` VARCHAR(4) NOT NULL,
  `postagePrice` DECIMAL(18, 0) NOT NULL,
  `postageType` VARCHAR(5) NULL,
  `VATRateID` INT NOT NULL,
  PRIMARY KEY (`orderID`),
  INDEX `IX_Order_CustomerID` (`customerID`),
  CONSTRAINT `FK_Order_currency` FOREIGN KEY (`salecurrency`) REFERENCES `Currency` (`code`),
  CONSTRAINT `FK_Order_Currency2` FOREIGN KEY (`acquiredcurrency`) REFERENCES `Currency` (`code`),
  CONSTRAINT `FK_Order_customerID` FOREIGN KEY (`customerID`) REFERENCES `Customer` (`customerID`),
  CONSTRAINT `FK_Order_locationID` FOREIGN KEY (`locationID`) REFERENCES `Location` (`locationID`),
  CONSTRAINT `FK_Order_market` FOREIGN KEY (`market`) REFERENCES `market` (`marketID`),
  CONSTRAINT `FK_Order_postageType` FOREIGN KEY (`postageType`) REFERENCES `PostageTypes` (`code`),
  CONSTRAINT `FK_Order_status` FOREIGN KEY (`status`) REFERENCES `OrderStatus` (`code`),
  CONSTRAINT `FK_Order_VATRates` FOREIGN KEY (`VATRateID`) REFERENCES `VATRates` (`VATRateID`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

" } },
            {
                typeof(DbSet<watch>),
                new List<string>{ @"CREATE TABLE IF NOT EXISTS `watch` (
        `watchID` INT NOT NULL AUTO_INCREMENT,
        `name` TEXT NOT NULL,
        `haspic` TINYINT(1) NOT NULL,
        `searchterm` TEXT NOT NULL,
        `watchCode` VARCHAR(50) NULL,
        PRIMARY KEY (`watchID`)
    ) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;"
                } },{
                typeof(DbSet<typeparassociation>),
                new List<string>{ @"CREATE TABLE IF NOT EXISTS `typeParAssociation` (
        `typeID` INT NOT NULL,
        `parameterID` INT NOT NULL,
        `pos` INT NOT NULL,
        PRIMARY KEY (`typeID`, `parameterID`),
        CONSTRAINT `FK_typeParAssociation_parameterID` FOREIGN KEY (`parameterID`) REFERENCES `parameters` (`parameterID`),
        CONSTRAINT `FK_typeParAssociation_typeID` FOREIGN KEY (`typeID`) REFERENCES `Types` (`typeID`) ON DELETE CASCADE
    ) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;"
                } },
            {
                typeof(DbSet<token>),
                new List<string>{ @"CREATE TABLE IF NOT EXISTS `Tokens` (
        `Id` INT NOT NULL,
        `ebayToken` TEXT NULL,
        `quickBooksToken` TEXT NULL,
        `quickBooksRefresh` TEXT NULL,
        `AmAccessKey` TEXT NULL,
        `AmSecret` TEXT NULL,
        `quickBooksAuthS` TEXT NULL,
        `ebayOauthToken` TEXT NULL,
        `ebayRefreshToken` TEXT NULL,
        `paypalToken` TEXT NULL,
        `locationID` INT NULL,
        `client_id` TEXT NULL,
        `secret_id` TEXT NULL,
        `DevID` TEXT NULL,
        `AppID` TEXT NULL,
        `CertID` TEXT NULL,
        `AmazonSPAPIToken` TEXT NULL,
        `AmazonSPAPIRefreshToken` TEXT NULL,
        `AmazonSPAPIClientID` TEXT NULL,
        `AmazonSPAPIClientSecret` TEXT NULL,
        PRIMARY KEY (`Id`)
    ) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;"
                } },{
                typeof(DbSet<stockshot>),
                new List<string>{ @"CREATE TABLE IF NOT EXISTS `stockShot` (
        `StockShotID` INT NOT NULL AUTO_INCREMENT,
        `bodyid` INT NOT NULL,
        `quantity` INT NOT NULL,
        `date` DATETIME NOT NULL,
        `locationID` INT NOT NULL,
        PRIMARY KEY (`StockShotID`),
        CONSTRAINT `FK_stockShot_bodyid` FOREIGN KEY (`bodyid`) REFERENCES `itemBody` (`itembodyID`),
        CONSTRAINT `FK_stockShot_location` FOREIGN KEY (`locationID`) REFERENCES `Location` (`locationID`)
    ) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;"
                } },
            {
                typeof(DbSet<shopitem>),
                new List<string>{ @"CREATE TABLE IF NOT EXISTS `shopitem` (
        `shopitemId` INT NOT NULL AUTO_INCREMENT,
        `price` DECIMAL(18, 0) NOT NULL,
        `itembodyID` INT NOT NULL,
        `locationID` INT NOT NULL,
        `quantity` INT NOT NULL,
        `currencyCode` VARCHAR(3) NOT NULL,
        `active` TINYINT(1) NOT NULL,
        `soldQuantity` INT DEFAULT NULL,
        PRIMARY KEY (`shopitemId`),
        UNIQUE KEY `UK_shopitem_itembodyID` (`itembodyID`),
        CONSTRAINT `FK_shopitem_currencyCode` FOREIGN KEY (`currencyCode`) REFERENCES `Currency` (`code`),
        CONSTRAINT `FK_shopitem_itembodyID` FOREIGN KEY (`itembodyID`) REFERENCES `itemBody` (`itembodyID`) ON DELETE CASCADE,
        CONSTRAINT `FK_shopitem_locationID` FOREIGN KEY (`locationID`) REFERENCES `Location` (`locationID`)
    ) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;"
                } },

            { typeof(DbSet<part2itemass>), new List<string>{ @"
CREATE TABLE IF NOT EXISTS `part2itemass` (
  `part2itemassID` int NOT NULL AUTO_INCREMENT,
  `itembodyID` int NOT NULL,
  `watchID` int NOT NULL,
  `watchsearchterm` longtext NOT NULL, -- 'nvarchar(max)' is equivalent to 'longtext' in MySQL
  PRIMARY KEY (`part2itemassID`),
  CONSTRAINT `FK_part2itemass_itembody` FOREIGN KEY (`itembodyID`) REFERENCES `itemBody` (`itembodyID`),
  CONSTRAINT `FK_part2itemass_watch` FOREIGN KEY (`watchID`) REFERENCES `watch` (`watchID`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
" } },

            { typeof(DbSet<multidrawer>), new List<string>{ @"
CREATE TABLE IF NOT EXISTS `MultiDrawer` (
  `MultiDrawerID` int NOT NULL AUTO_INCREMENT,
  `rows` int NOT NULL,
  `columns` int NOT NULL,
  `name` longtext NOT NULL,
  PRIMARY KEY (`MultiDrawerID`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
" } },
            { typeof(DbSet<logentry>), new List<string>{ @"
CREATE TABLE IF NOT EXISTS `logentry` (
  `logentryId` int NOT NULL AUTO_INCREMENT,
  `event` longtext NOT NULL,
  `eventdate` datetime NOT NULL,
  PRIMARY KEY (`logentryId`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
" } },
            { typeof(DbSet<itemheader>), new List<string>{ @"
CREATE TABLE IF NOT EXISTS `itemHeader` (
    `itemheaderID` int NOT NULL AUTO_INCREMENT,
    `locationID` int NOT NULL,
    `purchasedOn` datetime NOT NULL,
    `supplierID` int NOT NULL,
    `itembodyID` int NOT NULL,
    `pricePaid` DECIMAL(18, 0) NOT NULL,
    `purchasecurrency` VARCHAR(3),
    `acquiredcurrency` VARCHAR(3),
    `xchgrate` DECIMAL(18, 0) NOT NULL,
    `quantity` int NOT NULL,
`VATRateID` INT NOT NULL,
    PRIMARY KEY (`itemheaderID`),
    CONSTRAINT `FK_itemHeader_itemBody` FOREIGN KEY (`itembodyID`) REFERENCES `itemBody` (`itembodyID`) ON DELETE CASCADE,
    CONSTRAINT `FK_itemHeader_Location` FOREIGN KEY (`locationID`) REFERENCES `Location` (`locationID`),
    CONSTRAINT `FK_itemheader_currency` FOREIGN KEY (`purchasecurrency`) REFERENCES `Currency` (`code`),
    CONSTRAINT `FK_itemheader2_currency` FOREIGN KEY (`acquiredcurrency`) REFERENCES `Currency` (`code`),
    CONSTRAINT `FK_itemHeader_suppliers` FOREIGN KEY (`supplierID`) REFERENCES `suppliers` (`supplierID`),
CONSTRAINT `FK_itemheader_VATRates` FOREIGN KEY (`VATRateID`) REFERENCES `VATRates` (`VATRateID`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
",
                @"
CREATE TRIGGER IF NOT EXISTS produktysklepowe
AFTER UPDATE
ON itemheader FOR EACH ROW
BEGIN
    DECLARE new_quantity INT;

    -- Przypisujemy nową sumę do zmiennej `new_quantity`
    SELECT SUM(quantity) INTO new_quantity 
    FROM itemHeader 
    WHERE itembodyID = NEW.itembodyID AND locationID = NEW.locationID;

    -- Aktualizujemy `shopitem` nową sumą
    UPDATE shopitem 
    SET quantity = new_quantity
    WHERE itembodyID = NEW.itembodyID AND locationID = NEW.locationID;
END;"

                ,@"

CREATE TRIGGER IF NOT EXISTS produktysklepowe2
AFTER INSERT ON itemHeader
FOR EACH ROW
BEGIN
    DECLARE price DECIMAL(18, 0);
    DECLARE itembodyid INT;
    DECLARE loka INT;
    DECLARE qua INT DEFAULT 0;
    DECLARE ilosc INT DEFAULT 0;

    -- Przypisujemy wartości z nowo wstawionego wiersza do zmiennych
    SET price = NEW.pricePaid;
    SET itembodyid = NEW.itembodyID;
    SET loka = NEW.locationID;

    -- Obliczamy sumę i ilość dla danego `itembodyID`
    SELECT SUM(quantity) INTO qua FROM itemHeader WHERE itembodyID = itembodyid;
    SELECT COUNT(*) INTO ilosc FROM itemHeader WHERE itembodyID = itembodyid;

    IF ilosc > 1 THEN
        -- Aktualizujemy istniejący wpis w `shopitem`
        UPDATE shopitem
        SET quantity = qua
        WHERE itembodyID = itembodyid;
    ELSE
        -- Wstawiamy nowy wpis do `shopitem`
        INSERT INTO shopitem (price, itembodyID, quantity, locationID, currencyCode, active, soldQuantity)
        VALUES (price, itembodyid, qua, loka, 'GBP', 0, 1);
    END IF;
END;"

            } },{
                typeof(DbSet<zibiinvoice>),new List<string>{
                    @"CREATE TABLE IF NOT EXISTS `zibiinvoice` (
        `zibiinvoiceId` INT NOT NULL AUTO_INCREMENT,
        `mpn` VARCHAR(8) NOT NULL,
        `name` TEXT NULL,
        `price` DECIMAL(18, 0) NOT NULL,
        `discount` INT NOT NULL,
        `priceAfterD` DECIMAL(18, 0) NOT NULL,
        `vat` DECIMAL(18, 0) NOT NULL,
        `purchasedOn` DATETIME NOT NULL,
        `quantity` INT NOT NULL,
        PRIMARY KEY (`zibiinvoiceId`)
    ) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;"
                } },
            { typeof(DbSet<casioinvoice>),new List<string>{ @"
CREATE TABLE IF NOT EXISTS `CasioInvoice` (
  `CasioInvoiceId` INT NOT NULL AUTO_INCREMENT,
  `date` DATETIME NOT NULL,
  `mpn` CHAR(8) NOT NULL,
  `price` DECIMAL(18, 0) NULL,
  `quantity` INT NOT NULL,
  PRIMARY KEY (`CasioInvoiceId`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
"
            } },{ typeof(DbSet<casioukbackorder>), new List<string> { @"
CREATE TABLE IF NOT EXISTS `casioUKbackorder` (
  `casioUKbackorderId` INT NOT NULL AUTO_INCREMENT,
  `mpn` CHAR(8) NOT NULL,
  `quantity` INT NOT NULL,
  `orderedon` DATETIME NOT NULL,
  PRIMARY KEY (`casioUKbackorderId`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
" } },
            { typeof(DbSet<casioukcurrentorder>), new List<string> { @"
CREATE TABLE IF NOT EXISTS `casioUKcurrentOrder` (
  `casioUKcurrentOrderId` INT NOT NULL AUTO_INCREMENT,
  `mpn` CHAR(20) NOT NULL,
  `quantity` INT NOT NULL,
  PRIMARY KEY (`casioUKcurrentOrderId`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
" } },
            { typeof(DbSet<parametervalue>), new List<string> { @"
CREATE TABLE IF NOT EXISTS `parameterValues` (
  `parameterValueID` INT NOT NULL AUTO_INCREMENT,
  `parameterID` INT NOT NULL,
  `name` TEXT NOT NULL,
  `pos` INT NOT NULL,
  PRIMARY KEY (`parameterValueID`),
  INDEX `fk_parameterID_idx` (`parameterID`),
  CONSTRAINT `FK_cechyValues_parameterID` FOREIGN KEY (`parameterID`) REFERENCES `parameters` (`parameterID`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
" } },
            { typeof(DbSet<colourtranslation>), new List<string> { @"
CREATE TABLE IF NOT EXISTS `ColourTranslation` (
  `Id` INT NOT NULL AUTO_INCREMENT,
  `kodKoloru` INT NOT NULL,
  `schemat` VARCHAR(50) NOT NULL,
  `col1` INT NOT NULL,
  `col2` INT NULL,
  `col3` INT NULL,
  `col4` INT NULL,
  PRIMARY KEY (`Id`),
  CONSTRAINT `FK_ColourTranslation_kodKoloru` FOREIGN KEY (`kodKoloru`) REFERENCES `parameterValues` (`parameterValueID`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
" } },
            { typeof(DbSet<group4body>), new List<string> { @"
CREATE TABLE IF NOT EXISTS `group4bodies` (
  `group4bodiesID` INT NOT NULL AUTO_INCREMENT,
  `name` TEXT NULL,
  PRIMARY KEY (`group4bodiesID`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
" } },

            { typeof(DbSet<invoicetxn>), new List<string> { @"
CREATE TABLE IF NOT EXISTS `invoiceTXNs` (
  `invoiceTXNID` INT NOT NULL AUTO_INCREMENT,
  `platformID` INT NOT NULL,
  `qbInvoiceId` TEXT NULL,
  `platformTXN` TEXT NULL,
  `orderID` INT NOT NULL,
  `marketID` INT NOT NULL,
  PRIMARY KEY (`invoiceTXNID`),
  CONSTRAINT `FK_invoiceTXNs_market` FOREIGN KEY (`marketID`) REFERENCES `market` (`marketID`),
  CONSTRAINT `FK_invoiceTXNs_order` FOREIGN KEY (`orderID`) REFERENCES `Order` (`orderID`) ON DELETE CASCADE,
  CONSTRAINT `FK_invoiceTXNs_platforms` FOREIGN KEY (`platformID`) REFERENCES `platforms` (`platformID`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
" } },

            { typeof(DbSet<logevent>), new List<string> { @"
CREATE TABLE IF NOT EXISTS `logevent` (
  `logeventID` INT NOT NULL AUTO_INCREMENT,
  `happenedOn` DATETIME NOT NULL,
  `itemHeaderID` INT NULL,
  `event` TEXT NOT NULL,
  `itemBodyID` INT NOT NULL,
  `marketID` INT NULL,
  PRIMARY KEY (`logeventID`),
  CONSTRAINT `FK_logevent_itemBody` FOREIGN KEY (`itemBodyID`) REFERENCES `itemBody` (`itembodyID`),
  CONSTRAINT `FK_logevent_itemHeader` FOREIGN KEY (`itemHeaderID`) REFERENCES `itemHeader` (`itemheaderID`) ON DELETE CASCADE,
  CONSTRAINT `FK_logevent_market` FOREIGN KEY (`marketID`) REFERENCES `market` (`marketID`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
" } },

            { typeof(DbSet<bodiesgrouped>), new List<string> { @"
CREATE TABLE IF NOT EXISTS `bodiesGrouped` (
  `bodiesGroupedID` INT NOT NULL AUTO_INCREMENT,
  `group4bodiesID` INT NOT NULL,
  `itemBodyID` INT NOT NULL,
  PRIMARY KEY (`bodiesGroupedID`),
  CONSTRAINT `FK_bodiesGrouped_group4bodies` FOREIGN KEY (`group4bodiesID`) REFERENCES `group4bodies` (`group4bodiesID`) ON DELETE CASCADE,
  CONSTRAINT `FK_bodiesGrouped_itemBody` FOREIGN KEY (`itemBodyID`) REFERENCES `itemBody` (`itembodyID`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
" } },
            { typeof(DbSet<group4watch>), new List<string> { @"
CREATE TABLE IF NOT EXISTS `group4watches` (
  `group4watchesID` INT NOT NULL AUTO_INCREMENT,
  `name` TEXT NULL,
  PRIMARY KEY (`group4watchesID`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
" } },
            { typeof(DbSet<mayalsofit>), new List<string> { @"
CREATE TABLE IF NOT EXISTS `mayalsofit` (
  `mayalsofitID` INT NOT NULL AUTO_INCREMENT,
  `group4bodiesID` INT NOT NULL,
  `group4watchesID` INT NOT NULL,
  PRIMARY KEY (`mayalsofitID`),
  CONSTRAINT `FK_mayalsofit_group4bodies` FOREIGN KEY (`group4bodiesID`) REFERENCES `group4bodies` (`group4bodiesID`) ON DELETE CASCADE,
  CONSTRAINT `FK_mayalsofit_group4watches` FOREIGN KEY (`group4watchesID`) REFERENCES `group4watches` (`group4watchesID`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
" } },
            {
                typeof(DbSet<watchesgrouped>),
                new List<string>
                {
                    @"
CREATE TABLE IF NOT EXISTS `watchesGrouped` (
  `watchesGroupedID` INT NOT NULL AUTO_INCREMENT,
  `group4watchesID` INT NOT NULL,
  `watchID` INT NOT NULL,
  PRIMARY KEY (`watchesGroupedID`),
  CONSTRAINT `fk_watchesGrouped_group4watches`
    FOREIGN KEY (`group4watchesID`)
    REFERENCES `group4watches` (`group4watchesID`)
    ON DELETE CASCADE,
  CONSTRAINT `fk_watchesGrouped_watch`
    FOREIGN KEY (`watchID`)
    REFERENCES `watch` (`watchID`)
    ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
"
                }
            },
            {
                typeof(DbSet<bodyinthebox>),
                new List<string>
                {
                    @"
CREATE TABLE IF NOT EXISTS `BodyInTheBox` (
  `BodyInTheBoxID` INT NOT NULL AUTO_INCREMENT,
  `itembodyID` INT NOT NULL,
  `MultiDrawerID` INT NOT NULL,
  `row` INT NOT NULL,
  `column` INT NOT NULL,
  PRIMARY KEY (`BodyInTheBoxID`),
  INDEX `fk_itembodyID_idx` (`itembodyID`), -- Optional: Helps with the performance on joins
  INDEX `fk_MultiDrawerID_idx` (`MultiDrawerID`), -- Optional: Helps with the performance on joins
  CONSTRAINT `fk_BodyInTheBox_itembody`
    FOREIGN KEY (`itembodyID`)
    REFERENCES `itemBody` (`itembodyID`)
    ON DELETE CASCADE,
  CONSTRAINT `fk_BodyInTheBox_MultiDrawer`
    FOREIGN KEY (`MultiDrawerID`)
    REFERENCES `MultiDrawer` (`MultiDrawerID`)
    ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
"
                }
            },
            {
                typeof(DbSet<itmparameter>),
                new List<string>
                {
                    @"
CREATE TABLE IF NOT EXISTS `itmParameters` (
  `itmparameterID` INT NOT NULL AUTO_INCREMENT,
  `itembodyID` INT NOT NULL,
  `parameterID` INT NOT NULL,
  `parameterValueID` INT NOT NULL,
  PRIMARY KEY (`itmparameterID`),
  INDEX `fk_itembodyID_idx` (`itembodyID`),
  INDEX `fk_parameterID_idx` (`parameterID`),
  INDEX `fk_cechavalueID_idx` (`parameterValueID`),
  CONSTRAINT `fk_itmParameters_itembody`
    FOREIGN KEY (`itembodyID`)
    REFERENCES `itemBody` (`itembodyID`)
    ON DELETE CASCADE,
  CONSTRAINT `fk_itmParameters_cechy`
    FOREIGN KEY (`parameterID`)
    REFERENCES `parameters` (`parameterID`)
    ON DELETE CASCADE,
  CONSTRAINT `fk_itmParameters_cechyValues`
    FOREIGN KEY (`parameterValueID`)
    REFERENCES `parameterValues` (`parameterValueID`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
"
                }
            }, {
                typeof(DbSet<orderitem>),
                new List<string>
                {
                    @"
CREATE TABLE IF NOT EXISTS `OrderItem` (
  `OrderItemId` INT NOT NULL AUTO_INCREMENT,
  `itemName` TEXT NOT NULL,
  `quantity` INT NOT NULL,
  `itembodyID` INT NOT NULL,
  `OrderItemTypeId` INT NOT NULL,
  `price` DECIMAL(18, 0) NOT NULL,
  `orderID` INT NULL,
  `itmMarketAssID` INT NULL,
  `ItemWeight` INT NULL,
  PRIMARY KEY (`OrderItemId`),
  INDEX `fk_itemId_idx` (`itembodyID`),
  INDEX `fk_itemType_idx` (`OrderItemTypeId`),
  INDEX `fk_itmMarketAssID_idx` (`itmMarketAssID`),
  INDEX `fk_orderID_idx` (`orderID`),
  CONSTRAINT `fk_OrderItem_itemBody`
    FOREIGN KEY (`itembodyID`)
    REFERENCES `itemBody` (`itembodyID`),
  CONSTRAINT `fk_OrderItem_OrderItemType`
    FOREIGN KEY (`OrderItemTypeId`)
    REFERENCES `OrderItemType` (`OrderItemTypeId`),
  CONSTRAINT `fk_OrderItem_itmMarketAssoc`
    FOREIGN KEY (`itmMarketAssID`)
    REFERENCES `itmMarketAssoc` (`itmmarketassID`),
  CONSTRAINT `fk_OrderItem_Order`
    FOREIGN KEY (`orderID`)
    REFERENCES `Order` (`orderID`)
    ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
"
                }
            },
            {
                typeof(DbSet<photo>), // Replace DbSet<Photo> with the actual DbSet type from your Entity Framework context if it's different.
                new List<string>
                {
                    @"
CREATE TABLE IF NOT EXISTS `photo` (
  `photoID` INT NOT NULL AUTO_INCREMENT,
  `path` TEXT NOT NULL,
  `itembodyID` INT NOT NULL,
  `pos` INT NOT NULL,
  PRIMARY KEY (`photoID`),
  INDEX `fk_itembodyID_idx` (`itembodyID` ASC),
  CONSTRAINT `fk_photo_itemBody`
    FOREIGN KEY (`itembodyID`)
    REFERENCES `itemBody` (`itembodyID`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
"
                }
            }
            ,
            {
                typeof(DbSet<completeview>), // Replace DbSet<Photo> with the actual DbSet type from your Entity Framework context if it's different.
                new List<string>
                {
                    @"
CREATE OR REPLACE VIEW CompleteView AS
SELECT
  o.orderID,
  o.status,
  ba.CountryCode,
  o.quickbooked,
  o.customerID,
  o.paidOn,
  o.dispatchedOn,
  o.tracking,
  o.market,
  o.locationID,

  o.salecurrency,
  o.acquiredcurrency,
  o.saletotal,
  o.xchgrate,

 

  o.VAT,
  o.order_notes,
  o.postagePrice,
  o.postageType,
  o.VATRateID,
  c.Title,
  c.GivenName,
  c.MiddleName,
  c.FamilyName,
  c.CompanyName,
  c.Email,
  c.Phone,
  c.DisplayName,
  c.currency AS customer_currency,
  c.customer_notes,
  c.billaddrID,
  ba.Line1,
  ba.Line2,
  ba.City,
  ba.CountrySubDivisionCode,
  ba.PostalCode,
  ba.AddressAsAString,
  oi.OrderItemId,
  oi.itemName,
  oi.quantity,
  oi.itembodyID,
  oi.OrderItemTypeId,
  oi.price,
  oi.itmMarketAssID,
  oi.ItemWeight
FROM
  `order` o
JOIN
  `customer` c ON o.customerID = c.customerID
JOIN
  `billaddr` ba ON c.billaddrID = ba.billaddrID
LEFT JOIN
  `orderitem` oi ON o.orderID = oi.orderID;
"
                }
            },
            {
                typeof(DbSet<Models.parameteranditsvalue>), // Replace DbSet<Photo> with the actual DbSet type from your Entity Framework context if it's different.
                new List<string>
                {
                    @"
CREATE OR REPLACE VIEW parameteranditsvalues AS
SELECT 
    p.parameterID,
    p.name AS parameterName,
    pv.parameterValueID,
    pv.name AS valueName,
    pv.pos
FROM 
    parameters p
JOIN 
    parametervalues pv ON p.parameterID = pv.parameterID;

"
                }

            },

            {
                typeof(DbSet<Models.OauthToken>), 
                new List<string>
                {
                    @"CREATE TABLE IF NOT EXISTS oauth_tokens (
    oauth_tokenId INT AUTO_INCREMENT PRIMARY KEY,
    locationId INT NOT NULL,
    serviceId INT NOT NULL,
    access_token TEXT NULL,
    refresh_token TEXT NULL,
    client_id VARCHAR(255) NULL,
    client_secret VARCHAR(255) NULL,
    app_id VARCHAR(255) NULL,
    cert_id VARCHAR(255) NULL,
    dev_id VARCHAR(255) NULL,
    additional_data JSON NULL,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
    FOREIGN KEY (locationId) REFERENCES location(locationId),
    FOREIGN KEY (serviceId) REFERENCES oauth_services(serviceId),
    UNIQUE KEY unique_location_service (locationId, serviceId)
);"
                }

            },
            {
                typeof(DbSet<OauthService>),
                new List<string>
                {
                    @"CREATE TABLE IF NOT EXISTS oauth_services (
    serviceId INT AUTO_INCREMENT PRIMARY KEY,
    service_name VARCHAR(50) NOT NULL UNIQUE,
    description VARCHAR(255) NULL
);"
                }

            },

            { typeof(DbSet<Models.itemitsparametersandvalue>), // Replace DbSet<Photo> with the actual DbSet type from your Entity Framework context if it's different.
                new List<string>
                {
                    @"CREATE OR REPLACE VIEW itemitsparametersandvalues AS
SELECT 
    ib.itembodyID,
    ib.typeID,
    tpa.parameterID,
    COALESCE(p.name, 'Unknown') AS ParameterName,
    COALESCE(pv.parameterValueID, -1) AS parameterValueID,
    COALESCE(pv.name, 'Not Set') AS ParameterValueName,
    COALESCE(pv.pos, -1) AS pos
FROM 
    itembody ib
INNER JOIN 
    typeparassociation tpa ON ib.typeID = tpa.typeID
LEFT JOIN 
    itmparameters ip ON tpa.parameterID = ip.parameterID AND ip.itembodyID = ib.itembodyID
LEFT JOIN 
    parameters p ON tpa.parameterID = p.parameterID
LEFT JOIN 
    parametervalues pv ON ip.parameterValueID = pv.parameterValueID;
"
                }




            },
                

            { typeof(DbSet<Models.OauthTokenWithService >), 
                new List<string>
                {
                    @"CREATE OR REPLACE VIEW oauth_tokens_with_service AS
SELECT 
    ot.oauth_tokenId,
    ot.locationId,
    ot.service_id,
    ot.access_token,
    ot.refresh_token,
    ot.client_id,
    ot.client_secret,
    ot.app_id,
    ot.cert_id,
    ot.dev_id,
    ot.additional_data,
    ot.created_at,
    ot.updated_at,
    os.service_name
FROM oauth_tokens ot
JOIN oauth_services os ON ot.service_id = os.serviceId;
"
                }
            }
        };
        return dictionary;
    }




        


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .UseCollation("utf8mb4_general_ci")
            .HasCharSet("utf8mb4");

        modelBuilder.Entity<OauthToken>(entity =>
        {
            entity.ToTable("oauth_tokens");
            entity.HasKey(x => x.OauthTokenId);
            entity.Property(x => x.OauthTokenId).HasColumnName("oauth_tokenId");
            entity.Property(x => x.LocationId).HasColumnName("locationId");
            entity.Property(x => x.ServiceId).HasColumnName("service_id");

            entity.Property(x => x.AccessToken).HasColumnName("access_token");
            entity.Property(x => x.RefreshToken).HasColumnName("refresh_token");

            entity.Property(x => x.ClientId).HasColumnName("client_id");
            entity.Property(x => x.ClientSecret).HasColumnName("client_secret");
            entity.Property(x => x.AppId).HasColumnName("app_id");
            entity.Property(x => x.CertId).HasColumnName("cert_id");
            entity.Property(x => x.DevId).HasColumnName("dev_id");
            entity.Property(x => x.AdditionalData).HasColumnName("additional_data");

            entity.Property(x => x.CreatedAt).HasColumnName("created_at");
            entity.Property(x => x.UpdatedAt).HasColumnName("updated_at");

            entity.HasOne(x => x.Location)
                .WithMany(x => x.OauthTokens)
                .HasForeignKey(x => x.LocationId);

            entity.HasOne(x => x.Service)
                .WithMany(x => x.OauthTokens)
                .HasForeignKey(x => x.ServiceId);
        });


        modelBuilder.Entity<OauthService>(entity =>
        {
            entity.ToTable("oauth_services");
            entity.HasKey(x => x.OauthServiceId);
            entity.Property(x => x.OauthServiceId).HasColumnName("serviceId");
            entity.Property(x => x.ServiceName).HasColumnName("service_name");
            entity.Property(x => x.Description).HasColumnName("description");

            entity.HasMany(x => x.OauthTokens)
                .WithOne(x => x.Service)
                .HasForeignKey(x => x.ServiceId);
        });



        modelBuilder.Entity<amazonmarketplace>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity
                .ToTable("amazonmarketplace")
                .UseCollation("utf8mb4_unicode_ci");

            entity.HasIndex(e => e.locationID, "idx_locationID");

            entity.HasIndex(e => e.marketID, "idx_marketID");

            entity.Property(e => e.Id).HasColumnType("int(11)");
            entity.Property(e => e.code).HasMaxLength(50);
            entity.Property(e => e.locationID).HasColumnType("int(11)");
            entity.Property(e => e.marketID).HasColumnType("int(11)");

            entity.HasOne(d => d.location).WithMany(p => p.amazonmarketplaces)
                .HasForeignKey(d => d.locationID)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_AmazonMarketplace_Location");

            entity.HasOne(d => d.market).WithMany(p => p.amazonmarketplaces)
                .HasForeignKey(d => d.marketID)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_AmazonMarketplace_market");
        });

        modelBuilder.Entity<asinsku>(entity =>
        {
            entity.HasKey(e => e.asinskuID).HasName("PRIMARY");

            entity
                .ToTable("asinsku")
                .UseCollation("utf8mb4_unicode_ci");

            entity.HasIndex(e => new { e.asin, e.sku }, "UQ_AsinSku_asin_sku").IsUnique();

            entity.HasIndex(e => e.locationID, "idx_locationID");

            entity.Property(e => e.asinskuID).HasColumnType("int(11)");
            entity.Property(e => e.asin).HasMaxLength(50);
            entity.Property(e => e.locationID).HasColumnType("int(11)");
            entity.Property(e => e.sku).HasMaxLength(50);

            entity.HasOne(d => d.location).WithMany(p => p.asinskus)
                .HasForeignKey(d => d.locationID)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_AsinSku_Location");
        });

        modelBuilder.Entity<billaddr>(entity =>
        {
            entity.HasKey(e => e.billaddrID).HasName("PRIMARY");

            entity
                .ToTable("billaddr")
                .UseCollation("utf8mb4_unicode_ci");

            entity.HasIndex(e => e.CountryCode, "IX_BillAddr_CountryCode");

            entity.Property(e => e.billaddrID).HasColumnType("int(11)");
            entity.Property(e => e.AddressAsAString).HasColumnType("text");
            entity.Property(e => e.City).HasColumnType("text");
            entity.Property(e => e.CountryCode).HasMaxLength(2);
            entity.Property(e => e.CountrySubDivisionCode).HasColumnType("text");
            entity.Property(e => e.Line1).HasColumnType("text");
            entity.Property(e => e.Line2).HasColumnType("text");
            entity.Property(e => e.PostalCode).HasColumnType("text");

            entity.HasOne(d => d.CountryCodeNavigation).WithMany(p => p.billaddrs)
                .HasForeignKey(d => d.CountryCode)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_BillAddr_CountryCode");
        });

        modelBuilder.Entity<bodiesgrouped>(entity =>
        {
            entity.HasKey(e => e.bodiesGroupedID).HasName("PRIMARY");

            entity
                .ToTable("bodiesgrouped")
                .UseCollation("utf8mb4_unicode_ci");

            entity.HasIndex(e => e.group4bodiesID, "FK_bodiesGrouped_group4bodies");

            entity.HasIndex(e => e.itemBodyID, "FK_bodiesGrouped_itemBody");

            entity.Property(e => e.bodiesGroupedID).HasColumnType("int(11)");
            entity.Property(e => e.group4bodiesID).HasColumnType("int(11)");
            entity.Property(e => e.itemBodyID).HasColumnType("int(11)");

            entity.HasOne(d => d.group4bodies).WithMany(p => p.bodiesgroupeds)
                .HasForeignKey(d => d.group4bodiesID)
                .HasConstraintName("FK_bodiesGrouped_group4bodies");

            entity.HasOne(d => d.itemBody).WithMany(p => p.bodiesgroupeds)
                .HasForeignKey(d => d.itemBodyID)
                .HasConstraintName("FK_bodiesGrouped_itemBody");
        });

        modelBuilder.Entity<bodyinthebox>(entity =>
        {
            entity.HasKey(e => e.BodyInTheBoxID).HasName("PRIMARY");

            entity
                .ToTable("bodyinthebox")
                .UseCollation("utf8mb4_unicode_ci");

            entity.HasIndex(e => e.MultiDrawerID, "fk_MultiDrawerID_idx");

            entity.HasIndex(e => e.itembodyID, "fk_itembodyID_idx");

            entity.Property(e => e.BodyInTheBoxID).HasColumnType("int(11)");
            entity.Property(e => e.MultiDrawerID).HasColumnType("int(11)");
            entity.Property(e => e.column).HasColumnType("int(11)");
            entity.Property(e => e.itembodyID).HasColumnType("int(11)");
            entity.Property(e => e.row).HasColumnType("int(11)");

            entity.HasOne(d => d.MultiDrawer).WithMany(p => p.bodyintheboxes)
                .HasForeignKey(d => d.MultiDrawerID)
                .HasConstraintName("fk_BodyInTheBox_MultiDrawer");

            entity.HasOne(d => d.itembody).WithMany(p => p.bodyintheboxes)
                .HasForeignKey(d => d.itembodyID)
                .HasConstraintName("fk_BodyInTheBox_itembody");
        });

        modelBuilder.Entity<brand>(entity =>
        {
            entity.HasKey(e => e.brandID).HasName("PRIMARY");

            entity.Property(e => e.brandID).HasColumnType("int(11)");
            entity.Property(e => e.name).HasMaxLength(50);
        });

        modelBuilder.Entity<casioinvoice>(entity =>
        {
            entity.HasKey(e => e.CasioInvoiceId).HasName("PRIMARY");

            entity
                .ToTable("casioinvoice")
                .UseCollation("utf8mb4_unicode_ci");

            entity.Property(e => e.CasioInvoiceId).HasColumnType("int(11)");
            entity.Property(e => e.date).HasColumnType("datetime");
            entity.Property(e => e.mpn)
                .HasMaxLength(8)
                .IsFixedLength();
            entity.Property(e => e.price).HasPrecision(18, 4);
            entity.Property(e => e.quantity).HasColumnType("int(11)");
        });

        modelBuilder.Entity<casioukbackorder>(entity =>
        {
            entity.HasKey(e => e.casioUKbackorderId).HasName("PRIMARY");

            entity
                .ToTable("casioukbackorder")
                .UseCollation("utf8mb4_unicode_ci");

            entity.Property(e => e.casioUKbackorderId).HasColumnType("int(11)");
            entity.Property(e => e.mpn)
                .HasMaxLength(8)
                .IsFixedLength();
            entity.Property(e => e.orderedon).HasColumnType("datetime");
            entity.Property(e => e.quantity).HasColumnType("int(11)");
        });

        modelBuilder.Entity<casioukcurrentorder>(entity =>
        {
            entity.HasKey(e => e.casioUKcurrentOrderId).HasName("PRIMARY");

            entity
                .ToTable("casioukcurrentorder")
                .UseCollation("utf8mb4_unicode_ci");

            entity.Property(e => e.casioUKcurrentOrderId).HasColumnType("int(11)");
            entity.Property(e => e.mpn)
                .HasMaxLength(20)
                .IsFixedLength();
            entity.Property(e => e.quantity).HasColumnType("int(11)");
        });

        modelBuilder.Entity<colourtranslation>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity
                .ToTable("colourtranslation")
                .UseCollation("utf8mb4_unicode_ci");

            entity.HasIndex(e => e.kodKoloru, "FK_ColourTranslation_kodKoloru");

            entity.Property(e => e.Id).HasColumnType("int(11)");
            entity.Property(e => e.col1).HasColumnType("int(11)");
            entity.Property(e => e.col2).HasColumnType("int(11)");
            entity.Property(e => e.col3).HasColumnType("int(11)");
            entity.Property(e => e.col4).HasColumnType("int(11)");
            entity.Property(e => e.kodKoloru).HasColumnType("int(11)");
            entity.Property(e => e.schemat).HasMaxLength(50);

            entity.HasOne(d => d.kodKoloruNavigation).WithMany(p => p.colourtranslations)
                .HasForeignKey(d => d.kodKoloru)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ColourTranslation_kodKoloru");
        });

        modelBuilder.Entity<completeview>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("completeview");

            entity.Property(e => e.AddressAsAString)
                .HasColumnType("text")
                .UseCollation("utf8mb4_unicode_ci");
            entity.Property(e => e.City)
                .HasColumnType("text")
                .UseCollation("utf8mb4_unicode_ci");
            entity.Property(e => e.CompanyName)
                .HasMaxLength(255)
                .UseCollation("utf8mb4_unicode_ci");
            entity.Property(e => e.CountryCode)
                .HasMaxLength(2)
                .UseCollation("utf8mb4_unicode_ci");
            entity.Property(e => e.CountrySubDivisionCode)
                .HasColumnType("text")
                .UseCollation("utf8mb4_unicode_ci");
            entity.Property(e => e.DisplayName)
                .HasMaxLength(255)
                .UseCollation("utf8mb4_unicode_ci");
            entity.Property(e => e.Email)
                .HasMaxLength(255)
                .UseCollation("utf8mb4_unicode_ci");
            entity.Property(e => e.FamilyName)
                .HasMaxLength(255)
                .UseCollation("utf8mb4_unicode_ci");
            entity.Property(e => e.GivenName)
                .HasMaxLength(255)
                .UseCollation("utf8mb4_unicode_ci");
            entity.Property(e => e.ItemWeight).HasColumnType("int(11)");
            entity.Property(e => e.Line1)
                .HasColumnType("text")
                .UseCollation("utf8mb4_unicode_ci");
            entity.Property(e => e.Line2)
                .HasColumnType("text")
                .UseCollation("utf8mb4_unicode_ci");
            entity.Property(e => e.MiddleName)
                .HasMaxLength(255)
                .UseCollation("utf8mb4_unicode_ci");
            entity.Property(e => e.OrderItemId)
                .HasDefaultValueSql("'0'")
                .HasColumnType("int(11)");
            entity.Property(e => e.OrderItemTypeId).HasColumnType("int(11)");
            entity.Property(e => e.Phone)
                .HasMaxLength(255)
                .UseCollation("utf8mb4_unicode_ci");
            entity.Property(e => e.PostalCode)
                .HasColumnType("text")
                .UseCollation("utf8mb4_unicode_ci");
            entity.Property(e => e.Title)
                .HasMaxLength(50)
                .UseCollation("utf8mb4_unicode_ci");
            entity.Property(e => e.VATRateID).HasColumnType("int(11)");
            entity.Property(e => e.acquiredcurrency)
                .HasMaxLength(3)
                .UseCollation("utf8mb4_unicode_ci");
            entity.Property(e => e.billaddrID).HasColumnType("int(11)");
            entity.Property(e => e.customerID).HasColumnType("int(11)");
            entity.Property(e => e.customer_currency)
                .HasMaxLength(3)
                .UseCollation("utf8mb4_unicode_ci");
            entity.Property(e => e.customer_notes)
                .HasColumnType("text")
                .UseCollation("utf8mb4_unicode_ci");
            entity.Property(e => e.dispatchedOn).HasColumnType("datetime");
            entity.Property(e => e.itemName)
                .HasColumnType("text")
                .UseCollation("utf8mb4_unicode_ci");
            entity.Property(e => e.itembodyID).HasColumnType("int(11)");
            entity.Property(e => e.itmMarketAssID).HasColumnType("int(11)");
            entity.Property(e => e.locationID).HasColumnType("int(11)");
            entity.Property(e => e.market).HasColumnType("int(11)");
            entity.Property(e => e.orderID).HasColumnType("int(11)");
            entity.Property(e => e.order_notes)
                .HasColumnType("text")
                .UseCollation("utf8mb4_unicode_ci");
            entity.Property(e => e.paidOn).HasColumnType("datetime");
            entity.Property(e => e.postagePrice).HasPrecision(18, 4);
            entity.Property(e => e.postageType)
                .HasMaxLength(5)
                .UseCollation("utf8mb4_unicode_ci");
            entity.Property(e => e.price).HasPrecision(18, 4);
            entity.Property(e => e.quantity).HasColumnType("int(11)");
            entity.Property(e => e.salecurrency)
                .HasMaxLength(3)
                .UseCollation("utf8mb4_unicode_ci");
            entity.Property(e => e.saletotal).HasPrecision(18, 4);
            entity.Property(e => e.status)
                .HasMaxLength(4)
                .UseCollation("utf8mb4_unicode_ci");
            entity.Property(e => e.tracking)
                .HasColumnType("text")
                .UseCollation("utf8mb4_unicode_ci");
            entity.Property(e => e.xchgrate).HasPrecision(18, 4);
        });

        modelBuilder.Entity<country2rmass>(entity =>
        {
            entity.HasKey(e => e.Country2RMAssId).HasName("PRIMARY");

            entity
                .ToTable("country2rmass")
                .UseCollation("utf8mb4_unicode_ci");

            entity.HasIndex(e => e.code, "uq_code").IsUnique();

            entity.Property(e => e.Country2RMAssId).HasColumnType("int(11)");
            entity.Property(e => e.RMZoneID).HasColumnType("int(11)");
            entity.Property(e => e.code).HasMaxLength(2);

            entity.HasOne(d => d.codeNavigation).WithOne(p => p.country2rmass)
                .HasForeignKey<country2rmass>(d => d.code)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_country2rmass_code");
        });

        modelBuilder.Entity<countrycode>(entity =>
        {
            entity.HasKey(e => e.code).HasName("PRIMARY");

            entity
                .ToTable("countrycode")
                .UseCollation("utf8mb4_unicode_ci");

            entity.Property(e => e.code).HasMaxLength(2);
            entity.Property(e => e.name).HasColumnType("text");
        });

        modelBuilder.Entity<countryvatrrate>(entity =>
        {
            entity.HasKey(e => e.Countryvatrateid).HasName("PRIMARY");

            entity
                .ToTable("countryvatrrate")
                .UseCollation("utf8mb4_unicode_ci");

            entity.HasIndex(e => e.code, "FK_VatRrate_CountryCode");

            entity.Property(e => e.Countryvatrateid).HasColumnType("int(11)");
            entity.Property(e => e.code).HasMaxLength(2);
            entity.Property(e => e.rate).HasPrecision(18);

            entity.HasOne(d => d.codeNavigation).WithMany(p => p.countryvatrrates)
                .HasForeignKey(d => d.code)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_VatRrate_CountryCode");
        });

        modelBuilder.Entity<currency>(entity =>
        {
            entity.HasKey(e => e.code).HasName("PRIMARY");

            entity
                .ToTable("currency")
                .UseCollation("utf8mb4_unicode_ci");

            entity.Property(e => e.code).HasMaxLength(3);
            entity.Property(e => e.name).HasColumnType("text");
            entity.Property(e => e.symbol).HasColumnType("text");
        });

        modelBuilder.Entity<customer>(entity =>
        {
            entity.HasKey(e => e.customerID).HasName("PRIMARY");

            entity
                .ToTable("customer")
                .UseCollation("utf8mb4_unicode_ci");

            entity.HasIndex(e => e.currency, "FK_Customer_Currency");

            entity.HasIndex(e => e.billaddrID, "IX_Customer_BillAddrID");

            entity.Property(e => e.customerID).HasColumnType("int(11)");
            entity.Property(e => e.CompanyName).HasMaxLength(255);
            entity.Property(e => e.DisplayName).HasMaxLength(255);
            entity.Property(e => e.Email).HasMaxLength(255);
            entity.Property(e => e.FamilyName).HasMaxLength(255);
            entity.Property(e => e.GivenName).HasMaxLength(255);
            entity.Property(e => e.MiddleName).HasMaxLength(255);
            entity.Property(e => e.Phone).HasMaxLength(255);
            entity.Property(e => e.Title).HasMaxLength(50);
            entity.Property(e => e.billaddrID).HasColumnType("int(11)");
            entity.Property(e => e.currency).HasMaxLength(3);
            entity.Property(e => e.customer_notes).HasColumnType("text");

            entity.HasOne(d => d.billaddr).WithMany(p => p.customers)
                .HasForeignKey(d => d.billaddrID)
                .HasConstraintName("FK_Customer_BillAddr");

            entity.HasOne(d => d.currencyNavigation).WithMany(p => p.customers)
                .HasForeignKey(d => d.currency)
                .HasConstraintName("FK_Customer_Currency");
        });

        modelBuilder.Entity<deliveryprice>(entity =>
        {
            entity.HasKey(e => e.DeliveryPriceId).HasName("PRIMARY");

            entity
                .ToTable("deliveryprice")
                .UseCollation("utf8mb4_unicode_ci");

            entity.Property(e => e.DeliveryPriceId).HasColumnType("int(11)");
            entity.Property(e => e.code)
                .HasMaxLength(4)
                .IsFixedLength();
            entity.Property(e => e.name).HasMaxLength(50);
            entity.Property(e => e.price).HasPrecision(18, 2);
        });

        modelBuilder.Entity<group4body>(entity =>
        {
            entity.HasKey(e => e.group4bodiesID).HasName("PRIMARY");

            entity.UseCollation("utf8mb4_unicode_ci");

            entity.Property(e => e.group4bodiesID).HasColumnType("int(11)");
            entity.Property(e => e.name).HasColumnType("text");
        });

        modelBuilder.Entity<group4watch>(entity =>
        {
            entity.HasKey(e => e.group4watchesID).HasName("PRIMARY");

            entity.UseCollation("utf8mb4_unicode_ci");

            entity.Property(e => e.group4watchesID).HasColumnType("int(11)");
            entity.Property(e => e.name).HasColumnType("text");
        });

        modelBuilder.Entity<invoicetxn>(entity =>
        {
            entity.HasKey(e => e.invoiceTXNID).HasName("PRIMARY");

            entity.UseCollation("utf8mb4_unicode_ci");

            entity.HasIndex(e => e.marketID, "FK_invoiceTXNs_market");

            entity.HasIndex(e => e.orderID, "FK_invoiceTXNs_order");

            entity.HasIndex(e => e.platformID, "FK_invoiceTXNs_platforms");

            entity.Property(e => e.invoiceTXNID).HasColumnType("int(11)");
            entity.Property(e => e.marketID).HasColumnType("int(11)");
            entity.Property(e => e.orderID).HasColumnType("int(11)");
            entity.Property(e => e.platformID).HasColumnType("int(11)");
            entity.Property(e => e.platformTXN).HasColumnType("text");
            entity.Property(e => e.qbInvoiceId).HasColumnType("text");

            entity.HasOne(d => d.market).WithMany(p => p.invoicetxns)
                .HasForeignKey(d => d.marketID)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_invoiceTXNs_market");

            entity.HasOne(d => d.order).WithMany(p => p.invoicetxns)
                .HasForeignKey(d => d.orderID)
                .HasConstraintName("FK_invoiceTXNs_order");

            entity.HasOne(d => d.platform).WithMany(p => p.invoicetxns)
                .HasForeignKey(d => d.platformID)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_invoiceTXNs_platforms");
        });

        modelBuilder.Entity<itembody>(entity =>
        {
            entity.HasKey(e => e.itembodyID).HasName("PRIMARY");

            entity
                .ToTable("itembody")
                .UseCollation("utf8mb4_unicode_ci");

            entity.HasIndex(e => e.brandID, "FK_itemBody_brandID");

            entity.HasIndex(e => e.typeId, "FK_itemBody_type");

            entity.Property(e => e.itembodyID).HasColumnType("int(11)");
            entity.Property(e => e.brandID).HasColumnType("int(11)");
            entity.Property(e => e.description).HasColumnType("text");
            entity.Property(e => e.fullsearchterm).HasColumnType("text");
            entity.Property(e => e.logoPic).HasColumnType("text");
            entity.Property(e => e.mpn).HasColumnType("text");
            entity.Property(e => e.myname).HasColumnType("text");
            entity.Property(e => e.name).HasColumnType("text");
            entity.Property(e => e.packagePic).HasColumnType("text");
            entity.Property(e => e.typeId).HasColumnType("int(11)");
            entity.Property(e => e.weight).HasColumnType("int(11)");

            entity.HasOne(d => d.brand).WithMany(p => p.itembodies)
                .HasForeignKey(d => d.brandID)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_itemBody_brandID");

            entity.HasOne(d => d.type).WithMany(p => p.itembodies)
                .HasForeignKey(d => d.typeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_itemBody_type");
        });

        modelBuilder.Entity<itemheader>(entity =>
        {
            entity.HasKey(e => e.itemheaderID).HasName("PRIMARY");

            entity
                .ToTable("itemheader",tb=>
                {
                    tb.HasTrigger("produktysklepowe");
                    tb.HasTrigger("produktysklepowe2");
                })
                .UseCollation("utf8mb4_unicode_ci");

            entity.HasIndex(e => e.locationID, "FK_itemHeader_Location");

            entity.HasIndex(e => e.itembodyID, "FK_itemHeader_itemBody");

            entity.HasIndex(e => e.supplierID, "FK_itemHeader_suppliers");

            entity.HasIndex(e => e.acquiredcurrency, "FK_itemheader2_currency");

            entity.HasIndex(e => e.VATRateID, "FK_itemheader_VATRates");

            entity.HasIndex(e => e.purchasecurrency, "FK_itemheader_currency");

            entity.Property(e => e.itemheaderID).HasColumnType("int(11)");
            entity.Property(e => e.VATRateID).HasColumnType("int(11)");
            entity.Property(e => e.acquiredcurrency).HasMaxLength(3);
            entity.Property(e => e.itembodyID).HasColumnType("int(11)");
            entity.Property(e => e.locationID).HasColumnType("int(11)");
            entity.Property(e => e.pricePaid).HasPrecision(18, 4);
            entity.Property(e => e.purchasecurrency).HasMaxLength(3);
            entity.Property(e => e.purchasedOn).HasColumnType("datetime");
            entity.Property(e => e.quantity).HasColumnType("int(11)");
            entity.Property(e => e.supplierID).HasColumnType("int(11)");
            entity.Property(e => e.xchgrate).HasPrecision(18, 4);

            entity.HasOne(d => d.VATRate).WithMany(p => p.itemheaders)
                .HasForeignKey(d => d.VATRateID)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_itemheader_VATRates");

            entity.HasOne(d => d.acquiredcurrencyNavigation).WithMany(p => p.itemheaderacquiredcurrencyNavigations)
                .HasForeignKey(d => d.acquiredcurrency)
                .HasConstraintName("FK_itemheader2_currency");

            entity.HasOne(d => d.itembody).WithMany(p => p.itemheaders)
                .HasForeignKey(d => d.itembodyID)
                .HasConstraintName("FK_itemHeader_itemBody");

            entity.HasOne(d => d.location).WithMany(p => p.itemheaders)
                .HasForeignKey(d => d.locationID)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_itemHeader_Location");

            entity.HasOne(d => d.purchasecurrencyNavigation).WithMany(p => p.itemheaderpurchasecurrencyNavigations)
                .HasForeignKey(d => d.purchasecurrency)
                .HasConstraintName("FK_itemheader_currency");

            entity.HasOne(d => d.supplier).WithMany(p => p.itemheaders)
                .HasForeignKey(d => d.supplierID)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_itemHeader_suppliers");
        });

        modelBuilder.Entity<Models.itemitsparametersandvalue>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("itemitsparametersandvalues");

            entity.Property(e => e.ParameterName)
                .HasColumnType("mediumtext")
                .UseCollation("utf8mb4_unicode_ci");
            entity.Property(e => e.ParameterValueName)
                .HasColumnType("mediumtext")
                .UseCollation("utf8mb4_unicode_ci");
            entity.Property(e => e.itembodyID).HasColumnType("int(11)");
            entity.Property(e => e.parameterID).HasColumnType("int(11)");
            entity.Property(e => e.parameterValueID).HasColumnType("int(11)");
            entity.Property(e => e.pos).HasColumnType("int(11)");
            entity.Property(e => e.typeID).HasColumnType("int(11)");
        });

        modelBuilder.Entity<itmitmassociation>(entity =>
        {
            entity.HasKey(e => e.itmitmassID).HasName("PRIMARY");

            entity
                .ToTable("itmitmassociation")
                .UseCollation("utf8mb4_unicode_ci");

            entity.HasIndex(e => e.sourceBody, "FK_itmitmAssociation_sourceBody");

            entity.HasIndex(e => e.targetBody, "FK_itmitmAssociation_targetBody");

            entity.Property(e => e.itmitmassID).HasColumnType("int(11)");
            entity.Property(e => e.sourceBody).HasColumnType("int(11)");
            entity.Property(e => e.targetBody).HasColumnType("int(11)");

            entity.HasOne(d => d.sourceBodyNavigation).WithMany(p => p.itmitmassociationsourceBodyNavigations)
                .HasForeignKey(d => d.sourceBody)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_itmitmAssociation_sourceBody");

            entity.HasOne(d => d.targetBodyNavigation).WithMany(p => p.itmitmassociationtargetBodyNavigations)
                .HasForeignKey(d => d.targetBody)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_itmitmAssociation_targetBody");
        });

        modelBuilder.Entity<itmmarketassoc>(entity =>
        {
            entity.HasKey(e => e.itmmarketassID).HasName("PRIMARY");

            entity
                .ToTable("itmmarketassoc")
                .UseCollation("utf8mb4_unicode_ci");

            entity.HasIndex(e => e.itembodyID, "FK_itmMarketAssoc_itembodyID");

            entity.HasIndex(e => e.locationID, "FK_itmMarketAssoc_locationID");

            entity.HasIndex(e => e.marketID, "FK_itmMarketAssoc_marketID");

            entity.HasIndex(e => e.soldWith, "FK_itmMarketAssoc_soldWith");

            entity.Property(e => e.itmmarketassID).HasColumnType("int(11)");
            entity.Property(e => e.SEName).HasMaxLength(255);
            entity.Property(e => e.itemNumber).HasMaxLength(255);
            entity.Property(e => e.itembodyID).HasColumnType("int(11)");
            entity.Property(e => e.locationID).HasColumnType("int(11)");
            entity.Property(e => e.marketID).HasColumnType("int(11)");
            entity.Property(e => e.quantitySold).HasColumnType("int(11)");
            entity.Property(e => e.soldWith).HasColumnType("int(11)");

            entity.HasOne(d => d.itembody).WithMany(p => p.itmmarketassocitembodies)
                .HasForeignKey(d => d.itembodyID)
                .HasConstraintName("FK_itmMarketAssoc_itembodyID");

            entity.HasOne(d => d.location).WithMany(p => p.itmmarketassocs)
                .HasForeignKey(d => d.locationID)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_itmMarketAssoc_locationID");

            entity.HasOne(d => d.market).WithMany(p => p.itmmarketassocs)
                .HasForeignKey(d => d.marketID)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_itmMarketAssoc_marketID");

            entity.HasOne(d => d.soldWithNavigation).WithMany(p => p.itmmarketassocsoldWithNavigations)
                .HasForeignKey(d => d.soldWith)
                .HasConstraintName("FK_itmMarketAssoc_soldWith");
        });

        modelBuilder.Entity<itmparameter>(entity =>
        {
            entity.HasKey(e => e.itmparameterID).HasName("PRIMARY");

            entity.UseCollation("utf8mb4_unicode_ci");

            entity.HasIndex(e => e.parameterValueID, "fk_cechavalueID_idx");

            entity.HasIndex(e => e.itembodyID, "fk_itembodyID_idx");

            entity.HasIndex(e => e.parameterID, "fk_parameterID_idx");

            entity.Property(e => e.itmparameterID).HasColumnType("int(11)");
            entity.Property(e => e.itembodyID).HasColumnType("int(11)");
            entity.Property(e => e.parameterID).HasColumnType("int(11)");
            entity.Property(e => e.parameterValueID).HasColumnType("int(11)");

            entity.HasOne(d => d.itembody).WithMany(p => p.itmparameters)
                .HasForeignKey(d => d.itembodyID)
                .HasConstraintName("fk_itmParameters_itembody");

            entity.HasOne(d => d.parameter).WithMany(p => p.itmparameters)
                .HasForeignKey(d => d.parameterID)
                .HasConstraintName("fk_itmParameters_cechy");

            entity.HasOne(d => d.parameterValue).WithMany(p => p.itmparameters)
                .HasForeignKey(d => d.parameterValueID)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_itmParameters_cechyValues");
        });

        modelBuilder.Entity<keyvalue>(entity =>
        {
            entity.HasKey(e => e.keyvalueID).HasName("PRIMARY");

            entity
                .ToTable("keyvalue")
                .UseCollation("utf8mb4_unicode_ci");

            entity.Property(e => e.keyvalueID).HasColumnType("int(11)");
            entity.Property(e => e.key).HasColumnType("text");
            entity.Property(e => e.timestamp).HasColumnType("datetime");
            entity.Property(e => e.value).HasColumnType("text");
        });

        modelBuilder.Entity<location>(entity =>
        {
            entity.HasKey(e => e.locationID).HasName("PRIMARY");

            entity
                .ToTable("location")
                .UseCollation("utf8mb4_unicode_ci");

            entity.HasIndex(e => e.currency, "FK_Location_Currency");

            entity.Property(e => e.locationID).HasColumnType("int(11)");
            entity.Property(e => e.currency).HasMaxLength(3);
            entity.Property(e => e.name).HasColumnType("text");

            entity.HasOne(d => d.currencyNavigation).WithMany(p => p.locations)
                .HasForeignKey(d => d.currency)
                .HasConstraintName("FK_Location_Currency");
        });

        modelBuilder.Entity<locmarassociation>(entity =>
        {
            entity.HasKey(e => new { e.loc, e.reference })
                .HasName("PRIMARY")
                .HasAnnotation("MySql:IndexPrefixLength", new[] { 0, 0 });

            entity
                .ToTable("locmarassociation")
                .UseCollation("utf8mb4_unicode_ci");

            entity.HasIndex(e => e.reference, "fk_locMarAssociation_market");

            entity.Property(e => e.loc).HasColumnType("int(11)");
            entity.Property(e => e.reference).HasColumnType("int(11)");
            entity.Property(e => e.pos).HasColumnType("int(11)");

            entity.HasOne(d => d.locNavigation).WithMany(p => p.locmarassociations)
                .HasForeignKey(d => d.loc)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_locMarAssociation_location");

            entity.HasOne(d => d.referenceNavigation).WithMany(p => p.locmarassociations)
                .HasForeignKey(d => d.reference)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_locMarAssociation_market");
        });

        modelBuilder.Entity<logentry>(entity =>
        {
            entity.HasKey(e => e.logentryId).HasName("PRIMARY");

            entity
                .ToTable("logentry")
                .UseCollation("utf8mb4_unicode_ci");

            entity.Property(e => e.logentryId).HasColumnType("int(11)");
            entity.Property(e => e._event).HasColumnName("event");
            entity.Property(e => e.eventdate).HasColumnType("datetime");
        });

        modelBuilder.Entity<logevent>(entity =>
        {
            entity.HasKey(e => e.logeventID).HasName("PRIMARY");

            entity
                .ToTable("logevent")
                .UseCollation("utf8mb4_unicode_ci");

            entity.HasIndex(e => e.itemBodyID, "FK_logevent_itemBody");

            entity.HasIndex(e => e.itemHeaderID, "FK_logevent_itemHeader");

            entity.HasIndex(e => e.marketID, "FK_logevent_market");

            entity.Property(e => e.logeventID).HasColumnType("int(11)");
            entity.Property(e => e._event)
                .HasColumnType("text")
                .HasColumnName("event");
            entity.Property(e => e.happenedOn).HasColumnType("datetime");
            entity.Property(e => e.itemBodyID).HasColumnType("int(11)");
            entity.Property(e => e.itemHeaderID).HasColumnType("int(11)");
            entity.Property(e => e.marketID).HasColumnType("int(11)");

            entity.HasOne(d => d.itemBody).WithMany(p => p.logevents)
                .HasForeignKey(d => d.itemBodyID)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_logevent_itemBody");

            entity.HasOne(d => d.itemHeader).WithMany(p => p.logevents)
                .HasForeignKey(d => d.itemHeaderID)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_logevent_itemHeader");

            entity.HasOne(d => d.market).WithMany(p => p.logevents)
                .HasForeignKey(d => d.marketID)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_logevent_market");
        });

        modelBuilder.Entity<market>(entity =>
        {
            entity.HasKey(e => e.marketID).HasName("PRIMARY");

            entity
                .ToTable("market")
                .UseCollation("utf8mb4_unicode_ci");

            entity.HasIndex(e => e.currency, "FK_market_Currency");

            entity.Property(e => e.marketID).HasColumnType("int(11)");
            entity.Property(e => e.currency)
                .HasMaxLength(3)
                .IsFixedLength();
            entity.Property(e => e.name).HasColumnType("text");

            entity.HasOne(d => d.currencyNavigation).WithMany(p => p.markets)
                .HasForeignKey(d => d.currency)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_market_Currency");
        });

        modelBuilder.Entity<marketplatformassociation>(entity =>
        {
            entity.HasKey(e => e.MarketPlatformAssociationID).HasName("PRIMARY");

            entity
                .ToTable("marketplatformassociation")
                .UseCollation("utf8mb4_unicode_ci");

            entity.HasIndex(e => e.marketID, "idx_marketID");

            entity.HasIndex(e => e.platformID, "idx_platformID");

            entity.Property(e => e.MarketPlatformAssociationID).HasColumnType("int(11)");
            entity.Property(e => e.marketID).HasColumnType("int(11)");
            entity.Property(e => e.platformID).HasColumnType("int(11)");

            entity.HasOne(d => d.market).WithMany(p => p.marketplatformassociations)
                .HasForeignKey(d => d.marketID)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_MarketPlatformAssociation_market");

            entity.HasOne(d => d.platform).WithMany(p => p.marketplatformassociations)
                .HasForeignKey(d => d.platformID)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_MarketPlatformAssociation_platforms");
        });

        modelBuilder.Entity<mayalsofit>(entity =>
        {
            entity.HasKey(e => e.mayalsofitID).HasName("PRIMARY");

            entity
                .ToTable("mayalsofit")
                .UseCollation("utf8mb4_unicode_ci");

            entity.HasIndex(e => e.group4bodiesID, "FK_mayalsofit_group4bodies");

            entity.HasIndex(e => e.group4watchesID, "FK_mayalsofit_group4watches");

            entity.Property(e => e.mayalsofitID).HasColumnType("int(11)");
            entity.Property(e => e.group4bodiesID).HasColumnType("int(11)");
            entity.Property(e => e.group4watchesID).HasColumnType("int(11)");

            entity.HasOne(d => d.group4bodies).WithMany(p => p.mayalsofits)
                .HasForeignKey(d => d.group4bodiesID)
                .HasConstraintName("FK_mayalsofit_group4bodies");

            entity.HasOne(d => d.group4watches).WithMany(p => p.mayalsofits)
                .HasForeignKey(d => d.group4watchesID)
                .HasConstraintName("FK_mayalsofit_group4watches");
        });

        modelBuilder.Entity<multidrawer>(entity =>
        {
            entity.HasKey(e => e.MultiDrawerID).HasName("PRIMARY");

            entity
                .ToTable("multidrawer")
                .UseCollation("utf8mb4_unicode_ci");

            entity.Property(e => e.MultiDrawerID).HasColumnType("int(11)");
            entity.Property(e => e.columns).HasColumnType("int(11)");
            entity.Property(e => e.rows).HasColumnType("int(11)");
        });

        modelBuilder.Entity<order>(entity =>
        {
            entity.HasKey(e => e.orderID).HasName("PRIMARY");

            entity
                .ToTable("order")
                .UseCollation("utf8mb4_unicode_ci");

            entity.HasIndex(e => e.acquiredcurrency, "FK_Order_Currency2");

            entity.HasIndex(e => e.VATRateID, "FK_Order_VATRates");

            entity.HasIndex(e => e.salecurrency, "FK_Order_currency");

            entity.HasIndex(e => e.locationID, "FK_Order_locationID");

            entity.HasIndex(e => e.market, "FK_Order_market");

            entity.HasIndex(e => e.postageType, "FK_Order_postageType");

            entity.HasIndex(e => e.status, "FK_Order_status");

            entity.HasIndex(e => e.customerID, "IX_Order_CustomerID");

            entity.Property(e => e.orderID).HasColumnType("int(11)");
            entity.Property(e => e.VATRateID).HasColumnType("int(11)");
            entity.Property(e => e.acquiredcurrency).HasMaxLength(3);
            entity.Property(e => e.customerID).HasColumnType("int(11)");
            entity.Property(e => e.dispatchedOn).HasColumnType("datetime");
            entity.Property(e => e.locationID).HasColumnType("int(11)");
            entity.Property(e => e.market).HasColumnType("int(11)");
            entity.Property(e => e.order_notes).HasColumnType("text");
            entity.Property(e => e.paidOn).HasColumnType("datetime");
            entity.Property(e => e.postagePrice).HasPrecision(18, 4);
            entity.Property(e => e.postageType).HasMaxLength(5);
            entity.Property(e => e.salecurrency).HasMaxLength(3);
            entity.Property(e => e.saletotal).HasPrecision(18, 4);
            entity.Property(e => e.status).HasMaxLength(4);
            entity.Property(e => e.tracking).HasColumnType("text");
            entity.Property(e => e.xchgrate).HasPrecision(18, 4);

            entity.HasOne(d => d.VATRate).WithMany(p => p.orders)
                .HasForeignKey(d => d.VATRateID)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Order_VATRates");

            entity.HasOne(d => d.acquiredcurrencyNavigation).WithMany(p => p.orderacquiredcurrencyNavigations)
                .HasForeignKey(d => d.acquiredcurrency)
                .HasConstraintName("FK_Order_Currency2");

            entity.HasOne(d => d.customer).WithMany(p => p.orders)
                .HasForeignKey(d => d.customerID)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Order_customerID");

            entity.HasOne(d => d.location).WithMany(p => p.orders)
                .HasForeignKey(d => d.locationID)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Order_locationID");

            entity.HasOne(d => d.marketNavigation).WithMany(p => p.orders)
                .HasForeignKey(d => d.market)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Order_market");

            entity.HasOne(d => d.postageTypeNavigation).WithMany(p => p.orders)
                .HasForeignKey(d => d.postageType)
                .HasConstraintName("FK_Order_postageType");

            entity.HasOne(d => d.salecurrencyNavigation).WithMany(p => p.ordersalecurrencyNavigations)
                .HasForeignKey(d => d.salecurrency)
                .HasConstraintName("FK_Order_currency");

            entity.HasOne(d => d.statusNavigation).WithMany(p => p.orders)
                .HasForeignKey(d => d.status)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Order_status");
        });

        modelBuilder.Entity<orderitem>(entity =>
        {
            entity.HasKey(e => e.OrderItemId).HasName("PRIMARY");

            entity
                .ToTable("orderitem")
                .UseCollation("utf8mb4_unicode_ci");

            entity.HasIndex(e => e.itembodyID, "fk_itemId_idx");

            entity.HasIndex(e => e.OrderItemTypeId, "fk_itemType_idx");

            entity.HasIndex(e => e.itmMarketAssID, "fk_itmMarketAssID_idx");

            entity.HasIndex(e => e.orderID, "fk_orderID_idx");

            entity.Property(e => e.OrderItemId).HasColumnType("int(11)");
            entity.Property(e => e.ItemWeight).HasColumnType("int(11)");
            entity.Property(e => e.OrderItemTypeId).HasColumnType("int(11)");
            entity.Property(e => e.itemName).HasColumnType("text");
            entity.Property(e => e.itembodyID).HasColumnType("int(11)");
            entity.Property(e => e.itmMarketAssID).HasColumnType("int(11)");
            entity.Property(e => e.orderID).HasColumnType("int(11)");
            entity.Property(e => e.price).HasPrecision(18, 4);
            entity.Property(e => e.quantity).HasColumnType("int(11)");

            entity.HasOne(d => d.OrderItemType).WithMany(p => p.orderitems)
                .HasForeignKey(d => d.OrderItemTypeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_OrderItem_OrderItemType");

            entity.HasOne(d => d.itembody).WithMany(p => p.orderitems)
                .HasForeignKey(d => d.itembodyID)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_OrderItem_itemBody");

            entity.HasOne(d => d.itmMarketAss).WithMany(p => p.orderitems)
                .HasForeignKey(d => d.itmMarketAssID)
                .HasConstraintName("fk_OrderItem_itmMarketAssoc");

            entity.HasOne(d => d.order).WithMany(p => p.orderitems)
                .HasForeignKey(d => d.orderID)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("fk_OrderItem_Order");
        });

        modelBuilder.Entity<orderitemtype>(entity =>
        {
            entity.HasKey(e => e.OrderItemTypeId).HasName("PRIMARY");

            entity
                .ToTable("orderitemtype")
                .UseCollation("utf8mb4_unicode_ci");

            entity.Property(e => e.OrderItemTypeId).HasColumnType("int(11)");
            entity.Property(e => e.name).HasColumnType("text");
        });

        modelBuilder.Entity<orderstatus>(entity =>
        {
            entity.HasKey(e => e.code).HasName("PRIMARY");

            entity
                .ToTable("orderstatus")
                .UseCollation("utf8mb4_unicode_ci");

            entity.Property(e => e.code).HasMaxLength(4);
            entity.Property(e => e.name).HasColumnType("text");
        });

        modelBuilder.Entity<parameter>(entity =>
        {
            entity.HasKey(e => e.parameterID).HasName("PRIMARY");

            entity.UseCollation("utf8mb4_unicode_ci");

            entity.Property(e => e.parameterID).HasColumnType("int(11)");
            entity.Property(e => e.name).HasColumnType("text");
        });

        modelBuilder.Entity<Models.parameteranditsvalue>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("parameteranditsvalues");

            entity.Property(e => e.parameterID).HasColumnType("int(11)");
            entity.Property(e => e.parameterName)
                .HasColumnType("text")
                .UseCollation("utf8mb4_unicode_ci");
            entity.Property(e => e.parameterValueID).HasColumnType("int(11)");
            entity.Property(e => e.pos).HasColumnType("int(11)");
            entity.Property(e => e.valueName)
                .HasColumnType("text")
                .UseCollation("utf8mb4_unicode_ci");
        });

        modelBuilder.Entity<parametervalue>(entity =>
        {
            entity.HasKey(e => e.parameterValueID).HasName("PRIMARY");

            entity.UseCollation("utf8mb4_unicode_ci");

            entity.HasIndex(e => e.parameterID, "fk_parameterID_idx");

            entity.Property(e => e.parameterValueID).HasColumnType("int(11)");
            entity.Property(e => e.name).HasColumnType("text");
            entity.Property(e => e.parameterID).HasColumnType("int(11)");
            entity.Property(e => e.pos).HasColumnType("int(11)");

            entity.HasOne(d => d.parameter).WithMany(p => p.parametervalues)
                .HasForeignKey(d => d.parameterID)
                .HasConstraintName("FK_cechyValues_parameterID");
        });

        modelBuilder.Entity<part2itemass>(entity =>
        {
            entity.HasKey(e => e.part2itemassID).HasName("PRIMARY");

            entity
                .ToTable("part2itemass")
                .UseCollation("utf8mb4_unicode_ci");

            entity.HasIndex(e => e.itembodyID, "FK_part2itemass_itembody");

            entity.HasIndex(e => e.watchID, "FK_part2itemass_watch");

            entity.Property(e => e.part2itemassID).HasColumnType("int(11)");
            entity.Property(e => e.itembodyID).HasColumnType("int(11)");
            entity.Property(e => e.watchID).HasColumnType("int(11)");

            entity.HasOne(d => d.itembody).WithMany(p => p.part2itemasses)
                .HasForeignKey(d => d.itembodyID)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_part2itemass_itembody");

            entity.HasOne(d => d.watch).WithMany(p => p.part2itemasses)
                .HasForeignKey(d => d.watchID)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_part2itemass_watch");
        });

        modelBuilder.Entity<photo>(entity =>
        {
            entity.HasKey(e => e.photoID).HasName("PRIMARY");

            entity
                .ToTable("photo")
                .UseCollation("utf8mb4_unicode_ci");

            entity.HasIndex(e => e.itembodyID, "fk_itembodyID_idx");

            entity.Property(e => e.photoID).HasColumnType("int(11)");
            entity.Property(e => e.itembodyID).HasColumnType("int(11)");
            entity.Property(e => e.path).HasColumnType("text");
            entity.Property(e => e.pos).HasColumnType("int(11)");

            entity.HasOne(d => d.itembody).WithMany(p => p.photos)
                .HasForeignKey(d => d.itembodyID)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_photo_itemBody");
        });

        modelBuilder.Entity<platform>(entity =>
        {
            entity.HasKey(e => e.platformID).HasName("PRIMARY");

            entity.UseCollation("utf8mb4_unicode_ci");

            entity.Property(e => e.platformID).HasColumnType("int(11)");
            entity.Property(e => e.Description).HasColumnType("text");
            entity.Property(e => e.name).HasMaxLength(50);
        });

        modelBuilder.Entity<postagetype>(entity =>
        {
            entity.HasKey(e => e.code).HasName("PRIMARY");

            entity.UseCollation("utf8mb4_unicode_ci");

            entity.Property(e => e.code).HasMaxLength(5);
            entity.Property(e => e.name).HasMaxLength(50);
        });

        modelBuilder.Entity<rmzone>(entity =>
        {
            entity.HasKey(e => e.RMZoneId).HasName("PRIMARY");

            entity.UseCollation("utf8mb4_unicode_ci");

            entity.Property(e => e.RMZoneId).HasColumnType("int(11)");
            entity.Property(e => e.Zone).HasMaxLength(10);
        });

        modelBuilder.Entity<searchentry>(entity =>
        {
            entity.HasKey(e => e.searchentryId).HasName("PRIMARY");

            entity
                .ToTable("searchentry")
                .UseCollation("utf8mb4_unicode_ci");

            entity.Property(e => e.searchentryId).HasColumnType("int(11)");
            entity.Property(e => e.searchPhrase).HasColumnType("text");
            entity.Property(e => e.searchTimeStamp).HasColumnType("datetime");
        });

        modelBuilder.Entity<shopitem>(entity =>
        {
            entity.HasKey(e => e.shopitemId).HasName("PRIMARY");

            entity
                .ToTable("shopitem")
                .UseCollation("utf8mb4_unicode_ci");

            entity.HasIndex(e => e.currencyCode, "FK_shopitem_currencyCode");

            entity.HasIndex(e => e.locationID, "FK_shopitem_locationID");

            entity.HasIndex(e => e.itembodyID, "UK_shopitem_itembodyID").IsUnique();

            entity.Property(e => e.shopitemId).HasColumnType("int(11)");
            entity.Property(e => e.currencyCode).HasMaxLength(3);
            entity.Property(e => e.itembodyID).HasColumnType("int(11)");
            entity.Property(e => e.locationID).HasColumnType("int(11)");
            entity.Property(e => e.price).HasPrecision(18, 4);
            entity.Property(e => e.quantity).HasColumnType("int(11)");
            entity.Property(e => e.soldQuantity).HasColumnType("int(11)");

            entity.HasOne(d => d.currencyCodeNavigation).WithMany(p => p.shopitems)
                .HasForeignKey(d => d.currencyCode)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_shopitem_currencyCode");

            entity.HasOne(d => d.itembody).WithOne(p => p.shopitem)
                .HasForeignKey<shopitem>(d => d.itembodyID)
                .HasConstraintName("FK_shopitem_itembodyID");

            entity.HasOne(d => d.location).WithMany(p => p.shopitems)
                .HasForeignKey(d => d.locationID)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_shopitem_locationID");
        });

        modelBuilder.Entity<stockshot>(entity =>
        {
            entity.HasKey(e => e.StockShotID).HasName("PRIMARY");

            entity
                .ToTable("stockshot")
                .UseCollation("utf8mb4_unicode_ci");

            entity.HasIndex(e => e.bodyid, "FK_stockShot_bodyid");

            entity.HasIndex(e => e.locationID, "FK_stockShot_location");

            entity.Property(e => e.StockShotID).HasColumnType("int(11)");
            entity.Property(e => e.bodyid).HasColumnType("int(11)");
            entity.Property(e => e.date).HasColumnType("datetime");
            entity.Property(e => e.locationID).HasColumnType("int(11)");
            entity.Property(e => e.quantity).HasColumnType("int(11)");

            entity.HasOne(d => d.body).WithMany(p => p.stockshots)
                .HasForeignKey(d => d.bodyid)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_stockShot_bodyid");

            entity.HasOne(d => d.location).WithMany(p => p.stockshots)
                .HasForeignKey(d => d.locationID)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_stockShot_location");
        });

        modelBuilder.Entity<supplier>(entity =>
        {
            entity.HasKey(e => e.supplierID).HasName("PRIMARY");

            entity.UseCollation("utf8mb4_unicode_ci");

            entity.HasIndex(e => e.currency, "FK_Supplier_Currency");

            entity.Property(e => e.supplierID).HasColumnType("int(11)");
            entity.Property(e => e.currency)
                .HasMaxLength(3)
                .IsFixedLength();
            entity.Property(e => e.name).HasColumnType("text");

            entity.HasOne(d => d.currencyNavigation).WithMany(p => p.suppliers)
                .HasForeignKey(d => d.currency)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Supplier_Currency");
        });

        modelBuilder.Entity<token>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.UseCollation("utf8mb4_unicode_ci");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnType("int(11)");
            entity.Property(e => e.AmAccessKey).HasColumnType("text");
            entity.Property(e => e.AmSecret).HasColumnType("text");
            entity.Property(e => e.AmazonSPAPIClientID).HasColumnType("text");
            entity.Property(e => e.AmazonSPAPIClientSecret).HasColumnType("text");
            entity.Property(e => e.AmazonSPAPIRefreshToken).HasColumnType("text");
            entity.Property(e => e.AmazonSPAPIToken).HasColumnType("text");
            entity.Property(e => e.AppID).HasColumnType("text");
            entity.Property(e => e.CertID).HasColumnType("text");
            entity.Property(e => e.DevID).HasColumnType("text");
            entity.Property(e => e.client_id).HasColumnType("text");
            entity.Property(e => e.ebayOauthToken).HasColumnType("text");
            entity.Property(e => e.ebayRefreshToken).HasColumnType("text");
            entity.Property(e => e.ebayToken).HasColumnType("text");
            entity.Property(e => e.locationID).HasColumnType("int(11)");
            entity.Property(e => e.paypalToken).HasColumnType("text");
            entity.Property(e => e.quickBooksAuthS).HasColumnType("text");
            entity.Property(e => e.quickBooksRefresh).HasColumnType("text");
            entity.Property(e => e.quickBooksToken).HasColumnType("text");
            entity.Property(e => e.secret_id).HasColumnType("text");
        });

        modelBuilder.Entity<type>(entity =>
        {
            entity.HasKey(e => e.typeID).HasName("PRIMARY");

            entity.UseCollation("utf8mb4_unicode_ci");

            entity.Property(e => e.typeID).HasColumnType("int(11)");
            entity.Property(e => e.name).HasMaxLength(50);
        });

        modelBuilder.Entity<typeparassociation>(entity =>
        {
            entity.HasKey(e => new { e.typeID, e.parameterID })
                .HasName("PRIMARY")
                .HasAnnotation("MySql:IndexPrefixLength", new[] { 0, 0 });

            entity
                .ToTable("typeparassociation")
                .UseCollation("utf8mb4_unicode_ci");

            entity.HasIndex(e => e.parameterID, "FK_typeParAssociation_parameterID");

            entity.Property(e => e.typeID).HasColumnType("int(11)");
            entity.Property(e => e.parameterID).HasColumnType("int(11)");
            entity.Property(e => e.pos).HasColumnType("int(11)");

            entity.HasOne(d => d.parameter).WithMany(p => p.typeparassociations)
                .HasForeignKey(d => d.parameterID)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_typeParAssociation_parameterID");

            entity.HasOne(d => d.type).WithMany(p => p.typeparassociations)
                .HasForeignKey(d => d.typeID)
                .HasConstraintName("FK_typeParAssociation_typeID");
        });

        modelBuilder.Entity<vatrate>(entity =>
        {
            entity.HasKey(e => e.VATRateID).HasName("PRIMARY");

            entity.UseCollation("utf8mb4_unicode_ci");

            entity.Property(e => e.VATRateID).HasColumnType("int(11)");
            entity.Property(e => e.Rate).HasPrecision(5, 2);
            entity.Property(e => e.VATDescription).HasMaxLength(255);
        });

        modelBuilder.Entity<watch>(entity =>
        {
            entity.HasKey(e => e.watchID).HasName("PRIMARY");

            entity
                .ToTable("watch")
                .UseCollation("utf8mb4_unicode_ci");

            entity.Property(e => e.watchID).HasColumnType("int(11)");
            entity.Property(e => e.name).HasColumnType("text");
            entity.Property(e => e.searchterm).HasColumnType("text");
            entity.Property(e => e.watchCode).HasMaxLength(50);
        });

        modelBuilder.Entity<watchesgrouped>(entity =>
        {
            entity.HasKey(e => e.watchesGroupedID).HasName("PRIMARY");

            entity
                .ToTable("watchesgrouped")
                .UseCollation("utf8mb4_unicode_ci");

            entity.HasIndex(e => e.group4watchesID, "fk_watchesGrouped_group4watches");

            entity.HasIndex(e => e.watchID, "fk_watchesGrouped_watch");

            entity.Property(e => e.watchesGroupedID).HasColumnType("int(11)");
            entity.Property(e => e.group4watchesID).HasColumnType("int(11)");
            entity.Property(e => e.watchID).HasColumnType("int(11)");

            entity.HasOne(d => d.group4watches).WithMany(p => p.watchesgroupeds)
                .HasForeignKey(d => d.group4watchesID)
                .HasConstraintName("fk_watchesGrouped_group4watches");

            entity.HasOne(d => d.watch).WithMany(p => p.watchesgroupeds)
                .HasForeignKey(d => d.watchID)
                .HasConstraintName("fk_watchesGrouped_watch");
        });

        modelBuilder.Entity<xrate>(entity =>
        {
            entity.HasKey(e => e.XrateId).HasName("PRIMARY");

            entity
                .ToTable("xrate")
                .UseCollation("utf8mb4_unicode_ci");

            entity.HasIndex(e => e.code, "FK_Xrate_Currency");

            entity.HasIndex(e => e.SourceCurrencyCode, "FK_Xrate_Currency2");

            entity.Property(e => e.XrateId).HasColumnType("int(11)");
            entity.Property(e => e.SourceCurrencyCode)
                .HasMaxLength(3)
                .IsFixedLength();
            entity.Property(e => e.code)
                .HasMaxLength(3)
                .IsFixedLength();
            entity.Property(e => e.date).HasColumnType("datetime");
            entity.Property(e => e.rate).HasPrecision(18);

            entity.HasOne(d => d.SourceCurrencyCodeNavigation).WithMany(p => p.xrateSourceCurrencyCodeNavigations)
                .HasForeignKey(d => d.SourceCurrencyCode)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Xrate_Currency2");

            entity.HasOne(d => d.codeNavigation).WithMany(p => p.xratecodeNavigations)
                .HasForeignKey(d => d.code)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Xrate_Currency");
        });

        modelBuilder.Entity<zibiinvoice>(entity =>
        {
            entity.HasKey(e => e.zibiinvoiceId).HasName("PRIMARY");

            entity
                .ToTable("zibiinvoice")
                .UseCollation("utf8mb4_unicode_ci");

            entity.Property(e => e.zibiinvoiceId).HasColumnType("int(11)");
            entity.Property(e => e.discount).HasColumnType("int(11)");
            entity.Property(e => e.mpn).HasMaxLength(8);
            entity.Property(e => e.name).HasColumnType("text");
            entity.Property(e => e.price).HasPrecision(18, 4);
            entity.Property(e => e.priceAfterD).HasPrecision(18, 4);
            entity.Property(e => e.purchasedOn).HasColumnType("datetime");
            entity.Property(e => e.quantity).HasColumnType("int(11)");
            entity.Property(e => e.vat).HasPrecision(18, 4);
        });
        modelBuilder.Entity<OauthTokenWithService>(entity =>
        {
            entity.HasNoKey(); // Widoki nie mają klucza głównego

            entity.ToView("oauth_tokens_with_service"); // Nazwa widoku w bazie danych

            entity.Property(e => e.OauthTokenId).HasColumnName("oauth_tokenId");
            entity.Property(e => e.LocationId).HasColumnName("locationId");
            entity.Property(e => e.ServiceId).HasColumnName("service_id");
            entity.Property(e => e.AccessToken).HasColumnName("access_token");
            entity.Property(e => e.RefreshToken).HasColumnName("refresh_token");
            entity.Property(e => e.ClientId).HasColumnName("client_id");
            entity.Property(e => e.ClientSecret).HasColumnName("client_secret");
            entity.Property(e => e.AppId).HasColumnName("app_id");
            entity.Property(e => e.CertId).HasColumnName("cert_id");
            entity.Property(e => e.DevId).HasColumnName("dev_id");
            entity.Property(e => e.AdditionalData).HasColumnName("additional_data");
            entity.Property(e => e.CreatedAt).HasColumnName("created_at");
            entity.Property(e => e.UpdatedAt).HasColumnName("updated_at");
            entity.Property(e => e.ServiceName).HasColumnName("service_name");
        });
        OnModelCreatingPartial(modelBuilder);
    }
    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);

    //        public async Task<List<OrderData>> GetOrderDataAsync(string currency, Tuple<DateTime, DateTime> period, HashSet<string> countries)
    //        {
    //            List<OrderData> orderDataList = new List<OrderData>();


    //            var countriesList = string.Join("', '", countries);  // Lista krajów jako string, aby móc użyć w zapytaniu SQL

    //            string sqlQuery = $@"
    //WITH LatestRate AS (
    //    SELECT 
    //        o.orderID,
    //        MAX(xr.date) as MaxDate
    //    FROM 
    //        `Order` o
    //    LEFT JOIN 
    //        `Xrate` xr ON xr.date <= o.paidOn AND xr.code = o.currency
    //    WHERE
    //        o.paidOn BETWEEN '2023-01-01' AND '2023-12-31' -- period powinien być wstawiony bezpośrednio lub jako parametr
    //    GROUP BY
    //        o.orderID
    //),
    //FilteredOrders AS (
    //    SELECT 
    //        o.orderID,
    //        vr.VatRateID,
    //        o.total AS Total,
    //        CASE 
    //            WHEN o.VAT = 1 THEN o.total / (1 + (vr.Rate / 100)) 
    //            ELSE o.total 
    //        END AS NetTotal,
    //        ba.CountryCode,
    //        o.currency as OrderCurrency,
    //        o.Market as OrderMarketId,
    //        o.paidOn as OrderPaidOn,
    //        xr.rate
    //    FROM 
    //        `Order` o
    //    JOIN 
    //        `Customer` c ON o.customerID = c.customerID
    //    JOIN 
    //        `BillAddr` ba ON c.billaddrID = ba.billaddrID
    //    LEFT JOIN 
    //        LatestRate lr ON lr.orderID = o.orderID
    //    LEFT JOIN 
    //        `Xrate` xr ON xr.date = lr.MaxDate AND xr.code = o.currency
    //    LEFT JOIN 
    //        `VATRates` vr ON vr.VatRateID = o.VatRateID
    //    WHERE 
    //        ba.CountryCode IN ('Country1', 'Country2') -- countriesList powinno być wstawione bezpośrednio lub jako parametr
    //        AND o.paidOn BETWEEN '2023-01-01' AND '2023-12-31' -- period powinien być wstawiony bezpośrednio lub jako parametr
    //        AND o.status IN ('_NEW', 'PROC', 'SHIP')
    //        AND o.PostagePrice IS NOT NULL 
    //        AND o.LocationID=1
    //)
    //SELECT DISTINCT
    //    fo.orderID as OrderId,
    //    fo.Total as Total,
    //    fo.NetTotal as NetTotal,
    //    fo.CountryCode,
    //    fo.OrderMarketId as MarketId,
    //    fo.OrderPaidOn as paidOn,
    //    CASE WHEN o.VAT = 1 THEN 1 ELSE 0 END as IsVat  
    //FROM 
    //    FilteredOrders fo
    //JOIN 
    //    `Order` o ON fo.orderID = o.orderID;  

    //";

    //            using (var context = new DB_A2AAEE_pierwszaEntities())
    //            {
    //                using (var cmd = new SqlCommand(sqlQuery, (SqlConnection)context.Database.GetDbConnection()))
    //                {
    //                    cmd.Parameters.AddWithValue("@currency", currency);  // Dodanie parametrów do zapytania
    //                    cmd.Parameters.AddWithValue("@startDate", period.Item1);
    //                    cmd.Parameters.AddWithValue("@endDate", period.Item2);

    //                    await context.Database.OpenConnectionAsync().ConfigureAwait(false);  // Otwieranie połączenia asynchronicznie

    //                    using (var reader = await cmd.ExecuteReaderAsync().ConfigureAwait(false))  // Czytanie wyników asynchronicznie
    //                    {
    //                        while (await reader.ReadAsync().ConfigureAwait(false))
    //                        {

    //                            var orderData = new OrderData();
    //                            orderData.OrderId = (int)reader["OrderId"];
    //                            orderData.Total = (double)reader["Total"];
    //                            orderData.NetTotal = (double)reader["NetTotal"];
    //                            orderData.IsVat = (int)reader["IsVat"] == 1;
    //                            orderData.MarketId = (int)reader["MarketId"];
    //                            orderData.PaidOn = ((DateTime)reader["PaidOn"]).Date;
    //                            orderData.CountryCode = (string)reader["CountryCode"];
    //                            orderDataList.Add(orderData);  // Dodanie wyników do listy

    //                        }
    //                    }
    //                }
    //            }

    //            return (orderDataList);
    //        }

}

public class MyDatabase
{
}