using Camino.Core.Domain.Entities.MauMayXetNghiems;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Camino.Data.Mapping.MauMayXetNghiemMapping
{
    public class MauMayXetNghiemMap : CaminoEntityTypeConfiguration<MauMayXetNghiem>
    {
        public override void Configure(EntityTypeBuilder<MauMayXetNghiem> builder)
        {
            builder.ToTable(MappingDefaults.MauMayXetNghiemTable);
            base.Configure(builder);

            builder.HasOne(m => m.NhomDichVuBenhVien)
                .WithMany(u => u.MauMayXetNghiems)
                .HasForeignKey(m => m.NhomDichVuBenhVienId);
        }
    }
}
