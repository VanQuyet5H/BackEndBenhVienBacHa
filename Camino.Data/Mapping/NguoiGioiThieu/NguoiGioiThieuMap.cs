using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Camino.Data.Mapping.NguoiGioiThieu
{
    public class NguoiGioiThieuMap : CaminoEntityTypeConfiguration<Core.Domain.Entities.NguoiGioiThieus.NguoiGioiThieu>
    {
        public override void Configure(EntityTypeBuilder<Core.Domain.Entities.NguoiGioiThieus.NguoiGioiThieu> builder)
        {
            builder.ToTable(MappingDefaults.NguoiGioiThieuTable);
            base.Configure(builder);
        }
    }
}
