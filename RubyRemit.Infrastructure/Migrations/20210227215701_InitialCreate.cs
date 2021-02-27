using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace RubyRemit.Infrastructure.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Payments",
                columns: table => new
                {
                    PaymentId = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreditCardNumber = table.Column<string>(maxLength: 20, nullable: false),
                    CardHolder = table.Column<string>(maxLength: 25, nullable: false),
                    ExpirationDate = table.Column<DateTime>(nullable: false),
                    SecurityCode = table.Column<string>(maxLength: 3, nullable: true),
                    Amount = table.Column<decimal>(type: "decimal(18, 2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Payments", x => x.PaymentId);
                });

            migrationBuilder.CreateTable(
                name: "PaymentStates",
                columns: table => new
                {
                    PaymentStateId = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PaymentId = table.Column<long>(nullable: false),
                    State = table.Column<byte>(type: "tinyint", nullable: false),
                    DateAttempted = table.Column<DateTime>(nullable: false, defaultValueSql: "(getdate())"),
                    Gateway = table.Column<string>(maxLength: 50, nullable: false),
                    Remark = table.Column<string>(maxLength: 200, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PaymentStates", x => x.PaymentStateId);
                    table.ForeignKey(
                        name: "FK_PaymentStates_Payments_PaymentId",
                        column: x => x.PaymentId,
                        principalTable: "Payments",
                        principalColumn: "PaymentId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PaymentStates_PaymentId",
                table: "PaymentStates",
                column: "PaymentId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PaymentStates");

            migrationBuilder.DropTable(
                name: "Payments");
        }
    }
}
