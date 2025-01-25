using Camino.Core.Domain.Entities.KhoDuocPhams;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;


namespace Camino.Data.Mapping.KhoDuocPhamMapping
{
    public class KhoDuocPhamMap : CaminoEntityTypeConfiguration<Kho>
    {
        public override void Configure(EntityTypeBuilder<Kho> builder)
        {
            builder.ToTable(MappingDefaults.KhoDuocPhamTable);

            builder.HasOne(rf => rf.KhoaPhong)
                .WithMany(r => r.KhoDuocPhams)
                .HasForeignKey(rf => rf.KhoaPhongId);

            builder.HasOne(rf => rf.PhongBenhVien)
                .WithMany(r => r.KhoDuocPhams)
                .HasForeignKey(rf => rf.PhongBenhVienId);

            base.Configure(builder);
        }
    }
}
