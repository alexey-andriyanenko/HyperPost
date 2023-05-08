using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HyperPost.Models
{
    [Table("PackageStatus")]
    public class PackageStatusModel
    {
        public int Id { get; set; }

        [Column(TypeName = "nvarchar(30)")]
        [MaxLength(30)]
        public string Name { get; set; }
    }
}
