using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Camino.Data.Mapping.DichVuGiuongBenhVien
{
    public class DichVuGiuongBenhVienMap
        : CaminoEntityTypeConfiguration<Core.Domain.Entities.DichVuGiuongBenhViens
            .DichVuGiuongBenhVien>
    {
        public override void Configure(
            EntityTypeBuilder<Core.Domain.Entities.DichVuGiuongBenhViens
                .DichVuGiuongBenhVien> builder
        )
        {
            builder.ToTable(MappingDefaults.DichVuGiuongBenhVienTable);

            builder.HasOne(rf => rf.DichVuGiuong)
                .WithMany(r => r.DichVuGiuongBenhViens)
                .HasForeignKey(rf => rf.DichVuGiuongId);
            base.Configure(builder);
        }
    }
}
