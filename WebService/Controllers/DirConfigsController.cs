using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Models.Models;
using Models.Models.Context;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

[Route("api/[controller]")]
[ApiController]
public class DirConfigsController : ControllerBase
{
    private readonly DIRContext _context;

    public DirConfigsController(DIRContext context)
    {
        _context = context;
    }

    // GET: api/DirConfigs
    [HttpGet]
    public async Task<ActionResult<IEnumerable<DirConfig>>> GetDirConfigs()
    {
        return await _context.DirConfigs.Include(d => d.Protected).ToListAsync();
    }

    // GET: api/DirConfigs/5
    [HttpGet("{id}")]
    public async Task<ActionResult<DirConfig>> GetDirConfig(int id)
    {
        var dirConfig = await _context.DirConfigs.Include(d => d.Protected)
            .FirstOrDefaultAsync(d => d.Id == id);

        if (dirConfig == null)
        {
            return NotFound();
        }

        return dirConfig;
    }

    // POST: api/DirConfigs
    [HttpPost]
    public async Task<ActionResult<DirConfig>> PostDirConfig(DirConfig dirConfig)
    {
        _context.DirConfigs.Add(dirConfig);
        await _context.SaveChangesAsync();

        return CreatedAtAction("GetDirConfig", new { id = dirConfig.Id }, dirConfig);
    }

    // 其他必要的 API 方法（PUT, DELETE 等）

    // PUT: api/DirConfigs/5
    [HttpPut("{id}")]
    public async Task<IActionResult> PutDirConfig(int id, DirConfig dirConfig)
    {
        if (id != dirConfig.Id)
        {
            return BadRequest();
        }

        _context.Entry(dirConfig).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!DirConfigExists(id))
            {
                return NotFound();
            }
            else
            {
                throw;
            }
        }

        return NoContent();
    }

    private bool DirConfigExists(int id)
    {
        return _context.DirConfigs.Any(e => e.Id == id);
    }

    // DELETE: api/DirConfigs/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteDirConfig(int id)
    {
        var dirConfig = await _context.DirConfigs.FindAsync(id);
        if (dirConfig == null)
        {
            return NotFound();
        }

        _context.DirConfigs.Remove(dirConfig);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}
