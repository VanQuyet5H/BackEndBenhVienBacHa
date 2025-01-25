using Camino.Core.Domain.Entities.BHYT;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Camino.Data.Mapping.BHYT
{
    public class DuLieuGuiCongBHYTMap : CaminoEntityTypeConfiguration<DuLieuGuiCongBHYT>
    {
        public override void Configure(EntityTypeBuilder<DuLieuGuiCongBHYT> builder)
        {
            builder.ToTable(MappingDefaults.DuLieuGuiCongBHYTTable);

            base.Configure(builder);
        }
    }
}