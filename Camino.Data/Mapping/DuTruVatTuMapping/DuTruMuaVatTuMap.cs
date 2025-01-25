using Camino.Core.Domain.Entities.DuTruVatTus;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Camino.Data.Mapping.DuTruVatTuMapping
{
    public class DuTruMuaVatTuMap : CaminoEntityTypeConfiguration<DuTruMuaVatTu>
    {
        public override void Configure(EntityTypeBuilder<DuTruMuaVatTu> builder)
        {
            builder.ToTable(MappingDefaults.DuTruMuaVatTu);

            builder.HasOne(rf => rf.Kho)
            .WithMany(r => r.DuTruMuaVatTus)
            .HasForeignKey(rf => rf.KhoId);

            builder.HasOne(rf => rf.NhanVienYeuCau)
            .WithMany(r => r.DuTruMuaVatTuNhanVienYeuCaus)
            .HasForeignKey(rf => rf.NhanVienYeuCauId);

            builder.HasOne(rf => rf.TruongKhoa)
              .WithMany(r => r.DuTruMuaVatTuTruongKhoas)
              .HasForeignKey(rf => rf.TruongKhoaId);

            builder.HasOne(rf => rf.DuTruMuaVatTuTheoKhoa)
             .WithMany(r => r.DuTruMuaVatTus)
             .HasForeignKey(rf => rf.DuTruMuaVatTuTheoKhoaId);

            builder.HasOne(rf => rf.KyDuTruMuaDuocPhamVatTu)
                .WithMany(r => r.DuTruMuaVatTus)
                .HasForeignKey(rf => rf.KyDuTruMuaDuocPhamVatTuId);

            builder.HasOne(rf => rf.DuTruMuaVatTuKhoDuoc)
               .WithMany(r => r.DuTruMuaVatTus)
               .HasForeignKey(rf => rf.DuTruMuaVatTuKhoDuocId);

            base.Configure(builder);
        }
    }
}
