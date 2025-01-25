using System;
using System.Collections.Generic;
using System.Text;
using Camino.Core.Domain.Entities.Vouchers;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Camino.Data.Mapping.Vouchers
{
    public class VoucherMap : CaminoEntityTypeConfiguration<Voucher>
    {
        public override void Configure(EntityTypeBuilder<Voucher> builder)
        {
            builder.ToTable(MappingDefaults.VoucherTable);
            base.Configure(builder);
        }
    }
}
