using System.Collections.Generic;
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
    public class GroupController : ControllerBase
    {
        private readonly DatabaseContext _context;
        private readonly Utils _utils;

        public GroupController(DatabaseContext context, Utils utils)
        {
            _context = context;
            _utils = utils;
        }

        // GET: api/Group/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Group>> GetGroup(int id)
        {
            var group = await _context.Group.Include(t => t.GroupUsers)
                .Where(t => t.GroupId == id).SingleOrDefaultAsync();

            if (group == null) return NotFound();

            return group;
        }

        // PUT: api/Group/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutGroup(int id, Group group)
        {
            if (id != group.GroupId) return BadRequest();

            var userId = _utils.GetUserId(User);
            if (!await _context.Group
                .Where(t => t.GroupId == id && t.GroupUsers.Any(d => d.UserId == userId)).AnyAsync())
                return Forbid();

            _context.Entry(group).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!GroupExists(id))
                    return NotFound();
                throw;
            }

            return NoContent();
        }

        // POST: api/Group
        [HttpPost]
        public async Task<ActionResult<Group>> PostGroup(Group group)
        {
            group.GroupUsers = new List<GroupUser> {new GroupUser {Group = group, UserId = _utils.GetUserId(User)}};
            _context.Group.Add(group);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetGroup", new {id = group.GroupId}, group);
        }

        private bool GroupExists(int id)
        {
            return _context.Group.Any(e => e.GroupId == id);
        }
    }
}