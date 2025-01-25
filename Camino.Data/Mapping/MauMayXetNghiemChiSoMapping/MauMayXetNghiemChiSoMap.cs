using Camino.Core.Domain.Entities.MauMayXetNghiemChiSos;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Camino.Data.Mapping.MauMayXetNghiemChiSoMapping
{
    public class MauMayXetNghiemChiSoMap : CaminoEntityTypeConfiguration<MauMayXetNghiemChiSo>
    {
        public override void Configure(EntityTypeBuilder<MauMayXetNghiemChiSo> builder)
        {
            builder.ToTable(MappingDefaults.MauMayXetNghiemChiSoTable);
            base.Configure(builder);

            builder.HasOne(m => m.MauMayXetNghiem)
                .WithMany(u => u.MauMayXetNghiemChiSos)
                .HasForeignKey(m => m.MauMayXetNghiemId);
        }
    }
}
