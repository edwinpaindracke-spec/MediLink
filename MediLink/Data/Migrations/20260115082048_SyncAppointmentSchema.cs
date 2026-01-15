using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MediLink.Data.Migrations
{
    /// <inheritdoc />
    public partial class SyncAppointmentSchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "AppointmentDate",
                table: "Appointments",
                newName: "AppointmentDateTime");

            migrationBuilder.AddColumn<int>(
                name: "HospitalId",
                table: "Appointments",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Appointments_HospitalId",
                table: "Appointments",
                column: "HospitalId");

            migrationBuilder.AddForeignKey(
                name: "FK_Appointments_Hospitals_HospitalId",
                table: "Appointments",
                column: "HospitalId",
                principalTable: "Hospitals",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Appointments_Hospitals_HospitalId",
                table: "Appointments");

            migrationBuilder.DropIndex(
                name: "IX_Appointments_HospitalId",
                table: "Appointments");

            migrationBuilder.DropColumn(
                name: "HospitalId",
                table: "Appointments");

            migrationBuilder.RenameColumn(
                name: "AppointmentDateTime",
                table: "Appointments",
                newName: "AppointmentDate");
        }
    }
}
