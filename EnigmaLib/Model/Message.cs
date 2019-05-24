using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EnigmaLib.Model
{
    [Table("Message")]
    public class Message
    {
        [Key] public int MessageId { get; set; }

        public int FromUserId { get; set; }

        [ForeignKey("FromUserId")] public virtual User FromUser { get; set; }

        public int ToUserId { get; set; }

        [ForeignKey("ToUserId")] public virtual User ToUser { get; set; }

        public int GroupId { get; set; }

        [ForeignKey("GroupId")] public virtual Group Group { get; set; }

        public EncryptedData EncryptedData { get; set; }
    }
}