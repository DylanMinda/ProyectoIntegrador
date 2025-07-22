using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BeatHouse.Models;

namespace BeatHouse.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DetallePlaylistsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public DetallePlaylistsController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/DetallePlaylists
        [HttpGet]
        public async Task<ActionResult<IEnumerable<DetallePlaylist>>> GetDetallePlaylist()
        {
            return await _context.DetallePlaylists.ToListAsync();
        }

        // GET: api/DetallePlaylists/5
        [HttpGet("{id}")]
        public async Task<ActionResult<DetallePlaylist>> GetDetallePlaylist(int id)
        {
            var detallePlaylist = await _context.DetallePlaylists.FindAsync(id);

            if (detallePlaylist == null)
            {
                return NotFound();
            }

            return detallePlaylist;
        }

        // PUT: api/DetallePlaylists/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutDetallePlaylist(int id, DetallePlaylist detallePlaylist)
        {
            if (id != detallePlaylist.Id)
            {
                return BadRequest();
            }

            _context.Entry(detallePlaylist).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!DetallePlaylistExists(id))
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

        // POST: api/DetallePlaylists
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<DetallePlaylist>> PostDetallePlaylist(DetallePlaylist detallePlaylist)
        {
            _context.DetallePlaylists.Add(detallePlaylist);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetDetallePlaylist", new { id = detallePlaylist.Id }, detallePlaylist);
        }

        // DELETE: api/DetallePlaylists/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDetallePlaylist(int id)
        {
            var detallePlaylist = await _context.DetallePlaylists.FindAsync(id);
            if (detallePlaylist == null)
            {
                return NotFound();
            }

            _context.DetallePlaylists.Remove(detallePlaylist);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool DetallePlaylistExists(int id)
        {
            return _context.DetallePlaylists.Any(e => e.Id == id);
        }
    }
}
