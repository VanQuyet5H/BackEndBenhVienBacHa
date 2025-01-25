using Camino.Core.Domain.Entities.YeuCauKhamBenhs;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Camino.Data.Mapping.YeuCauKhamBenhMapping
{
    public class YeuCauDichVuKyThuatLuocDoPhauThuatMap : CaminoEntityTypeConfiguration<YeuCauDichVuKyThuatLuocDoPhauThuat>
    {
        public override void Configure(EntityTypeBuilder<YeuCauDichVuKyThuatLuocDoPhauThuat> builder)
        {
            builder.ToTable(MappingDefaults.YeuCauDichVuKyThuatLuocDoPhauThuatTable);

            builder.HasOne(rf => rf.YeuCauDichVuKyThuatTuongTrinhPTTT)
                .WithMany(r => r.YeuCauDichVuKyThuatLuocDoPhauThuats)
                .HasForeignKey(rf => rf.YeuCauDichVuKyThuatTuongTrinhPTTTId);

            base.Configure(builder);
        }
    }
}
