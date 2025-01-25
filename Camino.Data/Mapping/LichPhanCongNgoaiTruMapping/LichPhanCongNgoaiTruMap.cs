using Camino.Core.Domain.Entities.LichPhanCongNgoaiTrus;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Camino.Data.Mapping.LichPhanCongNgoaiTruMapping
{
    public class LichPhanCongNgoaiTruMap
        : CaminoEntityTypeConfiguration<LichPhanCongNgoaiTru>
    {
        public override void Configure(
            EntityTypeBuilder<LichPhanCongNgoaiTru> builder
        )
        {
            builder.ToTable(MappingDefaults.LichPhanCongNgoaiTruTable);

            builder.HasOne(rf => rf.PhongBenhVien)
                .WithMany(r => r.LichPhanCongNgoaiTrus)
                .HasForeignKey(rf => rf.PhongNgoaiTruId);

            builder.HasOne(rf => rf.NhanVien)
                .WithMany(r => r.LichPhanCongNgoaiTrus)
                .HasForeignKey(rf => rf.NhanVienId);

            base.Configure(builder);
        }
    }
}
