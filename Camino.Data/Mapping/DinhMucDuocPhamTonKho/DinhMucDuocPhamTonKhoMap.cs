using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Camino.Data.Mapping.DinhMucDuocPhamTonKho
{
    public class DinhMucDuocPhamTonKhoMap : CaminoEntityTypeConfiguration<Core.Domain.Entities.DinhMucDuocPhamTonKhos.DinhMucDuocPhamTonKho>
    {
        public override void Configure(EntityTypeBuilder<Core.Domain.Entities.DinhMucDuocPhamTonKhos.DinhMucDuocPhamTonKho> builder)
        {
            builder.ToTable(MappingDefaults.DinhMucDuocPhamTonKhoTable);

            builder.HasOne(rf => rf.DuocPhamBenhVien)
                .WithMany(r => r.DinhMucDuocPhamTonKhos)
                .HasForeignKey(rf => rf.DuocPhamBenhVienId);

            builder.HasOne(rf => rf.KhoDuocPham)
                .WithMany(r => r.DinhMucDuocPhamTonKhos)
                .HasForeignKey(rf => rf.KhoId);

            base.Configure(builder);
        }
    }
}
