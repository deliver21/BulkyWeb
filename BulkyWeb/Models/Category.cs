using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace BulkyWeb.Models
{
    public class Category
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [DisplayName("Category Name")]
        [MaxLength(30)]
        [MinLength(3, ErrorMessage = "Your Category Name is too Short [At least 3 letters required]")]
        public string Name { get; set; }
        [DisplayName("Display Order")]
        [Range(1, 150, ErrorMessage = "Display Order must be between (1-150)")]
        [Required]
        public int DisplayOrder { get; set; }
    }
}
