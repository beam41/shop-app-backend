using System.Collections.Generic;

namespace ShopAppBackend.Models.DTOs
{
    public class ProductDetailAdminDto
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public double Price { get; set; }

        public string Description { get; set; }

        public bool IsVisible { get; set; }

        public int TypeId { get; set; }

        public ICollection<ImageUrlDto> Images { get; set; }
    }
}