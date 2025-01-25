using Camino.Core.Domain.Entities.DuTruMuaDuocPhamKhoDuocs;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Camino.Data.Mapping.DuTruMuaDuocPhamKhoDuocMapping
{
    public class DuTruMuaDuocPhamKhoDuocMap : CaminoEntityTypeConfiguration<DuTruMuaDuocPhamKhoDuoc>
    {
        public override void Configure(EntityTypeBuilder<DuTruMuaDuocPhamKhoDuoc> builder)
        {
            builder.ToTable(MappingDefaults.DuTruMuaDuocPhamKhoDuocTable);


            builder.HasOne(rf => rf.NhanVienYeuCau)
             .WithMany(r => r.DuTruMuaDuocPhamKhoDuocNhanVienYeuCaus)
             .HasForeignKey(rf => rf.NhanVienYeuCauId);

            builder.HasOne(rf => rf.GiamDoc)
            .WithMany(r => r.DuTruMuaDuocPhamKhoDuocGiamDocs)
            .HasForeignKey(rf => rf.GiamDocId);

            builder.HasOne(rf => rf.KyDuTruMuaDuocPhamVatTu)
            .WithMany(r => r.DuTruMuaDuocPhamKhoDuocs)
            .HasForeignKey(rf => rf.KyDuTruMuaDuocPhamVatTuId);


            base.Configure(builder);
        }
    }
}
