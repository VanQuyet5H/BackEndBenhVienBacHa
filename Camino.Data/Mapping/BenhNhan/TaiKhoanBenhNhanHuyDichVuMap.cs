using System;
using System.Collections.Generic;
using System.Text;
using Camino.Core.Domain.Entities.BenhNhans;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Camino.Data.Mapping.BenhNhan
{
    public class TaiKhoanBenhNhanHuyDichVuMap : CaminoEntityTypeConfiguration<TaiKhoanBenhNhanHuyDichVu>
    {
        public override void Configure(EntityTypeBuilder<TaiKhoanBenhNhanHuyDichVu> builder)
        {
            builder.ToTable(MappingDefaults.TaiKhoanBenhNhanHuyDichVuTable);

            builder.HasOne(rf => rf.TaiKhoanBenhNhan)
                .WithMany(r => r.TaiKhoanBenhNhanHuyDichVus)
                .HasForeignKey(rf => rf.TaiKhoanBenhNhanId);
            builder.HasOne(rf => rf.YeuCauTiepNhan)
                .WithMany(r => r.TaiKhoanBenhNhanHuyDichVus)
                .HasForeignKey(rf => rf.YeuCauTiepNhanId);
            builder.HasOne(rf => rf.NhanVienThucHien)
                .WithMany(r => r.TaiKhoanBenhNhanHuyDichVus)
                .HasForeignKey(rf => rf.NhanVienThucHienId);
            builder.HasOne(rf => rf.NoiThucHien)
                .WithMany(r => r.TaiKhoanBenhNhanHuyDichVus)
                .HasForeignKey(rf => rf.NoiThucHienId);

            base.Configure(builder);
        }
    }
}
