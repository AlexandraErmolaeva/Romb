using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Romb.Application.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "planned_events",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    target_code = table.Column<string>(type: "text", nullable: true),
                    name = table.Column<string>(type: "text", nullable: true),
                    total_budget = table.Column<decimal>(type: "numeric(50,15)", nullable: false),
                    planned_cofinance_rate = table.Column<decimal>(type: "numeric(19,15)", nullable: false),
                    planned_local_budget = table.Column<decimal>(type: "numeric(50,15)", nullable: false),
                    planned_regional_budget = table.Column<decimal>(type: "numeric(50,15)", nullable: false),
                    is_actual_calculated = table.Column<bool>(type: "boolean", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_planned_events", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "actual_events",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    planned_event_id = table.Column<long>(type: "bigint", nullable: false),
                    completed_works_budget = table.Column<decimal>(type: "numeric(50,15)", nullable: false),
                    actual_cofinance_rate = table.Column<decimal>(type: "numeric(19,15)", nullable: false),
                    actual_local_budget = table.Column<decimal>(type: "numeric(50,15)", nullable: false),
                    actual_regional_budget = table.Column<decimal>(type: "numeric(50,15)", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_actual_events", x => x.id);
                    table.ForeignKey(
                        name: "FK_actual_events_planned_events_planned_event_id",
                        column: x => x.planned_event_id,
                        principalTable: "planned_events",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_actual_events_planned_event_id",
                table: "actual_events",
                column: "planned_event_id",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "actual_events");

            migrationBuilder.DropTable(
                name: "planned_events");
        }
    }
}
