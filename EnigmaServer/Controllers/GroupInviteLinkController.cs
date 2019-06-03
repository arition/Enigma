using System;
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
    [Route("api/invite")]
    [ApiController]
    public class GroupInviteLinkController : ControllerBase
    {
        private readonly DatabaseContext _context;
        private readonly Utils _utils;

        public GroupInviteLinkController(DatabaseContext context, Utils utils)
        {
            _context = context;
            _utils = utils;
        }

        [HttpGet("show/{id}/{inviteCode}")]
        public async Task<ActionResult<GroupInviteLink>> GetGroupInviteLink(int id, string inviteCode)
        {
            var groupInviteLink = await _context.GroupInviteLink.FindAsync(id);

            if (groupInviteLink == null || groupInviteLink.InviteCode != inviteCode)
                return NotFound();

            return groupInviteLink;
        }

        // GET: api/invite/create/5
        [HttpGet("create/{id}")]
        public async Task<ActionResult<GroupInviteLink>> AddGroupInviteLink(int id)
        {
            var user = await _utils.GetUserAsync(User);
            if (user.GroupUsers.All(t => t.GroupId != id))
                return Forbid();

            var groupInviteLink = new GroupInviteLink
            {
                Expires = DateTime.UtcNow + TimeSpan.FromHours(1),
                GroupId = id,
                InviteCode = Guid.NewGuid().ToString().Replace("-", "").Replace("{", "").Replace("}", "")
            };

            var b = new Guid().ToString();
            _context.GroupInviteLink.Add(groupInviteLink);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetGroupInviteLink",
                new
                {
                    id = groupInviteLink.GroupInviteLinkId,
                    inviteCode = groupInviteLink.InviteCode
                }, groupInviteLink);
        }

        // GET: api/invite/enter/12/5asqwe435fgt
        [HttpGet("enter/{id}/{inviteCode}")]
        public async Task<ActionResult<GroupInviteLink>> EnterGroupInviteLink(int id, string inviteCode)
        {
            var user = await _context.User.Include(t => t.GroupUsers)
                .Where(t => t.UserId == _utils.GetUserId(User)).SingleAsync();
            var invite = await _context.GroupInviteLink
                .Where(t => t.GroupInviteLinkId == id && t.InviteCode == inviteCode).SingleOrDefaultAsync();

            if (invite == null) return NotFound();
            if (user.GroupUsers.Any(t => t.GroupId == invite.GroupId)) return BadRequest();
            if (invite.Expires < DateTime.UtcNow) return BadRequest("Link expired");
            user.GroupUsers.Add(new GroupUser {GroupId = invite.GroupId, UserId = user.UserId});
            await _context.SaveChangesAsync();

            return invite;
        }

        // DELETE: api/invite/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<GroupInviteLink>> DeleteGroupInviteLink(int id)
        {
            var user = await _utils.GetUserAsync(User);
            if (user.GroupUsers.All(t => t.GroupId != id))
                return Forbid();

            var groupInviteLink = await _context.GroupInviteLink.FindAsync(id);
            if (groupInviteLink == null) return NotFound();

            _context.GroupInviteLink.Remove(groupInviteLink);
            await _context.SaveChangesAsync();

            return groupInviteLink;
        }

        private bool GroupInviteLinkExists(int id)
        {
            return _context.GroupInviteLink.Any(e => e.GroupInviteLinkId == id);
        }
    }
}