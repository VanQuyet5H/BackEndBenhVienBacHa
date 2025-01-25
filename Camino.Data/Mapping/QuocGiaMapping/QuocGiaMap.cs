using Camino.Core.Domain.Entities.QuocGias;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Camino.Data.Mapping.QuocGiaMapping
{
    public class QuocGiaMap : CaminoEntityTypeConfiguration<QuocGia>
    {
        public override void Configure(EntityTypeBuilder<QuocGia> builder)
        {
            builder.ToTable(MappingDefaults.QuocGiaTable);
            base.Configure(builder);
        }
    }
}
