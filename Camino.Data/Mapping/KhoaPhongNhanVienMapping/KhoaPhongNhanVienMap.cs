using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Camino.Data.Mapping.KhoaPhongNhanVienMapping
{
    public class KhoaPhongNhanVienMap
        : CaminoEntityTypeConfiguration<Core.Domain.Entities.KhoaPhongNhanViens
            .KhoaPhongNhanVien>
    {
        public override void Configure(
            EntityTypeBuilder<Core.Domain.Entities.KhoaPhongNhanViens
                .KhoaPhongNhanVien> builder
        )
        {
            builder.ToTable(MappingDefaults.KhoaPhongNhanVienTable);

            builder.HasOne(rf => rf.PhongBenhVien)
               .WithMany(r => r.KhoaPhongNhanViens)
               .HasForeignKey(rf => rf.PhongBenhVienId);

            builder.HasOne(rf => rf.KhoaPhong)
                .WithMany(r => r.KhoaPhongNhanViens)
                .HasForeignKey(rf => rf.KhoaPhongId)
                .IsRequired();

            builder.HasOne(rf => rf.NhanVien)
                .WithMany(r => r.KhoaPhongNhanViens)
                .HasForeignKey(rf => rf.NhanVienId)
                .IsRequired();

            base.Configure(builder);
        }
    }
}
