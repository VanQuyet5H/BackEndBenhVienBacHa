using Camino.Core.Domain.Entities.Thuocs;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Camino.Data.Mapping.Thuocs
{
    public class DuocPhamMap : CaminoEntityTypeConfiguration<DuocPham>
    {
        public override void Configure(EntityTypeBuilder<DuocPham> builder)
        {
            builder.ToTable(MappingDefaults.DuocPhamTable);

            builder.HasOne(rf => rf.DonViTinh)
                .WithMany(r => r.DuocPhams)
                .HasForeignKey(rf => rf.DonViTinhId);
            builder.HasOne(rf => rf.DuongDung)
                .WithMany(r => r.DuocPhams)
                .HasForeignKey(rf => rf.DuongDungId);

            builder.HasMany(rf => rf.ToaThuocMauChiTiets)
                .WithOne(r => r.DuocPham)
                .HasForeignKey(rf => rf.DuocPhamId);

            builder.HasMany(rf => rf.DuTruMuaDuocPhamChiTiets)
                .WithOne(r => r.DuocPham)
                .HasForeignKey(rf => rf.DuocPhamId);

            builder.HasMany(rf => rf.DuTruMuaDuocPhamTheoKhoaChiTiets)
                .WithOne(r => r.DuocPham)
                .HasForeignKey(rf => rf.DuocPhamId);

            builder.HasMany(rf => rf.DuTruMuaDuocPhamKhoDuocChiTiets)
                .WithOne(r => r.DuocPham)
                .HasForeignKey(rf => rf.DuocPhamId);
            base.Configure(builder);
        }
    }
}
