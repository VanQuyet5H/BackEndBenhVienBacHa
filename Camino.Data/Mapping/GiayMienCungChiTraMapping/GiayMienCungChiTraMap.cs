using Camino.Core.Domain.Entities.GiayMienCungChiTras;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Camino.Data.Mapping.GiayMienCungChiTraMapping
{
  
    public class GiayMienCungChiTraMap : CaminoEntityTypeConfiguration<GiayMienCungChiTra>
    {
        public override void Configure(EntityTypeBuilder<GiayMienCungChiTra> builder)
        {
            builder.ToTable(MappingDefaults.GiayMienCungChiTraTable);
            base.Configure(builder);
        }
    }
}
