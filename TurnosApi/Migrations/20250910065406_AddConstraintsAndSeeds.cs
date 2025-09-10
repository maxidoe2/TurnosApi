using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TurnosApi.Migrations
{
    /// <inheritdoc />
    public partial class AddConstraintsAndSeeds : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Medicos",
                columns: new[] { "Id", "Especialidad", "Matricula", "Nombre" },
                values: new object[] { 1, "Clínica", "M-123", "Dra. Gómez" });

            migrationBuilder.InsertData(
                table: "Pacientes",
                columns: new[] { "Id", "DNI", "Email", "Nombre" },
                values: new object[] { 1, "30111222", "juan@demo.com", "Juan Perez" });

            migrationBuilder.InsertData(
                table: "Turnos",
                columns: new[] { "Id", "Estado", "FechaHora", "MedicoId", "PacienteId" },
                values: new object[] { 1, "pendiente", new DateTime(2025, 9, 11, 6, 54, 5, 638, DateTimeKind.Utc).AddTicks(9791), 1, 1 });

            migrationBuilder.CreateIndex(
                name: "IX_Turnos_MedicoId",
                table: "Turnos",
                column: "MedicoId");

            migrationBuilder.CreateIndex(
                name: "IX_Turnos_PacienteId",
                table: "Turnos",
                column: "PacienteId");

            migrationBuilder.CreateIndex(
                name: "IX_Pacientes_DNI",
                table: "Pacientes",
                column: "DNI",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Medicos_Matricula",
                table: "Medicos",
                column: "Matricula",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Turnos_Medicos_MedicoId",
                table: "Turnos",
                column: "MedicoId",
                principalTable: "Medicos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Turnos_Pacientes_PacienteId",
                table: "Turnos",
                column: "PacienteId",
                principalTable: "Pacientes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Turnos_Medicos_MedicoId",
                table: "Turnos");

            migrationBuilder.DropForeignKey(
                name: "FK_Turnos_Pacientes_PacienteId",
                table: "Turnos");

            migrationBuilder.DropIndex(
                name: "IX_Turnos_MedicoId",
                table: "Turnos");

            migrationBuilder.DropIndex(
                name: "IX_Turnos_PacienteId",
                table: "Turnos");

            migrationBuilder.DropIndex(
                name: "IX_Pacientes_DNI",
                table: "Pacientes");

            migrationBuilder.DropIndex(
                name: "IX_Medicos_Matricula",
                table: "Medicos");

            migrationBuilder.DeleteData(
                table: "Turnos",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Medicos",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Pacientes",
                keyColumn: "Id",
                keyValue: 1);
        }
    }
}
