
using Camino.Core.Domain.Entities.YeuCauKhamBenhs;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;


namespace Camino.Data.Mapping.NoiTruChiDinhDuocPhamMapping
{
    public class NoiTruChiDinhPhaThuocTruyenMap : CaminoEntityTypeConfiguration<NoiTruChiDinhPhaThuocTruyen>

    {
        public override void Configure(EntityTypeBuilder<NoiTruChiDinhPhaThuocTruyen> builder)
        {
            builder.ToTable(MappingDefaults.NoiTruChiDinhPhaThuocTruyenTable);
            builder.HasOne(rf => rf.YeuCauTiepNhan)
                .WithMany(r => r.NoiTruChiDinhPhaThuocTruyens)
                .HasForeignKey(rf => rf.YeuCauTiepNhanId);

            builder.HasOne(rf => rf.NoiTruPhieuDieuTri)
               .WithMany(r => r.NoiTruChiDinhPhaThuocTruyens)
               .HasForeignKey(rf => rf.NoiTruPhieuDieuTriId);

            builder.HasOne(rf => rf.NhanVienChiDinh)
                           .WithMany(r => r.NoiTruChiDinhPhaThuocTruyens)
                           .HasForeignKey(rf => rf.NhanVienChiDinhId);

            builder.HasOne(rf => rf.NoiChiDinh)
               .WithMany(r => r.NoiTruChiDinhPhaThuocTruyens)
               .HasForeignKey(rf => rf.NoiChiDinhId);
        
            builder.HasOne(rf => rf.NoiTruBenhAn)
               .WithMany(r => r.NoiTruChiDinhPhaThuocTruyens)
               .HasForeignKey(rf => rf.NoiTruBenhAnId);
           
            base.Configure(builder);
        }
    }
}
