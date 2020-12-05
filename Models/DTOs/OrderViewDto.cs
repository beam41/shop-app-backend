using System.Collections.Generic;
using ShopAppBackend.Enums;

namespace ShopAppBackend.Models.DTOs
{
    public class OrderViewDto
    {
        public string Id { get; set; }

        public ICollection<ProductOrderDetailDto> Products { get; set; }

        public PurchaseMethodEnum PurchaseMethod { get; set; }

        public ICollection<OrderStateDto> OrderStates { get; set; }

        public DistributionMethod DistributionMethod { get; set; }

        public string TrackingNumber { get; set; }

        public string PhoneNumber { get; set; }

        public string FullName { get; set; }

        public string Address { get; set; }

        public string Province { get; set; }

        public string District { get; set; }

        public string SubDistrict { get; set; }

        public string PostalCode { get; set; }

        public string ProofOfPaymentFullImage { get; set; }

        public string ReceivedMessage { get; set; }

        public bool? CancelledByAdmin { get; set; }

        public string CancelledReason { get; set; }

        public User CreatedByUser { get; set; }
    }
}
