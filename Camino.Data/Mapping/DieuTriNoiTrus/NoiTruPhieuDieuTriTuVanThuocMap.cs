using Camino.Core.Domain.Entities.DieuTriNoiTrus;
using Camino.Core.Domain.Entities.KhamDoans;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Camino.Data.Mapping.DieuTriNoiTrus
{
    public class NoiTruPhieuDieuTriTuVanThuocMap : CaminoEntityTypeConfiguration<NoiTruPhieuDieuTriTuVanThuoc>
    {
        public override void Configure(EntityTypeBuilder<NoiTruPhieuDieuTriTuVanThuoc> builder)
        {
            builder.ToTable(MappingDefaults.NoiTruPhieuDieuTriTuVanThuocTable);

            builder.HasOne(x => x.YeuCauTiepNhan)
                .WithMany(y => y.NoiTruPhieuDieuTriTuVanThuocs)
                .HasForeignKey(x => x.YeuCauTiepNhanId);

            builder.HasOne(x => x.NoiTruPhieuDieuTri)
            .WithMany(y => y.NoiTruPhieuDieuTriTuVanThuocs)
            .HasForeignKey(x => x.NoiTruPhieuDieuTriId);

            builder.HasOne(x => x.DuocPham)
                .WithMany(y => y.NoiTruPhieuDieuTriTuVanThuocs)
                .HasForeignKey(x => x.DuocPhamId);

            builder.HasOne(x => x.DuongDung)
                .WithMany(y => y.NoiTruPhieuDieuTriTuVanThuocs)
                .HasForeignKey(x => x.DuongDungId);

            builder.HasOne(x => x.DonViTinh)
                .WithMany(y => y.NoiTruPhieuDieuTriTuVanThuocs)
                .HasForeignKey(x => x.DonViTinhId);

            builder.HasOne(rf => rf.NhanVienChiDinh)
                           .WithMany(r => r.NoiTruPhieuDieuTriTuVanThuocs)
                           .HasForeignKey(rf => rf.NhanVienChiDinhId);

            builder.HasOne(rf => rf.NoiChiDinh)
              .WithMany(r => r.NoiTruPhieuDieuTriTuVanThuocs)
              .HasForeignKey(rf => rf.NoiChiDinhId);
            base.Configure(builder);
        }
    }
}
