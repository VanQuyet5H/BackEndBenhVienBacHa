using Camino.Core.Domain.Entities.PhauThuatThuThuats;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Camino.Data.Mapping.PhauThuatThuThuatEkipDieuDuong
{
    public class PhauThuatThuThuatEkipDieuDuongMap : CaminoEntityTypeConfiguration<Camino.Core.Domain.Entities.PhauThuatThuThuats.PhauThuatThuThuatEkipDieuDuong>
    {
        public override void Configure(EntityTypeBuilder<Core.Domain.Entities.PhauThuatThuThuats.PhauThuatThuThuatEkipDieuDuong> builder)
        {
            builder.ToTable(MappingDefaults.PhauThuatThuThuatEkipDieuDuong);

            builder.HasOne(rf => rf.YeuCauDichVuKyThuatTuongTrinhPTTT)
                .WithMany(r => r.PhauThuatThuThuatEkipDieuDuongs)
                .HasForeignKey(rf => rf.YeuCauDichVuKyThuatTuongTrinhPTTTId);

            builder.HasOne(rf => rf.NhanVien)
                .WithMany(r => r.PhauThuatThuThuatEkipDieuDuongs)
                .HasForeignKey(rf => rf.NhanVienId);

            base.Configure(builder);
        }
    }
}