using Camino.Core.Domain.Entities.HopDongThauDuocPhams;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Camino.Data.Mapping.HopDongThauDuocPhamMapping
{
    public class HopDongThauDuocPhamMap
        : CaminoEntityTypeConfiguration<HopDongThauDuocPham>
    {
        public override void Configure(
               EntityTypeBuilder<HopDongThauDuocPham> builder
               )
        {
            builder.ToTable(MappingDefaults.HopDongThauDuocPhamTable);

            builder.HasOne(rf => rf.NhaThau)
               .WithMany(r => r.HopDongThauDuocPhams)
               .HasForeignKey(rf => rf.NhaThauId);

            base.Configure(builder);
        }
    }
}
