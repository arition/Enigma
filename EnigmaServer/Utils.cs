using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using EnigmaLib.Model;
using EnigmaServer.Database;
using Microsoft.EntityFrameworkCore;

namespace EnigmaServer
{
    public class Utils
    {
        private readonly DatabaseContext _database;

        public Utils(DatabaseContext database)
        {
            _database = database;
        }

        public async Task<User> GetUserAsync(ClaimsPrincipal principal)
        {
            var userId = principal.Claims.First(t => t.Type == ClaimTypes.NameIdentifier).Value;
            return await _database.User.Include(t => t.GroupUsers)
                .Where(t => t.UserId == int.Parse(userId)).SingleOrDefaultAsync();
        }

        public int GetUserId(ClaimsPrincipal principal)
        {
            return int.Parse(principal.Claims.First(t => t.Type == ClaimTypes.NameIdentifier).Value);
        }
    }
}