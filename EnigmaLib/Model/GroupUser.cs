using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;

namespace EnigmaLib.Model
{
    [Table("GroupUser")]
    public class GroupUser
    {
        [Key] public int GroupUserId { get; set; }

        [Required] public int GroupId { get; set; }

        [JsonIgnore] [ForeignKey("GroupId")] public virtual Group Group { get; set; }

        [Required] public int UserId { get; set; }

        [JsonIgnore] [ForeignKey("UserId")] public virtual User User { get; set; }
    }
}