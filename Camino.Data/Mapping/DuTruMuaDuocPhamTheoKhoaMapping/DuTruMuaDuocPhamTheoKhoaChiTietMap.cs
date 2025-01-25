using Camino.Core.Domain.Entities.DuTruMuaDuocPhamTheoKhoas;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Camino.Data.Mapping.DuTruMuaDuocPhamTheoKhoaMapping
{
    public class DuTruMuaDuocPhamTheoKhoaChiTietMap : CaminoEntityTypeConfiguration<DuTruMuaDuocPhamTheoKhoaChiTiet>
    {
        public override void Configure(EntityTypeBuilder<DuTruMuaDuocPhamTheoKhoaChiTiet> builder)
        {
            builder.ToTable(MappingDefaults.DuTruMuaDuocPhamTheoKhoaChiTietTable);

            builder.HasOne(rf => rf.DuTruMuaDuocPhamKhoDuocChiTiet)
            .WithMany(r => r.DuTruMuaDuocPhamTheoKhoaChiTiets)
            .HasForeignKey(rf => rf.DuTruMuaDuocPhamKhoDuocChiTietId);

            builder.HasOne(rf => rf.DuTruMuaDuocPhamTheoKhoa)
           .WithMany(r => r.DuTruMuaDuocPhamTheoKhoaChiTiets)
           .HasForeignKey(rf => rf.DuTruMuaDuocPhamTheoKhoaId);

            builder.HasOne(rf => rf.DuocPham)
            .WithMany(r => r.DuTruMuaDuocPhamTheoKhoaChiTiets)
            .HasForeignKey(rf => rf.DuocPhamId);

            base.Configure(builder);
        }
    }
}
