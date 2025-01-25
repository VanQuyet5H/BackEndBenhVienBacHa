using Camino.Core.Domain.Entities.YeuCauKhamBenhs;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Camino.Data.Mapping.YeuCauKhamBenhMapping
{
    public class YeuCauDichVuKyThuatTuongTrinhPTTTMap : CaminoEntityTypeConfiguration<YeuCauDichVuKyThuatTuongTrinhPTTT>
    {
        public override void Configure(EntityTypeBuilder<YeuCauDichVuKyThuatTuongTrinhPTTT> builder)
        {
            builder.ToTable(MappingDefaults.YeuCauDichVuKyThuatTuongTrinhPTTTTable);


            builder.HasOne(rf => rf.YeuCauDichVuKyThuat)
                .WithOne(r => r.YeuCauDichVuKyThuatTuongTrinhPTTT)
                .HasForeignKey<YeuCauDichVuKyThuatTuongTrinhPTTT>(c => c.Id);

            builder.HasOne(rf => rf.ICDTruocPhauThuat)
                .WithMany(r => r.YeuCauDichVuKyThuatTuongTrinhPTTTTruocPhauThuats)
                .HasForeignKey(rf => rf.ICDTruocPhauThuatId);
            builder.HasOne(rf => rf.ICDSauPhauThuat)
                .WithMany(r => r.YeuCauDichVuKyThuatTuongTrinhPTTTSauPhauThuats)
                .HasForeignKey(rf => rf.ICDSauPhauThuatId);
            builder.HasOne(rf => rf.PhuongPhapVoCam)
                .WithMany(r => r.YeuCauDichVuKyThuatTuongTrinhPTTTs)
                .HasForeignKey(rf => rf.PhuongPhapVoCamId);
            builder.HasOne(rf => rf.NhanVienTuongTrinh)
                .WithMany(r => r.YeuCauDichVuKyThuatTuongTrinhPTTTs)
                .HasForeignKey(rf => rf.NhanVienTuongTrinhId);
            base.Configure(builder);
        }
    }
}