using Camino.Core.Domain.Entities.XetNghiems;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Camino.Data.Mapping.XetNghiemMaps
{
    public class KetQuaXetNghiemChiTietMap : CaminoEntityTypeConfiguration<KetQuaXetNghiemChiTiet>
    {
        public override void Configure(EntityTypeBuilder<KetQuaXetNghiemChiTiet> builder)
        {
            builder.ToTable(MappingDefaults.KetQuaXetNghiemChiTietTable);

            builder.HasOne(rf => rf.PhienXetNghiemChiTiet)
                .WithMany(r => r.KetQuaXetNghiemChiTiets)
                .HasForeignKey(rf => rf.PhienXetNghiemChiTietId);

            builder.HasOne(rf => rf.YeuCauDichVuKyThuat)
                .WithMany(r => r.KetQuaXetNghiemChiTiets)
                .HasForeignKey(rf => rf.YeuCauDichVuKyThuatId);

            builder.HasOne(rf => rf.DichVuKyThuatBenhVien)
                .WithMany(r => r.KetQuaXetNghiemChiTiets)
                .HasForeignKey(rf => rf.DichVuKyThuatBenhVienId);

            builder.HasOne(rf => rf.NhomDichVuBenhVien)
                .WithMany(r => r.KetQuaXetNghiemChiTiets)
                .HasForeignKey(rf => rf.NhomDichVuBenhVienId);

            builder.HasOne(rf => rf.DichVuXetNghiem)
                .WithMany(r => r.KetQuaXetNghiemChiTiets)
                .HasForeignKey(rf => rf.DichVuXetNghiemId);

            builder.HasOne(rf => rf.DichVuXetNghiemCha)
                .WithMany(r => r.KetQuaXetNghiemChiTietChas)
                .HasForeignKey(rf => rf.DichVuXetNghiemChaId);

            builder.HasOne(rf => rf.DichVuXetNghiemKetNoiChiSo)
                .WithMany(r => r.KetQuaXetNghiemChiTiets)
                .HasForeignKey(rf => rf.DichVuXetNghiemKetNoiChiSoId);

            builder.HasOne(rf => rf.MauMayXetNghiem)
                .WithMany(r => r.KetQuaXetNghiemChiTiets)
                .HasForeignKey(rf => rf.MauMayXetNghiemId);

            builder.HasOne(rf => rf.MayXetNghiem)
                .WithMany(r => r.KetQuaXetNghiemChiTiets)
                .HasForeignKey(rf => rf.MayXetNghiemId);

            builder.HasOne(rf => rf.NhanVienNhapTay)
                .WithMany(r => r.KetQuaXetNghiemChiTietNhanVienNhapTays)
                .HasForeignKey(rf => rf.NhanVienNhapTayId);

            builder.HasOne(rf => rf.NhanVienDuyet)
                .WithMany(r => r.KetQuaXetNghiemChiTietNhanVienDuyets)
                .HasForeignKey(rf => rf.NhanVienDuyetId);

            base.Configure(builder);
        }
    }
}
