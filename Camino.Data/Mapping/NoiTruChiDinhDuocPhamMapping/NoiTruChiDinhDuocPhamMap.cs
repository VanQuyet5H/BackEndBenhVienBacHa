
using Camino.Core.Domain.Entities.YeuCauKhamBenhs;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;


namespace Camino.Data.Mapping.NoiTruChiDinhDuocPhamMapping
{
    public class NoiTruChiDinhDuocPhamMap : CaminoEntityTypeConfiguration<NoiTruChiDinhDuocPham>
    {
        public override void Configure(EntityTypeBuilder<NoiTruChiDinhDuocPham> builder)
        {
            builder.ToTable(MappingDefaults.NoiTruChiDinhDuocPhamTable);
            builder.HasOne(rf => rf.YeuCauTiepNhan)
                .WithMany(r => r.NoiTruChiDinhDuocPhams)
                .HasForeignKey(rf => rf.YeuCauTiepNhanId);

            builder.HasOne(rf => rf.DuocPhamBenhVien)
               .WithMany(r => r.NoiTruChiDinhDuocPhams)
               .HasForeignKey(rf => rf.DuocPhamBenhVienId);

            builder.HasOne(rf => rf.NoiTruPhieuDieuTri)
               .WithMany(r => r.NoiTruChiDinhDuocPhams)
               .HasForeignKey(rf => rf.NoiTruPhieuDieuTriId);

            builder.HasOne(rf => rf.DuongDung)
               .WithMany(r => r.NoiTruChiDinhDuocPhams)
               .HasForeignKey(rf => rf.DuongDungId);

            builder.HasOne(rf => rf.DonViTinh)
               .WithMany(r => r.NoiTruChiDinhDuocPhams)
               .HasForeignKey(rf => rf.DonViTinhId);

            builder.HasOne(rf => rf.NhanVienChiDinh)
                           .WithMany(r => r.NoiTruChiDinhDuocPhamChiDinhs)
                           .HasForeignKey(rf => rf.NhanVienChiDinhId);

            builder.HasOne(rf => rf.NhanVienCapThuoc)
                        .WithMany(r => r.NoiTruChiDinhDuocPhamCapThuocs)
                        .HasForeignKey(rf => rf.NhanVienCapThuocId);

            builder.HasOne(rf => rf.NoiChiDinh)
               .WithMany(r => r.NoiTruChiDinhDuocPhamChiDinhs)
               .HasForeignKey(rf => rf.NoiChiDinhId);

            builder.HasOne(rf => rf.NoiCapThuoc)
             .WithMany(r => r.NoiTruChiDinhDuocPhamCapThuocs)
             .HasForeignKey(rf => rf.NoiCapThuocId);

            builder.HasOne(rf => rf.NoiTruChiDinhPhaThuocTiem)
               .WithMany(r => r.NoiTruChiDinhDuocPhams)
               .HasForeignKey(rf => rf.NoiTruChiDinhPhaThuocTiemId);

            builder.HasOne(rf => rf.NoiTruChiDinhPhaThuocTruyen)
               .WithMany(r => r.NoiTruChiDinhDuocPhams)
               .HasForeignKey(rf => rf.NoiTruChiDinhPhaThuocTruyenId);
            base.Configure(builder);
        }
    }
}
