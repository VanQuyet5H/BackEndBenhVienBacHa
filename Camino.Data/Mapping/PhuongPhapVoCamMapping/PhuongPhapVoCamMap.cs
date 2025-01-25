using Camino.Core.Domain.Entities.PhuongPhapVoCams;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Camino.Data.Mapping.PhuongPhapVoCamMapping
{
    public class PhuongPhapVoCamMap : CaminoEntityTypeConfiguration<PhuongPhapVoCam>
    {
        public override void Configure(EntityTypeBuilder<PhuongPhapVoCam> builder)
        {
            builder.ToTable(MappingDefaults.PhuongPhapVoCamTable);
            base.Configure(builder);
        }
    }
}