using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Data.Sqlite;
using TurnosApi.Data;
using TurnosApi.Models;

namespace TurnosApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MedicosController(AppDbContext db) : ControllerBase
{
    // GET: api/medicos
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Medico>>> GetAll()
        => await db.Medicos.AsNoTracking().ToListAsync();

    // GET: api/medicos/5
    [HttpGet("{id:int}")]
    public async Task<ActionResult<Medico>> GetById(int id)
    {
        var m = await db.Medicos.FindAsync(id);
        return m is null ? NotFound() : Ok(m);
    }

    // POST: api/medicos
    [Authorize]
    [HttpPost]
    public async Task<ActionResult<Medico>> Create(Medico medico)
    {
        // Pre-chequeo de matrícula única
        var existsMat = await db.Medicos.AnyAsync(m => m.Matricula == medico.Matricula);
        if (existsMat) return Conflict(new { message = "La matrícula ya existe." });

        db.Medicos.Add(medico);
        try
        {
            await db.SaveChangesAsync();
            return CreatedAtAction(nameof(GetById), new { id = medico.Id }, medico);
        }
        catch (DbUpdateException ex) when (ex.InnerException is SqliteException sql && sql.SqliteErrorCode == 19)
        {
            return Conflict(new { message = "La matrícula ya existe." });
        }
    }

    // PUT: api/medicos/5
    [Authorize]
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, Medico medico)
    {
        if (id != medico.Id) return BadRequest("El id del body no coincide con la URL.");

        var exists = await db.Medicos.AnyAsync(x => x.Id == id);
        if (!exists) return NotFound();

        var matUsed = await db.Medicos.AnyAsync(m => m.Matricula == medico.Matricula && m.Id != id);
        if (matUsed) return Conflict(new { message = "La matrícula ya existe para otro médico." });

        db.Entry(medico).State = EntityState.Modified;

        try
        {
            await db.SaveChangesAsync();
            return NoContent();
        }
        catch (DbUpdateException ex) when (ex.InnerException is SqliteException sql && sql.SqliteErrorCode == 19)
        {
            return Conflict(new { message = "La matrícula ya existe para otro médico." });
        }
    }

    // DELETE: api/medicos/5
    [Authorize]
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var m = await db.Medicos.FindAsync(id);
        if (m is null) return NotFound();

        db.Medicos.Remove(m);
        await db.SaveChangesAsync();
        return NoContent();
    }
}
