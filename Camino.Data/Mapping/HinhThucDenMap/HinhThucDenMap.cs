using Camino.Core.Domain.Entities.HinhThucDens;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Camino.Data.Mapping.HinhThucDenMap
{
    public class HinhThucDenMap : CaminoEntityTypeConfiguration<HinhThucDen>
    {
        public override void Configure(EntityTypeBuilder<HinhThucDen> builder)
        {
            builder.ToTable(MappingDefaults.HinhThucDenTable);
            base.Configure(builder);
        }
    }
}