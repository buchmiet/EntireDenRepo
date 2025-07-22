using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataServicesNET80.Migrations
{
    /// <inheritdoc />
    public partial class InitialMigration : Migration
    {

        protected override void Up(MigrationBuilder migrationBuilder)
        {
            
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            
        }
        /// <inheritdoc />
        // protected override void Up(MigrationBuilder migrationBuilder)
        // {
        //     migrationBuilder.AlterDatabase()
        //         .Annotation("MySql:CharSet", "utf8mb4");
        //
        //     migrationBuilder.CreateTable(
        //         name: "brands",
        //         columns: table => new
        //         {
        //             brandID = table.Column<int>(type: "int(11)", nullable: false)
        //                 .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
        //             name = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true, collation: "utf8mb4_general_ci")
        //                 .Annotation("MySql:CharSet", "utf8mb4")
        //         },
        //         constraints: table =>
        //         {
        //             table.PrimaryKey("PRIMARY", x => x.brandID);
        //         })
        //         .Annotation("MySql:CharSet", "utf8mb4")
        //         .Annotation("Relational:Collation", "utf8mb4_general_ci");
        //
        //     migrationBuilder.CreateTable(
        //         name: "casioinvoice",
        //         columns: table => new
        //         {
        //             CasioInvoiceId = table.Column<int>(type: "int(11)", nullable: false)
        //                 .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
        //             date = table.Column<DateTime>(type: "datetime", nullable: false),
        //             mpn = table.Column<string>(type: "char(8)", fixedLength: true, maxLength: 8, nullable: false, collation: "utf8mb4_unicode_ci")
        //                 .Annotation("MySql:CharSet", "utf8mb4"),
        //             price = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: true),
        //             quantity = table.Column<int>(type: "int(11)", nullable: false)
        //         },
        //         constraints: table =>
        //         {
        //             table.PrimaryKey("PRIMARY", x => x.CasioInvoiceId);
        //         })
        //         .Annotation("MySql:CharSet", "utf8mb4")
        //         .Annotation("Relational:Collation", "utf8mb4_unicode_ci");
        //
        //     migrationBuilder.CreateTable(
        //         name: "casioukbackorder",
        //         columns: table => new
        //         {
        //             casioUKbackorderId = table.Column<int>(type: "int(11)", nullable: false)
        //                 .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
        //             mpn = table.Column<string>(type: "char(8)", fixedLength: true, maxLength: 8, nullable: false, collation: "utf8mb4_unicode_ci")
        //                 .Annotation("MySql:CharSet", "utf8mb4"),
        //             quantity = table.Column<int>(type: "int(11)", nullable: false),
        //             orderedon = table.Column<DateTime>(type: "datetime", nullable: false)
        //         },
        //         constraints: table =>
        //         {
        //             table.PrimaryKey("PRIMARY", x => x.casioUKbackorderId);
        //         })
        //         .Annotation("MySql:CharSet", "utf8mb4")
        //         .Annotation("Relational:Collation", "utf8mb4_unicode_ci");
        //
        //     migrationBuilder.CreateTable(
        //         name: "casioukcurrentorder",
        //         columns: table => new
        //         {
        //             casioUKcurrentOrderId = table.Column<int>(type: "int(11)", nullable: false)
        //                 .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
        //             mpn = table.Column<string>(type: "char(20)", fixedLength: true, maxLength: 20, nullable: false, collation: "utf8mb4_unicode_ci")
        //                 .Annotation("MySql:CharSet", "utf8mb4"),
        //             quantity = table.Column<int>(type: "int(11)", nullable: false)
        //         },
        //         constraints: table =>
        //         {
        //             table.PrimaryKey("PRIMARY", x => x.casioUKcurrentOrderId);
        //         })
        //         .Annotation("MySql:CharSet", "utf8mb4")
        //         .Annotation("Relational:Collation", "utf8mb4_unicode_ci");
        //
        //     migrationBuilder.CreateTable(
        //         name: "countrycode",
        //         columns: table => new
        //         {
        //             code = table.Column<string>(type: "varchar(2)", maxLength: 2, nullable: false, collation: "utf8mb4_unicode_ci")
        //                 .Annotation("MySql:CharSet", "utf8mb4"),
        //             name = table.Column<string>(type: "text", nullable: false, collation: "utf8mb4_unicode_ci")
        //                 .Annotation("MySql:CharSet", "utf8mb4")
        //         },
        //         constraints: table =>
        //         {
        //             table.PrimaryKey("PRIMARY", x => x.code);
        //         })
        //         .Annotation("MySql:CharSet", "utf8mb4")
        //         .Annotation("Relational:Collation", "utf8mb4_unicode_ci");
        //
        //     migrationBuilder.CreateTable(
        //         name: "currency",
        //         columns: table => new
        //         {
        //             code = table.Column<string>(type: "varchar(3)", maxLength: 3, nullable: false, collation: "utf8mb4_unicode_ci")
        //                 .Annotation("MySql:CharSet", "utf8mb4"),
        //             name = table.Column<string>(type: "text", nullable: false, collation: "utf8mb4_unicode_ci")
        //                 .Annotation("MySql:CharSet", "utf8mb4"),
        //             symbol = table.Column<string>(type: "text", nullable: false, collation: "utf8mb4_unicode_ci")
        //                 .Annotation("MySql:CharSet", "utf8mb4")
        //         },
        //         constraints: table =>
        //         {
        //             table.PrimaryKey("PRIMARY", x => x.code);
        //         })
        //         .Annotation("MySql:CharSet", "utf8mb4")
        //         .Annotation("Relational:Collation", "utf8mb4_unicode_ci");
        //
        //     migrationBuilder.CreateTable(
        //         name: "deliveryprice",
        //         columns: table => new
        //         {
        //             DeliveryPriceId = table.Column<int>(type: "int(11)", nullable: false)
        //                 .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
        //             code = table.Column<string>(type: "char(4)", fixedLength: true, maxLength: 4, nullable: false, collation: "utf8mb4_unicode_ci")
        //                 .Annotation("MySql:CharSet", "utf8mb4"),
        //             name = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false, collation: "utf8mb4_unicode_ci")
        //                 .Annotation("MySql:CharSet", "utf8mb4"),
        //             price = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false)
        //         },
        //         constraints: table =>
        //         {
        //             table.PrimaryKey("PRIMARY", x => x.DeliveryPriceId);
        //         })
        //         .Annotation("MySql:CharSet", "utf8mb4")
        //         .Annotation("Relational:Collation", "utf8mb4_unicode_ci");
        //
        //     migrationBuilder.CreateTable(
        //         name: "group4bodies",
        //         columns: table => new
        //         {
        //             group4bodiesID = table.Column<int>(type: "int(11)", nullable: false)
        //                 .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
        //             name = table.Column<string>(type: "text", nullable: true, collation: "utf8mb4_unicode_ci")
        //                 .Annotation("MySql:CharSet", "utf8mb4")
        //         },
        //         constraints: table =>
        //         {
        //             table.PrimaryKey("PRIMARY", x => x.group4bodiesID);
        //         })
        //         .Annotation("MySql:CharSet", "utf8mb4")
        //         .Annotation("Relational:Collation", "utf8mb4_unicode_ci");
        //
        //     migrationBuilder.CreateTable(
        //         name: "group4watches",
        //         columns: table => new
        //         {
        //             group4watchesID = table.Column<int>(type: "int(11)", nullable: false)
        //                 .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
        //             name = table.Column<string>(type: "text", nullable: true, collation: "utf8mb4_unicode_ci")
        //                 .Annotation("MySql:CharSet", "utf8mb4")
        //         },
        //         constraints: table =>
        //         {
        //             table.PrimaryKey("PRIMARY", x => x.group4watchesID);
        //         })
        //         .Annotation("MySql:CharSet", "utf8mb4")
        //         .Annotation("Relational:Collation", "utf8mb4_unicode_ci");
        //
        //     migrationBuilder.CreateTable(
        //         name: "keyvalue",
        //         columns: table => new
        //         {
        //             keyvalueID = table.Column<int>(type: "int(11)", nullable: false)
        //                 .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
        //             key = table.Column<string>(type: "text", nullable: false, collation: "utf8mb4_unicode_ci")
        //                 .Annotation("MySql:CharSet", "utf8mb4"),
        //             value = table.Column<string>(type: "text", nullable: false, collation: "utf8mb4_unicode_ci")
        //                 .Annotation("MySql:CharSet", "utf8mb4"),
        //             timestamp = table.Column<DateTime>(type: "datetime", nullable: false)
        //         },
        //         constraints: table =>
        //         {
        //             table.PrimaryKey("PRIMARY", x => x.keyvalueID);
        //         })
        //         .Annotation("MySql:CharSet", "utf8mb4")
        //         .Annotation("Relational:Collation", "utf8mb4_unicode_ci");
        //
        //     migrationBuilder.CreateTable(
        //         name: "logentry",
        //         columns: table => new
        //         {
        //             logentryId = table.Column<int>(type: "int(11)", nullable: false)
        //                 .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
        //             @event = table.Column<string>(name: "event", type: "longtext", nullable: false, collation: "utf8mb4_unicode_ci")
        //                 .Annotation("MySql:CharSet", "utf8mb4"),
        //             eventdate = table.Column<DateTime>(type: "datetime", nullable: false)
        //         },
        //         constraints: table =>
        //         {
        //             table.PrimaryKey("PRIMARY", x => x.logentryId);
        //         })
        //         .Annotation("MySql:CharSet", "utf8mb4")
        //         .Annotation("Relational:Collation", "utf8mb4_unicode_ci");
        //
        //     migrationBuilder.CreateTable(
        //         name: "multidrawer",
        //         columns: table => new
        //         {
        //             MultiDrawerID = table.Column<int>(type: "int(11)", nullable: false)
        //                 .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
        //             rows = table.Column<int>(type: "int(11)", nullable: false),
        //             columns = table.Column<int>(type: "int(11)", nullable: false),
        //             name = table.Column<string>(type: "longtext", nullable: false, collation: "utf8mb4_unicode_ci")
        //                 .Annotation("MySql:CharSet", "utf8mb4")
        //         },
        //         constraints: table =>
        //         {
        //             table.PrimaryKey("PRIMARY", x => x.MultiDrawerID);
        //         })
        //         .Annotation("MySql:CharSet", "utf8mb4")
        //         .Annotation("Relational:Collation", "utf8mb4_unicode_ci");
        //
        //     migrationBuilder.CreateTable(
        //         name: "orderitemtype",
        //         columns: table => new
        //         {
        //             OrderItemTypeId = table.Column<int>(type: "int(11)", nullable: false)
        //                 .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
        //             name = table.Column<string>(type: "text", nullable: false, collation: "utf8mb4_unicode_ci")
        //                 .Annotation("MySql:CharSet", "utf8mb4")
        //         },
        //         constraints: table =>
        //         {
        //             table.PrimaryKey("PRIMARY", x => x.OrderItemTypeId);
        //         })
        //         .Annotation("MySql:CharSet", "utf8mb4")
        //         .Annotation("Relational:Collation", "utf8mb4_unicode_ci");
        //
        //     migrationBuilder.CreateTable(
        //         name: "orderstatus",
        //         columns: table => new
        //         {
        //             code = table.Column<string>(type: "varchar(4)", maxLength: 4, nullable: false, collation: "utf8mb4_unicode_ci")
        //                 .Annotation("MySql:CharSet", "utf8mb4"),
        //             name = table.Column<string>(type: "text", nullable: false, collation: "utf8mb4_unicode_ci")
        //                 .Annotation("MySql:CharSet", "utf8mb4")
        //         },
        //         constraints: table =>
        //         {
        //             table.PrimaryKey("PRIMARY", x => x.code);
        //         })
        //         .Annotation("MySql:CharSet", "utf8mb4")
        //         .Annotation("Relational:Collation", "utf8mb4_unicode_ci");
        //
        //     migrationBuilder.CreateTable(
        //         name: "parameters",
        //         columns: table => new
        //         {
        //             parameterID = table.Column<int>(type: "int(11)", nullable: false)
        //                 .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
        //             name = table.Column<string>(type: "text", nullable: false, collation: "utf8mb4_unicode_ci")
        //                 .Annotation("MySql:CharSet", "utf8mb4")
        //         },
        //         constraints: table =>
        //         {
        //             table.PrimaryKey("PRIMARY", x => x.parameterID);
        //         })
        //         .Annotation("MySql:CharSet", "utf8mb4")
        //         .Annotation("Relational:Collation", "utf8mb4_unicode_ci");
        //
        //     migrationBuilder.CreateTable(
        //         name: "platforms",
        //         columns: table => new
        //         {
        //             platformID = table.Column<int>(type: "int(11)", nullable: false)
        //                 .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
        //             name = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true, collation: "utf8mb4_unicode_ci")
        //                 .Annotation("MySql:CharSet", "utf8mb4"),
        //             Description = table.Column<string>(type: "text", nullable: true, collation: "utf8mb4_unicode_ci")
        //                 .Annotation("MySql:CharSet", "utf8mb4")
        //         },
        //         constraints: table =>
        //         {
        //             table.PrimaryKey("PRIMARY", x => x.platformID);
        //         })
        //         .Annotation("MySql:CharSet", "utf8mb4")
        //         .Annotation("Relational:Collation", "utf8mb4_unicode_ci");
        //
        //     migrationBuilder.CreateTable(
        //         name: "postagetypes",
        //         columns: table => new
        //         {
        //             code = table.Column<string>(type: "varchar(5)", maxLength: 5, nullable: false, collation: "utf8mb4_unicode_ci")
        //                 .Annotation("MySql:CharSet", "utf8mb4"),
        //             name = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false, collation: "utf8mb4_unicode_ci")
        //                 .Annotation("MySql:CharSet", "utf8mb4")
        //         },
        //         constraints: table =>
        //         {
        //             table.PrimaryKey("PRIMARY", x => x.code);
        //         })
        //         .Annotation("MySql:CharSet", "utf8mb4")
        //         .Annotation("Relational:Collation", "utf8mb4_unicode_ci");
        //
        //     migrationBuilder.CreateTable(
        //         name: "rmzones",
        //         columns: table => new
        //         {
        //             RMZoneId = table.Column<int>(type: "int(11)", nullable: false)
        //                 .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
        //             Zone = table.Column<string>(type: "varchar(10)", maxLength: 10, nullable: false, collation: "utf8mb4_unicode_ci")
        //                 .Annotation("MySql:CharSet", "utf8mb4")
        //         },
        //         constraints: table =>
        //         {
        //             table.PrimaryKey("PRIMARY", x => x.RMZoneId);
        //         })
        //         .Annotation("MySql:CharSet", "utf8mb4")
        //         .Annotation("Relational:Collation", "utf8mb4_unicode_ci");
        //
        //     migrationBuilder.CreateTable(
        //         name: "searchentry",
        //         columns: table => new
        //         {
        //             searchentryId = table.Column<int>(type: "int(11)", nullable: false)
        //                 .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
        //             searchPhrase = table.Column<string>(type: "text", nullable: false, collation: "utf8mb4_unicode_ci")
        //                 .Annotation("MySql:CharSet", "utf8mb4"),
        //             searchTimeStamp = table.Column<DateTime>(type: "datetime", nullable: false)
        //         },
        //         constraints: table =>
        //         {
        //             table.PrimaryKey("PRIMARY", x => x.searchentryId);
        //         })
        //         .Annotation("MySql:CharSet", "utf8mb4")
        //         .Annotation("Relational:Collation", "utf8mb4_unicode_ci");
        //
        //     migrationBuilder.CreateTable(
        //         name: "tokens",
        //         columns: table => new
        //         {
        //             Id = table.Column<int>(type: "int(11)", nullable: false),
        //             ebayToken = table.Column<string>(type: "text", nullable: true, collation: "utf8mb4_unicode_ci")
        //                 .Annotation("MySql:CharSet", "utf8mb4"),
        //             quickBooksToken = table.Column<string>(type: "text", nullable: true, collation: "utf8mb4_unicode_ci")
        //                 .Annotation("MySql:CharSet", "utf8mb4"),
        //             quickBooksRefresh = table.Column<string>(type: "text", nullable: true, collation: "utf8mb4_unicode_ci")
        //                 .Annotation("MySql:CharSet", "utf8mb4"),
        //             AmAccessKey = table.Column<string>(type: "text", nullable: true, collation: "utf8mb4_unicode_ci")
        //                 .Annotation("MySql:CharSet", "utf8mb4"),
        //             AmSecret = table.Column<string>(type: "text", nullable: true, collation: "utf8mb4_unicode_ci")
        //                 .Annotation("MySql:CharSet", "utf8mb4"),
        //             quickBooksAuthS = table.Column<string>(type: "text", nullable: true, collation: "utf8mb4_unicode_ci")
        //                 .Annotation("MySql:CharSet", "utf8mb4"),
        //             ebayOauthToken = table.Column<string>(type: "text", nullable: true, collation: "utf8mb4_unicode_ci")
        //                 .Annotation("MySql:CharSet", "utf8mb4"),
        //             ebayRefreshToken = table.Column<string>(type: "text", nullable: true, collation: "utf8mb4_unicode_ci")
        //                 .Annotation("MySql:CharSet", "utf8mb4"),
        //             paypalToken = table.Column<string>(type: "text", nullable: true, collation: "utf8mb4_unicode_ci")
        //                 .Annotation("MySql:CharSet", "utf8mb4"),
        //             locationID = table.Column<int>(type: "int(11)", nullable: true),
        //             client_id = table.Column<string>(type: "text", nullable: true, collation: "utf8mb4_unicode_ci")
        //                 .Annotation("MySql:CharSet", "utf8mb4"),
        //             secret_id = table.Column<string>(type: "text", nullable: true, collation: "utf8mb4_unicode_ci")
        //                 .Annotation("MySql:CharSet", "utf8mb4"),
        //             DevID = table.Column<string>(type: "text", nullable: true, collation: "utf8mb4_unicode_ci")
        //                 .Annotation("MySql:CharSet", "utf8mb4"),
        //             AppID = table.Column<string>(type: "text", nullable: true, collation: "utf8mb4_unicode_ci")
        //                 .Annotation("MySql:CharSet", "utf8mb4"),
        //             CertID = table.Column<string>(type: "text", nullable: true, collation: "utf8mb4_unicode_ci")
        //                 .Annotation("MySql:CharSet", "utf8mb4"),
        //             AmazonSPAPIToken = table.Column<string>(type: "text", nullable: true, collation: "utf8mb4_unicode_ci")
        //                 .Annotation("MySql:CharSet", "utf8mb4"),
        //             AmazonSPAPIRefreshToken = table.Column<string>(type: "text", nullable: true, collation: "utf8mb4_unicode_ci")
        //                 .Annotation("MySql:CharSet", "utf8mb4"),
        //             AmazonSPAPIClientID = table.Column<string>(type: "text", nullable: true, collation: "utf8mb4_unicode_ci")
        //                 .Annotation("MySql:CharSet", "utf8mb4"),
        //             AmazonSPAPIClientSecret = table.Column<string>(type: "text", nullable: true, collation: "utf8mb4_unicode_ci")
        //                 .Annotation("MySql:CharSet", "utf8mb4")
        //         },
        //         constraints: table =>
        //         {
        //             table.PrimaryKey("PRIMARY", x => x.Id);
        //         })
        //         .Annotation("MySql:CharSet", "utf8mb4")
        //         .Annotation("Relational:Collation", "utf8mb4_unicode_ci");
        //
        //     migrationBuilder.CreateTable(
        //         name: "types",
        //         columns: table => new
        //         {
        //             typeID = table.Column<int>(type: "int(11)", nullable: false)
        //                 .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
        //             name = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true, collation: "utf8mb4_unicode_ci")
        //                 .Annotation("MySql:CharSet", "utf8mb4")
        //         },
        //         constraints: table =>
        //         {
        //             table.PrimaryKey("PRIMARY", x => x.typeID);
        //         })
        //         .Annotation("MySql:CharSet", "utf8mb4")
        //         .Annotation("Relational:Collation", "utf8mb4_unicode_ci");
        //
        //     migrationBuilder.CreateTable(
        //         name: "vatrates",
        //         columns: table => new
        //         {
        //             VATRateID = table.Column<int>(type: "int(11)", nullable: false)
        //                 .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
        //             Rate = table.Column<decimal>(type: "decimal(5,2)", precision: 5, scale: 2, nullable: false),
        //             VATDescription = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: false, collation: "utf8mb4_unicode_ci")
        //                 .Annotation("MySql:CharSet", "utf8mb4")
        //         },
        //         constraints: table =>
        //         {
        //             table.PrimaryKey("PRIMARY", x => x.VATRateID);
        //         })
        //         .Annotation("MySql:CharSet", "utf8mb4")
        //         .Annotation("Relational:Collation", "utf8mb4_unicode_ci");
        //
        //     migrationBuilder.CreateTable(
        //         name: "watch",
        //         columns: table => new
        //         {
        //             watchID = table.Column<int>(type: "int(11)", nullable: false)
        //                 .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
        //             name = table.Column<string>(type: "text", nullable: false, collation: "utf8mb4_unicode_ci")
        //                 .Annotation("MySql:CharSet", "utf8mb4"),
        //             haspic = table.Column<bool>(type: "tinyint(1)", nullable: false),
        //             searchterm = table.Column<string>(type: "text", nullable: false, collation: "utf8mb4_unicode_ci")
        //                 .Annotation("MySql:CharSet", "utf8mb4"),
        //             watchCode = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true, collation: "utf8mb4_unicode_ci")
        //                 .Annotation("MySql:CharSet", "utf8mb4")
        //         },
        //         constraints: table =>
        //         {
        //             table.PrimaryKey("PRIMARY", x => x.watchID);
        //         })
        //         .Annotation("MySql:CharSet", "utf8mb4")
        //         .Annotation("Relational:Collation", "utf8mb4_unicode_ci");
        //
        //     migrationBuilder.CreateTable(
        //         name: "zibiinvoice",
        //         columns: table => new
        //         {
        //             zibiinvoiceId = table.Column<int>(type: "int(11)", nullable: false)
        //                 .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
        //             mpn = table.Column<string>(type: "varchar(8)", maxLength: 8, nullable: false, collation: "utf8mb4_unicode_ci")
        //                 .Annotation("MySql:CharSet", "utf8mb4"),
        //             name = table.Column<string>(type: "text", nullable: true, collation: "utf8mb4_unicode_ci")
        //                 .Annotation("MySql:CharSet", "utf8mb4"),
        //             price = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: false),
        //             discount = table.Column<int>(type: "int(11)", nullable: false),
        //             priceAfterD = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: false),
        //             vat = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: false),
        //             purchasedOn = table.Column<DateTime>(type: "datetime", nullable: false),
        //             quantity = table.Column<int>(type: "int(11)", nullable: false)
        //         },
        //         constraints: table =>
        //         {
        //             table.PrimaryKey("PRIMARY", x => x.zibiinvoiceId);
        //         })
        //         .Annotation("MySql:CharSet", "utf8mb4")
        //         .Annotation("Relational:Collation", "utf8mb4_unicode_ci");
        //
        //     migrationBuilder.CreateTable(
        //         name: "billaddr",
        //         columns: table => new
        //         {
        //             billaddrID = table.Column<int>(type: "int(11)", nullable: false)
        //                 .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
        //             Line1 = table.Column<string>(type: "text", nullable: true, collation: "utf8mb4_unicode_ci")
        //                 .Annotation("MySql:CharSet", "utf8mb4"),
        //             Line2 = table.Column<string>(type: "text", nullable: true, collation: "utf8mb4_unicode_ci")
        //                 .Annotation("MySql:CharSet", "utf8mb4"),
        //             City = table.Column<string>(type: "text", nullable: true, collation: "utf8mb4_unicode_ci")
        //                 .Annotation("MySql:CharSet", "utf8mb4"),
        //             CountryCode = table.Column<string>(type: "varchar(2)", maxLength: 2, nullable: false, collation: "utf8mb4_unicode_ci")
        //                 .Annotation("MySql:CharSet", "utf8mb4"),
        //             CountrySubDivisionCode = table.Column<string>(type: "text", nullable: true, collation: "utf8mb4_unicode_ci")
        //                 .Annotation("MySql:CharSet", "utf8mb4"),
        //             PostalCode = table.Column<string>(type: "text", nullable: false, collation: "utf8mb4_unicode_ci")
        //                 .Annotation("MySql:CharSet", "utf8mb4"),
        //             AddressAsAString = table.Column<string>(type: "text", nullable: true, collation: "utf8mb4_unicode_ci")
        //                 .Annotation("MySql:CharSet", "utf8mb4")
        //         },
        //         constraints: table =>
        //         {
        //             table.PrimaryKey("PRIMARY", x => x.billaddrID);
        //             table.ForeignKey(
        //                 name: "FK_BillAddr_CountryCode",
        //                 column: x => x.CountryCode,
        //                 principalTable: "countrycode",
        //                 principalColumn: "code");
        //         })
        //         .Annotation("MySql:CharSet", "utf8mb4")
        //         .Annotation("Relational:Collation", "utf8mb4_unicode_ci");
        //
        //     migrationBuilder.CreateTable(
        //         name: "country2rmass",
        //         columns: table => new
        //         {
        //             Country2RMAssId = table.Column<int>(type: "int(11)", nullable: false)
        //                 .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
        //             code = table.Column<string>(type: "varchar(2)", maxLength: 2, nullable: false, collation: "utf8mb4_unicode_ci")
        //                 .Annotation("MySql:CharSet", "utf8mb4"),
        //             RMZoneID = table.Column<int>(type: "int(11)", nullable: false)
        //         },
        //         constraints: table =>
        //         {
        //             table.PrimaryKey("PRIMARY", x => x.Country2RMAssId);
        //             table.ForeignKey(
        //                 name: "FK_country2rmass_code",
        //                 column: x => x.code,
        //                 principalTable: "countrycode",
        //                 principalColumn: "code");
        //         })
        //         .Annotation("MySql:CharSet", "utf8mb4")
        //         .Annotation("Relational:Collation", "utf8mb4_unicode_ci");
        //
        //     migrationBuilder.CreateTable(
        //         name: "countryvatrrate",
        //         columns: table => new
        //         {
        //             Countryvatrateid = table.Column<int>(type: "int(11)", nullable: false)
        //                 .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
        //             code = table.Column<string>(type: "varchar(2)", maxLength: 2, nullable: false, collation: "utf8mb4_unicode_ci")
        //                 .Annotation("MySql:CharSet", "utf8mb4"),
        //             rate = table.Column<decimal>(type: "decimal(18)", precision: 18, nullable: false)
        //         },
        //         constraints: table =>
        //         {
        //             table.PrimaryKey("PRIMARY", x => x.Countryvatrateid);
        //             table.ForeignKey(
        //                 name: "FK_VatRrate_CountryCode",
        //                 column: x => x.code,
        //                 principalTable: "countrycode",
        //                 principalColumn: "code");
        //         })
        //         .Annotation("MySql:CharSet", "utf8mb4")
        //         .Annotation("Relational:Collation", "utf8mb4_unicode_ci");
        //
        //     migrationBuilder.CreateTable(
        //         name: "location",
        //         columns: table => new
        //         {
        //             locationID = table.Column<int>(type: "int(11)", nullable: false)
        //                 .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
        //             name = table.Column<string>(type: "text", nullable: false, collation: "utf8mb4_unicode_ci")
        //                 .Annotation("MySql:CharSet", "utf8mb4"),
        //             currency = table.Column<string>(type: "varchar(3)", maxLength: 3, nullable: true, collation: "utf8mb4_unicode_ci")
        //                 .Annotation("MySql:CharSet", "utf8mb4"),
        //             active = table.Column<bool>(type: "tinyint(1)", nullable: true)
        //         },
        //         constraints: table =>
        //         {
        //             table.PrimaryKey("PRIMARY", x => x.locationID);
        //             table.ForeignKey(
        //                 name: "FK_Location_Currency",
        //                 column: x => x.currency,
        //                 principalTable: "currency",
        //                 principalColumn: "code");
        //         })
        //         .Annotation("MySql:CharSet", "utf8mb4")
        //         .Annotation("Relational:Collation", "utf8mb4_unicode_ci");
        //
        //     migrationBuilder.CreateTable(
        //         name: "market",
        //         columns: table => new
        //         {
        //             marketID = table.Column<int>(type: "int(11)", nullable: false)
        //                 .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
        //             name = table.Column<string>(type: "text", nullable: true, collation: "utf8mb4_unicode_ci")
        //                 .Annotation("MySql:CharSet", "utf8mb4"),
        //             IsInUse = table.Column<bool>(type: "tinyint(1)", nullable: false),
        //             currency = table.Column<string>(type: "char(3)", fixedLength: true, maxLength: 3, nullable: false, collation: "utf8mb4_unicode_ci")
        //                 .Annotation("MySql:CharSet", "utf8mb4")
        //         },
        //         constraints: table =>
        //         {
        //             table.PrimaryKey("PRIMARY", x => x.marketID);
        //             table.ForeignKey(
        //                 name: "FK_market_Currency",
        //                 column: x => x.currency,
        //                 principalTable: "currency",
        //                 principalColumn: "code");
        //         })
        //         .Annotation("MySql:CharSet", "utf8mb4")
        //         .Annotation("Relational:Collation", "utf8mb4_unicode_ci");
        //
        //     migrationBuilder.CreateTable(
        //         name: "suppliers",
        //         columns: table => new
        //         {
        //             supplierID = table.Column<int>(type: "int(11)", nullable: false)
        //                 .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
        //             name = table.Column<string>(type: "text", nullable: true, collation: "utf8mb4_unicode_ci")
        //                 .Annotation("MySql:CharSet", "utf8mb4"),
        //             currency = table.Column<string>(type: "char(3)", fixedLength: true, maxLength: 3, nullable: false, collation: "utf8mb4_unicode_ci")
        //                 .Annotation("MySql:CharSet", "utf8mb4")
        //         },
        //         constraints: table =>
        //         {
        //             table.PrimaryKey("PRIMARY", x => x.supplierID);
        //             table.ForeignKey(
        //                 name: "FK_Supplier_Currency",
        //                 column: x => x.currency,
        //                 principalTable: "currency",
        //                 principalColumn: "code");
        //         })
        //         .Annotation("MySql:CharSet", "utf8mb4")
        //         .Annotation("Relational:Collation", "utf8mb4_unicode_ci");
        //
        //     migrationBuilder.CreateTable(
        //         name: "xrate",
        //         columns: table => new
        //         {
        //             XrateId = table.Column<int>(type: "int(11)", nullable: false)
        //                 .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
        //             date = table.Column<DateTime>(type: "datetime", nullable: false),
        //             rate = table.Column<decimal>(type: "decimal(18)", precision: 18, nullable: false),
        //             code = table.Column<string>(type: "char(3)", fixedLength: true, maxLength: 3, nullable: false, collation: "utf8mb4_unicode_ci")
        //                 .Annotation("MySql:CharSet", "utf8mb4"),
        //             SourceCurrencyCode = table.Column<string>(type: "char(3)", fixedLength: true, maxLength: 3, nullable: false, collation: "utf8mb4_unicode_ci")
        //                 .Annotation("MySql:CharSet", "utf8mb4")
        //         },
        //         constraints: table =>
        //         {
        //             table.PrimaryKey("PRIMARY", x => x.XrateId);
        //             table.ForeignKey(
        //                 name: "FK_Xrate_Currency",
        //                 column: x => x.code,
        //                 principalTable: "currency",
        //                 principalColumn: "code");
        //             table.ForeignKey(
        //                 name: "FK_Xrate_Currency2",
        //                 column: x => x.SourceCurrencyCode,
        //                 principalTable: "currency",
        //                 principalColumn: "code");
        //         })
        //         .Annotation("MySql:CharSet", "utf8mb4")
        //         .Annotation("Relational:Collation", "utf8mb4_unicode_ci");
        //
        //     migrationBuilder.CreateTable(
        //         name: "mayalsofit",
        //         columns: table => new
        //         {
        //             mayalsofitID = table.Column<int>(type: "int(11)", nullable: false)
        //                 .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
        //             group4bodiesID = table.Column<int>(type: "int(11)", nullable: false),
        //             group4watchesID = table.Column<int>(type: "int(11)", nullable: false)
        //         },
        //         constraints: table =>
        //         {
        //             table.PrimaryKey("PRIMARY", x => x.mayalsofitID);
        //             table.ForeignKey(
        //                 name: "FK_mayalsofit_group4bodies",
        //                 column: x => x.group4bodiesID,
        //                 principalTable: "group4bodies",
        //                 principalColumn: "group4bodiesID",
        //                 onDelete: ReferentialAction.Cascade);
        //             table.ForeignKey(
        //                 name: "FK_mayalsofit_group4watches",
        //                 column: x => x.group4watchesID,
        //                 principalTable: "group4watches",
        //                 principalColumn: "group4watchesID",
        //                 onDelete: ReferentialAction.Cascade);
        //         })
        //         .Annotation("MySql:CharSet", "utf8mb4")
        //         .Annotation("Relational:Collation", "utf8mb4_unicode_ci");
        //
        //     migrationBuilder.CreateTable(
        //         name: "parametervalues",
        //         columns: table => new
        //         {
        //             parameterValueID = table.Column<int>(type: "int(11)", nullable: false)
        //                 .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
        //             parameterID = table.Column<int>(type: "int(11)", nullable: false),
        //             name = table.Column<string>(type: "text", nullable: false, collation: "utf8mb4_unicode_ci")
        //                 .Annotation("MySql:CharSet", "utf8mb4"),
        //             pos = table.Column<int>(type: "int(11)", nullable: false)
        //         },
        //         constraints: table =>
        //         {
        //             table.PrimaryKey("PRIMARY", x => x.parameterValueID);
        //             table.ForeignKey(
        //                 name: "FK_cechyValues_parameterID",
        //                 column: x => x.parameterID,
        //                 principalTable: "parameters",
        //                 principalColumn: "parameterID",
        //                 onDelete: ReferentialAction.Cascade);
        //         })
        //         .Annotation("MySql:CharSet", "utf8mb4")
        //         .Annotation("Relational:Collation", "utf8mb4_unicode_ci");
        //
        //     migrationBuilder.CreateTable(
        //         name: "itembody",
        //         columns: table => new
        //         {
        //             itembodyID = table.Column<int>(type: "int(11)", nullable: false)
        //                 .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
        //             brandID = table.Column<int>(type: "int(11)", nullable: false),
        //             name = table.Column<string>(type: "text", nullable: false, collation: "utf8mb4_unicode_ci")
        //                 .Annotation("MySql:CharSet", "utf8mb4"),
        //             myname = table.Column<string>(type: "text", nullable: false, collation: "utf8mb4_unicode_ci")
        //                 .Annotation("MySql:CharSet", "utf8mb4"),
        //             mpn = table.Column<string>(type: "text", nullable: false, collation: "utf8mb4_unicode_ci")
        //                 .Annotation("MySql:CharSet", "utf8mb4"),
        //             visible = table.Column<bool>(type: "tinyint(1)", nullable: false),
        //             description = table.Column<string>(type: "text", nullable: true, collation: "utf8mb4_unicode_ci")
        //                 .Annotation("MySql:CharSet", "utf8mb4"),
        //             readyTotrack = table.Column<bool>(type: "tinyint(1)", nullable: false),
        //             typeId = table.Column<int>(type: "int(11)", nullable: false),
        //             logoPic = table.Column<string>(type: "text", nullable: true, collation: "utf8mb4_unicode_ci")
        //                 .Annotation("MySql:CharSet", "utf8mb4"),
        //             packagePic = table.Column<string>(type: "text", nullable: true, collation: "utf8mb4_unicode_ci")
        //                 .Annotation("MySql:CharSet", "utf8mb4"),
        //             fullsearchterm = table.Column<string>(type: "text", nullable: true, collation: "utf8mb4_unicode_ci")
        //                 .Annotation("MySql:CharSet", "utf8mb4"),
        //             weight = table.Column<int>(type: "int(11)", nullable: false)
        //         },
        //         constraints: table =>
        //         {
        //             table.PrimaryKey("PRIMARY", x => x.itembodyID);
        //             table.ForeignKey(
        //                 name: "FK_itemBody_brandID",
        //                 column: x => x.brandID,
        //                 principalTable: "brands",
        //                 principalColumn: "brandID");
        //             table.ForeignKey(
        //                 name: "FK_itemBody_type",
        //                 column: x => x.typeId,
        //                 principalTable: "types",
        //                 principalColumn: "typeID");
        //         })
        //         .Annotation("MySql:CharSet", "utf8mb4")
        //         .Annotation("Relational:Collation", "utf8mb4_unicode_ci");
        //
        //     migrationBuilder.CreateTable(
        //         name: "typeparassociation",
        //         columns: table => new
        //         {
        //             typeID = table.Column<int>(type: "int(11)", nullable: false),
        //             parameterID = table.Column<int>(type: "int(11)", nullable: false),
        //             pos = table.Column<int>(type: "int(11)", nullable: false)
        //         },
        //         constraints: table =>
        //         {
        //             table.PrimaryKey("PRIMARY", x => new { x.typeID, x.parameterID })
        //                 .Annotation("MySql:IndexPrefixLength", new[] { 0, 0 });
        //             table.ForeignKey(
        //                 name: "FK_typeParAssociation_parameterID",
        //                 column: x => x.parameterID,
        //                 principalTable: "parameters",
        //                 principalColumn: "parameterID");
        //             table.ForeignKey(
        //                 name: "FK_typeParAssociation_typeID",
        //                 column: x => x.typeID,
        //                 principalTable: "types",
        //                 principalColumn: "typeID",
        //                 onDelete: ReferentialAction.Cascade);
        //         })
        //         .Annotation("MySql:CharSet", "utf8mb4")
        //         .Annotation("Relational:Collation", "utf8mb4_unicode_ci");
        //
        //     migrationBuilder.CreateTable(
        //         name: "watchesgrouped",
        //         columns: table => new
        //         {
        //             watchesGroupedID = table.Column<int>(type: "int(11)", nullable: false)
        //                 .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
        //             group4watchesID = table.Column<int>(type: "int(11)", nullable: false),
        //             watchID = table.Column<int>(type: "int(11)", nullable: false)
        //         },
        //         constraints: table =>
        //         {
        //             table.PrimaryKey("PRIMARY", x => x.watchesGroupedID);
        //             table.ForeignKey(
        //                 name: "fk_watchesGrouped_group4watches",
        //                 column: x => x.group4watchesID,
        //                 principalTable: "group4watches",
        //                 principalColumn: "group4watchesID",
        //                 onDelete: ReferentialAction.Cascade);
        //             table.ForeignKey(
        //                 name: "fk_watchesGrouped_watch",
        //                 column: x => x.watchID,
        //                 principalTable: "watch",
        //                 principalColumn: "watchID",
        //                 onDelete: ReferentialAction.Cascade);
        //         })
        //         .Annotation("MySql:CharSet", "utf8mb4")
        //         .Annotation("Relational:Collation", "utf8mb4_unicode_ci");
        //
        //     migrationBuilder.CreateTable(
        //         name: "customer",
        //         columns: table => new
        //         {
        //             customerID = table.Column<int>(type: "int(11)", nullable: false)
        //                 .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
        //             Title = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true, collation: "utf8mb4_unicode_ci")
        //                 .Annotation("MySql:CharSet", "utf8mb4"),
        //             GivenName = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true, collation: "utf8mb4_unicode_ci")
        //                 .Annotation("MySql:CharSet", "utf8mb4"),
        //             MiddleName = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true, collation: "utf8mb4_unicode_ci")
        //                 .Annotation("MySql:CharSet", "utf8mb4"),
        //             FamilyName = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true, collation: "utf8mb4_unicode_ci")
        //                 .Annotation("MySql:CharSet", "utf8mb4"),
        //             CompanyName = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true, collation: "utf8mb4_unicode_ci")
        //                 .Annotation("MySql:CharSet", "utf8mb4"),
        //             Email = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true, collation: "utf8mb4_unicode_ci")
        //                 .Annotation("MySql:CharSet", "utf8mb4"),
        //             Phone = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true, collation: "utf8mb4_unicode_ci")
        //                 .Annotation("MySql:CharSet", "utf8mb4"),
        //             DisplayName = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true, collation: "utf8mb4_unicode_ci")
        //                 .Annotation("MySql:CharSet", "utf8mb4"),
        //             currency = table.Column<string>(type: "varchar(3)", maxLength: 3, nullable: true, collation: "utf8mb4_unicode_ci")
        //                 .Annotation("MySql:CharSet", "utf8mb4"),
        //             customer_notes = table.Column<string>(type: "text", nullable: true, collation: "utf8mb4_unicode_ci")
        //                 .Annotation("MySql:CharSet", "utf8mb4"),
        //             billaddrID = table.Column<int>(type: "int(11)", nullable: true)
        //         },
        //         constraints: table =>
        //         {
        //             table.PrimaryKey("PRIMARY", x => x.customerID);
        //             table.ForeignKey(
        //                 name: "FK_Customer_BillAddr",
        //                 column: x => x.billaddrID,
        //                 principalTable: "billaddr",
        //                 principalColumn: "billaddrID");
        //             table.ForeignKey(
        //                 name: "FK_Customer_Currency",
        //                 column: x => x.currency,
        //                 principalTable: "currency",
        //                 principalColumn: "code");
        //         })
        //         .Annotation("MySql:CharSet", "utf8mb4")
        //         .Annotation("Relational:Collation", "utf8mb4_unicode_ci");
        //
        //     migrationBuilder.CreateTable(
        //         name: "asinsku",
        //         columns: table => new
        //         {
        //             asinskuID = table.Column<int>(type: "int(11)", nullable: false)
        //                 .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
        //             asin = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false, collation: "utf8mb4_unicode_ci")
        //                 .Annotation("MySql:CharSet", "utf8mb4"),
        //             sku = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false, collation: "utf8mb4_unicode_ci")
        //                 .Annotation("MySql:CharSet", "utf8mb4"),
        //             locationID = table.Column<int>(type: "int(11)", nullable: false)
        //         },
        //         constraints: table =>
        //         {
        //             table.PrimaryKey("PRIMARY", x => x.asinskuID);
        //             table.ForeignKey(
        //                 name: "FK_AsinSku_Location",
        //                 column: x => x.locationID,
        //                 principalTable: "location",
        //                 principalColumn: "locationID");
        //         })
        //         .Annotation("MySql:CharSet", "utf8mb4")
        //         .Annotation("Relational:Collation", "utf8mb4_unicode_ci");
        //
        //     migrationBuilder.CreateTable(
        //         name: "amazonmarketplace",
        //         columns: table => new
        //         {
        //             Id = table.Column<int>(type: "int(11)", nullable: false)
        //                 .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
        //             locationID = table.Column<int>(type: "int(11)", nullable: false),
        //             marketID = table.Column<int>(type: "int(11)", nullable: false),
        //             code = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false, collation: "utf8mb4_unicode_ci")
        //                 .Annotation("MySql:CharSet", "utf8mb4")
        //         },
        //         constraints: table =>
        //         {
        //             table.PrimaryKey("PRIMARY", x => x.Id);
        //             table.ForeignKey(
        //                 name: "FK_AmazonMarketplace_Location",
        //                 column: x => x.locationID,
        //                 principalTable: "location",
        //                 principalColumn: "locationID");
        //             table.ForeignKey(
        //                 name: "FK_AmazonMarketplace_market",
        //                 column: x => x.marketID,
        //                 principalTable: "market",
        //                 principalColumn: "marketID");
        //         })
        //         .Annotation("MySql:CharSet", "utf8mb4")
        //         .Annotation("Relational:Collation", "utf8mb4_unicode_ci");
        //
        //     migrationBuilder.CreateTable(
        //         name: "locmarassociation",
        //         columns: table => new
        //         {
        //             loc = table.Column<int>(type: "int(11)", nullable: false),
        //             reference = table.Column<int>(type: "int(11)", nullable: false),
        //             pos = table.Column<int>(type: "int(11)", nullable: false)
        //         },
        //         constraints: table =>
        //         {
        //             table.PrimaryKey("PRIMARY", x => new { x.loc, x.reference })
        //                 .Annotation("MySql:IndexPrefixLength", new[] { 0, 0 });
        //             table.ForeignKey(
        //                 name: "fk_locMarAssociation_location",
        //                 column: x => x.loc,
        //                 principalTable: "location",
        //                 principalColumn: "locationID");
        //             table.ForeignKey(
        //                 name: "fk_locMarAssociation_market",
        //                 column: x => x.reference,
        //                 principalTable: "market",
        //                 principalColumn: "marketID");
        //         })
        //         .Annotation("MySql:CharSet", "utf8mb4")
        //         .Annotation("Relational:Collation", "utf8mb4_unicode_ci");
        //
        //     migrationBuilder.CreateTable(
        //         name: "marketplatformassociation",
        //         columns: table => new
        //         {
        //             MarketPlatformAssociationID = table.Column<int>(type: "int(11)", nullable: false)
        //                 .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
        //             marketID = table.Column<int>(type: "int(11)", nullable: false),
        //             platformID = table.Column<int>(type: "int(11)", nullable: false)
        //         },
        //         constraints: table =>
        //         {
        //             table.PrimaryKey("PRIMARY", x => x.MarketPlatformAssociationID);
        //             table.ForeignKey(
        //                 name: "FK_MarketPlatformAssociation_market",
        //                 column: x => x.marketID,
        //                 principalTable: "market",
        //                 principalColumn: "marketID");
        //             table.ForeignKey(
        //                 name: "FK_MarketPlatformAssociation_platforms",
        //                 column: x => x.platformID,
        //                 principalTable: "platforms",
        //                 principalColumn: "platformID");
        //         })
        //         .Annotation("MySql:CharSet", "utf8mb4")
        //         .Annotation("Relational:Collation", "utf8mb4_unicode_ci");
        //
        //     migrationBuilder.CreateTable(
        //         name: "colourtranslation",
        //         columns: table => new
        //         {
        //             Id = table.Column<int>(type: "int(11)", nullable: false)
        //                 .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
        //             kodKoloru = table.Column<int>(type: "int(11)", nullable: false),
        //             schemat = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false, collation: "utf8mb4_unicode_ci")
        //                 .Annotation("MySql:CharSet", "utf8mb4"),
        //             col1 = table.Column<int>(type: "int(11)", nullable: false),
        //             col2 = table.Column<int>(type: "int(11)", nullable: true),
        //             col3 = table.Column<int>(type: "int(11)", nullable: true),
        //             col4 = table.Column<int>(type: "int(11)", nullable: true)
        //         },
        //         constraints: table =>
        //         {
        //             table.PrimaryKey("PRIMARY", x => x.Id);
        //             table.ForeignKey(
        //                 name: "FK_ColourTranslation_kodKoloru",
        //                 column: x => x.kodKoloru,
        //                 principalTable: "parametervalues",
        //                 principalColumn: "parameterValueID");
        //         })
        //         .Annotation("MySql:CharSet", "utf8mb4")
        //         .Annotation("Relational:Collation", "utf8mb4_unicode_ci");
        //
        //     migrationBuilder.CreateTable(
        //         name: "bodiesgrouped",
        //         columns: table => new
        //         {
        //             bodiesGroupedID = table.Column<int>(type: "int(11)", nullable: false)
        //                 .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
        //             group4bodiesID = table.Column<int>(type: "int(11)", nullable: false),
        //             itemBodyID = table.Column<int>(type: "int(11)", nullable: false)
        //         },
        //         constraints: table =>
        //         {
        //             table.PrimaryKey("PRIMARY", x => x.bodiesGroupedID);
        //             table.ForeignKey(
        //                 name: "FK_bodiesGrouped_group4bodies",
        //                 column: x => x.group4bodiesID,
        //                 principalTable: "group4bodies",
        //                 principalColumn: "group4bodiesID",
        //                 onDelete: ReferentialAction.Cascade);
        //             table.ForeignKey(
        //                 name: "FK_bodiesGrouped_itemBody",
        //                 column: x => x.itemBodyID,
        //                 principalTable: "itembody",
        //                 principalColumn: "itembodyID",
        //                 onDelete: ReferentialAction.Cascade);
        //         })
        //         .Annotation("MySql:CharSet", "utf8mb4")
        //         .Annotation("Relational:Collation", "utf8mb4_unicode_ci");
        //
        //     migrationBuilder.CreateTable(
        //         name: "bodyinthebox",
        //         columns: table => new
        //         {
        //             BodyInTheBoxID = table.Column<int>(type: "int(11)", nullable: false)
        //                 .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
        //             itembodyID = table.Column<int>(type: "int(11)", nullable: false),
        //             MultiDrawerID = table.Column<int>(type: "int(11)", nullable: false),
        //             row = table.Column<int>(type: "int(11)", nullable: false),
        //             column = table.Column<int>(type: "int(11)", nullable: false)
        //         },
        //         constraints: table =>
        //         {
        //             table.PrimaryKey("PRIMARY", x => x.BodyInTheBoxID);
        //             table.ForeignKey(
        //                 name: "fk_BodyInTheBox_MultiDrawer",
        //                 column: x => x.MultiDrawerID,
        //                 principalTable: "multidrawer",
        //                 principalColumn: "MultiDrawerID",
        //                 onDelete: ReferentialAction.Cascade);
        //             table.ForeignKey(
        //                 name: "fk_BodyInTheBox_itembody",
        //                 column: x => x.itembodyID,
        //                 principalTable: "itembody",
        //                 principalColumn: "itembodyID",
        //                 onDelete: ReferentialAction.Cascade);
        //         })
        //         .Annotation("MySql:CharSet", "utf8mb4")
        //         .Annotation("Relational:Collation", "utf8mb4_unicode_ci");
        //
        //     migrationBuilder.CreateTable(
        //         name: "itemheader",
        //         columns: table => new
        //         {
        //             itemheaderID = table.Column<int>(type: "int(11)", nullable: false)
        //                 .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
        //             locationID = table.Column<int>(type: "int(11)", nullable: false),
        //             purchasedOn = table.Column<DateTime>(type: "datetime", nullable: true),
        //             supplierID = table.Column<int>(type: "int(11)", nullable: false),
        //             itembodyID = table.Column<int>(type: "int(11)", nullable: false),
        //             pricePaid = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: false),
        //             purchasecurrency = table.Column<string>(type: "varchar(3)", maxLength: 3, nullable: true, collation: "utf8mb4_unicode_ci")
        //                 .Annotation("MySql:CharSet", "utf8mb4"),
        //             acquiredcurrency = table.Column<string>(type: "varchar(3)", maxLength: 3, nullable: true, collation: "utf8mb4_unicode_ci")
        //                 .Annotation("MySql:CharSet", "utf8mb4"),
        //             xchgrate = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: false),
        //             quantity = table.Column<int>(type: "int(11)", nullable: false),
        //             VATRateID = table.Column<int>(type: "int(11)", nullable: false)
        //         },
        //         constraints: table =>
        //         {
        //             table.PrimaryKey("PRIMARY", x => x.itemheaderID);
        //             table.ForeignKey(
        //                 name: "FK_itemHeader_Location",
        //                 column: x => x.locationID,
        //                 principalTable: "location",
        //                 principalColumn: "locationID");
        //             table.ForeignKey(
        //                 name: "FK_itemHeader_itemBody",
        //                 column: x => x.itembodyID,
        //                 principalTable: "itembody",
        //                 principalColumn: "itembodyID",
        //                 onDelete: ReferentialAction.Cascade);
        //             table.ForeignKey(
        //                 name: "FK_itemHeader_suppliers",
        //                 column: x => x.supplierID,
        //                 principalTable: "suppliers",
        //                 principalColumn: "supplierID");
        //             table.ForeignKey(
        //                 name: "FK_itemheader2_currency",
        //                 column: x => x.acquiredcurrency,
        //                 principalTable: "currency",
        //                 principalColumn: "code");
        //             table.ForeignKey(
        //                 name: "FK_itemheader_VATRates",
        //                 column: x => x.VATRateID,
        //                 principalTable: "vatrates",
        //                 principalColumn: "VATRateID");
        //             table.ForeignKey(
        //                 name: "FK_itemheader_currency",
        //                 column: x => x.purchasecurrency,
        //                 principalTable: "currency",
        //                 principalColumn: "code");
        //         })
        //         .Annotation("MySql:CharSet", "utf8mb4")
        //         .Annotation("Relational:Collation", "utf8mb4_unicode_ci");
        //
        //     migrationBuilder.CreateTable(
        //         name: "itmitmassociation",
        //         columns: table => new
        //         {
        //             itmitmassID = table.Column<int>(type: "int(11)", nullable: false)
        //                 .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
        //             sourceBody = table.Column<int>(type: "int(11)", nullable: false),
        //             targetBody = table.Column<int>(type: "int(11)", nullable: false)
        //         },
        //         constraints: table =>
        //         {
        //             table.PrimaryKey("PRIMARY", x => x.itmitmassID);
        //             table.ForeignKey(
        //                 name: "FK_itmitmAssociation_sourceBody",
        //                 column: x => x.sourceBody,
        //                 principalTable: "itembody",
        //                 principalColumn: "itembodyID");
        //             table.ForeignKey(
        //                 name: "FK_itmitmAssociation_targetBody",
        //                 column: x => x.targetBody,
        //                 principalTable: "itembody",
        //                 principalColumn: "itembodyID");
        //         })
        //         .Annotation("MySql:CharSet", "utf8mb4")
        //         .Annotation("Relational:Collation", "utf8mb4_unicode_ci");
        //
        //     migrationBuilder.CreateTable(
        //         name: "itmmarketassoc",
        //         columns: table => new
        //         {
        //             itmmarketassID = table.Column<int>(type: "int(11)", nullable: false)
        //                 .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
        //             itembodyID = table.Column<int>(type: "int(11)", nullable: false),
        //             marketID = table.Column<int>(type: "int(11)", nullable: false),
        //             quantitySold = table.Column<int>(type: "int(11)", nullable: false),
        //             soldWith = table.Column<int>(type: "int(11)", nullable: true),
        //             locationID = table.Column<int>(type: "int(11)", nullable: false),
        //             itemNumber = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: false, collation: "utf8mb4_unicode_ci")
        //                 .Annotation("MySql:CharSet", "utf8mb4"),
        //             SEName = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true, collation: "utf8mb4_unicode_ci")
        //                 .Annotation("MySql:CharSet", "utf8mb4")
        //         },
        //         constraints: table =>
        //         {
        //             table.PrimaryKey("PRIMARY", x => x.itmmarketassID);
        //             table.ForeignKey(
        //                 name: "FK_itmMarketAssoc_itembodyID",
        //                 column: x => x.itembodyID,
        //                 principalTable: "itembody",
        //                 principalColumn: "itembodyID",
        //                 onDelete: ReferentialAction.Cascade);
        //             table.ForeignKey(
        //                 name: "FK_itmMarketAssoc_locationID",
        //                 column: x => x.locationID,
        //                 principalTable: "location",
        //                 principalColumn: "locationID");
        //             table.ForeignKey(
        //                 name: "FK_itmMarketAssoc_marketID",
        //                 column: x => x.marketID,
        //                 principalTable: "market",
        //                 principalColumn: "marketID");
        //             table.ForeignKey(
        //                 name: "FK_itmMarketAssoc_soldWith",
        //                 column: x => x.soldWith,
        //                 principalTable: "itembody",
        //                 principalColumn: "itembodyID");
        //         })
        //         .Annotation("MySql:CharSet", "utf8mb4")
        //         .Annotation("Relational:Collation", "utf8mb4_unicode_ci");
        //
        //     migrationBuilder.CreateTable(
        //         name: "itmparameters",
        //         columns: table => new
        //         {
        //             itmparameterID = table.Column<int>(type: "int(11)", nullable: false)
        //                 .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
        //             itembodyID = table.Column<int>(type: "int(11)", nullable: false),
        //             parameterID = table.Column<int>(type: "int(11)", nullable: false),
        //             parameterValueID = table.Column<int>(type: "int(11)", nullable: false)
        //         },
        //         constraints: table =>
        //         {
        //             table.PrimaryKey("PRIMARY", x => x.itmparameterID);
        //             table.ForeignKey(
        //                 name: "fk_itmParameters_cechy",
        //                 column: x => x.parameterID,
        //                 principalTable: "parameters",
        //                 principalColumn: "parameterID",
        //                 onDelete: ReferentialAction.Cascade);
        //             table.ForeignKey(
        //                 name: "fk_itmParameters_cechyValues",
        //                 column: x => x.parameterValueID,
        //                 principalTable: "parametervalues",
        //                 principalColumn: "parameterValueID");
        //             table.ForeignKey(
        //                 name: "fk_itmParameters_itembody",
        //                 column: x => x.itembodyID,
        //                 principalTable: "itembody",
        //                 principalColumn: "itembodyID",
        //                 onDelete: ReferentialAction.Cascade);
        //         })
        //         .Annotation("MySql:CharSet", "utf8mb4")
        //         .Annotation("Relational:Collation", "utf8mb4_unicode_ci");
        //
        //     migrationBuilder.CreateTable(
        //         name: "part2itemass",
        //         columns: table => new
        //         {
        //             part2itemassID = table.Column<int>(type: "int(11)", nullable: false)
        //                 .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
        //             itembodyID = table.Column<int>(type: "int(11)", nullable: false),
        //             watchID = table.Column<int>(type: "int(11)", nullable: false),
        //             watchsearchterm = table.Column<string>(type: "longtext", nullable: false, collation: "utf8mb4_unicode_ci")
        //                 .Annotation("MySql:CharSet", "utf8mb4")
        //         },
        //         constraints: table =>
        //         {
        //             table.PrimaryKey("PRIMARY", x => x.part2itemassID);
        //             table.ForeignKey(
        //                 name: "FK_part2itemass_itembody",
        //                 column: x => x.itembodyID,
        //                 principalTable: "itembody",
        //                 principalColumn: "itembodyID");
        //             table.ForeignKey(
        //                 name: "FK_part2itemass_watch",
        //                 column: x => x.watchID,
        //                 principalTable: "watch",
        //                 principalColumn: "watchID");
        //         })
        //         .Annotation("MySql:CharSet", "utf8mb4")
        //         .Annotation("Relational:Collation", "utf8mb4_unicode_ci");
        //
        //     migrationBuilder.CreateTable(
        //         name: "photo",
        //         columns: table => new
        //         {
        //             photoID = table.Column<int>(type: "int(11)", nullable: false)
        //                 .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
        //             path = table.Column<string>(type: "text", nullable: false, collation: "utf8mb4_unicode_ci")
        //                 .Annotation("MySql:CharSet", "utf8mb4"),
        //             itembodyID = table.Column<int>(type: "int(11)", nullable: false),
        //             pos = table.Column<int>(type: "int(11)", nullable: false)
        //         },
        //         constraints: table =>
        //         {
        //             table.PrimaryKey("PRIMARY", x => x.photoID);
        //             table.ForeignKey(
        //                 name: "fk_photo_itemBody",
        //                 column: x => x.itembodyID,
        //                 principalTable: "itembody",
        //                 principalColumn: "itembodyID");
        //         })
        //         .Annotation("MySql:CharSet", "utf8mb4")
        //         .Annotation("Relational:Collation", "utf8mb4_unicode_ci");
        //
        //     migrationBuilder.CreateTable(
        //         name: "shopitem",
        //         columns: table => new
        //         {
        //             shopitemId = table.Column<int>(type: "int(11)", nullable: false)
        //                 .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
        //             price = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: false),
        //             itembodyID = table.Column<int>(type: "int(11)", nullable: false),
        //             locationID = table.Column<int>(type: "int(11)", nullable: false),
        //             quantity = table.Column<int>(type: "int(11)", nullable: false),
        //             currencyCode = table.Column<string>(type: "varchar(3)", maxLength: 3, nullable: false, collation: "utf8mb4_unicode_ci")
        //                 .Annotation("MySql:CharSet", "utf8mb4"),
        //             active = table.Column<bool>(type: "tinyint(1)", nullable: false),
        //             soldQuantity = table.Column<int>(type: "int(11)", nullable: true)
        //         },
        //         constraints: table =>
        //         {
        //             table.PrimaryKey("PRIMARY", x => x.shopitemId);
        //             table.ForeignKey(
        //                 name: "FK_shopitem_currencyCode",
        //                 column: x => x.currencyCode,
        //                 principalTable: "currency",
        //                 principalColumn: "code");
        //             table.ForeignKey(
        //                 name: "FK_shopitem_itembodyID",
        //                 column: x => x.itembodyID,
        //                 principalTable: "itembody",
        //                 principalColumn: "itembodyID",
        //                 onDelete: ReferentialAction.Cascade);
        //             table.ForeignKey(
        //                 name: "FK_shopitem_locationID",
        //                 column: x => x.locationID,
        //                 principalTable: "location",
        //                 principalColumn: "locationID");
        //         })
        //         .Annotation("MySql:CharSet", "utf8mb4")
        //         .Annotation("Relational:Collation", "utf8mb4_unicode_ci");
        //
        //     migrationBuilder.CreateTable(
        //         name: "stockshot",
        //         columns: table => new
        //         {
        //             StockShotID = table.Column<int>(type: "int(11)", nullable: false)
        //                 .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
        //             bodyid = table.Column<int>(type: "int(11)", nullable: false),
        //             quantity = table.Column<int>(type: "int(11)", nullable: false),
        //             date = table.Column<DateTime>(type: "datetime", nullable: false),
        //             locationID = table.Column<int>(type: "int(11)", nullable: false)
        //         },
        //         constraints: table =>
        //         {
        //             table.PrimaryKey("PRIMARY", x => x.StockShotID);
        //             table.ForeignKey(
        //                 name: "FK_stockShot_bodyid",
        //                 column: x => x.bodyid,
        //                 principalTable: "itembody",
        //                 principalColumn: "itembodyID");
        //             table.ForeignKey(
        //                 name: "FK_stockShot_location",
        //                 column: x => x.locationID,
        //                 principalTable: "location",
        //                 principalColumn: "locationID");
        //         })
        //         .Annotation("MySql:CharSet", "utf8mb4")
        //         .Annotation("Relational:Collation", "utf8mb4_unicode_ci");
        //
        //     migrationBuilder.CreateTable(
        //         name: "order",
        //         columns: table => new
        //         {
        //             orderID = table.Column<int>(type: "int(11)", nullable: false)
        //                 .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
        //             quickbooked = table.Column<bool>(type: "tinyint(1)", nullable: false),
        //             customerID = table.Column<int>(type: "int(11)", nullable: false),
        //             paidOn = table.Column<DateTime>(type: "datetime", nullable: false),
        //             dispatchedOn = table.Column<DateTime>(type: "datetime", nullable: false),
        //             tracking = table.Column<string>(type: "text", nullable: false, collation: "utf8mb4_unicode_ci")
        //                 .Annotation("MySql:CharSet", "utf8mb4"),
        //             market = table.Column<int>(type: "int(11)", nullable: false),
        //             locationID = table.Column<int>(type: "int(11)", nullable: false),
        //             salecurrency = table.Column<string>(type: "varchar(3)", maxLength: 3, nullable: true, collation: "utf8mb4_unicode_ci")
        //                 .Annotation("MySql:CharSet", "utf8mb4"),
        //             acquiredcurrency = table.Column<string>(type: "varchar(3)", maxLength: 3, nullable: true, collation: "utf8mb4_unicode_ci")
        //                 .Annotation("MySql:CharSet", "utf8mb4"),
        //             saletotal = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: false),
        //             xchgrate = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: false),
        //             VAT = table.Column<bool>(type: "tinyint(1)", nullable: false),
        //             order_notes = table.Column<string>(type: "text", nullable: true, collation: "utf8mb4_unicode_ci")
        //                 .Annotation("MySql:CharSet", "utf8mb4"),
        //             status = table.Column<string>(type: "varchar(4)", maxLength: 4, nullable: false, collation: "utf8mb4_unicode_ci")
        //                 .Annotation("MySql:CharSet", "utf8mb4"),
        //             postagePrice = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: false),
        //             postageType = table.Column<string>(type: "varchar(5)", maxLength: 5, nullable: true, collation: "utf8mb4_unicode_ci")
        //                 .Annotation("MySql:CharSet", "utf8mb4"),
        //             VATRateID = table.Column<int>(type: "int(11)", nullable: false)
        //         },
        //         constraints: table =>
        //         {
        //             table.PrimaryKey("PRIMARY", x => x.orderID);
        //             table.ForeignKey(
        //                 name: "FK_Order_Currency2",
        //                 column: x => x.acquiredcurrency,
        //                 principalTable: "currency",
        //                 principalColumn: "code");
        //             table.ForeignKey(
        //                 name: "FK_Order_VATRates",
        //                 column: x => x.VATRateID,
        //                 principalTable: "vatrates",
        //                 principalColumn: "VATRateID");
        //             table.ForeignKey(
        //                 name: "FK_Order_currency",
        //                 column: x => x.salecurrency,
        //                 principalTable: "currency",
        //                 principalColumn: "code");
        //             table.ForeignKey(
        //                 name: "FK_Order_customerID",
        //                 column: x => x.customerID,
        //                 principalTable: "customer",
        //                 principalColumn: "customerID");
        //             table.ForeignKey(
        //                 name: "FK_Order_locationID",
        //                 column: x => x.locationID,
        //                 principalTable: "location",
        //                 principalColumn: "locationID");
        //             table.ForeignKey(
        //                 name: "FK_Order_market",
        //                 column: x => x.market,
        //                 principalTable: "market",
        //                 principalColumn: "marketID");
        //             table.ForeignKey(
        //                 name: "FK_Order_postageType",
        //                 column: x => x.postageType,
        //                 principalTable: "postagetypes",
        //                 principalColumn: "code");
        //             table.ForeignKey(
        //                 name: "FK_Order_status",
        //                 column: x => x.status,
        //                 principalTable: "orderstatus",
        //                 principalColumn: "code");
        //         })
        //         .Annotation("MySql:CharSet", "utf8mb4")
        //         .Annotation("Relational:Collation", "utf8mb4_unicode_ci");
        //
        //     migrationBuilder.CreateTable(
        //         name: "logevent",
        //         columns: table => new
        //         {
        //             logeventID = table.Column<int>(type: "int(11)", nullable: false)
        //                 .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
        //             happenedOn = table.Column<DateTime>(type: "datetime", nullable: false),
        //             itemHeaderID = table.Column<int>(type: "int(11)", nullable: true),
        //             @event = table.Column<string>(name: "event", type: "text", nullable: false, collation: "utf8mb4_unicode_ci")
        //                 .Annotation("MySql:CharSet", "utf8mb4"),
        //             itemBodyID = table.Column<int>(type: "int(11)", nullable: false),
        //             marketID = table.Column<int>(type: "int(11)", nullable: true)
        //         },
        //         constraints: table =>
        //         {
        //             table.PrimaryKey("PRIMARY", x => x.logeventID);
        //             table.ForeignKey(
        //                 name: "FK_logevent_itemBody",
        //                 column: x => x.itemBodyID,
        //                 principalTable: "itembody",
        //                 principalColumn: "itembodyID");
        //             table.ForeignKey(
        //                 name: "FK_logevent_itemHeader",
        //                 column: x => x.itemHeaderID,
        //                 principalTable: "itemheader",
        //                 principalColumn: "itemheaderID",
        //                 onDelete: ReferentialAction.Cascade);
        //             table.ForeignKey(
        //                 name: "FK_logevent_market",
        //                 column: x => x.marketID,
        //                 principalTable: "market",
        //                 principalColumn: "marketID",
        //                 onDelete: ReferentialAction.Cascade);
        //         })
        //         .Annotation("MySql:CharSet", "utf8mb4")
        //         .Annotation("Relational:Collation", "utf8mb4_unicode_ci");
        //
        //     migrationBuilder.CreateTable(
        //         name: "invoicetxns",
        //         columns: table => new
        //         {
        //             invoiceTXNID = table.Column<int>(type: "int(11)", nullable: false)
        //                 .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
        //             platformID = table.Column<int>(type: "int(11)", nullable: false),
        //             qbInvoiceId = table.Column<string>(type: "text", nullable: true, collation: "utf8mb4_unicode_ci")
        //                 .Annotation("MySql:CharSet", "utf8mb4"),
        //             platformTXN = table.Column<string>(type: "text", nullable: true, collation: "utf8mb4_unicode_ci")
        //                 .Annotation("MySql:CharSet", "utf8mb4"),
        //             orderID = table.Column<int>(type: "int(11)", nullable: false),
        //             marketID = table.Column<int>(type: "int(11)", nullable: false)
        //         },
        //         constraints: table =>
        //         {
        //             table.PrimaryKey("PRIMARY", x => x.invoiceTXNID);
        //             table.ForeignKey(
        //                 name: "FK_invoiceTXNs_market",
        //                 column: x => x.marketID,
        //                 principalTable: "market",
        //                 principalColumn: "marketID");
        //             table.ForeignKey(
        //                 name: "FK_invoiceTXNs_order",
        //                 column: x => x.orderID,
        //                 principalTable: "order",
        //                 principalColumn: "orderID",
        //                 onDelete: ReferentialAction.Cascade);
        //             table.ForeignKey(
        //                 name: "FK_invoiceTXNs_platforms",
        //                 column: x => x.platformID,
        //                 principalTable: "platforms",
        //                 principalColumn: "platformID");
        //         })
        //         .Annotation("MySql:CharSet", "utf8mb4")
        //         .Annotation("Relational:Collation", "utf8mb4_unicode_ci");
        //
        //     migrationBuilder.CreateTable(
        //         name: "orderitem",
        //         columns: table => new
        //         {
        //             OrderItemId = table.Column<int>(type: "int(11)", nullable: false)
        //                 .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
        //             itemName = table.Column<string>(type: "text", nullable: false, collation: "utf8mb4_unicode_ci")
        //                 .Annotation("MySql:CharSet", "utf8mb4"),
        //             quantity = table.Column<int>(type: "int(11)", nullable: false),
        //             itembodyID = table.Column<int>(type: "int(11)", nullable: false),
        //             OrderItemTypeId = table.Column<int>(type: "int(11)", nullable: false),
        //             price = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: false),
        //             orderID = table.Column<int>(type: "int(11)", nullable: true),
        //             itmMarketAssID = table.Column<int>(type: "int(11)", nullable: true),
        //             ItemWeight = table.Column<int>(type: "int(11)", nullable: true)
        //         },
        //         constraints: table =>
        //         {
        //             table.PrimaryKey("PRIMARY", x => x.OrderItemId);
        //             table.ForeignKey(
        //                 name: "fk_OrderItem_Order",
        //                 column: x => x.orderID,
        //                 principalTable: "order",
        //                 principalColumn: "orderID",
        //                 onDelete: ReferentialAction.Cascade);
        //             table.ForeignKey(
        //                 name: "fk_OrderItem_OrderItemType",
        //                 column: x => x.OrderItemTypeId,
        //                 principalTable: "orderitemtype",
        //                 principalColumn: "OrderItemTypeId");
        //             table.ForeignKey(
        //                 name: "fk_OrderItem_itemBody",
        //                 column: x => x.itembodyID,
        //                 principalTable: "itembody",
        //                 principalColumn: "itembodyID");
        //             table.ForeignKey(
        //                 name: "fk_OrderItem_itmMarketAssoc",
        //                 column: x => x.itmMarketAssID,
        //                 principalTable: "itmmarketassoc",
        //                 principalColumn: "itmmarketassID");
        //         })
        //         .Annotation("MySql:CharSet", "utf8mb4")
        //         .Annotation("Relational:Collation", "utf8mb4_unicode_ci");
        //
        //     migrationBuilder.CreateIndex(
        //         name: "idx_locationID",
        //         table: "amazonmarketplace",
        //         column: "locationID");
        //
        //     migrationBuilder.CreateIndex(
        //         name: "idx_marketID",
        //         table: "amazonmarketplace",
        //         column: "marketID");
        //
        //     migrationBuilder.CreateIndex(
        //         name: "idx_locationID1",
        //         table: "asinsku",
        //         column: "locationID");
        //
        //     migrationBuilder.CreateIndex(
        //         name: "UQ_AsinSku_asin_sku",
        //         table: "asinsku",
        //         columns: new[] { "asin", "sku" },
        //         unique: true);
        //
        //     migrationBuilder.CreateIndex(
        //         name: "IX_BillAddr_CountryCode",
        //         table: "billaddr",
        //         column: "CountryCode");
        //
        //     migrationBuilder.CreateIndex(
        //         name: "FK_bodiesGrouped_group4bodies",
        //         table: "bodiesgrouped",
        //         column: "group4bodiesID");
        //
        //     migrationBuilder.CreateIndex(
        //         name: "FK_bodiesGrouped_itemBody",
        //         table: "bodiesgrouped",
        //         column: "itemBodyID");
        //
        //     migrationBuilder.CreateIndex(
        //         name: "fk_itembodyID_idx",
        //         table: "bodyinthebox",
        //         column: "itembodyID");
        //
        //     migrationBuilder.CreateIndex(
        //         name: "fk_MultiDrawerID_idx",
        //         table: "bodyinthebox",
        //         column: "MultiDrawerID");
        //
        //     migrationBuilder.CreateIndex(
        //         name: "FK_ColourTranslation_kodKoloru",
        //         table: "colourtranslation",
        //         column: "kodKoloru");
        //
        //     migrationBuilder.CreateIndex(
        //         name: "uq_code",
        //         table: "country2rmass",
        //         column: "code",
        //         unique: true);
        //
        //     migrationBuilder.CreateIndex(
        //         name: "FK_VatRrate_CountryCode",
        //         table: "countryvatrrate",
        //         column: "code");
        //
        //     migrationBuilder.CreateIndex(
        //         name: "FK_Customer_Currency",
        //         table: "customer",
        //         column: "currency");
        //
        //     migrationBuilder.CreateIndex(
        //         name: "IX_Customer_BillAddrID",
        //         table: "customer",
        //         column: "billaddrID");
        //
        //     migrationBuilder.CreateIndex(
        //         name: "FK_invoiceTXNs_market",
        //         table: "invoicetxns",
        //         column: "marketID");
        //
        //     migrationBuilder.CreateIndex(
        //         name: "FK_invoiceTXNs_order",
        //         table: "invoicetxns",
        //         column: "orderID");
        //
        //     migrationBuilder.CreateIndex(
        //         name: "FK_invoiceTXNs_platforms",
        //         table: "invoicetxns",
        //         column: "platformID");
        //
        //     migrationBuilder.CreateIndex(
        //         name: "FK_itemBody_brandID",
        //         table: "itembody",
        //         column: "brandID");
        //
        //     migrationBuilder.CreateIndex(
        //         name: "FK_itemBody_type",
        //         table: "itembody",
        //         column: "typeId");
        //
        //     migrationBuilder.CreateIndex(
        //         name: "FK_itemheader_currency",
        //         table: "itemheader",
        //         column: "purchasecurrency");
        //
        //     migrationBuilder.CreateIndex(
        //         name: "FK_itemHeader_itemBody",
        //         table: "itemheader",
        //         column: "itembodyID");
        //
        //     migrationBuilder.CreateIndex(
        //         name: "FK_itemHeader_Location",
        //         table: "itemheader",
        //         column: "locationID");
        //
        //     migrationBuilder.CreateIndex(
        //         name: "FK_itemHeader_suppliers",
        //         table: "itemheader",
        //         column: "supplierID");
        //
        //     migrationBuilder.CreateIndex(
        //         name: "FK_itemheader_VATRates",
        //         table: "itemheader",
        //         column: "VATRateID");
        //
        //     migrationBuilder.CreateIndex(
        //         name: "FK_itemheader2_currency",
        //         table: "itemheader",
        //         column: "acquiredcurrency");
        //
        //     migrationBuilder.CreateIndex(
        //         name: "FK_itmitmAssociation_sourceBody",
        //         table: "itmitmassociation",
        //         column: "sourceBody");
        //
        //     migrationBuilder.CreateIndex(
        //         name: "FK_itmitmAssociation_targetBody",
        //         table: "itmitmassociation",
        //         column: "targetBody");
        //
        //     migrationBuilder.CreateIndex(
        //         name: "FK_itmMarketAssoc_itembodyID",
        //         table: "itmmarketassoc",
        //         column: "itembodyID");
        //
        //     migrationBuilder.CreateIndex(
        //         name: "FK_itmMarketAssoc_locationID",
        //         table: "itmmarketassoc",
        //         column: "locationID");
        //
        //     migrationBuilder.CreateIndex(
        //         name: "FK_itmMarketAssoc_marketID",
        //         table: "itmmarketassoc",
        //         column: "marketID");
        //
        //     migrationBuilder.CreateIndex(
        //         name: "FK_itmMarketAssoc_soldWith",
        //         table: "itmmarketassoc",
        //         column: "soldWith");
        //
        //     migrationBuilder.CreateIndex(
        //         name: "fk_cechavalueID_idx",
        //         table: "itmparameters",
        //         column: "parameterValueID");
        //
        //     migrationBuilder.CreateIndex(
        //         name: "fk_itembodyID_idx1",
        //         table: "itmparameters",
        //         column: "itembodyID");
        //
        //     migrationBuilder.CreateIndex(
        //         name: "fk_parameterID_idx",
        //         table: "itmparameters",
        //         column: "parameterID");
        //
        //     migrationBuilder.CreateIndex(
        //         name: "FK_Location_Currency",
        //         table: "location",
        //         column: "currency");
        //
        //     migrationBuilder.CreateIndex(
        //         name: "fk_locMarAssociation_market",
        //         table: "locmarassociation",
        //         column: "reference");
        //
        //     migrationBuilder.CreateIndex(
        //         name: "FK_logevent_itemBody",
        //         table: "logevent",
        //         column: "itemBodyID");
        //
        //     migrationBuilder.CreateIndex(
        //         name: "FK_logevent_itemHeader",
        //         table: "logevent",
        //         column: "itemHeaderID");
        //
        //     migrationBuilder.CreateIndex(
        //         name: "FK_logevent_market",
        //         table: "logevent",
        //         column: "marketID");
        //
        //     migrationBuilder.CreateIndex(
        //         name: "FK_market_Currency",
        //         table: "market",
        //         column: "currency");
        //
        //     migrationBuilder.CreateIndex(
        //         name: "idx_marketID1",
        //         table: "marketplatformassociation",
        //         column: "marketID");
        //
        //     migrationBuilder.CreateIndex(
        //         name: "idx_platformID",
        //         table: "marketplatformassociation",
        //         column: "platformID");
        //
        //     migrationBuilder.CreateIndex(
        //         name: "FK_mayalsofit_group4bodies",
        //         table: "mayalsofit",
        //         column: "group4bodiesID");
        //
        //     migrationBuilder.CreateIndex(
        //         name: "FK_mayalsofit_group4watches",
        //         table: "mayalsofit",
        //         column: "group4watchesID");
        //
        //     migrationBuilder.CreateIndex(
        //         name: "FK_Order_currency",
        //         table: "order",
        //         column: "salecurrency");
        //
        //     migrationBuilder.CreateIndex(
        //         name: "FK_Order_Currency2",
        //         table: "order",
        //         column: "acquiredcurrency");
        //
        //     migrationBuilder.CreateIndex(
        //         name: "FK_Order_locationID",
        //         table: "order",
        //         column: "locationID");
        //
        //     migrationBuilder.CreateIndex(
        //         name: "FK_Order_market",
        //         table: "order",
        //         column: "market");
        //
        //     migrationBuilder.CreateIndex(
        //         name: "FK_Order_postageType",
        //         table: "order",
        //         column: "postageType");
        //
        //     migrationBuilder.CreateIndex(
        //         name: "FK_Order_status",
        //         table: "order",
        //         column: "status");
        //
        //     migrationBuilder.CreateIndex(
        //         name: "FK_Order_VATRates",
        //         table: "order",
        //         column: "VATRateID");
        //
        //     migrationBuilder.CreateIndex(
        //         name: "IX_Order_CustomerID",
        //         table: "order",
        //         column: "customerID");
        //
        //     migrationBuilder.CreateIndex(
        //         name: "fk_itemId_idx",
        //         table: "orderitem",
        //         column: "itembodyID");
        //
        //     migrationBuilder.CreateIndex(
        //         name: "fk_itemType_idx",
        //         table: "orderitem",
        //         column: "OrderItemTypeId");
        //
        //     migrationBuilder.CreateIndex(
        //         name: "fk_itmMarketAssID_idx",
        //         table: "orderitem",
        //         column: "itmMarketAssID");
        //
        //     migrationBuilder.CreateIndex(
        //         name: "fk_orderID_idx",
        //         table: "orderitem",
        //         column: "orderID");
        //
        //     migrationBuilder.CreateIndex(
        //         name: "fk_parameterID_idx1",
        //         table: "parametervalues",
        //         column: "parameterID");
        //
        //     migrationBuilder.CreateIndex(
        //         name: "FK_part2itemass_itembody",
        //         table: "part2itemass",
        //         column: "itembodyID");
        //
        //     migrationBuilder.CreateIndex(
        //         name: "FK_part2itemass_watch",
        //         table: "part2itemass",
        //         column: "watchID");
        //
        //     migrationBuilder.CreateIndex(
        //         name: "fk_itembodyID_idx2",
        //         table: "photo",
        //         column: "itembodyID");
        //
        //     migrationBuilder.CreateIndex(
        //         name: "FK_shopitem_currencyCode",
        //         table: "shopitem",
        //         column: "currencyCode");
        //
        //     migrationBuilder.CreateIndex(
        //         name: "FK_shopitem_locationID",
        //         table: "shopitem",
        //         column: "locationID");
        //
        //     migrationBuilder.CreateIndex(
        //         name: "UK_shopitem_itembodyID",
        //         table: "shopitem",
        //         column: "itembodyID",
        //         unique: true);
        //
        //     migrationBuilder.CreateIndex(
        //         name: "FK_stockShot_bodyid",
        //         table: "stockshot",
        //         column: "bodyid");
        //
        //     migrationBuilder.CreateIndex(
        //         name: "FK_stockShot_location",
        //         table: "stockshot",
        //         column: "locationID");
        //
        //     migrationBuilder.CreateIndex(
        //         name: "FK_Supplier_Currency",
        //         table: "suppliers",
        //         column: "currency");
        //
        //     migrationBuilder.CreateIndex(
        //         name: "FK_typeParAssociation_parameterID",
        //         table: "typeparassociation",
        //         column: "parameterID");
        //
        //     migrationBuilder.CreateIndex(
        //         name: "fk_watchesGrouped_group4watches",
        //         table: "watchesgrouped",
        //         column: "group4watchesID");
        //
        //     migrationBuilder.CreateIndex(
        //         name: "fk_watchesGrouped_watch",
        //         table: "watchesgrouped",
        //         column: "watchID");
        //
        //     migrationBuilder.CreateIndex(
        //         name: "FK_Xrate_Currency",
        //         table: "xrate",
        //         column: "code");
        //
        //     migrationBuilder.CreateIndex(
        //         name: "FK_Xrate_Currency2",
        //         table: "xrate",
        //         column: "SourceCurrencyCode");
        // }

        /// <inheritdoc />
        // protected override void Down(MigrationBuilder migrationBuilder)
        // {
        //     migrationBuilder.DropTable(
        //         name: "amazonmarketplace");
        //
        //     migrationBuilder.DropTable(
        //         name: "asinsku");
        //
        //     migrationBuilder.DropTable(
        //         name: "bodiesgrouped");
        //
        //     migrationBuilder.DropTable(
        //         name: "bodyinthebox");
        //
        //     migrationBuilder.DropTable(
        //         name: "casioinvoice");
        //
        //     migrationBuilder.DropTable(
        //         name: "casioukbackorder");
        //
        //     migrationBuilder.DropTable(
        //         name: "casioukcurrentorder");
        //
        //     migrationBuilder.DropTable(
        //         name: "colourtranslation");
        //
        //     migrationBuilder.DropTable(
        //         name: "country2rmass");
        //
        //     migrationBuilder.DropTable(
        //         name: "countryvatrrate");
        //
        //     migrationBuilder.DropTable(
        //         name: "deliveryprice");
        //
        //     migrationBuilder.DropTable(
        //         name: "invoicetxns");
        //
        //     migrationBuilder.DropTable(
        //         name: "itmitmassociation");
        //
        //     migrationBuilder.DropTable(
        //         name: "itmparameters");
        //
        //     migrationBuilder.DropTable(
        //         name: "keyvalue");
        //
        //     migrationBuilder.DropTable(
        //         name: "locmarassociation");
        //
        //     migrationBuilder.DropTable(
        //         name: "logentry");
        //
        //     migrationBuilder.DropTable(
        //         name: "logevent");
        //
        //     migrationBuilder.DropTable(
        //         name: "marketplatformassociation");
        //
        //     migrationBuilder.DropTable(
        //         name: "mayalsofit");
        //
        //     migrationBuilder.DropTable(
        //         name: "orderitem");
        //
        //     migrationBuilder.DropTable(
        //         name: "part2itemass");
        //
        //     migrationBuilder.DropTable(
        //         name: "photo");
        //
        //     migrationBuilder.DropTable(
        //         name: "rmzones");
        //
        //     migrationBuilder.DropTable(
        //         name: "searchentry");
        //
        //     migrationBuilder.DropTable(
        //         name: "shopitem");
        //
        //     migrationBuilder.DropTable(
        //         name: "stockshot");
        //
        //     migrationBuilder.DropTable(
        //         name: "tokens");
        //
        //     migrationBuilder.DropTable(
        //         name: "typeparassociation");
        //
        //     migrationBuilder.DropTable(
        //         name: "watchesgrouped");
        //
        //     migrationBuilder.DropTable(
        //         name: "xrate");
        //
        //     migrationBuilder.DropTable(
        //         name: "zibiinvoice");
        //
        //     migrationBuilder.DropTable(
        //         name: "multidrawer");
        //
        //     migrationBuilder.DropTable(
        //         name: "parametervalues");
        //
        //     migrationBuilder.DropTable(
        //         name: "itemheader");
        //
        //     migrationBuilder.DropTable(
        //         name: "platforms");
        //
        //     migrationBuilder.DropTable(
        //         name: "group4bodies");
        //
        //     migrationBuilder.DropTable(
        //         name: "order");
        //
        //     migrationBuilder.DropTable(
        //         name: "orderitemtype");
        //
        //     migrationBuilder.DropTable(
        //         name: "itmmarketassoc");
        //
        //     migrationBuilder.DropTable(
        //         name: "group4watches");
        //
        //     migrationBuilder.DropTable(
        //         name: "watch");
        //
        //     migrationBuilder.DropTable(
        //         name: "parameters");
        //
        //     migrationBuilder.DropTable(
        //         name: "suppliers");
        //
        //     migrationBuilder.DropTable(
        //         name: "vatrates");
        //
        //     migrationBuilder.DropTable(
        //         name: "customer");
        //
        //     migrationBuilder.DropTable(
        //         name: "postagetypes");
        //
        //     migrationBuilder.DropTable(
        //         name: "orderstatus");
        //
        //     migrationBuilder.DropTable(
        //         name: "itembody");
        //
        //     migrationBuilder.DropTable(
        //         name: "location");
        //
        //     migrationBuilder.DropTable(
        //         name: "market");
        //
        //     migrationBuilder.DropTable(
        //         name: "billaddr");
        //
        //     migrationBuilder.DropTable(
        //         name: "brands");
        //
        //     migrationBuilder.DropTable(
        //         name: "types");
        //
        //     migrationBuilder.DropTable(
        //         name: "currency");
        //
        //     migrationBuilder.DropTable(
        //         name: "countrycode");
        // }
    }
}
