using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EnigmaLib.Model
{
    [Table("GroupInviteLink")]
    public class GroupInviteLink
    {
        [Key] public int GroupInviteLinkId { get; set; }
        
        [Required]
        public int GroupId { get; set; }

        [ForeignKey("GroupId")] public virtual Group Group { get; set; }

        [Required]
        public string InviteCode { get; set; }

        [Required]
        public DateTime Expires { get; set; }
    }
}