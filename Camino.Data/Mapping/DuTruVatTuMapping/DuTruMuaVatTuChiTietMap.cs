using Camino.Core.Domain.Entities.DuTruVatTus;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Camino.Data.Mapping.DuTruVatTuMapping
{
    public class DuTruMuaVatTuChiTietMap : CaminoEntityTypeConfiguration<DuTruMuaVatTuChiTiet>
    {
        public override void Configure(EntityTypeBuilder<DuTruMuaVatTuChiTiet> builder)
        {
            builder.ToTable(MappingDefaults.DuTruMuaVatTuChiTiet);

            builder.HasOne(rf => rf.VatTu)
            .WithMany(r => r.DuTruMuaVatTuChiTiets)
            .HasForeignKey(rf => rf.VatTuId);

            builder.HasOne(rf => rf.DuTruMuaVatTu)
             .WithMany(r => r.DuTruMuaVatTuChiTiets)
             .HasForeignKey(rf => rf.DuTruMuaVatTuId);

            builder.HasOne(rf => rf.DuTruMuaVatTuTheoKhoaChiTiet)
             .WithMany(r => r.DuTruMuaVatTuChiTiets)
             .HasForeignKey(rf => rf.DuTruMuaVatTuTheoKhoaChiTietId);

            builder.HasOne(rf => rf.DuTruMuaVatTuKhoDuocChiTiet)
                .WithMany(r => r.DuTruMuaVatTuChiTiets)
                .HasForeignKey(rf => rf.DuTruMuaVatTuKhoDuocChiTietId);

            base.Configure(builder);
        }
    }
}
