using Camino.Core.Domain.Entities.ChiSoXetNghiems;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Camino.Data.Mapping.ChiSoXetNghiems
{
    public class ChiSoXetNghiemMap : CaminoEntityTypeConfiguration<ChiSoXetNghiem>
    {
        public override void Configure(EntityTypeBuilder<ChiSoXetNghiem> builder)
        {
            builder.ToTable(MappingDefaults.ChiSoXetNghiemTable);
            base.Configure(builder);
        }
    }
}
