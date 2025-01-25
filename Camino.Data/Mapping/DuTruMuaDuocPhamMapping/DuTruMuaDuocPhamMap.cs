using Camino.Core.Domain.Entities.DuTruMuaDuocPhams;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Camino.Data.Mapping.DuTruMuaDuocPhamMapping
{
    public class DuTruMuaDuocPhamMap : CaminoEntityTypeConfiguration<DuTruMuaDuocPham>
    {
        public override void Configure(EntityTypeBuilder<DuTruMuaDuocPham> builder)
        {
            builder.ToTable(MappingDefaults.DuTruMuaDuocPhamTable);

            builder.HasOne(rf => rf.Kho)
            .WithMany(r => r.DuTruMuaDuocPhams)
            .HasForeignKey(rf => rf.KhoId);

            builder.HasOne(rf => rf.NhanVienYeuCau)
              .WithMany(r => r.DuTruMuaDuocPhamNhanVienYeuCaus)
              .HasForeignKey(rf => rf.NhanVienYeuCauId);

            builder.HasOne(rf => rf.TruongKhoa)
              .WithMany(r => r.DuTruMuaDuocPhamTruongKhoas)
              .HasForeignKey(rf => rf.TruongKhoaId);

            builder.HasOne(rf => rf.KyDuTruMuaDuocPhamVatTu)
               .WithMany(r => r.DuTruMuaDuocPhams)
               .HasForeignKey(rf => rf.KyDuTruMuaDuocPhamVatTuId);

            builder.HasOne(rf => rf.DuTruMuaDuocPhamTheoKhoa)
              .WithMany(r => r.DuTruMuaDuocPhams)
              .HasForeignKey(rf => rf.DuTruMuaDuocPhamTheoKhoaId);

            builder.HasOne(rf => rf.DuTruMuaDuocPhamKhoDuoc)
               .WithMany(r => r.DuTruMuaDuocPhams)
               .HasForeignKey(rf => rf.DuTruMuaDuocPhamKhoDuocId);

            base.Configure(builder);
        }
    }
}
