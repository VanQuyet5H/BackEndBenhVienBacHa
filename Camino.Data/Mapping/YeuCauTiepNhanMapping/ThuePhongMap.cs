using System;
using System.Collections.Generic;
using System.Text;
using Camino.Core.Domain.Entities.YeuCauTiepNhans;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Camino.Data.Mapping.YeuCauTiepNhanMapping
{
    public class ThuePhongMap : CaminoEntityTypeConfiguration<ThuePhong>
    {
        public override void Configure(EntityTypeBuilder<ThuePhong> builder)
        {
            builder.ToTable(MappingDefaults.ThuePhongTable);

            builder.HasOne(rf => rf.YeuCauTiepNhan)
                .WithMany(r => r.ThuePhongs)
                .HasForeignKey(rf => rf.YeuCauTiepNhanId);
            builder.HasOne(rf => rf.YeuCauDichVuKyThuatThuePhong)
                .WithMany(r => r.ThuePhongs)
                .HasForeignKey(rf => rf.YeuCauDichVuKyThuatThuePhongId);
            builder.HasOne(rf => rf.CauHinhThuePhong)
                .WithMany(r => r.ThuePhongs)
                .HasForeignKey(rf => rf.CauHinhThuePhongId);
            builder.HasOne(rf => rf.LoaiThuePhongPhauThuat)
                .WithMany(r => r.ThuePhongs)
                .HasForeignKey(rf => rf.LoaiThuePhongPhauThuatId);
            builder.HasOne(rf => rf.LoaiThuePhongNoiThucHien)
                .WithMany(r => r.ThuePhongs)
                .HasForeignKey(rf => rf.LoaiThuePhongNoiThucHienId);
            builder.HasOne(rf => rf.NhanVienChiDinh)
                .WithMany(r => r.ThuePhongs)
                .HasForeignKey(rf => rf.NhanVienChiDinhId);
            builder.HasOne(rf => rf.NoiChiDinh)
                .WithMany(r => r.ThuePhongs)
                .HasForeignKey(rf => rf.NoiChiDinhId);
            builder.HasOne(rf => rf.YeuCauDichVuKyThuatTinhChiPhi)
                .WithMany(r => r.TinhChiPhiThuePhongs)
                .HasForeignKey(rf => rf.YeuCauDichVuKyThuatTinhChiPhiId);

            base.Configure(builder);
        }
    }
}
