
using Camino.Core.Domain.Entities.YeuCauKhamBenhs;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Camino.Data.Mapping.NoiTruChiDinhDuocPhamMapping
{
    public class NoiTruChiDinhPhaThuocTiemMap : CaminoEntityTypeConfiguration<NoiTruChiDinhPhaThuocTiem>
    {
        public override void Configure(EntityTypeBuilder<NoiTruChiDinhPhaThuocTiem> builder)
        {
            builder.ToTable(MappingDefaults.NoiTruChiDinhPhaThuocTiemTable);
            builder.HasOne(rf => rf.YeuCauTiepNhan)
                .WithMany(r => r.NoiTruChiDinhPhaThuocTiems)
                .HasForeignKey(rf => rf.YeuCauTiepNhanId);

            builder.HasOne(rf => rf.NoiTruPhieuDieuTri)
               .WithMany(r => r.NoiTruChiDinhPhaThuocTiems)
               .HasForeignKey(rf => rf.NoiTruPhieuDieuTriId);

            builder.HasOne(rf => rf.NhanVienChiDinh)
                           .WithMany(r => r.NoiTruChiDinhPhaThuocTiems)
                           .HasForeignKey(rf => rf.NhanVienChiDinhId);

            builder.HasOne(rf => rf.NoiChiDinh)
               .WithMany(r => r.NoiTruChiDinhPhaThuocTiems)
               .HasForeignKey(rf => rf.NoiChiDinhId);

            builder.HasOne(rf => rf.NoiTruBenhAn)
               .WithMany(r => r.NoiTruChiDinhPhaThuocTiems)
               .HasForeignKey(rf => rf.NoiTruBenhAnId);

            base.Configure(builder);
        }
    }
}
