using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RESTfulWebAPITask2.Model
{
    public class CartItem
    {
        [ForeignKey("Cart")]
        public string CartId { get; set; } //CartId from Cart table
        
        [Key]
        public int Id { get; set; } //PrimaryKey
        public string Name { get; set; }
        public string? ImageUrl { get; set; }
        public string? ImageAltText { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }

        //
        //public Cart Cart { get; set; }
    }
}
