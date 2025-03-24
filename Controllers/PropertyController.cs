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
    public class PropertyController : ControllerBase
    {
        private readonly HomyWayContext _context;

        public PropertyController(HomyWayContext context)
        {
            _context = context;
        }

        // GET: api/Property
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PropertyTbl>>> GetPropertyTbls()
        {
            return await _context.PropertyTbls.ToListAsync();
        }

        // GET: api/Property/5
        [HttpGet("{id}")]
        public async Task<ActionResult<PropertyTbl>> GetPropertyTbl(int id)
        {
            var propertyTbl = await _context.PropertyTbls.FindAsync(id);

            if (propertyTbl == null)
            {
                return NotFound();
            }

            return propertyTbl;
        }

        // PUT: api/Property/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutPropertyTbl(int id, PropertyDTO propertyDTO)
        {
            
            var existingProperty = await _context.PropertyTbls.FindAsync(id);
            if (existingProperty == null)
            {
                return NotFound("Property is not Found!");
            }

            existingProperty.PropertyName = propertyDTO.PropertyName;
            existingProperty.PropertyDescription = propertyDTO.PropertyDescription;
            existingProperty.PropertyAdderss = propertyDTO.PropertyAdderss;
            existingProperty.PropertyCity = propertyDTO.PropertyCity;
            existingProperty.PropertyState = propertyDTO.PropertyState;
            existingProperty.PropertyCountry = propertyDTO.PropertyCountry;
            existingProperty.MaxGuests = propertyDTO.MaxGuests;
            existingProperty.BedRoom = propertyDTO.BedRoom;
            existingProperty.Bed = propertyDTO.Bed;
            existingProperty.Bathroom = propertyDTO.Bathroom;
            existingProperty.Status = propertyDTO.Status;
            existingProperty.PropertyPrice = propertyDTO.PropertyPrice;
            existingProperty.CategoryId = propertyDTO.CategoryId;

            try
            {
                
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PropertyTblExists(id))
                {
                    return NotFound("Property not exist!");
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Property
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<PropertyTbl>> PostPropertyTbl(PropertyDTO propertyDTO)
        {
            var newProperty = new PropertyTbl
            {
                HostId = propertyDTO.HostId,
                PropertyName = propertyDTO.PropertyName,
                PropertyDescription = propertyDTO.PropertyDescription,
                PropertyAdderss = propertyDTO.PropertyAdderss,
                PropertyCity = propertyDTO.PropertyCity,
                PropertyState = propertyDTO.PropertyState,
                PropertyCountry = propertyDTO.PropertyCountry,
                MaxGuests = propertyDTO.MaxGuests,
                BedRoom = propertyDTO.BedRoom,
                Bed = propertyDTO.Bed,
                Bathroom = propertyDTO.Bathroom,
                Status = propertyDTO.Status,
                PropertyPrice = propertyDTO.PropertyPrice,
                CategoryId = propertyDTO.CategoryId,
                
            };
            _context.PropertyTbls.Add(newProperty);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetPropertyTbl", new { id = newProperty.PropertyId }, newProperty);
        }

        // DELETE: api/Property/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePropertyTbl(int id)
        {
            var propertyTbl = await _context.PropertyTbls.FindAsync(id);
            if (propertyTbl == null)
            {
                return NotFound();
            }

            _context.PropertyTbls.Remove(propertyTbl);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool PropertyTblExists(int id)
        {
            return _context.PropertyTbls.Any(e => e.PropertyId == id);
        }
    }
}
