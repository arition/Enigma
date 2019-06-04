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
    public class MessageController : ControllerBase
    {
        private readonly DatabaseContext _context;
        private readonly Utils _utils;

        public MessageController(DatabaseContext context, Utils utils)
        {
            _context = context;
            _utils = utils;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Message>> GetSingleMessage(int id)
        {
            var userId = _utils.GetUserId(User);
            var message = await _context.Message
                .Include(t => t.FromUser)
                .Include(t => t.EncryptedData)
                .Where(t => t.MessageId == id && t.ToUserId == userId)
                .SingleOrDefaultAsync();

            return message;
        }

        /// <summary>
        ///     Get latest 20 messages
        ///     GET: api/Message/latest/5
        /// </summary>
        /// <param name="groupId"></param>
        /// <returns></returns>
        [HttpGet("latest/{groupId}")]
        public async Task<ActionResult<List<Message>>> GetMessage(int groupId)
        {
            var userId = _utils.GetUserId(User);
            var message = await _context.Message
                .Include(t => t.FromUser)
                .Include(t => t.EncryptedData)
                .Where(t => t.GroupId == groupId && t.ToUserId == userId)
                .OrderByDescending(t => t.MessageId).Take(20).OrderBy(t => t.MessageId).ToListAsync();

            return message;
        }

        /// <summary>
        ///     Get prev 20 messages from messageId
        ///     GET: api/Message/prev/5/6
        /// </summary>
        /// <param name="groupId"></param>
        /// <param name="messageId"></param>
        /// <returns></returns>
        [HttpGet("prev/{groupId}/{messageId}")]
        public async Task<ActionResult<List<Message>>> GetPrevMessage(int groupId, int messageId)
        {
            var userId = _utils.GetUserId(User);
            var message = await _context.Message
                .Include(t => t.FromUser)
                .Include(t => t.EncryptedData)
                .Where(t => t.GroupId == groupId && t.ToUserId == userId && t.MessageId < messageId)
                .OrderByDescending(t => t.MessageId).Take(20).OrderBy(t => t.MessageId).ToListAsync();

            return message;
        }

        /// <summary>
        ///     Get next 20 messages from messageId
        ///     GET: api/Message/prev/5/6
        /// </summary>
        /// <param name="groupId"></param>
        /// <param name="messageId"></param>
        /// <returns></returns>
        [HttpGet("next/{groupId}/{messageId}")]
        public async Task<ActionResult<List<Message>>> GetNextMessage(int groupId, int messageId)
        {
            var userId = _utils.GetUserId(User);
            var message = await _context.Message
                .Include(t => t.FromUser)
                .Include(t => t.EncryptedData)
                .Where(t => t.GroupId == groupId && t.ToUserId == userId && t.MessageId > messageId)
                .OrderBy(t => t.MessageId).Take(20).ToListAsync();

            return message;
        }

        // POST: api/Message
        [HttpPost]
        public async Task<ActionResult<Message>> PostMessage(Message message)
        {
            _context.Message.Add(message);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetSingleMessage", new {id = message.MessageId}, message);
        }

        private bool MessageExists(int id)
        {
            return _context.Message.Any(e => e.MessageId == id);
        }
    }
}