using Camino.Core.Domain.Entities.CauHinhs;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Camino.Data.Mapping.CauHinh
{
    public class CauHinhTheoThoiGianMap : CaminoEntityTypeConfiguration<CauHinhTheoThoiGian>
    {
        public override void Configure(EntityTypeBuilder<CauHinhTheoThoiGian> builder)
        {
            builder.ToTable(MappingDefaults.CauHinhTheoThoiGianTable);
            base.Configure(builder);
        }
    }
}
