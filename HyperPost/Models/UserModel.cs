using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HyperPost.Models
{
    [Table("User")]
    [Index(nameof(Email), IsUnique = true)]
    [Index(nameof(PhoneNumber), IsUnique = true)]
    public class UserModel
    {
        public int Id { get; set; }

        [Column(TypeName = "nvarchar(30)")]
        [MaxLength(30)]
        public string FirstName { get; set; }

        [Column(TypeName = "nvarchar(30)")]
        [MaxLength(30)]
        public string LastName { get; set; }

        [Column(TypeName = "nvarchar(30)")]
        [MaxLength(30)]
        public string? Email { get; set; }

        public int RoleId { get; set; }

        [Column(TypeName = "nvarchar(20)")]
        [MaxLength(20)]
        public string PhoneNumber { get; set; }

        [Column(TypeName = "nvarchar(30)")]
        [MaxLength(30)]
        public string? Password { get; set; }
    }
}
