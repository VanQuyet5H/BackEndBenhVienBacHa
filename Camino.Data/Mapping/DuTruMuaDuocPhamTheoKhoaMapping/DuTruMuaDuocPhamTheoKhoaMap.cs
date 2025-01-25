using Camino.Core.Domain.Entities.DuTruMuaDuocPhamTheoKhoas;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Camino.Data.Mapping.DuTruMuaDuocPhamTheoKhoaMapping
{
    public class DuTruMuaDuocPhamTheoKhoaMap : CaminoEntityTypeConfiguration<DuTruMuaDuocPhamTheoKhoa>
    {
        public override void Configure(EntityTypeBuilder<DuTruMuaDuocPhamTheoKhoa> builder)
        {
            builder.ToTable(MappingDefaults.DuTruMuaDuocPhamTheoKhoaTable);

            builder.HasOne(rf => rf.KhoaPhong)
            .WithMany(r => r.DuTruMuaDuocPhamTheoKhoas)
            .HasForeignKey(rf => rf.KhoaPhongId);

            builder.HasOne(rf => rf.DuTruMuaDuocPhamKhoDuoc)
              .WithMany(r => r.DuTruMuaDuocPhamTheoKhoas)
              .HasForeignKey(rf => rf.DuTruMuaDuocPhamKhoDuocId);

            builder.HasOne(rf => rf.KyDuTruMuaDuocPhamVatTu)
             .WithMany(r => r.DuTruMuaDuocPhamTheoKhoas)
             .HasForeignKey(rf => rf.KyDuTruMuaDuocPhamVatTuId);

            builder.HasOne(rf => rf.NhanVienYeuCau)
             .WithMany(r => r.DuTruMuaDuocPhamTheoKhoaNhanVienYeuCaus)
             .HasForeignKey(rf => rf.NhanVienYeuCauId);

            builder.HasOne(rf => rf.NhanVienKhoDuoc)
             .WithMany(r => r.DuTruMuaDuocPhamTheoKhoaNhanVienKhoDuocs)
             .HasForeignKey(rf => rf.NhanVienKhoDuocId);

            base.Configure(builder);
        }

    }
}
