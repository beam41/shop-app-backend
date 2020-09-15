using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace ShopAppBackend.Enums
{
    public enum DistributionMethodEnum
    {
        [EnumMember(Value = "KERRY")]
        Kerry,
        [EnumMember(Value = "THAILAND_POST")]
        ThailandPost,
        [EnumMember(Value = "THAILAND_POST_EMS")]
        ThailandPostEms,
    }
}
