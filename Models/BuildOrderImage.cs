using System.ComponentModel.DataAnnotations;

namespace ShopAppBackend.Models
{
    public class BuildOrderImage
    {
        public int Id { get; set; }

        [Required]
        public BuildOrder BuildOrder { get; set; }

        [Required]
        public string ImageFileName { get; set; }
    }
}
