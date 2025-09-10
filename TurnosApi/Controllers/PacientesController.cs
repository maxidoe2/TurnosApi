using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Data.Sqlite;
using TurnosApi.Data;
using TurnosApi.Models;

namespace TurnosApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PacientesController(AppDbContext db) : ControllerBase
{
    // GET: api/pacientes
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Paciente>>> GetAll()
        => await db.Pacientes.AsNoTracking().ToListAsync();

    // GET: api/pacientes/5
    [HttpGet("{id:int}")]
    public async Task<ActionResult<Paciente>> GetById(int id)
    {
        var p = await db.Pacientes.FindAsync(id);
        return p is null ? NotFound() : Ok(p);
    }

    // POST: api/pacientes
    [Authorize]
    [HttpPost]
    public async Task<ActionResult<Paciente>> Create(Paciente paciente)
    {
        // Pre-chequeo de DNI único (rápido de entender)
        var existsDni = await db.Pacientes.AnyAsync(p => p.DNI == paciente.DNI);
        if (existsDni) return Conflict(new { message = "El DNI ya existe." });

        db.Pacientes.Add(paciente);
        try
        {
            await db.SaveChangesAsync();
            return CreatedAtAction(nameof(GetById), new { id = paciente.Id }, paciente);
        }
        catch (DbUpdateException ex) when (ex.InnerException is SqliteException sql && sql.SqliteErrorCode == 19)
        {
            // Respaldo por si se cuela una carrera de datos.
            return Conflict(new { message = "El DNI ya existe." });
        }
    }

    // PUT: api/pacientes/5
    [Authorize]
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, Paciente paciente)
    {
        if (id != paciente.Id) return BadRequest("El id del body no coincide con la URL.");

        var exists = await db.Pacientes.AnyAsync(x => x.Id == id);
        if (!exists) return NotFound();

        // DNI único (excluyendo el propio registro)
        var dniUsed = await db.Pacientes.AnyAsync(p => p.DNI == paciente.DNI && p.Id != id);
        if (dniUsed) return Conflict(new { message = "El DNI ya existe para otro paciente." });

        db.Entry(paciente).State = EntityState.Modified;

        try
        {
            await db.SaveChangesAsync();
            return NoContent();
        }
        catch (DbUpdateException ex) when (ex.InnerException is SqliteException sql && sql.SqliteErrorCode == 19)
        {
            return Conflict(new { message = "El DNI ya existe para otro paciente." });
        }
    }

    // DELETE: api/pacientes/5
    [Authorize]
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var p = await db.Pacientes.FindAsync(id);
        if (p is null) return NotFound();

        db.Pacientes.Remove(p);
        await db.SaveChangesAsync();
        return NoContent();
    }
}
