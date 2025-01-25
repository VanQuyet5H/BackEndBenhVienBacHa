using Camino.Core.Domain.Entities.ChiSoXetNghiems;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;


namespace Camino.Data.Mapping.ChiSoXetNghiems
{
    public class KetQuaXetNghiemMap : CaminoEntityTypeConfiguration<KetQuaXetNghiem>
    {
        public override void Configure(EntityTypeBuilder<KetQuaXetNghiem> builder)
        {
            builder.ToTable(MappingDefaults.KetQuaXetNghiemTable);

                builder.HasOne(m => m.ChiSoXetNghiem)
                    .WithMany(u => u.KetQuaXetNghiems)
                    .HasForeignKey(m => m.ChiSoXetNghiemId);

                builder.HasOne(m => m.YeuCauDichVuKyThuat)
                    .WithMany(u => u.KetQuaXetNghiems)
                    .HasForeignKey(m => m.YeuCauDichVuKyThuatId);

            base.Configure(builder);
        }

    }
}
