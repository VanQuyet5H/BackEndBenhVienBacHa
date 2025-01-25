using Camino.Core.Domain.Entities.NoiDungGhiChuMiemGiams;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Camino.Data.Mapping.NoiDungGhiChuMiemGiamMapping
{
    public class NoiDungGhiChuMiemGiamMapping : CaminoEntityTypeConfiguration<NoiDungGhiChuMiemGiam>
    {
        public override void Configure(EntityTypeBuilder<NoiDungGhiChuMiemGiam> builder)
        {
            builder.ToTable(MappingDefaults.NoiDungGhiChuMiemGiamTable);
            base.Configure(builder);
        }
    }
}
