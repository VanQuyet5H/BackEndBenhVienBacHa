using System;
using System.Collections.Generic;
using System.Text;
using Camino.Core.Domain.Entities.DichVuKhamBenhBenhViens;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Camino.Data.Mapping.DichVuKhamBenhBenhViens
{
    public class DichVuKhamBenhBenhVienNoiThucHienMap : CaminoEntityTypeConfiguration<DichVuKhamBenhBenhVienNoiThucHien>
    {
        public override void Configure(EntityTypeBuilder<DichVuKhamBenhBenhVienNoiThucHien> builder)
        {
            builder.ToTable(MappingDefaults.DichVuKhamBenhBenhVienNoiThucHienTable);

            builder.HasOne(rf => rf.DichVuKhamBenhBenhVien)
                .WithMany(r => r.DichVuKhamBenhBenhVienNoiThucHiens)
                .HasForeignKey(rf => rf.DichVuKhamBenhBenhVienId);

            builder.HasOne(rf => rf.KhoaPhong)
                .WithMany(r => r.DichVuKhamBenhBenhVienNoiThucHiens)
                .HasForeignKey(rf => rf.KhoaPhongId);

            builder.HasOne(rf => rf.PhongBenhVien)
                .WithMany(r => r.DichVuKhamBenhBenhVienNoiThucHiens)
                .HasForeignKey(rf => rf.PhongBenhVienId);

            base.Configure(builder);
        }
    }
}
