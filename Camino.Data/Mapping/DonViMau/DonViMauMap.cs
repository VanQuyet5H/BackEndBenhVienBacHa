using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
namespace Camino.Data.Mapping.DonViMau
{
    public class DonViMauMap : CaminoEntityTypeConfiguration<Core.Domain.Entities.DonViMaus.DonViMau>

    {
        public override void Configure(EntityTypeBuilder<Core.Domain.Entities.DonViMaus.DonViMau> builder)
        {
            builder.ToTable(MappingDefaults.DonViMauTable);

            base.Configure(builder);
        }
    }
}
