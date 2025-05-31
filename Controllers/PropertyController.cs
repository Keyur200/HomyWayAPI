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
using Humanizer;

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

        [HttpGet("slugName/{slug}")]
        public async Task<ActionResult<PropertyTbl>> GetPropertyBySlug(string slug)
        {
            var propertyTbl = await _context.PropertyTbls.Include(p => p.ImagesNavigation).Include(c => c.Category).Include(h => h.Host).FirstOrDefaultAsync(p => p.SlugName.ToString() == slug.ToString());

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

        [HttpGet("category/{id}")]
        public async Task<ActionResult<PropertyTbl>> GetPropertyByCategory(int id)
        {
            var property = await _context.PropertyTbls.Include(p => p.ImagesNavigation).Include(c => c.Category).Include(h => h.Host).Where(p => p.CategoryId == id).ToListAsync();

            return Ok(property);
        }


        //All filter api 

        [HttpGet("filter")]
        public async Task<ActionResult<List<PropertyTbl>>> GetFilteredProperties(
            [FromQuery] int? category,
            [FromQuery] decimal? minPrice,
    [FromQuery] decimal? maxPrice,
    [FromQuery] string? city)
        {
            var query = _context.PropertyTbls
                .Include(p => p.ImagesNavigation)
                .Include(p => p.Category)
                .Include(p => p.Host)
                .Where(p => p.CategoryId == category)
                .AsQueryable();

            // If neither filter is provided, return all properties
            if (!minPrice.HasValue && !maxPrice.HasValue && string.IsNullOrEmpty(city))
            {
                var allProps = await query.ToListAsync();
                return Ok(allProps);
            }
            
            // Build an OR filter
            query = query.Where(p =>
           
                (minPrice.HasValue && maxPrice.HasValue && p.PropertyPrice >= minPrice.Value &&  p.PropertyPrice <= maxPrice.Value) ||
                (!string.IsNullOrEmpty(city) && p.PropertyCity == city) 
            );

            var result = await query.ToListAsync();

            return Ok(result);
        }



        // PUT: api/Property/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<ActionResult<PropertyTbl>> PutPropertyTbl(int id, PropertyDTO propertyDTO)
        {
            var existingProperty = await _context.PropertyTbls.FindAsync(id);
            if (existingProperty == null)
            {
                return NotFound($"No property found with ID {id}");
            }

            // Update property fields
            existingProperty.HostId = propertyDTO.HostId;
            existingProperty.PropertyName = propertyDTO.PropertyName;
            existingProperty.SlugName = GenerateSlug(propertyDTO.PropertyName);
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
            existingProperty.Latitude = propertyDTO.Latitude;
            existingProperty.Longitude = propertyDTO.Longitude;
            existingProperty.Amenities = JsonSerializer.Serialize(propertyDTO.Amenities);

            // Optional: Remove existing images (if updating images completely)
            var existingImages = JsonSerializer.Deserialize<List<int>>(existingProperty.Images ?? "[]");

            // Handle new image uploads
            foreach (var file in propertyDTO.files)
            {
                using var stream = file.OpenReadStream();
                var uploadParams = new ImageUploadParams
                {
                    File = new FileDescription(file.FileName, stream),
                    Folder = "HomyWayImages"
                };

                var uploadResult = await _cloudinary.UploadAsync(uploadParams);
                if (uploadResult.Error != null)
                {
                    return BadRequest(uploadResult.Error.Message);
                }

                var newImage = new Image
                {
                    PropertId = id,
                    ImageUrl = uploadResult.SecureUrl.ToString()
                };

                _context.Images.Add(newImage);
                await _context.SaveChangesAsync();

                existingImages.Add(newImage.Id);
            }

            // Update property image list
            existingProperty.Images = JsonSerializer.Serialize(existingImages);
            _context.PropertyTbls.Update(existingProperty);
            await _context.SaveChangesAsync();

            return Ok(existingProperty);
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
                SlugName = GenerateSlug(propertyDTO.PropertyName),
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
                Latitude = propertyDTO.Latitude,
                Longitude = propertyDTO.Longitude,
                Amenities = JsonSerializer.Serialize(propertyDTO.Amenities),
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

        [HttpGet("TestDelete")]
        public async Task<IActionResult> TestDelete()
        {
            var publicId = "HomyWayImages/xjymw7fcw7xxhuzg7uzl";

            var deletionParams = new DeletionParams(publicId);
            var result = await _cloudinary.DestroyAsync(deletionParams);

            return Ok(result);
        }


        [HttpDelete("DeleteImage/{id}")]
        public async Task<IActionResult> DeleteImage(int id)
        {
            var image = await _context.Images.FindAsync(id);
            if (image == null)
                return NotFound("Image not found.");

            var property = await _context.PropertyTbls.FindAsync(image.PropertId);
            if (property == null)
                return NotFound("Related property not found.");

            var imageIds = JsonSerializer.Deserialize<List<int>>(property.Images ?? "[]");

            imageIds.Remove(id);

            property.Images = JsonSerializer.Serialize(imageIds);
            _context.PropertyTbls.Update(property);

            _context.Images.Remove(image);

            await _context.SaveChangesAsync();

            return Ok("Image deleted from database and property updated.");
        }




        private string GenerateSlug(string phrase)
        {
            string str = phrase.ToLowerInvariant();
            // Remove all invalid characters
            str = System.Text.RegularExpressions.Regex.Replace(str, @"[^a-z0-9\s-]", "");
            // Convert multiple spaces into one space
            str = System.Text.RegularExpressions.Regex.Replace(str, @"\s+", " ").Trim();
            // Replace spaces with hyphens
            str = str.Replace(" ", "-");
            return str;
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
            _context.Bookings.RemoveRange(propertyTbl.Bookings);
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
