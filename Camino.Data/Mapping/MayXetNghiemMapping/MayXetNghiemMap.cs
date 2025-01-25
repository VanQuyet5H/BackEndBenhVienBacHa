using Camino.Core.Domain.Entities.MayXetNghiems;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Camino.Data.Mapping.MayXetNghiemMapping
{
    public class MayXetNghiemMap : CaminoEntityTypeConfiguration<MayXetNghiem>
    {
        public override void Configure(EntityTypeBuilder<MayXetNghiem> builder)
        {
            builder.ToTable(MappingDefaults.MayXetNghiemTable);
            base.Configure(builder);

            builder.HasOne(m => m.MauMayXetNghiem)
                .WithMany(u => u.MayXetNghiems)
                .HasForeignKey(m => m.MauMayXetNghiemID);
        }
    }
}
