using System;
using System.Collections.Generic;
using System.Text;
using Camino.Core.Domain.Entities.DichVuKyThuats;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Camino.Data.Mapping.DichVuKyThuat
{
    public class DichVuKyThuatBenhVienNoiThucHienMap: CaminoEntityTypeConfiguration<DichVuKyThuatBenhVienNoiThucHien>
    {
        public override void Configure(EntityTypeBuilder<DichVuKyThuatBenhVienNoiThucHien> builder)
        {
            builder.ToTable(MappingDefaults.DichVuKyThuatBenhVienNoiThucHienTable);

            builder.HasOne(rf => rf.DichVuKyThuatBenhVien)
                .WithMany(r => r.DichVuKyThuatBenhVienNoiThucHiens)
                .HasForeignKey(rf => rf.DichVuKyThuatBenhVienId);

            builder.HasOne(rf => rf.KhoaPhong)
                .WithMany(r => r.DichVuKyThuatBenhVienNoiThucHiens)
                .HasForeignKey(rf => rf.KhoaPhongId);

            builder.HasOne(rf => rf.PhongBenhVien)
                .WithMany(r => r.DichVuKyThuatBenhVienNoiThucHiens)
                .HasForeignKey(rf => rf.PhongBenhVienId);

            base.Configure(builder);
        }
    }
}
