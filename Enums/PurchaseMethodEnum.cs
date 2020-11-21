using System.Runtime.Serialization;

namespace ShopAppBackend.Enums
{
    public enum PurchaseMethodEnum
    {
        [EnumMember(Value = "BANK")] Bank,

        [EnumMember(Value = "ON_DELIVERY")] OnDelivery
    }
}
