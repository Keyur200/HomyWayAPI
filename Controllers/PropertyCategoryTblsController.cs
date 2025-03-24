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
    public class PropertyCategoryTblsController : ControllerBase
    {
        private readonly HomyWayContext _context;

        public PropertyCategoryTblsController(HomyWayContext context)
        {
            _context = context;
        }

        // GET: api/PropertyCategoryTbls
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PropertyCategoryTbl>>> GetPropertyCategoryTbls()
        {
            return await _context.PropertyCategoryTbls.ToListAsync();
        }

        // GET: api/PropertyCategoryTbls/5
        [HttpGet("{id}")]
        public async Task<ActionResult<PropertyCategoryTbl>> GetPropertyCategoryTbl(int id)
        {
            var propertyCategoryTbl = await _context.PropertyCategoryTbls.FindAsync(id);

            if (propertyCategoryTbl == null)
            {
                return NotFound();
            }

            return propertyCategoryTbl;
        }

        // PUT: api/PropertyCategoryTbls/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutPropertyCategoryTbl(int id, CategoryDTO categoryDTO)
        {
           
            var existingCategory = await _context.PropertyCategoryTbls.FindAsync(id);
            if (existingCategory == null)
            {
                return NotFound();
            }

            // Update the existing entity with the DTO data
            existingCategory.CategoryName = categoryDTO.CategoryName;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PropertyCategoryTblExists(id))
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


        // POST: api/PropertyCategoryTbls
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<PropertyCategoryTbl>> PostPropertyCategoryTbl(CategoryDTO categoryDTO)
        {
            var newPropertyCategoryTbl = new PropertyCategoryTbl
            {
                CategoryName = categoryDTO.CategoryName
            };
            _context.PropertyCategoryTbls.Add(newPropertyCategoryTbl);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetPropertyCategoryTbl", new { id = newPropertyCategoryTbl.CategoryId }, newPropertyCategoryTbl);
        }

        // DELETE: api/PropertyCategoryTbls/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePropertyCategoryTbl(int id)
        {
            var propertyCategoryTbl = await _context.PropertyCategoryTbls.FindAsync(id);
            if (propertyCategoryTbl == null)
            {
                return NotFound();
            }

            _context.PropertyCategoryTbls.Remove(propertyCategoryTbl);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool PropertyCategoryTblExists(int id)
        {
            return _context.PropertyCategoryTbls.Any(e => e.CategoryId == id);
        }
    }
}
