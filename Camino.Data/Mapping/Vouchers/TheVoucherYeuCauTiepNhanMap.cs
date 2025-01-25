using System;
using System.Collections.Generic;
using System.Text;
using Camino.Core.Domain.Entities.Vouchers;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Camino.Data.Mapping.Vouchers
{
    public class TheVoucherYeuCauTiepNhanMap : CaminoEntityTypeConfiguration<TheVoucherYeuCauTiepNhan>
    {
        public override void Configure(EntityTypeBuilder<TheVoucherYeuCauTiepNhan> builder)
        {
            builder.ToTable(MappingDefaults.TheVoucherYeuCauTiepNhanTable);

            builder.HasOne(rf => rf.TheVoucher)
                .WithMany(r => r.TheVoucherYeuCauTiepNhans)
                .HasForeignKey(rf => rf.TheVoucherId);
            builder.HasOne(rf => rf.YeuCauTiepNhan)
                .WithMany(r => r.TheVoucherYeuCauTiepNhans)
                .HasForeignKey(rf => rf.YeuCauTiepNhanId);
            builder.HasOne(rf => rf.BenhNhan)
                .WithMany(r => r.TheVoucherYeuCauTiepNhans)
                .HasForeignKey(rf => rf.BenhNhanId);
            base.Configure(builder);
        }
    }
}
