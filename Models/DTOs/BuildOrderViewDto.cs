using System;
using System.Collections.Generic;

namespace ShopAppBackend.Models.DTOs
{
    public class BuildOrderViewDto
    {
        public int Id { get; set; }

        public User CreatedByUser { get; set; }

        public ICollection<OrderStateDto> OrderStates { get; set; }

        public DistributionMethod DistributionMethod { get; set; }

        public string FullName { get; set; }

        public string OrderDescription { get; set; }

        public ICollection<ImageUrlDto> DescriptionImagesUrl { get; set; }

        public string PhoneNumber { get; set; }

        public string AddressPhoneNumber { get; set; }

        public string AddressFullName { get; set; }

        public string Address { get; set; }

        public string Province { get; set; }

        public string District { get; set; }

        public string SubDistrict { get; set; }

        public string PostalCode { get; set; }

        public string ProofOfPaymentFullImage { get; set; }

        public string ProofOfPaymentDepositImage { get; set; }

        public string TrackingNumber { get; set; }

        public string ReceivedMessage { get; set; }

        public bool? CancelledByAdmin { get; set; }

        public string CancelledReason { get; set; }

        public double? DepositPrice { get; set; }

        public double? FullPrice { get; set; }

        public DateTimeOffset? ExpectedCompleteDate { get; set; }
    }
}
