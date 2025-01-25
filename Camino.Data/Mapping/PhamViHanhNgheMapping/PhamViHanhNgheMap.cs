using Camino.Core.Domain.Entities.PhamViHanhNghes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Camino.Data.Mapping.PhamViHanhNgheMapping
{
    public class PhamViHanhNgheMap : CaminoEntityTypeConfiguration<PhamViHanhNghe>
    {
        public override void Configure(EntityTypeBuilder<PhamViHanhNghe> builder)
        {
            builder.ToTable(MappingDefaults.PhamViHanhNgheTable);
            base.Configure(builder);
        }
    }
}
