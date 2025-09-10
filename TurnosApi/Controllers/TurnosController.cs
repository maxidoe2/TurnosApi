using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Data.Sqlite;
using TurnosApi.Data;
using TurnosApi.Models;

namespace TurnosApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TurnosController(AppDbContext db) : ControllerBase
{
    private static readonly HashSet<string> EstadosPermitidos = new(StringComparer.OrdinalIgnoreCase)
        { "pendiente", "confirmado", "cancelado" };

    // GET: api/turnos
    // Devuelve paciente y médico incluidos
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Turno>>> GetAll()
        => await db.Turnos
                   .AsNoTracking()
                   .Include(t => t.Paciente)
                   .Include(t => t.Medico)
                   .OrderBy(t => t.FechaHora)
                   .ToListAsync();

    // GET: api/turnos/5
    [HttpGet("{id:int}")]
    public async Task<ActionResult<Turno>> GetById(int id)
    {
        var t = await db.Turnos
                        .AsNoTracking()
                        .Include(x => x.Paciente)
                        .Include(x => x.Medico)
                        .FirstOrDefaultAsync(x => x.Id == id);
        return t is null ? NotFound() : Ok(t);
    }

    // POST: api/turnos
    [Authorize]
    [HttpPost]
    public async Task<ActionResult<Turno>> Create(Turno turno)
    {
        var error = await ValidarTurnoAsync(turno);
        if (error is not null) return BadRequest(new { message = error });

        db.Turnos.Add(turno);
        try
        {
            await db.SaveChangesAsync();
            // Devuelve el turno con includes
            var saved = await db.Turnos
                                .Include(x => x.Paciente)
                                .Include(x => x.Medico)
                                .FirstAsync(x => x.Id == turno.Id);
            return CreatedAtAction(nameof(GetById), new { id = saved.Id }, saved);
        }
        catch (DbUpdateException ex) when (ex.InnerException is SqliteException sql && sql.SqliteErrorCode == 19)
        {
            // Por si hay violaciones de integridad (FK, etc.)
            return BadRequest(new { message = "Violación de integridad. Verifique PacienteId/MedicoId/Estado." });
        }
    }

    // PUT: api/turnos/5
    [Authorize]
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, Turno turno)
    {
        if (id != turno.Id) return BadRequest(new { message = "El id del body no coincide con la URL." });

        var exists = await db.Turnos.AnyAsync(x => x.Id == id);
        if (!exists) return NotFound();

        var error = await ValidarTurnoAsync(turno);
        if (error is not null) return BadRequest(new { message = error });

        db.Entry(turno).State = EntityState.Modified;

        try
        {
            await db.SaveChangesAsync();
            return NoContent();
        }
        catch (DbUpdateException ex) when (ex.InnerException is SqliteException sql && sql.SqliteErrorCode == 19)
        {
            return BadRequest(new { message = "Violación de integridad. Verifique PacienteId/MedicoId/Estado." });
        }
    }

    // DELETE: api/turnos/5
    [Authorize]
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var t = await db.Turnos.FindAsync(id);
        if (t is null) return NotFound();

        db.Turnos.Remove(t);
        await db.SaveChangesAsync();
        return NoContent();
    }

    // --------------------------
    // Validaciones de negocio
    // --------------------------
    private async Task<string?> ValidarTurnoAsync(Turno t)
    {
        if (!EstadosPermitidos.Contains(t.Estado))
            return "Estado inválido. Valores permitidos: pendiente | confirmado | cancelado.";

        // fecha futura (permitimos >= ahora + 1 minuto por tolerancia)
        if (t.FechaHora <= DateTime.UtcNow.AddMinutes(-1))
            return "La fecha/hora del turno debe ser futura.";

        var pacienteOk = await db.Pacientes.AnyAsync(p => p.Id == t.PacienteId);
        if (!pacienteOk) return "PacienteId no existe.";

        var medicoOk = await db.Medicos.AnyAsync(m => m.Id == t.MedicoId);
        if (!medicoOk) return "MedicoId no existe.";

        return null;
    }
}
