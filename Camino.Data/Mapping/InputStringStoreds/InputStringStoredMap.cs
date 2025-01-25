using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Camino.Data.Mapping.InputStringStoreds
{
    public class InputStringStoredMap : CaminoEntityTypeConfiguration<Core.Domain.Entities.InputStringStoreds.InputStringStored>
    {
        public override void Configure(EntityTypeBuilder<Core.Domain.Entities.InputStringStoreds.InputStringStored> builder)
        {
            builder.ToTable(MappingDefaults.InputStringStoredTable);
            base.Configure(builder);
        }
    }
}
