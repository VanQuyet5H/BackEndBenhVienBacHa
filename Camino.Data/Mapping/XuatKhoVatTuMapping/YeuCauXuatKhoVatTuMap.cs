using Camino.Core.Domain.Entities.XuatKhoVatTus;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Camino.Data.Mapping.XuatKhoVatTuMapping
{
    public class YeuCauXuatKhoVatTuMap : CaminoEntityTypeConfiguration<YeuCauXuatKhoVatTu>
    {
        public override void Configure(EntityTypeBuilder<YeuCauXuatKhoVatTu> builder)
        {
            builder.ToTable(MappingDefaults.YeuCauXuatKhoVatTuTable);

            builder.HasOne(m => m.KhoXuat)
                .WithMany(u => u.YeuCauXuatKhoVatTus)
                .HasForeignKey(m => m.KhoXuatId);

            builder.HasOne(m => m.NguoiXuat)
                .WithMany(u => u.YeuCauXuatKhoVatTuNguoiXuats)
                .HasForeignKey(m => m.NguoiXuatId);

            builder.HasOne(m => m.NguoiNhan)
                .WithMany(u => u.YeuCauXuatKhoVatTuNguoiNhans)
                .HasForeignKey(m => m.NguoiNhanId);

            builder.HasOne(m => m.NhanVienDuyet)
                .WithMany(u => u.YeuCauXuatKhoVatTuNhanVienDuyets)
                .HasForeignKey(m => m.NhanVienDuyetId);

            builder.HasOne(m => m.NhaThau)
              .WithMany(u => u.YeuCauXuatKhoVatTus)
              .HasForeignKey(m => m.NhaThauId);
            base.Configure(builder);
        }
    }
}
