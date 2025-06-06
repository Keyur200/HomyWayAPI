﻿using System;
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
    public class BookingsController : ControllerBase
    {
        private readonly HomyWayContext _context;

        public BookingsController(HomyWayContext context)
        {
            _context = context;
        }

        // GET: api/Bookings
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Booking>>> GetBookings()
        {
            return await _context.Bookings.Include(p => p.Property).Include(b => b.Property.ImagesNavigation).ToListAsync();
        }

        [HttpGet("total/{hostId}")]
        public async Task<ActionResult<long>> getAllEarnings(int hostId)
        {
            var earning = await _context.Bookings.Where(b=>b.Property.HostId == hostId).SumAsync(b=>(long?)b.Amount)??0;
            return Ok(earning);
        }

        [HttpGet("adminEarnings")]
        public async Task<ActionResult<long>> getAllEarningsOfAdmin()
        {
            var earning = await _context.Bookings.SumAsync(b => (long?)b.HomywayCharges) ?? 0;
            return Ok(earning);
        }

        [HttpGet("host/{hostId}")]
        public async Task<ActionResult<long>> getAllBookingByHostId(int hostId)
        {
            var booking = await _context.Bookings.Include(b => b.Property).Where(b => b.Property.HostId == hostId).ToListAsync();
            return Ok(booking);
        }

        [HttpGet("user/{userId}")]
        public async Task<ActionResult<Booking>> getAllBookingByUser(int userId)
        {
            var bookings = await _context.Bookings.Include(p=>p.Property).Include(b=>b.Property.ImagesNavigation).Where(b=>b.UserId == userId).OrderByDescending(b => b.Checkkin).ToListAsync();
            return Ok(bookings);    
        }
        // GET: api/Bookings/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Booking>> GetBooking(int id)
        {
            var booking = await _context.Bookings.FindAsync(id);

            if (booking == null)
            {
                return NotFound();
            }

            return booking;
        }

        //Get: api/Bookings/property/{propertyId}
        [HttpGet("property/{propertyId}")]
        public async Task<ActionResult<IEnumerable<Booking>>> GetBookingsByPropertyId(int propertyId)
        {
            var bookings = await _context.Bookings.Where(b => b.PropertyId == propertyId).ToListAsync();
            if (bookings == null || !bookings.Any())
            {
                return NotFound();
            }
            return bookings;
        }

        // PUT: api/Bookings/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutBooking(int id, Booking booking)
        {
            if (id != booking.Id)
            {
                return BadRequest();
            }

            _context.Entry(booking).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!BookingExists(id))
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

        // POST: api/Bookings
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Booking>> PostBooking(BookingDto booking)
        {
            var b = new Booking
            {
                UserId = booking.UserId,
                PropertyId = booking.PropertyId,
                Checkkin = booking.Checkkin,
                Checkout = booking.Checkout,
                Guests = booking.Guests,
                Nights = booking.Nights,
                Name = booking.Name,
                Phone = booking.Phone,
                Amount = booking.Amount,
                CreatedAt = DateTime.Now,
                HomywayCharges = booking.HomywayCharges,
            };
            _context.Bookings.Add(b);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetBooking", new { id = booking.Id }, booking);
        }

        // DELETE: api/Bookings/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBooking(int id)
        {
            var booking = await _context.Bookings.FindAsync(id);
            if (booking == null)
            {
                return NotFound();
            }

            _context.Bookings.Remove(booking);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool BookingExists(int id)
        {
            return _context.Bookings.Any(e => e.Id == id);
        }
    }
}
