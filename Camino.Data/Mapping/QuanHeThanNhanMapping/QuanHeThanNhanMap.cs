using Camino.Core.Domain.Entities.QuanHeThanNhans;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Camino.Data.Mapping.QuanHeThanNhanMapping
{
    public class QuanHeThanNhanMap : CaminoEntityTypeConfiguration<QuanHeThanNhan>
    {
        public override void Configure(EntityTypeBuilder<QuanHeThanNhan> builder)
        {
            builder.ToTable(MappingDefaults.QuanHeThanNhanTable);
            base.Configure(builder);
        }
    }
}
