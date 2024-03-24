
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AppsServer.Models.Entities
{
    [Table("master_product")]
    [Index(nameof(Code), IsUnique = true)]
    public class MasterProduct
    {
        [Key]
        [Required]
        [Column("id", Order = 0)]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        [Column("code", Order = 1), MaxLength(50)]
        public string? Code { get; set; }

        [Required]
        [Column("name", Order = 2), MaxLength(200)]
        public string? Name { get; set; }

        [Required]
        [Column("active_flag", Order = 3)]
        public bool ActiveFlag { get; set; } = true;
    }
}
