using Camino.Core.Domain.Entities.DuTruVatTus;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Camino.Data.Mapping.DuTruVatTuMapping
{
    public class DuTruMuaVatTuTheoKhoaChiTietMap : CaminoEntityTypeConfiguration<DuTruMuaVatTuTheoKhoaChiTiet>
    {
        public override void Configure(EntityTypeBuilder<DuTruMuaVatTuTheoKhoaChiTiet> builder)
        {
            builder.ToTable(MappingDefaults.DuTruMuaVatTuTheoKhoaChiTiet);

            builder.HasOne(rf => rf.DuTruMuaVatTuKhoDuocChiTiet)
           .WithMany(r => r.DuTruMuaVatTuTheoKhoaChiTiets)
           .HasForeignKey(rf => rf.DuTruMuaVatTuKhoDuocChiTietId);

            builder.HasOne(rf => rf.DuTruMuaVatTuTheoKhoa)
           .WithMany(r => r.DuTruMuaVatTuTheoKhoaChiTiets)
           .HasForeignKey(rf => rf.DuTruMuaVatTuTheoKhoaId);

            builder.HasOne(rf => rf.VatTu)
            .WithMany(r => r.DuTruMuaVatTuTheoKhoaChiTiets)
            .HasForeignKey(rf => rf.VatTuId);

            base.Configure(builder);
        }
    }
}
