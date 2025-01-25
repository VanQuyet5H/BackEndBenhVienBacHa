using Camino.Core.Domain.Entities.KetQuaSinhHieus;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Camino.Data.Mapping.KetQuaSinhHieuMapping
{
    public class KetQuaSinhHieuMap : CaminoEntityTypeConfiguration<KetQuaSinhHieu>
    {
        public override void Configure(EntityTypeBuilder<KetQuaSinhHieu> builder)
        {
            builder.ToTable(MappingDefaults.KetQuaSinhHieuTable);

            builder.HasOne(rf => rf.YeuCauTiepNhan)
                .WithMany(r => r.KetQuaSinhHieus)
                .HasForeignKey(rf => rf.YeuCauTiepNhanId);

            builder.HasOne(rf => rf.NoiThucHien)
                .WithMany(r => r.KetQuaSinhHieus)
                .HasForeignKey(rf => rf.NoiThucHienId);

            builder.HasOne(rf => rf.NhanVienThucHien)
                .WithMany(r => r.KetQuaSinhHieus)
                .HasForeignKey(rf => rf.NhanVienThucHienId);

            builder.HasOne(rf => rf.NoiTruPhieuDieuTri)
                .WithMany(r => r.KetQuaSinhHieus)
                .HasForeignKey(rf => rf.NoiTruPhieuDieuTriId);

            builder.HasOne(rf => rf.YeuCauDichVuKyThuatKhamSangLocTiemChung)
                .WithMany(r => r.KetQuaSinhHieus)
                .HasForeignKey(rf => rf.YeuCauDichVuKyThuatKhamSangLocTiemChungId);

            base.Configure(builder);
        }
    }
}
