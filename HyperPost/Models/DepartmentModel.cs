using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HyperPost.Models
{
    [Table("Department")]
    [Index(nameof(Number), IsUnique = true)]
    public class DepartmentModel
    {
        public int Id { get; set; }
        public int Number { get; set; }

        [Column(TypeName = "nvarchar(100)")]
        [MaxLength(100)]
        public string FullAddress { get; set; }
    }
}
