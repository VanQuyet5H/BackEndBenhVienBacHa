using System;
using System.Collections.Generic;
using System.Text;
using Camino.Core.Domain.Entities.DichVuGiuongBenhViens;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Camino.Data.Mapping.DichVuGiuongBenhVien
{
    public class DichVuGiuongBenhVienNoiThucHienMap: CaminoEntityTypeConfiguration<DichVuGiuongBenhVienNoiThucHien>
    {
        public override void Configure(EntityTypeBuilder<DichVuGiuongBenhVienNoiThucHien> builder)
        {
            builder.ToTable(MappingDefaults.DichVuGiuongBenhVienNoiThucHienTable);

            builder.HasOne(rf => rf.DichVuGiuongBenhVien)
                .WithMany(r => r.DichVuGiuongBenhVienNoiThucHiens)
                .HasForeignKey(rf => rf.DichVuGiuongBenhVienId);

            builder.HasOne(rf => rf.KhoaPhong)
                .WithMany(r => r.DichVuGiuongBenhVienNoiThucHiens)
                .HasForeignKey(rf => rf.KhoaPhongId);

            builder.HasOne(rf => rf.PhongBenhVien)
                .WithMany(r => r.DichVuGiuongBenhVienNoiThucHiens)
                .HasForeignKey(rf => rf.PhongBenhVienId);

            base.Configure(builder);
        }
    }
}
