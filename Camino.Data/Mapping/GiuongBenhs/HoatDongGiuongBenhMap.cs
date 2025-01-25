using Camino.Core.Domain.Entities.GiuongBenhs;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Camino.Data.Mapping.GiuongBenhs
{
    public class HoatDongGiuongBenhMap : CaminoEntityTypeConfiguration<HoatDongGiuongBenh>
    {
        public override void Configure(EntityTypeBuilder<HoatDongGiuongBenh> builder)
        {
            builder.ToTable(MappingDefaults.HoatDongGiuongBenhTable);

            builder.HasOne(m => m.GiuongBenh)
                .WithMany(u => u.HoatDongGiuongBenhs)
                .HasForeignKey(m => m.GiuongBenhId)
                .IsRequired();

            builder.HasOne(m => m.YeuCauDichVuGiuongBenhVien)
                .WithMany(u => u.HoatDongGiuongBenhs)
                .HasForeignKey(m => m.YeuCauDichVuGiuongBenhVienId)
                .IsRequired();

            builder.HasOne(m => m.YeuCauDichVuKyThuat)
                .WithMany(u => u.HoatDongGiuongBenhs)
                .HasForeignKey(m => m.YeuCauDichVuKyThuatId);

            builder.HasOne(m => m.YeuCauKhamBenh)
                .WithMany(u => u.HoatDongGiuongBenhs)
                .HasForeignKey(m => m.YeuCauKhamBenhId);

            builder.HasOne(m => m.YeuCauTiepNhan)
                .WithMany(u => u.HoatDongGiuongBenhs)
                .HasForeignKey(m => m.YeuCauTiepNhanId)
                .IsRequired();

            base.Configure(builder);
        }
    }
}