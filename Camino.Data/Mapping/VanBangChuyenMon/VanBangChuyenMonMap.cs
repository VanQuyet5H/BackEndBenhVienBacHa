using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;


namespace Camino.Data.Mapping.VanBangChuyenMon
{
    public class VanBangChuyenMonMap : CaminoEntityTypeConfiguration<Core.Domain.Entities.VanBangChuyenMons.VanBangChuyenMon>
    {
        public override void Configure(EntityTypeBuilder<Core.Domain.Entities.VanBangChuyenMons.VanBangChuyenMon> builder)
        {
            builder.ToTable(MappingDefaults.VanBangChuyenMonTable);
            base.Configure(builder);
        }
    }
}
