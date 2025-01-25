using Camino.Core.Domain.Entities.DichVuKyThuats;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Camino.Data.Mapping.DichVuKyThuat
{
    public class DichVuKyThuatBenhVienMap : CaminoEntityTypeConfiguration<DichVuKyThuatBenhVien>
    {
        public override void Configure(EntityTypeBuilder<DichVuKyThuatBenhVien> builder)
        {
            builder.ToTable(MappingDefaults.DichVuKyThuatBenhVienTable);

            builder.HasOne(rf => rf.DichVuKyThuat)
                .WithMany(r => r.DichVuKyThuatBenhViens)
                .HasForeignKey(rf => rf.DichVuKyThuatId);

            builder.HasOne(rf => rf.NhomDichVuBenhVien)
                .WithMany(r => r.DichVuKyThuatBenhViens)
                .HasForeignKey(rf => rf.NhomDichVuBenhVienId);

            builder.HasOne(rf => rf.DichVuXetNghiem)
               .WithMany(r => r.DichVuKyThuatBenhViens)
               .HasForeignKey(rf => rf.DichVuXetNghiemId);

            builder.HasMany(rf => rf.DichVuKyThuatBenhVienDinhMucDuocPhamVatTus)
               .WithOne(r => r.DichVuKyThuatBenhVien)
               .HasForeignKey(rf => rf.DichVuKyThuatBenhVienId);

            builder.HasOne(rf => rf.KhoaPhongTinhDoanhThu)
                .WithMany(r => r.DichVuKyThuatBenhViens)
                .HasForeignKey(rf => rf.KhoaPhongTinhDoanhThuId);

            base.Configure(builder);
        }
    }
}