using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EnigmaLib.Model
{
    [Table("Group")]
    public class Group
    {
        [Key] public int GroupId { get; set; }

        public virtual List<GroupUser> GroupUsers { get; set; }

        [Required]
        [MinLength(1)]
        [MaxLength(40)]
        public string GroupName { get; set; }
    }
}