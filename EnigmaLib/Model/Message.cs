using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EnigmaLib.Model
{
    [Table("Message")]
    public class Message
    {
        [Key] public int MessageId { get; set; }

        [Required] public int FromUserId { get; set; }

        [ForeignKey("FromUserId")] public virtual User FromUser { get; set; }

        [Required] public int ToUserId { get; set; }

        [ForeignKey("ToUserId")] public virtual User ToUser { get; set; }

        [Required] public int GroupId { get; set; }

        [ForeignKey("GroupId")] public virtual Group Group { get; set; }

        [Required] public EncryptedData EncryptedData { get; set; }
    }
}