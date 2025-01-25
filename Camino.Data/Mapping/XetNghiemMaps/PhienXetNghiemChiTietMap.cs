using Camino.Core.Domain.Entities.XetNghiems;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Camino.Data.Mapping.XetNghiemMaps
{
    public class PhienXetNghiemChiTietMap : CaminoEntityTypeConfiguration<PhienXetNghiemChiTiet>
    {
        public override void Configure(EntityTypeBuilder<PhienXetNghiemChiTiet> builder)
        {
            builder.ToTable(MappingDefaults.PhienXetNghiemChiTietTable);

            builder.HasOne(rf => rf.PhienXetNghiem)
                .WithMany(r => r.PhienXetNghiemChiTiets)
                .HasForeignKey(rf => rf.PhienXetNghiemId);

            builder.HasOne(rf => rf.NhomDichVuBenhVien)
                .WithMany(r => r.PhienXetNghiemChiTiets)
                .HasForeignKey(rf => rf.NhomDichVuBenhVienId);

            builder.HasOne(rf => rf.YeuCauDichVuKyThuat)
                .WithMany(r => r.PhienXetNghiemChiTiets)
                .HasForeignKey(rf => rf.YeuCauDichVuKyThuatId);

            builder.HasOne(rf => rf.DichVuKyThuatBenhVien)
                .WithMany(r => r.PhienXetNghiemChiTiets)
                .HasForeignKey(rf => rf.DichVuKyThuatBenhVienId);

            builder.HasOne(rf => rf.NhanVienKetLuan)
                .WithMany(r => r.PhienXetNghiemChiTiets)
                .HasForeignKey(rf => rf.NhanVienKetLuanId);

            builder.HasOne(rf => rf.YeuCauChayLaiXetNghiem)
                .WithMany(r => r.PhienXetNghiemChiTiets)
                .HasForeignKey(rf => rf.YeuCauChayLaiXetNghiemId);

            builder.HasOne(x => x.NhanVienLayMau)
                .WithMany(x => x.PhienXetNghiemChiTietNhanVienLayMaus)
                .HasForeignKey(x => x.NhanVienLayMauId);
            builder.HasOne(x => x.PhongLayMau)
                .WithMany(x => x.PhienXetNghiemChiTietPhongLayMaus)
                .HasForeignKey(x => x.PhongLayMauId);
            builder.HasOne(x => x.NhanVienNhanMau)
                .WithMany(x => x.PhienXetNghiemChiTietNhanVienNhanMaus)
                .HasForeignKey(x => x.NhanVienNhanMauId);
            builder.HasOne(x => x.PhongNhanMau)
                .WithMany(x => x.PhienXetNghiemChiTietPhongNhanMaus)
                .HasForeignKey(x => x.PhongNhanMauId);

            base.Configure(builder);
        }
    }
}
