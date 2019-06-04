using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Security.Cryptography;
using Newtonsoft.Json;

namespace EnigmaLib.Model
{
    [Table("User")]
    public class User
    {
        private RSAParameters? _publicKey;
        [Key] public int UserId { get; set; }

        [Required] public string PublicKeyString { get; set; }

        [NotMapped]
        public RSAParameters PublicKey
        {
            get
            {
                if (_publicKey == null)
                    _publicKey = JsonConvert.DeserializeObject<RSAParameters>(PublicKeyString);
                return _publicKey.Value;
            }
            set
            {
                _publicKey = value;
                PublicKeyString = JsonConvert.SerializeObject(_publicKey);
            }
        }

        [Required]
        [MinLength(1)]
        [MaxLength(20)]
        public string Username { get; set; }

        /// <summary>
        ///     For EF Database
        /// </summary>
        public virtual List<GroupUser> GroupUsers { get; set; }

        public override string ToString()
        {
            return $"Username: {Username}\nPublicKey: {(PublicKeyString != null ? "True" : "False")}";
        }
    }
}