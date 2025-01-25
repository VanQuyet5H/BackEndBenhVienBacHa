using Camino.Core.Domain.Entities.KyDuTruMuaDuocPhamVatTus;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Camino.Data.Mapping.KyDuTruMuaDuocPhamVatTuMapping
{
    public class KyDuTruMuaDuocPhamVatTuMap : CaminoEntityTypeConfiguration<KyDuTruMuaDuocPhamVatTu>
    {
        public override void Configure(EntityTypeBuilder<KyDuTruMuaDuocPhamVatTu> builder)
        {
            builder.ToTable(MappingDefaults.KyDuTruMuaDuocPhamVatTuTable);

            builder.HasOne(rf => rf.NhanVien)
              .WithMany(r => r.KyDuTruMuaDuocPhamVatTus)
              .HasForeignKey(rf => rf.NhanVienTaoId);

            base.Configure(builder);
        }
    }
}
