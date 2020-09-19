using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace ShopAppBackend.Enums
{
    public enum PurchaseMethodEnum
    {
        [EnumMember(Value = "BANK")]
        Bank,
        [EnumMember(Value = "ON_DELIVERY")]
        OnDelivery
    }
}
