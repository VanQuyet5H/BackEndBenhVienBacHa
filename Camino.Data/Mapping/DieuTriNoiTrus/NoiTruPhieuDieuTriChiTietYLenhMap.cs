using System;
using System.Collections.Generic;
using System.Text;
using Camino.Core.Domain.Entities.DieuTriNoiTrus;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Camino.Data.Mapping.DieuTriNoiTrus
{
    public class NoiTruPhieuDieuTriChiTietYLenhMap : CaminoEntityTypeConfiguration<NoiTruPhieuDieuTriChiTietYLenh>
    {
        public override void Configure(EntityTypeBuilder<NoiTruPhieuDieuTriChiTietYLenh> builder)
        {
            builder.ToTable(MappingDefaults.NoiTruPhieuDieuTriChiTietYLenhTable);

            //builder.HasOne(rf => rf.NoiTruPhieuDieuTri)
            //    .WithMany(r => r.NoiTruPhieuDieuTriChiTietYLenhs)
            //    .HasForeignKey(rf => rf.NoiTruPhieuDieuTriId);

            builder.HasOne(rf => rf.YeuCauDichVuKyThuat)
                .WithMany(r => r.NoiTruPhieuDieuTriChiTietYLenhs)
                .HasForeignKey(rf => rf.YeuCauDichVuKyThuatId);

            builder.HasOne(rf => rf.NoiTruChiDinhDuocPham)
                .WithMany(r => r.NoiTruPhieuDieuTriChiTietYLenhs)
                .HasForeignKey(rf => rf.NoiTruChiDinhDuocPhamId);

            builder.HasOne(rf => rf.YeuCauVatTuBenhVien)
                .WithMany(r => r.NoiTruPhieuDieuTriChiTietYLenhs)
                .HasForeignKey(rf => rf.YeuCauVatTuBenhVienId);

            builder.HasOne(rf => rf.NhanVienXacNhanThucHien)
                .WithMany(r => r.NhanVienXacNhanThucHienNoiTruPhieuDieuTriChiTietYLenhs)
                .HasForeignKey(rf => rf.NhanVienXacNhanThucHienId);

            builder.HasOne(rf => rf.YeuCauTruyenMau)
                .WithMany(r => r.NoiTruPhieuDieuTriChiTietYLenhs)
                .HasForeignKey(rf => rf.YeuCauTruyenMauId);

            builder.HasOne(rf => rf.NhanVienChiDinh)
                .WithMany(r => r.NhanVienChiDinhNoiTruPhieuDieuTriChiTietYLenhs)
                .HasForeignKey(rf => rf.NhanVienChiDinhId);

            builder.HasOne(rf => rf.NoiChiDinh)
                .WithMany(r => r.NoiTruPhieuDieuTriChiTietYLenhs)
                .HasForeignKey(rf => rf.NoiChiDinhId);

            builder.HasOne(rf => rf.NhanVienCapNhat)
                .WithMany(r => r.NhanVienCapNhatNoiTruPhieuDieuTriChiTietYLenhs)
                .HasForeignKey(rf => rf.NhanVienCapNhatId);

            #region //BVHD-3312
            builder.HasOne(rf => rf.NoiTruBenhAn)
                .WithMany(r => r.NoiTruPhieuDieuTriChiTietYLenhs)
                .HasForeignKey(rf => rf.NoiTruBenhAnId);


            #endregion

            base.Configure(builder);
        }
    }
}
