using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Camino.Data.Mapping.HocViHocHamMapping
{
    public class HocViHocHamMap : CaminoEntityTypeConfiguration<Core.Domain.Entities.HocViHocHams.HocViHocHam>
    {
        public override void Configure(EntityTypeBuilder<Core.Domain.Entities.HocViHocHams.HocViHocHam> builder)
        {
            builder.ToTable(MappingDefaults.HocViHocHamTable);
            base.Configure(builder);
        }
    }
}
