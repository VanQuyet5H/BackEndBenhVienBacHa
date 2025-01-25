using Camino.Core.Domain.Entities.DuTruMuaDuocPhams;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Camino.Data.Mapping.DuTruMuaDuocPhamMapping
{
    public class DuTruMuaDuocPhamChiTietMap : CaminoEntityTypeConfiguration<DuTruMuaDuocPhamChiTiet>
    {

        public override void Configure(EntityTypeBuilder<DuTruMuaDuocPhamChiTiet> builder)
        {
            builder.ToTable(MappingDefaults.DuTruMuaDuocPhamChiTietTable);

            builder.HasOne(rf => rf.DuTruMuaDuocPham)
              .WithMany(r => r.DuTruMuaDuocPhamChiTiets)
              .HasForeignKey(rf => rf.DuTruMuaDuocPhamId);

            builder.HasOne(rf => rf.DuTruMuaDuocPhamTheoKhoaChiTiet)
                .WithMany(r => r.DuTruMuaDuocPhamChiTiets)
                .HasForeignKey(rf => rf.DuTruMuaDuocPhamTheoKhoaChiTietId);

            builder.HasOne(rf => rf.DuTruMuaDuocPhamKhoDuocChiTiet)
            .WithMany(r => r.DuTruMuaDuocPhamChiTiets)
            .HasForeignKey(rf => rf.DuTruMuaDuocPhamKhoDuocChiTietId);

            builder.HasOne(rf => rf.DuocPham)
             .WithMany(r => r.DuTruMuaDuocPhamChiTiets)
             .HasForeignKey(rf => rf.DuocPhamId);

            base.Configure(builder);
        }


    }
}
