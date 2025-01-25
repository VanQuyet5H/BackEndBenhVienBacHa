using System;
using System.Collections.Generic;
using System.Text;
using Camino.Core.Domain.Entities.MauVaChePhams;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Camino.Data.Mapping.MauVaChePham
{
    public class YeuCauTruyenMauMap : CaminoEntityTypeConfiguration<YeuCauTruyenMau>
    {
        public override void Configure(EntityTypeBuilder<YeuCauTruyenMau> builder)
        {
            builder.ToTable(MappingDefaults.YeuCauTruyenMauTable);

            builder.HasOne(rf => rf.YeuCauTiepNhan)
                .WithMany(r => r.YeuCauTruyenMaus)
                .HasForeignKey(rf => rf.YeuCauTiepNhanId);
            builder.HasOne(rf => rf.NoiTruPhieuDieuTri)
                .WithMany(r => r.YeuCauTruyenMaus)
                .HasForeignKey(rf => rf.NoiTruPhieuDieuTriId);
            builder.HasOne(rf => rf.MauVaChePham)
                .WithMany(r => r.YeuCauTruyenMaus)
                .HasForeignKey(rf => rf.MauVaChePhamId);
            builder.HasOne(rf => rf.XuatKhoMauChiTiet)
                .WithMany(r => r.YeuCauTruyenMaus)
                .HasForeignKey(rf => rf.XuatKhoMauChiTietId);
            builder.HasOne(rf => rf.NhanVienDuyetBaoHiem)
                .WithMany(r => r.NhanVienDuyetBaoHiemYeuCauTruyenMaus)
                .HasForeignKey(rf => rf.NhanVienDuyetBaoHiemId);
            builder.HasOne(rf => rf.NhanVienHuyThanhToan)
                .WithMany(r => r.NhanVienHuyThanhToanYeuCauTruyenMaus)
                .HasForeignKey(rf => rf.NhanVienHuyThanhToanId);
            builder.HasOne(rf => rf.NhanVienChiDinh)
                .WithMany(r => r.NhanVienChiDinhYeuCauTruyenMaus)
                .HasForeignKey(rf => rf.NhanVienChiDinhId);
            builder.HasOne(rf => rf.NoiChiDinh)
                .WithMany(r => r.NoiChiDinhYeuCauTruyenMaus)
                .HasForeignKey(rf => rf.NoiChiDinhId);
            builder.HasOne(rf => rf.NoiThucHien)
                .WithMany(r => r.NoiThucHienYeuCauTruyenMaus)
                .HasForeignKey(rf => rf.NoiThucHienId);
            builder.HasOne(rf => rf.NhanVienThucHien)
                .WithMany(r => r.NhanVienThucHienYeuCauTruyenMaus)
                .HasForeignKey(rf => rf.NhanVienThucHienId);
            builder.HasOne(rf => rf.YeuCauTiepNhanTheBHYT)
                .WithMany(r => r.YeuCauTruyenMaus)
                .HasForeignKey(rf => rf.YeuCauTiepNhanTheBHYTId);

            //BVHD-3731
            builder
                .HasOne(sc => sc.NoiDungGhiChuMiemGiam)
                .WithMany(s => s.YeuCauTruyenMaus)
                .HasForeignKey(sc => sc.NoiDungGhiChuMiemGiamId);

            base.Configure(builder);
        }
    }
}
