using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace Web.Migrations
{
    public partial class AddCommercialInvoice : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
         

            migrationBuilder.CreateTable(
                name: "CommercialInvoice",
                columns: table => new
                {
                    CommercialInvoiceId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CommercialInvoiceDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    CommercialInvoiceDueDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    InvoiceId = table.Column<int>(type: "integer", nullable: false),
                    Seller = table.Column<string>(type: "text", nullable: true),
                    Consignee = table.Column<string>(type: "text", nullable: true),
                    Buyer = table.Column<string>(type: "text", nullable: true),
                    LoadingDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    CountryOfBeneficiary = table.Column<string>(type: "text", nullable: true),
                    FreightForwarder = table.Column<string>(type: "text", nullable: true),
                    CountryOfOrigin = table.Column<string>(type: "text", nullable: true),
                    CountryOfDestination = table.Column<string>(type: "text", nullable: true),
                    PartialShipment = table.Column<string>(type: "text", nullable: true),
                    TermsOfDelivery = table.Column<string>(type: "text", nullable: true),
                    RelevantLocation = table.Column<string>(type: "text", nullable: true),
                    TransportBy = table.Column<string>(type: "text", nullable: true),
                    Port = table.Column<string>(type: "text", nullable: true),
                    TermsOfPayment = table.Column<string>(type: "text", nullable: true),
                    HsCode = table.Column<string>(type: "text", nullable: true),
                    PackageDescription = table.Column<string>(type: "text", nullable: true),
                    TotalGross = table.Column<string>(type: "text", nullable: true),
                    Size = table.Column<string>(type: "text", nullable: true),
                    TotalUsd = table.Column<double>(type: "double precision", nullable: false),
                    Tax = table.Column<double>(type: "double precision", nullable: false),
                    Kdv = table.Column<double>(type: "double precision", nullable: false),
                    ExchangeRate = table.Column<double>(type: "double precision", nullable: false),
                    Discount = table.Column<double>(type: "double precision", nullable: false),
                    Paid = table.Column<double>(type: "double precision", nullable: false),
                    Currency = table.Column<string>(type: "text", nullable: true),
                    Comment = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CommercialInvoice", x => x.CommercialInvoiceId);
                });

            migrationBuilder.CreateTable(
                name: "CommercialInvoiceDataView",
                columns: table => new
                {
                    CommercialInvoiceId = table.Column<int>(type: "integer", nullable: false),
                    Data = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CommercialInvoiceDataView", x => x.CommercialInvoiceId);
                });

         

            migrationBuilder.CreateTable(
                name: "CommercialInvoiceDocumentFile",
                columns: table => new
                {
                    DocumentFileId = table.Column<int>(type: "integer", nullable: false),
                    CommercialInvoiceId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CommercialInvoiceDocumentFile", x => new { x.CommercialInvoiceId, x.DocumentFileId });
                    table.ForeignKey(
                        name: "FK_CommercialInvoiceDocumentFile_CommercialInvoice_CommercialI~",
                        column: x => x.CommercialInvoiceId,
                        principalTable: "CommercialInvoice",
                        principalColumn: "CommercialInvoiceId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CommercialInvoiceDocumentFile_DocumentFile_DocumentFileId",
                        column: x => x.DocumentFileId,
                        principalTable: "DocumentFile",
                        principalColumn: "DocumentFileId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CommercialInvoiceExtra",
                columns: table => new
                {
                    CommercialInvoiceExtraId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Description = table.Column<string>(type: "text", nullable: false),
                    Value = table.Column<string>(type: "text", nullable: false),
                    CommercialInvoiceId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CommercialInvoiceExtra", x => x.CommercialInvoiceExtraId);
                    table.ForeignKey(
                        name: "FK_CommercialInvoiceExtra_CommercialInvoice_CommercialInvoiceId",
                        column: x => x.CommercialInvoiceId,
                        principalTable: "CommercialInvoice",
                        principalColumn: "CommercialInvoiceId",
                        onDelete: ReferentialAction.Restrict);
                });

  

            migrationBuilder.CreateIndex(
                name: "IX_CommercialInvoiceExtra_CommercialInvoiceId",
                table: "CommercialInvoiceExtra",
                column: "CommercialInvoiceId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CommercialInvoiceDataView");

            migrationBuilder.DropTable(
                name: "CommercialInvoiceDocumentFile");

            migrationBuilder.DropTable(
                name: "CommercialInvoiceExtra");

        
            migrationBuilder.DropTable(
                name: "CommercialInvoice");
        }


    }
}
