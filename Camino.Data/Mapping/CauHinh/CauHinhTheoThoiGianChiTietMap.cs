using Camino.Core.Domain.Entities.CauHinhs;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Camino.Data.Mapping.CauHinh
{
    public class CauHinhTheoThoiGianChiTietMap : CaminoEntityTypeConfiguration<CauHinhTheoThoiGianChiTiet>
    {
        public override void Configure(EntityTypeBuilder<CauHinhTheoThoiGianChiTiet> builder)
        {
            builder.ToTable(MappingDefaults.CauHinhTheoThoiGianChiTietTable);
            builder.HasOne(m => m.CauHinhTheoThoiGian)
                .WithMany(u => u.CauHinhTheoThoiGianChiTiets)
                .HasForeignKey(m => m.CauHinhTheoThoiGianId)
                .IsRequired();

            base.Configure(builder);
        }
    }
}
