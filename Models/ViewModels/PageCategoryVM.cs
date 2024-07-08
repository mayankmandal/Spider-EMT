using Spider_EMT.Models.ValidationAttributes;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Spider_EMT.Models.ViewModels
{
    public class PageCategoryVM
    {
        public int PageCatId { get; set; }
        [DisplayName("Category Name")]
        [Required]
        [RegularExpression(@"^[a-zA-Z0-9_\s]*$", ErrorMessage = "Category Name can only contain alphabets, numbers, whitespaces and underscore.")]
        [StringLength(100, ErrorMessage = "Category Name cannot exceed 100 characters.")]
        [CheckUniquenessinDB("catagoryname")]
        public string CategoryName { get; set; }
        // Navigation property for Pages
        public int PageId { get; set; }
    }
}
