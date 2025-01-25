
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Camino.Data.Mapping.NoiGioiThieu
{
    public class NoiGioiThieuMap : CaminoEntityTypeConfiguration<Core.Domain.Entities.NoiGioiThieu.NoiGioiThieu>
    {
        public override void Configure(EntityTypeBuilder<Core.Domain.Entities.NoiGioiThieu.NoiGioiThieu> builder)
        {
            builder.ToTable(MappingDefaults.NoiGioiThieuTable);

            builder
            .HasOne(sc => sc.DonViMau)
            .WithMany(s => s.NoiGioiThieus)
            .HasForeignKey(sc => sc.DonViMauId);

            base.Configure(builder);
        }
    }
}