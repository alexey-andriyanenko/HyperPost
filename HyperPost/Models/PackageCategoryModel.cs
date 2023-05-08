using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HyperPost.Models
{
    [Table("PackageCategory")]
    public class PackageCategoryModel
    {
        public int Id { get; set; }

        [Column(TypeName = "nvarchar(50)")]
        [MaxLength(50)]
        public string Name { get; set; }
    }
}
