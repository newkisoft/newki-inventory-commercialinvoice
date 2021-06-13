using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace Web.Migrations
{
    public partial class ExtraValueToDouble : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
            name: "Value",
            table: "CommercialInvoiceExtra");


            migrationBuilder.AddColumn<double>(
                name: "Value",
                table: "CommercialInvoiceExtra",
                nullable: true);
        }


        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
           name: "Value",
           table: "CommercialInvoiceExtra");

            migrationBuilder.AddColumn<string>(
                name: "Value",
                table: "CommercialInvoiceExtra",
                nullable: true);
        }
    }
}
