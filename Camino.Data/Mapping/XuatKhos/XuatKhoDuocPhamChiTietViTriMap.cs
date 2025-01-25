using Camino.Core.Domain.Entities.XuatKhos;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Camino.Data.Mapping.XuatKhos
{
    public class XuatKhoDuocPhamChiTietViTriMap : CaminoEntityTypeConfiguration<XuatKhoDuocPhamChiTietViTri>
    {
        public override void Configure(EntityTypeBuilder<XuatKhoDuocPhamChiTietViTri> builder)
        {
            builder.ToTable(MappingDefaults.XuatKhoDuocPhamChiTietViTriTable);

            builder.HasOne(m => m.XuatKhoDuocPhamChiTiet)
                .WithMany(u => u.XuatKhoDuocPhamChiTietViTris)
                .HasForeignKey(m => m.XuatKhoDuocPhamChiTietId)
                .IsRequired();
            builder.HasOne(m => m.NhapKhoDuocPhamChiTiet)
                .WithMany(u => u.XuatKhoDuocPhamChiTietViTris)
                .HasForeignKey(m => m.NhapKhoDuocPhamChiTietId)
                .IsRequired();

            builder.HasOne(m => m.YeuCauTraDuocPhamTuBenhNhanChiTiet)
                .WithMany(u => u.XuatKhoDuocPhamChiTietViTris)
                .HasForeignKey(m => m.YeuCauTraDuocPhamTuBenhNhanChiTietId);

            builder.HasOne(m => m.MayXetNghiem)
                .WithMany(u => u.XuatKhoDuocPhamChiTietViTris)
                .HasForeignKey(m => m.MayXetNghiemId);

            base.Configure(builder);
        }
    }
}