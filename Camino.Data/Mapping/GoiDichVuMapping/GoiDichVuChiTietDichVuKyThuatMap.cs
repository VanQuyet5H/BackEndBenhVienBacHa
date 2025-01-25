using Camino.Core.Domain.Entities.GoiDichVus;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Camino.Data.Mapping.GoiDichVuMapping
{
    public class GoiDichVuChiTietDichVuKyThuatMap : CaminoEntityTypeConfiguration<GoiDichVuChiTietDichVuKyThuat>
    {
        public override void Configure(EntityTypeBuilder<GoiDichVuChiTietDichVuKyThuat> builder)
        {
            builder.ToTable(MappingDefaults.GoiDichVuChiTietDichVuKyThuatTable);

            builder.HasOne(rf => rf.DichVuKyThuatBenhVien)
                      .WithMany(r => r.GoiDichVuChiTietDichVuKyThuats)
                      .HasForeignKey(rf => rf.DichVuKyThuatBenhVienId);

            builder.HasOne(rf => rf.NhomGiaDichVuKyThuatBenhVien)
                .WithMany(r => r.GoiDichVuChiTietDichVuKyThuats)
                .HasForeignKey(rf => rf.NhomGiaDichVuKyThuatBenhVienId);

            builder.HasOne(rf => rf.GoiDichVu)
                      .WithMany(r => r.GoiDichVuChiTietDichVuKyThuats)
                      .HasForeignKey(rf => rf.GoiDichVuId);

            base.Configure(builder);
        }
    }
}
