using System;
using System.Collections.Generic;
using System.Text;
using Camino.Core.Domain.Entities.YeuCauNhapViens;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Camino.Data.Mapping.YeuCauNhapViens
{
    public class YeuCauNhapVienMap : CaminoEntityTypeConfiguration<YeuCauNhapVien>
    {
        public override void Configure(EntityTypeBuilder<YeuCauNhapVien> builder)
        {
            builder.ToTable(MappingDefaults.YeuCauNhapVienTable);

            builder.HasOne(rf => rf.BenhNhan)
                .WithMany(r => r.YeuCauNhapViens)
                .HasForeignKey(rf => rf.BenhNhanId);

            builder.HasOne(rf => rf.BacSiChiDinh)
                .WithMany(r => r.YeuCauNhapViens)
                .HasForeignKey(rf => rf.BacSiChiDinhId);

            builder.HasOne(rf => rf.NoiChiDinh)
                .WithMany(r => r.YeuCauNhapViens)
                .HasForeignKey(rf => rf.NoiChiDinhId);

            builder.HasOne(rf => rf.YeuCauKhamBenh)
                .WithMany(r => r.YeuCauNhapViens)
                .HasForeignKey(rf => rf.YeuCauKhamBenhId);

            builder.HasOne(rf => rf.KhoaPhongNhapVien)
                .WithMany(r => r.YeuCauNhapViens)
                .HasForeignKey(rf => rf.KhoaPhongNhapVienId);

            builder.HasOne(rf => rf.ChanDoanNhapVienICD)
                .WithMany(r => r.YeuCauNhapViens)
                .HasForeignKey(rf => rf.ChanDoanNhapVienICDId);

            builder.HasOne(rf => rf.YeuCauTiepNhanMe)
                .WithMany(r => r.YeuCauNhapVienCons)
                .HasForeignKey(rf => rf.YeuCauTiepNhanMeId);

            base.Configure(builder);
        }
    }
}
