using System.Linq;
using System.Threading.Tasks;
using EnigmaLib.Model;
using EnigmaServer.Database;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EnigmaServer.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly DatabaseContext _context;
        private readonly Utils _utils;

        public UserController(DatabaseContext context, Utils utils)
        {
            _context = context;
            _utils = utils;
        }

        // GET: api/User/me
        [HttpGet("me")]
        public async Task<ActionResult<User>> GetMe()
        {
            var userId = _utils.GetUserId(User);
            var user = await _context.User.Include(t => t.GroupUsers)
                .Where(t => t.UserId == userId).SingleOrDefaultAsync();

            if (user == null) return NotFound();

            return user;
        }

        // GET: api/User/5
        [HttpGet("{id}")]
        public async Task<ActionResult<User>> GetUser(int id)
        {
            var user = await _context.User.Include(t => t.GroupUsers)
                .Where(t => t.UserId == id).SingleOrDefaultAsync();

            if (user == null) return NotFound();

            return user;
        }
        
        // POST: api/User
        [AllowAnonymous]
        [HttpPost]
        public async Task<ActionResult<User>> PostUser(User user)
        {
            _context.User.Add(user);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetUser", new {id = user.UserId}, user);
        }
    }
}