using DotLiquid.Util;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Camino.Data.Mapping.PhauThuatThuThuatEkipBacSi
{
    public class PhauThuatThuThuatEkipBacSiMap : CaminoEntityTypeConfiguration<Camino.Core.Domain.Entities.PhauThuatThuThuats.PhauThuatThuThuatEkipBacSi>
    {
        public override void Configure(EntityTypeBuilder<Core.Domain.Entities.PhauThuatThuThuats.PhauThuatThuThuatEkipBacSi> builder)
        {
            builder.ToTable(MappingDefaults.PhauThuatThuThuatEkipBacSi);

            builder.HasOne(rf => rf.YeuCauDichVuKyThuatTuongTrinhPTTT)
                .WithMany(r => r.PhauThuatThuThuatEkipBacSis)
                .HasForeignKey(rf => rf.YeuCauDichVuKyThuatTuongTrinhPTTTId);

            builder.HasOne(rf => rf.NhanVien)
                .WithMany(r => r.PhauThuatThuThuatEkipBacSis)
                .HasForeignKey(rf => rf.NhanVienId);

            base.Configure(builder);
        }
    }
}