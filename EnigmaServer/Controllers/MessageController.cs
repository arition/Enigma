using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EnigmaLib.Model;
using EnigmaServer.Database;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EnigmaServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MessageController : ControllerBase
    {
        private readonly DatabaseContext _context;

        public MessageController(DatabaseContext context)
        {
            _context = context;
        }

        /// <summary>
        ///     Get latest 20 messages
        ///     GET: api/Message/latest/5/6
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="groupId"></param>
        /// <returns></returns>
        [HttpGet("latest/{groupId}/{userId}")]
        public async Task<ActionResult<List<Message>>> GetMessage(int userId, int groupId)
        {
            var message = await _context.Message.Where(t => t.GroupId == groupId && t.ToUserId == userId)
                .OrderByDescending(t => t.MessageId).Take(20).ToListAsync();

            return message;
        }

        /// <summary>
        ///     Get prev 20 messages from messageId
        ///     GET: api/Message/prev/5/6/7
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="groupId"></param>
        /// <param name="messageId"></param>
        /// <returns></returns>
        [HttpGet("prev/{groupId}/{userId}/{messageId}")]
        public async Task<ActionResult<List<Message>>> GetPrevMessage(int userId, int groupId, int messageId)
        {
            var message = await _context.Message
                .Where(t => t.GroupId == groupId && t.ToUserId == userId && t.MessageId < messageId)
                .OrderByDescending(t => t.MessageId).Take(20).ToListAsync();

            return message;
        }

        /// <summary>
        ///     Get next 20 messages from messageId
        ///     GET: api/Message/prev/5/6/7
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="groupId"></param>
        /// <param name="messageId"></param>
        /// <returns></returns>
        [HttpGet("next/{groupId}/{userId}/{messageId}")]
        public async Task<ActionResult<List<Message>>> GetNextMessage(int userId, int groupId, int messageId)
        {
            var message = await _context.Message
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

            return CreatedAtAction("GetMessage", new {id = message.MessageId}, message);
        }

        private bool MessageExists(int id)
        {
            return _context.Message.Any(e => e.MessageId == id);
        }
    }
}