using Camino.Core.Domain.Entities.YeuCauDieuChuyenDuocPhams;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Camino.Data.Mapping.YeuCauDieuChuyenDuocPhamMapping
{
    public class YeuCauDieuChuyenDuocPhamMap : CaminoEntityTypeConfiguration<YeuCauDieuChuyenDuocPham>
    {
        public override void Configure(EntityTypeBuilder<YeuCauDieuChuyenDuocPham> builder)
        {
            builder.ToTable(MappingDefaults.YeuCauDieuChuyenDuocPhamTable);

            builder.HasOne(m => m.KhoNhap)
                .WithMany(u => u.YeuCauDieuChuyenDuocPhamNhaps)
                .HasForeignKey(m => m.KhoNhapId);

            builder.HasOne(m => m.KhoXuat)
                .WithMany(u => u.YeuCauDieuChuyenDuocPhamXuats)
                .HasForeignKey(m => m.KhoXuatId);

            builder.HasOne(m => m.NguoiXuat)
             .WithMany(u => u.YeuCauDieuChuyenDuocPhamNguoiXuats)
             .HasForeignKey(m => m.NguoiXuatId);

            builder.HasOne(m => m.NguoiNhap)
                .WithMany(u => u.YeuCauDieuChuyenDuocPhamNguoiNhaps)
                .HasForeignKey(m => m.NguoiNhapId);

            builder.HasOne(m => m.NhanVienDuyet)
                .WithMany(u => u.YeuCauDieuChuyenDuocPhamNguoiNhanVienDuyets)
                .HasForeignKey(m => m.NhanVienDuyetId);

            base.Configure(builder);
        }
    }
}
