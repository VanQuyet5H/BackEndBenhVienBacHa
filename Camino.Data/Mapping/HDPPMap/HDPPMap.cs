using Camino.Core.Domain.Entities.HDPP;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Camino.Data.Mapping.HinhThucDenMap
{
    public class HDPPMap : CaminoEntityTypeConfiguration<HDPP>
    {
        public override void Configure(EntityTypeBuilder<HDPP> builder)
        {
            builder.ToTable(MappingDefaults.HDPPTable);
            base.Configure(builder);
        }
    }
}