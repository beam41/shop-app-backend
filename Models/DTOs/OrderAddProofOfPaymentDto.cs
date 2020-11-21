using Microsoft.AspNetCore.Http;

namespace ShopAppBackend.Models.DTOs
{
    public class OrderAddProofOfPaymentDto
    {
        public IFormFile Image { get; set; }
    }
}
