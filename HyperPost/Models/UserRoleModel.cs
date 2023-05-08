using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HyperPost.Models
{
    [Table("UserRole")]
    public class UserRoleModel
    {
        public int Id { get; set; }

        [Column(TypeName = "nvarchar(20)")]
        [MaxLength(20)]
        public string Name { get; set; }
    }
}
