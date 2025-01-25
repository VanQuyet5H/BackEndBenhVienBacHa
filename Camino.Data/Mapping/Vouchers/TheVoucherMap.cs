using System;
using System.Collections.Generic;
using System.Text;
using Camino.Core.Domain.Entities.Vouchers;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Camino.Data.Mapping.Vouchers
{
    public class TheVoucherMap : CaminoEntityTypeConfiguration<TheVoucher>
    {
        public override void Configure(EntityTypeBuilder<TheVoucher> builder)
        {
            builder.ToTable(MappingDefaults.TheVoucherTable);

            builder.HasOne(rf => rf.Voucher)
                .WithMany(r => r.TheVouchers)
                .HasForeignKey(rf => rf.VoucherId);
            base.Configure(builder);
        }
    }
}
