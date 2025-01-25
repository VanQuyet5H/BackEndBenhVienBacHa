using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Camino.Data.Mapping.KhoaPhongMapping
{
    public class KhoaPhongMap
        : CaminoEntityTypeConfiguration<Core.Domain.Entities.KhoaPhongs.KhoaPhong>
    {
        public override void Configure(EntityTypeBuilder<Core.Domain.Entities.KhoaPhongs.KhoaPhong> builder)
        {
            builder.ToTable(MappingDefaults.KhoaPhongTable);
            base.Configure(builder);
        }
    }
}
