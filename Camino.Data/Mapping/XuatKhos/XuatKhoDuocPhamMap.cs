using Camino.Core.Domain.Entities.XuatKhos;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Camino.Data.Mapping.XuatKhos
{
    public class XuatKhoDuocPhamMap : CaminoEntityTypeConfiguration<XuatKhoDuocPham>
    {
        public override void Configure(EntityTypeBuilder<XuatKhoDuocPham> builder)
        {
            builder.ToTable(MappingDefaults.XuatKhoDuocPhamTable);
            builder.HasOne(m => m.KhoDuocPhamNhap)
                .WithMany(u => u.XuatKhoDuocPhamNhaps)
                .HasForeignKey(m => m.KhoNhapId);

            builder.HasOne(m => m.KhoDuocPhamXuat)
                .WithMany(u => u.XuatKhoDuocPhamXuats)
                .HasForeignKey(m => m.KhoXuatId)
                .IsRequired();

            builder.HasOne(m => m.NguoiXuat)
                .WithMany(u => u.XuatKhoDuocPhamNguoiXuats)
                .HasForeignKey(m => m.NguoiXuatId)
                .IsRequired();

            builder.HasOne(m => m.NguoiNhan)
                .WithMany(u => u.XuatKhoDuocPhamNguoiNhans)
                .HasForeignKey(m => m.NguoiNhanId);

            builder.HasOne(m => m.NhaThau)
             .WithMany(u => u.XuatKhoDuocPhams)
             .HasForeignKey(m => m.NhaThauId);
            base.Configure(builder);
        }
    }
}