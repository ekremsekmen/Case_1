using System.ComponentModel.DataAnnotations;

namespace Case_1.Core.Application.DTOs
{
    public class UpdateProductDto
    {
        [StringLength(100, MinimumLength = 1)]
        public string? Name { get; set; }
        
        [StringLength(500)]
        public string? Description { get; set; }
        
        [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than 0")]
        public decimal? Price { get; set; }
        
        [Range(0, int.MaxValue, ErrorMessage = "Stock must be non-negative")]
        public int? Stock { get; set; }
        
        [StringLength(50)]
        public string? Category { get; set; }
    }
}
