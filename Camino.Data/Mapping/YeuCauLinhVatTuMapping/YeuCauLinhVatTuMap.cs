using Camino.Core.Domain.Entities.YeuCauLinhVatTus;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Camino.Data.Mapping.YeuCauLinhVatTuMapping
{
    public class YeuCauLinhVatTuMap : CaminoEntityTypeConfiguration<YeuCauLinhVatTu>
    {
        public override void Configure(EntityTypeBuilder<YeuCauLinhVatTu> builder)
        {
            builder.ToTable(MappingDefaults.YeuCauLinhVatTuTable);

            builder.HasOne(rf => rf.KhoXuat)
                .WithMany(r => r.YeuCauLinhVatTuKhoXuats)
                .HasForeignKey(rf => rf.KhoXuatId);

            builder.HasOne(rf => rf.KhoNhap)
                .WithMany(r => r.YeuCauLinhVatTuKhoNhaps)
                .HasForeignKey(rf => rf.KhoNhapId);

            builder.HasOne(rf => rf.NhanVienYeuCau)
                .WithMany(r => r.YeuCauLinhVatTuNhanVienYeuCaus)
                .HasForeignKey(rf => rf.NhanVienYeuCauId);

            builder.HasOne(rf => rf.NhanVienDuyet)
                .WithMany(r => r.YeuCauLinhVatTuNhanVienDuyets)
                .HasForeignKey(rf => rf.NhanVienDuyetId);

            builder.HasOne(rf => rf.NoiYeuCau)
                .WithMany(r => r.YeuCauLinhVatTus)
                .HasForeignKey(rf => rf.NoiYeuCauId);

            base.Configure(builder);
        }
    }
}
