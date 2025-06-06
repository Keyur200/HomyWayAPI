using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HomyWayAPI.Models;
using HomyWayAPI.DTO;

namespace HomyWayAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WishlistsController : ControllerBase
    {
        private readonly HomyWayContext _context;

        public WishlistsController(HomyWayContext context)
        {
            _context = context;
        }

        // GET: api/Wishlists
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Wishlist>>> GetWishlists()
        {
            return await _context.Wishlists.ToListAsync();
        }

        // GET: api/Wishlists/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Wishlist>> GetWishlist(int id)
        {
            var wishlist = await _context.Wishlists.FindAsync(id);

            if (wishlist == null)
            {
                return NotFound();
            }

            return wishlist;
        }

        // PUT: api/Wishlists/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutWishlist(int id, Wishlist wishlist)
        {
            if (id != wishlist.Id)
            {
                return BadRequest();
            }

            _context.Entry(wishlist).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!WishlistExists(id))
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

        // POST: api/Wishlists
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Wishlist>> PostWishlist(WishlistDTO wishlistDTO)
        {
            var existingWishlist = await _context.Wishlists.FirstOrDefaultAsync(w => w.UserId == wishlistDTO.UserId && w.PropertyId == wishlistDTO.PropertyId);

            if (existingWishlist != null)
            {
                return Ok(new { message = "This property is already in the your wishlist." });
            }
            var wishlist = new Wishlist
            {
                UserId = wishlistDTO.UserId,
                PropertyId = wishlistDTO.PropertyId,
            };
            _context.Wishlists.Add(wishlist);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetWishlist", new { id = wishlist.Id }, wishlist);
        }

        [HttpGet("User/{userId}")]
        public async Task<ActionResult<IEnumerable<Wishlist>>> GetWishlistByUser(int userId)
        {
            var userWishlist = await _context.Wishlists
                .Include(w => w.Property)
                .Include(w => w.Property.ImagesNavigation)
                .Where(w => w.UserId == userId)
                .ToListAsync();

            return Ok(userWishlist);
        }


        [HttpDelete("{userId}/{propertyId}")]
        public async Task<IActionResult> DeleteWishlist(int userId, int propertyId)
        {
            var wishlistItem = await _context.Wishlists
                .FirstOrDefaultAsync(w => w.UserId == userId && w.PropertyId == propertyId);

            if (wishlistItem == null)
            {
                return NotFound();
            }

            _context.Wishlists.Remove(wishlistItem);
            await _context.SaveChangesAsync();

            return NoContent();
        }


        private bool WishlistExists(int id)
        {
            return _context.Wishlists.Any(e => e.Id == id);
        }
    }
}
