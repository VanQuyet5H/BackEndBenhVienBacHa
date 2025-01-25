using Camino.Core.Domain.Entities.PhauThuatThuThuats;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Camino.Data.Mapping.KhamTheoDoiBoPhankhac
{
    public class KhamTheoDoiBoPhanKhacMap : CaminoEntityTypeConfiguration<KhamTheoDoiBoPhanKhac>
    {
        public override void Configure(EntityTypeBuilder<KhamTheoDoiBoPhanKhac> builder)
        {
            builder.ToTable(MappingDefaults.KhamTheoDoiBoPhanKhac);

            builder.HasOne(rf => rf.KhamTheoDoi)
                .WithMany(r => r.KhamTheoDoiBoPhanKhacs)
                .HasForeignKey(rf => rf.KhamTheoDoiId);

            base.Configure(builder);
        }
    }
}
