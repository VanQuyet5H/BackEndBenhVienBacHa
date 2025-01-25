using Camino.Core.Domain.Entities.YeuCauTiepNhans;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Camino.Data.Mapping.YeuCauTiepNhanMapping
{
    public class YeuCauTiepNhanMap : CaminoEntityTypeConfiguration<YeuCauTiepNhan>
    {
        public override void Configure(EntityTypeBuilder<YeuCauTiepNhan> builder)
        {
            builder.ToTable(MappingDefaults.YeuCauTiepNhanTable);

            builder.HasOne(m => m.BenhNhan)
                .WithMany(u => u.YeuCauTiepNhans)
                .HasForeignKey(m => m.BenhNhanId);

            //builder.HasOne(m => m.TrieuChung)
            //    .WithMany(u => u.YeuCauTiepNhans)
            //    .HasForeignKey(m => m.TrieuChungId)
            //    .IsRequired();

            builder.HasOne(rf => rf.NoiTiepNhan)
                 .WithMany(r => r.YeuCauTiepNhan)
                 .HasForeignKey(rf => rf.NoiTiepNhanId);

            //builder.HasOne(rf => rf.NoiThanhToan)
            //     .WithMany(r => r.YeuCauTiepNhanNoiThanhToans)
            //     .HasForeignKey(rf => rf.NoiThanhToanId);

            //builder.HasOne(rf => rf.PhongKetLuan)
            //            .WithMany(r => r.YeuCauTiepNhanPhongKetLuans)
            //            .HasForeignKey(rf => rf.PhongKetLuanId);

            //builder.HasOne(rf => rf.PhongKham)
            //           .WithMany(r => r.YeuCauTiepNhanPhongKhams)
            //           .HasForeignKey(rf => rf.PhongKhamId);

            //builder.HasOne(m => m.BacSiKetLuan)
            //    .WithMany(u => u.YeuCauTiepNhanBacSiKetLuans)
            //    .HasForeignKey(m => m.BacSiKetLuanId);

            //builder.HasOne(m => m.BacSiKham)
            //    .WithMany(u => u.YeuCauTiepNhanBacSiKhams)
            //    .HasForeignKey(m => m.BacSiKhamId);

            //builder.HasOne(m => m.BHTNCongTyBaoHiem)
            //    .WithMany(u => u.YeuCauTiepNhans)
            //    .HasForeignKey(m => m.BHTNCongTyBaoHiemId);

            builder.HasOne(m => m.BHYTGiayMienCungChiTra)
                .WithMany(u => u.YeuCauTiepNhans)
                .HasForeignKey(m => m.BHYTGiayMienCungChiTraId);

            //builder.HasOne(m => m.BHYTnoiDangKy)
            //    .WithMany(u => u.YeuCauTiepNhans)
            //    .HasForeignKey(m => m.BHYTnoiDangKyId);

            builder.HasOne(m => m.DanToc)
                .WithMany(u => u.YeuCauTiepNhans)
                .HasForeignKey(m => m.DanTocId);

            builder.HasOne(m => m.DoiTuongUuTienKhamChuaBenh)
                .WithMany(u => u.YeuCauTiepNhans)
                .HasForeignKey(m => m.DoiTuongUuTienKhamChuaBenhId);

            builder.HasOne(m => m.GiayChuyenVien)
                .WithMany(u => u.YeuCauTiepNhans)
                .HasForeignKey(m => m.GiayChuyenVienId);

            //builder.HasOne(m => m.Icdchinh)
            //    .WithMany(u => u.YeuCauTiepNhans)
            //    .HasForeignKey(m => m.IcdchinhId);

            builder.HasOne(m => m.NgheNghiep)
                .WithMany(u => u.YeuCauTiepNhans)
                .HasForeignKey(m => m.NgheNghiepId);

            builder.HasOne(m => m.NguoiLienHePhuongXa)
                .WithMany(u => u.YeuCauTiepNhanNguoiLienHePhuongXas)
                .HasForeignKey(m => m.NguoiLienHePhuongXaId);

            builder.HasOne(m => m.NguoiLienHeQuanHuyen)
                .WithMany(u => u.YeuCauTiepNhanNguoiLienHeQuanHuyens)
                .HasForeignKey(m => m.NguoiLienHeQuanHuyenId);

            builder.HasOne(m => m.NguoiLienHeTinhThanh)
                .WithMany(u => u.YeuCauTiepNhanNguoiLienHeTinhThanhs)
                .HasForeignKey(m => m.NguoiLienHeTinhThanhId);

            builder.HasOne(m => m.PhuongXa)
                .WithMany(u => u.YeuCauTiepNhanPhuongXas)
                .HasForeignKey(m => m.PhuongXaId);

            builder.HasOne(m => m.QuanHuyen)
                .WithMany(u => u.YeuCauTiepNhanQuanHuyens)
                .HasForeignKey(m => m.QuanHuyenId);

            builder.HasOne(m => m.TinhThanh)
                .WithMany(u => u.YeuCauTiepNhanTinhThanhs)
                .HasForeignKey(m => m.TinhThanhId);

            builder.HasOne(m => m.NguoiLienHeQuanHeNhanThan)
                .WithMany(u => u.YeuCauTiepNhans)
                .HasForeignKey(m => m.NguoiLienHeQuanHeNhanThanId);

            //builder.HasOne(m => m.NhanVienThanhToan)
            //    .WithMany(u => u.YeuCauTiepNhanNhanVienThanhToans)
            //    .HasForeignKey(m => m.NhanVienThanhToanId);

            builder.HasOne(m => m.NhanVienTiepNhan)
                .WithMany(u => u.YeuCauTiepNhanVienTiepNhans)
                .HasForeignKey(m => m.NhanVienTiepNhanId);

            builder.HasOne(m => m.QuocTich)
                .WithMany(u => u.YeuCauTiepNhans)
                .HasForeignKey(m => m.QuocTichId);

            builder.HasOne(rf => rf.NoiGioiThieu)
                .WithMany(r => r.YeuCauTiepNhans)
                .HasForeignKey(rf => rf.NoiGioiThieuId);

            builder.HasOne(rf => rf.HinhThucDen)
                .WithMany(r => r.YeuCauTiepNhans)
                .HasForeignKey(rf => rf.HinhThucDenId);

            builder.HasOne(rf => rf.DoiTuongUuDai)
                .WithMany(r => r.YeuCauTiepNhans)
                .HasForeignKey(rf => rf.DoiTuongUuDaiId);

            builder.HasOne(rf => rf.CongTyUuDai)
                .WithMany(r => r.YeuCauTiepNhans)
                .HasForeignKey(rf => rf.CongTyUuDaiId);

            builder.HasMany(m => m.YeuCauTiepNhanCongTyBaoHiemTuNhans)
                .WithOne(u => u.YeuCauTiepNhan)
                .HasForeignKey(m => m.YeuCauTiepNhanId)
                .IsRequired();

            builder.HasOne(rf => rf.NoiChuyen)
                .WithMany(r => r.YeuCauTiepNhans)
                .HasForeignKey(rf => rf.NoiChuyenId);

            builder.HasOne(rf => rf.GiayMienGiamThem)
                .WithMany(r => r.YeuCauTiepNhans)
                .HasForeignKey(rf => rf.GiayMienGiamThemId);


            builder.HasOne(rf => rf.NhanVienDuyetMienGiamThem)
                .WithMany(r => r.YeuCauTiepNhanNhanVienDuyetMienGiamThems)
                .HasForeignKey(rf => rf.NhanVienDuyetMienGiamThemId);

            builder.HasOne(rf => rf.NguoiGioiThieu)
                .WithMany(r => r.YeuCauTiepNhanNguoiGioiThieus)
                .HasForeignKey(rf => rf.NguoiGioiThieuId);

            builder.HasOne(rf => rf.LyDoTiepNhan)
                .WithMany(r => r.YeuCauTiepNhans)
                .HasForeignKey(rf => rf.LyDoTiepNhanId);

            builder.HasOne(rf => rf.YeuCauNhapVien)
                .WithMany(r => r.YeuCauTiepNhans)
                .HasForeignKey(rf => rf.YeuCauNhapVienId);

            builder.HasOne(rf => rf.YeuCauTiepNhanNgoaiTruCanQuyetToan)
                .WithMany(r => r.YeuCauTiepNhanNoiTruQuyetToans)
                .HasForeignKey(rf => rf.YeuCauTiepNhanNgoaiTruCanQuyetToanId);

            builder.HasOne(rf => rf.NgheNghiepCuaBo)
                .WithMany(r => r.YeuCauTiepNhanNgheNghiepCuaBos)
                .HasForeignKey(rf => rf.NgheNghiepCuaBoId);
            builder.HasOne(rf => rf.NgheNghiepCuaMe)
                .WithMany(r => r.YeuCauTiepNhanNgheNghiepCuaMes)
                .HasForeignKey(rf => rf.NgheNghiepCuaMeId);

            builder.HasOne(x => x.HopDongKhamSucKhoeNhanVien)
                .WithMany(y => y.YeuCauTiepNhans)
                .HasForeignKey(x => x.HopDongKhamSucKhoeNhanVienId);

            builder.HasOne(x => x.KSKNhanVienDanhGiaCanLamSang)
                .WithMany(y => y.YeuCauTiepNhanKSKNhanVienDanhGiaCanLamSangs)
                .HasForeignKey(x => x.KSKNhanVienDanhGiaCanLamSangId);

            builder.HasOne(x => x.KSKNhanVienKetLuan)
                .WithMany(y => y.YeuCauTiepNhanKSKNhanVienKetLuans)
                .HasForeignKey(x => x.KSKNhanVienKetLuanId);

            builder.HasOne(x => x.YeuCauTiepNhanKhamSucKhoe)
                .WithMany(y => y.YeuCauTiepNhanNgoaiTruKhamSucKhoes)
                .HasForeignKey(x => x.YeuCauTiepNhanKhamSucKhoeId);

            base.Configure(builder);
        }
    }
}
