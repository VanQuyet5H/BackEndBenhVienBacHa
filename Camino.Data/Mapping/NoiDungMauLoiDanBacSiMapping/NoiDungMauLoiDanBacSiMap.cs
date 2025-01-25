using Camino.Core.Domain.Entities.NoiDungMauLoiDanBacSi;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Camino.Data.Mapping.NoiDungMauLoiDanBacSiMapping
{
    public class NoiDungMauLoiDanBacSiMap : CaminoEntityTypeConfiguration<NoiDungMauLoiDanBacSi>
    {
        public override void Configure(EntityTypeBuilder<NoiDungMauLoiDanBacSi> builder)
        {
            builder.ToTable(MappingDefaults.NoiDungMauLoiDanBacSiTable);

            base.Configure(builder);
        }
    }
}
