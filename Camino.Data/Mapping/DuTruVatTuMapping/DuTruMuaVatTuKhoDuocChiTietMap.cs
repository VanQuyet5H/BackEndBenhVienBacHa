using System.Collections.Generic;
using System.Text;
using Camino.Core.Domain.Entities.DuocPhamBenhViens;
using Camino.Core.Domain.Entities.DuTruVatTus;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Camino.Data.Mapping.DuTruVatTuMapping
{
    public class DuTruMuaVatTuKhoDuocChiTietMap : CaminoEntityTypeConfiguration<DuTruMuaVatTuKhoDuocChiTiet>
    {
        public override void Configure(EntityTypeBuilder<DuTruMuaVatTuKhoDuocChiTiet> builder)
        {
            builder.ToTable(MappingDefaults.DuTruMuaVatTuKhoDuocChiTiet);

            builder.HasOne(rf => rf.VatTu)
               .WithMany(r => r.DuTruMuaVatTuKhoDuocChiTiets)
               .HasForeignKey(rf => rf.VatTuId);

            builder.HasOne(rf => rf.DuTruMuaVatTuKhoDuoc)
             .WithMany(r => r.DuTruMuaVatTuKhoDuocChiTiets)
             .HasForeignKey(rf => rf.DuTruMuaVatTuKhoDuocId);

            base.Configure(builder);
        }
    }
}
