using Camino.Core.Domain.Entities.DuTruVatTus;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Camino.Data.Mapping.DuTruVatTuMapping
{
    public class DuTruMuaVatTuTheoKhoaMap : CaminoEntityTypeConfiguration<DuTruMuaVatTuTheoKhoa>
    {
        public override void Configure(EntityTypeBuilder<DuTruMuaVatTuTheoKhoa> builder)
        {
            builder.ToTable(MappingDefaults.DuTruMuaVatTuTheoKhoa);

            builder.HasOne(rf => rf.KhoaPhong)
              .WithMany(r => r.DuTruMuaVatTuTheoKhoas)
              .HasForeignKey(rf => rf.KhoaPhongId);

            builder.HasOne(rf => rf.NhanVienYeuCau)
             .WithMany(r => r.DuTruMuaVatTuTheoKhoaNhanVienYeuCaus)
             .HasForeignKey(rf => rf.NhanVienYeuCauId);

            builder.HasOne(rf => rf.NhanVienKhoDuoc)
              .WithMany(r => r.DuTruMuaVatTuTheoKhoaNhanVienKhoDuocs)
              .HasForeignKey(rf => rf.NhanVienKhoDuocId);

            builder.HasOne(rf => rf.KyDuTruMuaDuocPhamVatTu)
              .WithMany(r => r.DuTruMuaVatTuTheoKhoas)
              .HasForeignKey(rf => rf.KyDuTruMuaDuocPhamVatTuId);

            builder.HasOne(rf => rf.DuTruMuaVatTuKhoDuoc)
              .WithMany(r => r.DuTruMuaVatTuTheoKhoas)
              .HasForeignKey(rf => rf.DuTruMuaVatTuKhoDuocId);

            base.Configure(builder);
        }
    }
}
