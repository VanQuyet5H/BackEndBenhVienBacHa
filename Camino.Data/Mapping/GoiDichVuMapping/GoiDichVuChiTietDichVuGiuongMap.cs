using Camino.Core.Domain.Entities.GoiDichVus;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Camino.Data.Mapping.GoiDichVuMapping
{
    public class GoiDichVuChiTietDichVuGiuongMap : CaminoEntityTypeConfiguration<GoiDichVuChiTietDichVuGiuong>
    {
        public override void Configure(EntityTypeBuilder<GoiDichVuChiTietDichVuGiuong> builder)
        {
            builder.ToTable(MappingDefaults.GoiDichVuChiTietDichVuGiuongTable);

            builder.HasOne(rf => rf.GoiDichVu)
                .WithMany(r => r.GoiDichVuChiTietDichVuGiuongs)
                .HasForeignKey(rf => rf.GoiDichVuId);

            builder.HasOne(rf => rf.DichVuGiuongBenhVien)
                .WithMany(r => r.GoiDichVuChiTietDichVuGiuongs)
                .HasForeignKey(rf => rf.DichVuGiuongBenhVienId);

            builder.HasOne(rf => rf.NhomGiaDichVuGiuongBenhVien)
                .WithMany(r => r.GoiDichVuChiTietDichVuGiuongs)
                .HasForeignKey(rf => rf.NhomGiaDichVuGiuongBenhVienId);

            base.Configure(builder);
        }
    }
}
