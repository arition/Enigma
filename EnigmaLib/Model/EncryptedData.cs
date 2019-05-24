using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EnigmaLib.Model
{
    [Table("EncryptedData")]
    public class EncryptedData
    {
        [Key] public int EncryptedDataId { get; set; }

        public byte[] AESEncryptedData { get; set; }

        [Required] public byte[] RSAEncryptedAESKey { get; set; }
    }
}