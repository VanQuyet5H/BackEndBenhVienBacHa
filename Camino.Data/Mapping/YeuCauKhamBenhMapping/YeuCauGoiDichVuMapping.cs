using Camino.Core.Domain.Entities.YeuCauKhamBenhs;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
namespace Camino.Data.Mapping.YeuCauKhamBenhMapping
{
    public class YeuCauGoiDichVuMapping : CaminoEntityTypeConfiguration<YeuCauGoiDichVu>
    {
        public override void Configure(EntityTypeBuilder<YeuCauGoiDichVu> builder)
        {
            builder.ToTable(MappingDefaults.YeuCauGoiDichVuTable);
            
            builder.HasOne(rf => rf.NhanVienChiDinh)
                     .WithMany(r => r.YeuCauGoiDichVuNhanVienChiDinhs)
                     .HasForeignKey(rf => rf.NhanVienChiDinhId);

            builder.HasOne(rf => rf.NhanVienTuVan)
                .WithMany(r => r.YeuCauGoiDichVuNhanVienTuVans)
                .HasForeignKey(rf => rf.NhanVienTuVanId);

            builder.HasOne(rf => rf.NhanVienQuyetToan)
                .WithMany(r => r.YeuCauGoiDichVuNhanVienQuyetToans)
                .HasForeignKey(rf => rf.NhanVienQuyetToanId);

            builder.HasOne(rf => rf.NhanVienHuyQuyetToan)
                .WithMany()
                .HasForeignKey(rf => rf.NhanVienHuyQuyetToanId);

            builder.HasOne(rf => rf.NoiChiDinh)
                   .WithMany(r => r.YeuCauGoiDichVuNoiChiDinhs)
                   .HasForeignKey(rf => rf.NoiChiDinhId);
            builder.HasOne(rf => rf.NoiQuyetToan)
                .WithMany(r => r.YeuCauGoiDichVuNoiQuyetToans)
                .HasForeignKey(rf => rf.NoiQuyetToanId);

            builder.HasOne(rf => rf.BenhNhan)
                .WithMany(r => r.YeuCauGoiDichVus)
                .HasForeignKey(rf => rf.BenhNhanId);

            builder.HasOne(rf => rf.BenhNhanSoSinh)
                .WithMany(r => r.YeuCauGoiDichVuSoSinhs)
                .HasForeignKey(rf => rf.BenhNhanSoSinhId);

            builder.HasOne(rf => rf.ChuongTrinhGoiDichVu)
                .WithMany(r => r.YeuCauGoiDichVus)
                .HasForeignKey(rf => rf.ChuongTrinhGoiDichVuId);


            //BVHD-3731
            builder
                .HasOne(sc => sc.NoiDungGhiChuMiemGiam)
                .WithMany(s => s.YeuCauGoiDichVus)
                .HasForeignKey(sc => sc.NoiDungGhiChuMiemGiamId);

            base.Configure(builder);
        }
    }
}
