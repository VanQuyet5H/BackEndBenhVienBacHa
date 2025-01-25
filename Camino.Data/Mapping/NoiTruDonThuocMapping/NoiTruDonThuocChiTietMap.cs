using Camino.Core.Domain.Entities.NoiTruDonThuocs;
using Camino.Core.Domain.Entities.YeuCauKhamBenhs;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Camino.Data.Mapping.NoiTruDonThuocMapping
{
    public class NoiTruDonThuocChiTietMap : CaminoEntityTypeConfiguration<NoiTruDonThuocChiTiet>

    {
        public override void Configure(EntityTypeBuilder<NoiTruDonThuocChiTiet> builder)
        {
            builder.ToTable(MappingDefaults.NoiTruDonThuocChiTietTable);

            builder.HasOne(rf => rf.DuocPham)
                   .WithMany(rf => rf.NoiTruDonThuocChiTiets)
                   .HasForeignKey(rf => rf.DuocPhamId);

            builder.HasOne(rf => rf.DuongDung)
                   .WithMany(rf => rf.NoiTruDonThuocChiTiets)
                   .HasForeignKey(rf => rf.DuongDungId);

            builder.HasOne(rf => rf.DonViTinh)
                   .WithMany(rf => rf.NoiTruDonThuocChiTiets)
                   .HasForeignKey(rf => rf.DonViTinhId);

            builder.HasOne(rf => rf.NoiTruDonThuoc)
                 .WithMany(rf => rf.NoiTruDonThuocChiTiets)
                 .HasForeignKey(rf => rf.NoiTruDonThuocId);

            base.Configure(builder);
        }
    }
}
