using Camino.Core.Domain.Entities.YeuCauTiepNhans;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Camino.Data.Mapping.YeuCauTiepNhanMapping
{
    public class LoaiHoSoYeuCauTiepNhanMap : CaminoEntityTypeConfiguration<LoaiHoSoYeuCauTiepNhan>
    {
        public override void Configure(EntityTypeBuilder<LoaiHoSoYeuCauTiepNhan> builder)
        {
            builder.ToTable(MappingDefaults.LoaiHoSoYeuCauTiepNhanTable);

            base.Configure(builder);
        }
    }
}