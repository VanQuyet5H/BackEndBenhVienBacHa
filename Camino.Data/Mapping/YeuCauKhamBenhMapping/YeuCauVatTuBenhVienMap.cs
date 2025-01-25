using Camino.Core.Domain.Entities.YeuCauKhamBenhs;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Camino.Data.Mapping.YeuCauKhamBenhMapping
{
    public class YeuCauVatTuBenhVienMap : CaminoEntityTypeConfiguration<YeuCauVatTuBenhVien>
    {
        public override void Configure(EntityTypeBuilder<YeuCauVatTuBenhVien> builder)
        {
            builder.ToTable(MappingDefaults.YeuCauVatTuBenhVienTable);

            builder.HasOne(rf => rf.NhanVienCapVatTu)
                                   .WithMany(r => r.YeuCauVatTuBenhVienNoiCapVatTus)
                                   .HasForeignKey(rf => rf.NhanVienCapVatTuId);

            builder.HasOne(rf => rf.NhanVienChiDinh)
                                   .WithMany(r => r.YeuCauVatTuBenhVienNoiChiDinhs)
                                   .HasForeignKey(rf => rf.NhanVienChiDinhId)
                .IsRequired();

            builder.HasOne(rf => rf.NhanVienThanhToan)
                .WithMany(r => r.YeuCauVatTuBenhVienNoiThanhToans)
                .HasForeignKey(rf => rf.NhanVienHuyThanhToanId);
                //.IsRequired();

            builder.HasOne(rf => rf.NoiCapVatTu)
                                .WithMany(r => r.YeuCauVatTuBenhVienNoiCapVatTus)
                                .HasForeignKey(rf => rf.NoiCapVatTuId);

            builder.HasOne(rf => rf.NoiChiDinh)
                                .WithMany(r => r.YeuCauVatTuBenhVienNoiChiDinhs)
                                .HasForeignKey(rf => rf.NoiChiDinhId)
                .IsRequired();

            builder.HasOne(rf => rf.VatTuBenhVien)
                              .WithMany(r => r.YeuCauVatTuBenhViens)
                              .HasForeignKey(rf => rf.VatTuBenhVienId);

            builder.HasOne(rf => rf.YeuCauDichVuKyThuat)
                           .WithMany(r => r.YeuCauVatTuBenhViens)
                           .HasForeignKey(rf => rf.YeuCauDichVuKyThuatId);

            builder.HasOne(rf => rf.YeuCauKhamBenh)
                        .WithMany(r => r.YeuCauVatTuBenhViens)
                        .HasForeignKey(rf => rf.YeuCauKhamBenhId);

            builder.HasOne(rf => rf.YeuCauTiepNhan)
                      .WithMany(r => r.YeuCauVatTuBenhViens)
                      .HasForeignKey(rf => rf.YeuCauTiepNhanId)
                      .IsRequired();

            builder.HasOne(rf => rf.NhanVienDuyetBaoHiem)
                .WithMany(r => r.YeuCauVatTuBenhVienNhanVienDuyetBaoHiems)
                .HasForeignKey(rf => rf.NhanVienDuyetBaoHiemId);

            builder.HasOne(rf => rf.NhomVatTu)
                .WithMany(r => r.YeuCauVatTuBenhViens)
                .HasForeignKey(rf => rf.NhomVatTuId)
                .IsRequired();

            builder.HasOne(rf => rf.YeuCauGoiDichVu)
                   .WithMany(r => r.YeuCauVatTuBenhViens)
                   .HasForeignKey(rf => rf.YeuCauGoiDichVuId);

            builder.HasOne(rf => rf.XuatKhoVatTuChiTiet)
                   .WithMany(r => r.YeuCauVatTuBenhViens)
                   .HasForeignKey(rf => rf.XuatKhoVatTuChiTietId);

            builder.HasOne(rf => rf.YeuCauLinhVatTu)
                   .WithMany(r => r.YeuCauVatTuBenhViens)
                   .HasForeignKey(rf => rf.YeuCauLinhVatTuId);

            builder.HasOne(rf => rf.HopDongThauVatTu)
                .WithMany(r => r.YeuCauVatTuBenhViens)
                .HasForeignKey(rf => rf.HopDongThauVatTuId);
            builder.HasOne(rf => rf.NhaThau)
                .WithMany(r => r.YeuCauVatTuBenhViens)
                .HasForeignKey(rf => rf.NhaThauId);
            builder.HasOne(rf => rf.KhoLinh)
                 .WithMany(r => r.YeuCauVatTuBenhViens)
                 .HasForeignKey(rf => rf.KhoLinhId);

            builder.HasOne(rf => rf.NoiTruPhieuDieuTri)
                .WithMany(r => r.YeuCauVatTuBenhViens)
                .HasForeignKey(rf => rf.NoiTruPhieuDieuTriId);
            builder.HasOne(rf => rf.YeuCauTiepNhanTheBHYT)
                .WithMany(r => r.YeuCauVatTuBenhViens)
                .HasForeignKey(rf => rf.YeuCauTiepNhanTheBHYTId);


            //BVHD-3731
            builder
                .HasOne(sc => sc.NoiDungGhiChuMiemGiam)
                .WithMany(s => s.YeuCauVatTuBenhViens)
                .HasForeignKey(sc => sc.NoiDungGhiChuMiemGiamId);

            base.Configure(builder);
        }
    }
}
