using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Camino.Core.Domain.Entities.PhauThuatThuThuats;

namespace Camino.Data.Mapping.TheoDoiSauPhauThuatThuThuats
{
    public class TheoDoiSauPhauThuatThuThuatMap : CaminoEntityTypeConfiguration<TheoDoiSauPhauThuatThuThuat>
    {
        public override void Configure(EntityTypeBuilder<TheoDoiSauPhauThuatThuThuat> builder)
        {
            builder.ToTable(MappingDefaults.TheoDoiSauPhauThuatThuThuatTable);

            builder.HasOne(rf => rf.YeuCauTiepNhan)
                .WithMany(r => r.TheoDoiSauPhauThuatThuThuats)
                .HasForeignKey(rf => rf.YeuCauTiepNhanId);

            builder.HasOne(rf => rf.BacSiPhuTrachTheoDoi)
                .WithMany(r => r.TheoDoiSauPhauThuatThuThuatBacSiPhuTrachs)
                .HasForeignKey(rf => rf.BacSiPhuTrachTheoDoiId);

            builder.HasOne(rf => rf.DieuDuongPhuTrachTheoDoi)
                .WithMany(r => r.TheoDoiSauPhauThuatThuThuatDieuDuongPhuTrachs)
                .HasForeignKey(rf => rf.DieuDuongPhuTrachTheoDoiId);

            base.Configure(builder);
        }
    }
}