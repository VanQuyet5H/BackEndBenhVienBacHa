using Camino.Core.Domain.Entities.XuatKhos;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Camino.Data.Mapping.XuatKhos
{
    public class YeuCauXuatKhoDuocPhamMap : CaminoEntityTypeConfiguration<YeuCauXuatKhoDuocPham>
    {
        public override void Configure(EntityTypeBuilder<YeuCauXuatKhoDuocPham> builder)
        {
            builder.ToTable(MappingDefaults.YeuCauXuatKhoDuocPhamTable);

            builder.HasOne(m => m.KhoXuat)
                .WithMany(u => u.YeuCauXuatKhoDuocPhams)
                .HasForeignKey(m => m.KhoXuatId);

            builder.HasOne(m => m.NguoiXuat)
                .WithMany(u => u.YeuCauXuatKhoDuocPhamNguoiXuats)
                .HasForeignKey(m => m.NguoiXuatId);

            builder.HasOne(m => m.NguoiNhan)
                .WithMany(u => u.YeuCauXuatKhoDuocPhamNguoiNhans)
                .HasForeignKey(m => m.NguoiNhanId);

            builder.HasOne(m => m.NhanVienDuyet)
                .WithMany(u => u.YeuCauXuatKhoDuocPhamNhanVienDuyets)
                .HasForeignKey(m => m.NhanVienDuyetId);

            builder.HasOne(m => m.NhaThau)
                .WithMany(u => u.YeuCauXuatKhoDuocPhams)
                .HasForeignKey(m => m.NhaThauId);
            base.Configure(builder);
        }
    }
}
