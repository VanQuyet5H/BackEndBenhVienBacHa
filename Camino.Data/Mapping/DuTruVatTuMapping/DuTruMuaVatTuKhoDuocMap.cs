using Camino.Core.Domain.Entities.DuTruVatTus;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Camino.Data.Mapping.DuTruVatTuMapping
{
    public class DuTruMuaVatTuKhoDuocMap : CaminoEntityTypeConfiguration<DuTruMuaVatTuKhoDuoc>
    {
        public override void Configure(EntityTypeBuilder<DuTruMuaVatTuKhoDuoc> builder)
        {
            builder.ToTable(MappingDefaults.DuTruMuaVatTuKhoDuoc);

            builder.HasOne(rf => rf.NhanVienYeuCau)
             .WithMany(r => r.DuTruMuaVatTuKhoDuocNhanVienYeuCaus)
             .HasForeignKey(rf => rf.NhanVienYeuCauId);

            builder.HasOne(rf => rf.GiamDoc)
            .WithMany(r => r.DuTruMuaVatTuKhoDuocGiamDocs)
            .HasForeignKey(rf => rf.GiamDocId);

            builder.HasOne(rf => rf.KyDuTruMuaDuocPhamVatTu)
            .WithMany(r => r.DuTruMuaVatTuKhoDuocs)
            .HasForeignKey(rf => rf.KyDuTruMuaDuocPhamVatTuId);


            base.Configure(builder);
        }
    }
}
