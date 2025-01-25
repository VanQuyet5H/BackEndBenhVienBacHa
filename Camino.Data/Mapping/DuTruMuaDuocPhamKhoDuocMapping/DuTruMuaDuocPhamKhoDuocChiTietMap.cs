using Camino.Core.Domain.Entities.DuTruMuaDuocPhamKhoDuocs;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Camino.Data.Mapping.DuTruMuaDuocPhamKhoDuocMapping
{
    public class DuTruMuaDuocPhamKhoDuocChiTietMap : CaminoEntityTypeConfiguration<DuTruMuaDuocPhamKhoDuocChiTiet>
    {
        public override void Configure(EntityTypeBuilder<DuTruMuaDuocPhamKhoDuocChiTiet> builder)
        {
            builder.ToTable(MappingDefaults.DuTruMuaDuocPhamKhoDuocChiTietTable);

            builder.HasOne(rf => rf.DuTruMuaDuocPhamKhoDuoc)
            .WithMany(r => r.DuTruMuaDuocPhamKhoDuocChiTiets)
            .HasForeignKey(rf => rf.DuTruMuaDuocPhamKhoDuocId);

            builder.HasOne(rf => rf.DuocPham)
            .WithMany(r => r.DuTruMuaDuocPhamKhoDuocChiTiets)
            .HasForeignKey(rf => rf.DuocPhamId);

            base.Configure(builder);
        }
    }
}
