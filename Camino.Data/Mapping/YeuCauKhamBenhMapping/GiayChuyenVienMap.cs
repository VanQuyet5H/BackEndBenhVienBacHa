using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Camino.Data.Mapping.YeuCauKhamBenhMapping
{
    public class GiayChuyenVienMap : CaminoEntityTypeConfiguration<Core.Domain.Entities.YeuCauKhamBenhs.GiayChuyenVien>
    {
        public override void Configure(EntityTypeBuilder<Core.Domain.Entities.YeuCauKhamBenhs.GiayChuyenVien> builder)
        {
            builder.ToTable(MappingDefaults.GiayChuyenVienTable);

            //builder.HasOne(m => m.YeuCauKhamBenh)
            //    .WithOne(u => u.GiayChuyenVien)
            //    .HasForeignKey<Core.Domain.Entities.YeuCauKhamBenhs.YeuCauKhamBenh>(m => m.GiayChuyenVienId);

            //builder.Property(u => u.Ten).HasMaxLength(20);
            //builder.Property(u => u.TenVietTat).HasMaxLength(200);
            base.Configure(builder);
        }
    }
}