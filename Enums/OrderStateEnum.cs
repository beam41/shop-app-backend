using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace ShopAppBackend.Enums
{
    public enum OrderStateEnum
    {
        [EnumMember(Value = "KERRY")]
        CREATED, // 1
        [EnumMember(Value = "ADDED_PURCHASE_METHOD")]
        AddedPurchaseMethod,
        [EnumMember(Value = "ADDED_ADDRESS")]
        AddedAddress,
        [EnumMember(Value = "EDITED_ADDRESS")]
        EditedAddress,
        [EnumMember(Value = "ADDED_PROOF_OF_PAYMENT_FULL")]
        AddedProofOfPaymentFull, // 2
        [EnumMember(Value = "APPROVED_PROOF_OF_PAYMENT_FULL")]
        ApprovedProofOfPaymentFull, // 3
        [EnumMember(Value = "SENT")]
        Sent, // 4
        [EnumMember(Value = "RECEIVED")]
        Received, // 5

        [EnumMember(Value = "CANCELLED")]
        Cancelled,
        // for build order
        [EnumMember(Value = "IS_ABLE_TO_BUILT")]
        IsAbleToBuilt,
        [EnumMember(Value = "IsUnableToBuilt")]
        IsUnableToBuilt,
        [EnumMember(Value = "CONTACT_MADE")]
        ContactMade,
        [EnumMember(Value = "ADDED_PROOF_OF_PAYMENT_DEPOSIT")]
        AddedProofOfPaymentDeposit,
        [EnumMember(Value = "APPROVED_PROOF_OF_PAYMENT_DEPOSIT")]
        ApprovedProofOfPaymentDeposit,
        [EnumMember(Value = "BUILT_COMPLETE")]
        BuiltComplete,
        // loop back to AddedProofOfPaymentFull
    }
}
