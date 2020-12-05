using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.ValueGeneration;

namespace ShopAppBackend.Generators
{
    public class OrderIdGenerator : ValueGenerator
    {
        public override bool GeneratesTemporaryValues => false;

        private const string StrSet = "0123456789" + "ABCDEFGHIJKLMNOPQRSTUVQXYZ" + "abcdefghijklmnopqrstuvwxyz";

        protected override object NextValue(EntityEntry entry)
        {
            return Nanoid.Nanoid.Generate(StrSet, 10);
        }
    }
}
