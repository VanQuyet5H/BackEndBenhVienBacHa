using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Camino.Data.Mapping.KhamTheoDoi
{
    public class KhamTheoDoiMap : CaminoEntityTypeConfiguration<Camino.Core.Domain.Entities.PhauThuatThuThuats.KhamTheoDoi>
    {
        public override void Configure(EntityTypeBuilder<Core.Domain.Entities.PhauThuatThuThuats.KhamTheoDoi> builder)
        {
            builder.ToTable(MappingDefaults.KhamTheoDoi);

            builder.HasOne(rf => rf.TheoDoiSauPhauThuatThuThuat)
                .WithMany(r => r.KhamTheoDois)
                .HasForeignKey(rf => rf.TheoDoiSauPhauThuatThuThuatId);

            builder.HasOne(rf => rf.NoiThucHien)
                .WithMany(r => r.KhamTheoDois)
                .HasForeignKey(rf => rf.NoiThucHienId);

            builder.HasOne(rf => rf.NhanVienThucHien)
                .WithMany(r => r.KhamTheoDois)
                .HasForeignKey(rf => rf.NhanVienThucHienId);

            base.Configure(builder);
        }
    }
}
