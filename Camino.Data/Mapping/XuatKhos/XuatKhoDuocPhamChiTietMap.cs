using Camino.Core.Domain.Entities.XuatKhos;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Camino.Data.Mapping.XuatKhos
{
    public class XuatKhoDuocPhamChiTietMap : CaminoEntityTypeConfiguration<XuatKhoDuocPhamChiTiet>
    {
        public override void Configure(EntityTypeBuilder<XuatKhoDuocPhamChiTiet> builder)
        {
            builder.ToTable(MappingDefaults.XuatKhoDuocPhamChiTietTable);

            builder.HasOne(m => m.DuocPhamBenhVien)
                .WithMany(u => u.XuatKhoDuocPhamChiTiets)
                .HasForeignKey(m => m.DuocPhamBenhVienId)
                .IsRequired();

            builder.HasOne(m => m.XuatKhoDuocPham)
                .WithMany(u => u.XuatKhoDuocPhamChiTiets)
                .HasForeignKey(m => m.XuatKhoDuocPhamId);

            base.Configure(builder);
        }
    }
}