using Camino.Core.Domain.Entities.NhaThaus;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Camino.Data.Mapping.NhaThauMapping
{
    public class NhaThauMap : CaminoEntityTypeConfiguration<NhaThau>
    {
        public override void Configure(EntityTypeBuilder<NhaThau> builder)
        {
            builder.ToTable(MappingDefaults.NhaThauTable);
            base.Configure(builder);
        }
    }
}
