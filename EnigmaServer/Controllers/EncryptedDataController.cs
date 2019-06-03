using System.Linq;
using System.Threading.Tasks;
using EnigmaLib.Model;
using EnigmaServer.Database;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EnigmaServer.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class EncryptedDataController : ControllerBase
    {
        private readonly DatabaseContext _context;

        public EncryptedDataController(DatabaseContext context)
        {
            _context = context;
        }

        // GET: api/EncryptedData/5
        [HttpGet("{id}")]
        public async Task<ActionResult<EncryptedData>> GetEncryptedData(int id)
        {
            var encryptedData = await _context.EncryptedData.FindAsync(id);

            if (encryptedData == null) return NotFound();

            return encryptedData;
        }

        // POST: api/EncryptedData
        [HttpPost]
        public async Task<ActionResult<EncryptedData>> PostEncryptedData(EncryptedData encryptedData)
        {
            _context.EncryptedData.Add(encryptedData);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetEncryptedData", new {id = encryptedData.EncryptedDataId}, encryptedData);
        }

        private bool EncryptedDataExists(int id)
        {
            return _context.EncryptedData.Any(e => e.EncryptedDataId == id);
        }
    }
}