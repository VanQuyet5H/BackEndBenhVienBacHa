using Camino.Core.Domain.Entities.ChuanDoanHinhAnhs;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Camino.Data.Mapping.ChuanDoanHinhAnhMapping
{
    public class KetQuaChuanDoanHinhAnhMap : CaminoEntityTypeConfiguration<KetQuaChuanDoanHinhAnh>
    {
        public override void Configure(EntityTypeBuilder<KetQuaChuanDoanHinhAnh> builder)
        {
            builder.ToTable(MappingDefaults.KetQuaChuanDoanHinhAnhTable);

            builder.HasOne(m => m.ChuanDoanHinhAnh)
                .WithMany(u => u.KetQuaChuanDoanHinhAnhs)
                .HasForeignKey(m => m.ChuanDoanHinhAnhId);

            builder.HasOne(m => m.YeuCauDichVuKyThuat)
                .WithMany(u => u.KetQuaChuanDoanHinhAnhs)
                .HasForeignKey(m => m.YeuCauDichVuKyThuatId);

            base.Configure(builder);
        }

    }
}
