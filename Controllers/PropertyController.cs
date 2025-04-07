using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HomyWayAPI.Models;
using HomyWayAPI.DTO;
using CloudinaryDotNet.Actions;
using CloudinaryDotNet;
using System.Text.Json;

namespace HomyWayAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PropertyController : ControllerBase
    {
        private readonly HomyWayContext _context;
        private readonly CloudinaryDotNet.Cloudinary _cloudinary;
        public PropertyController(HomyWayContext context, CloudinaryDotNet.Cloudinary cloudinary)
        {
            _context = context;
            _cloudinary = cloudinary;
        }

        // GET: api/Property
        [HttpGet]
        public async Task<IActionResult> GetPropertyTbls()
        {
            var property  = await _context.PropertyTbls.Include(p => p.ImagesNavigation).Include(c => c.Category).Include(h => h.Host).ToListAsync();
            return Ok(property);
        }

        // GET: api/Property/5
        [HttpGet("{id}")]
        public async Task<ActionResult<PropertyTbl>> GetPropertyTbl(int id)
        {
            var propertyTbl = await _context.PropertyTbls.Include(p => p.ImagesNavigation).Include(c => c.Category).Include(h => h.Host).FirstOrDefaultAsync(p => p.PropertyId == id);

            if (propertyTbl == null)
            {
                return NotFound();
            }

            return propertyTbl;
        }

        [HttpGet("host/{id}")]
        public async Task<ActionResult<PropertyTbl>> GetPropertyByHost(int id)
        {
            var property = await _context.PropertyTbls.Include(p => p.ImagesNavigation).Include(c => c.Category).Include(h => h.Host).Where(p => p.HostId == id).ToListAsync();
            return Ok(property);
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
        //
        // +To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<PropertyTbl>> PostPropertyTbl(PropertyDTO propertyDTO)
        {
            //if(propertyDTO.files == null || propertyDTO.files.Count == 0)
            //{
            //    return BadRequest();
            //}
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

            var images = new List<int>();

            foreach(var file in propertyDTO.files)
            {
                using var stream = file.OpenReadStream();
                var upload = new ImageUploadParams()
                {
                    File = new CloudinaryDotNet.FileDescription(file.FileName, stream),
                    Folder = "HomyWayImages"
                };

                var uploadResult = await _cloudinary.UploadAsync(upload);

                if(uploadResult.Error != null)
                {
                    return BadRequest(uploadResult.Error.Message);
                }

                var image = new Image
                {
                    ImageUrl = uploadResult.SecureUrl.ToString(),
                    PropertId = newProperty.PropertyId,
                };

                _context.Images.Add(image);
                await _context.SaveChangesAsync();
                images.Add(image.Id);
            }

            newProperty.Images = JsonSerializer.Serialize(images);
            _context.PropertyTbls.Update(newProperty);
            await _context.SaveChangesAsync();   

            return CreatedAtAction("GetPropertyTbl", new { id = newProperty.PropertyId }, newProperty);
        }

        // DELETE: api/Property/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePropertyTbl(int id)
        {
            var propertyTbl = await _context.PropertyTbls.Include(p => p.ImagesNavigation).FirstOrDefaultAsync(p => p.PropertyId == id);
            if (propertyTbl == null)
            {
                return NotFound();
            }

            _context.Images.RemoveRange(propertyTbl.ImagesNavigation);
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
