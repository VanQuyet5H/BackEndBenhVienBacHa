using Camino.Core.Domain.Entities.XuatKhoVatTus;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Camino.Data.Mapping.XuatKhoVatTuMapping
{
    public class YeuCauXuatKhoVatTuChiTietMap : CaminoEntityTypeConfiguration<YeuCauXuatKhoVatTuChiTiet>
    {
        public override void Configure(EntityTypeBuilder<YeuCauXuatKhoVatTuChiTiet> builder)
        {
            builder.ToTable(MappingDefaults.YeuCauXuatKhoVatTuChiTietTable);

            builder.HasOne(m => m.YeuCauXuatKhoVatTu)
                .WithMany(u => u.YeuCauXuatKhoVatTuChiTiets)
                .HasForeignKey(m => m.YeuCauXuatKhoVatTuId);

            builder.HasOne(m => m.XuatKhoVatTuChiTietViTri)
                .WithMany(u => u.YeuCauXuatKhoVatTuChiTiets)
                .HasForeignKey(m => m.XuatKhoVatTuChiTietViTriId);

            builder.HasOne(m => m.VatTuBenhVien)
                .WithMany(u => u.YeuCauXuatKhoVatTuChiTiets)
                .HasForeignKey(m => m.VatTuBenhVienId);

            builder.HasOne(m => m.HopDongThauVatTu)
              .WithMany(u => u.YeuCauXuatKhoVatTuChiTiets)
              .HasForeignKey(m => m.HopDongThauVatTuId);
            base.Configure(builder);
        }
    }
}
