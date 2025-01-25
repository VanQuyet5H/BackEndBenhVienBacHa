using Camino.Core.Domain.Entities.HopDongThauDuocPhams;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Camino.Data.Mapping.HopDongThauDuocPhamMapping
{
    public class HopDongThauDuocPhamChiTietMap
        : CaminoEntityTypeConfiguration<HopDongThauDuocPhamChiTiet>
    {
        public override void Configure(
               EntityTypeBuilder<HopDongThauDuocPhamChiTiet> builder
               )
        {
            builder.ToTable(MappingDefaults.HopDongThauDuocPhamChiTietTable);

            builder.HasOne(rf => rf.HopDongThauDuocPham)
               .WithMany(r => r.HopDongThauDuocPhamChiTiets)
               .HasForeignKey(rf => rf.HopDongThauDuocPhamId);

            builder.HasOne(rf => rf.DuocPham)
               .WithMany(r => r.HopDongThauDuocPhamChiTiets)
               .HasForeignKey(rf => rf.DuocPhamId);

            base.Configure(builder);
        }
    }
}
