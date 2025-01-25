using Camino.Core.Domain.Entities.YeuCauKhamBenhs;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Camino.Data.Mapping.YeuCauKhamBenhMapping
{
    public class YeuCauKhamBenhDonThuocChiTietMap : CaminoEntityTypeConfiguration<YeuCauKhamBenhDonThuocChiTiet>
    {
        public override void Configure(EntityTypeBuilder<YeuCauKhamBenhDonThuocChiTiet> builder)
        {
            builder.ToTable(MappingDefaults.YeuCauKhamBenhDonThuocChiTietTietTable);

            builder.HasOne(rf => rf.DuocPham)
                   .WithMany(rf => rf.YeuCauKhamBenhDonThuocChiTiets)
                   .HasForeignKey(rf => rf.DuocPhamId);

            builder.HasOne(rf => rf.DuongDung)
                   .WithMany(rf => rf.YeuCauKhamBenhDonThuocChiTiets)
                   .HasForeignKey(rf => rf.DuongDungId);

            builder.HasOne(rf => rf.YeuCauKhamBenhDonThuoc)
                 .WithMany(rf => rf.YeuCauKhamBenhDonThuocChiTiets)
                 .HasForeignKey(rf => rf.YeuCauKhamBenhDonThuocId);

            base.Configure(builder);
        }
    }
}
