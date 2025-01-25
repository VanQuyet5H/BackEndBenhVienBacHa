using System;
using System.Collections.Generic;
using System.Text;
using Camino.Core.Domain.Entities.BenhNhans;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Camino.Data.Mapping.BenhNhan
{
    public class TaiKhoanBenhNhanChiThongTinMap : CaminoEntityTypeConfiguration<TaiKhoanBenhNhanChiThongTin>
    {
        public override void Configure(EntityTypeBuilder<TaiKhoanBenhNhanChiThongTin> builder)
        {
            builder.ToTable(MappingDefaults.TaiKhoanBenhNhanChiThongTinTable);

            builder.HasOne(rf => rf.TaiKhoanBenhNhanChi)
              .WithOne(r => r.TaiKhoanBenhNhanChiThongTin).
              HasForeignKey<TaiKhoanBenhNhanChiThongTin>(c => c.Id);

            base.Configure(builder);
        }
    }
}
