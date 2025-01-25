using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Camino.Api.Models.DanToc;
using Camino.Api.Models.DonViHanhChinh;
using Camino.Api.Models.KetQuaSinhHieu;
using Camino.Api.Models.LyDoTiepNhan;
using Camino.Api.Models.NgheNghiep;
using Camino.Core.Domain;
using Camino.Core.Helpers;

namespace Camino.Api.Models.TiemChung
{
    public class TiemChungYeuCauTiepNhanViewModel : BaseViewModel
    {
        public TiemChungYeuCauTiepNhanViewModel()
        {
            //KetQuaSinhHieus = new List<TiemChungKetQuaSinhHieuViewModel>();
            YeuCauTiepNhanDichVuKyThuats = new List<YeuCauKhamTiemChungViewModel>();
        }
        public string MaYeuCauTiepNhan { get; set; }
        public long? BenhNhanId { get; set; }
        public string HoTen { get; set; }
        public int? NgaySinh { get; set; }
        public int? ThangSinh { get; set; }
        public int? NamSinh { get; set; }

        public int? Tuoi
        {
            get { return NamSinh == null ? 0 : DateTime.Now.Year - NamSinh.Value; }
        }

        public string SoChungMinhThu { get; set; }
        public Enums.LoaiGioiTinh? GioiTinh { get; set; }

        public string TenGioiTinh
        {
            get { return GioiTinh == null ? null : GioiTinh.GetDescription(); }
        }
         
        public string DiaChi { get; set; }

        public string DiaChiDisplay
        {
            get
            {
                return AddressHelper.ApplyFormatAddress(TinhThanh?.Ten, QuanHuyen?.Ten, PhuongXa?.Ten, DiaChi);
            }
        }
        
        public string SoDienThoai { get; set; }
        public string SoDienThoaiDisplay
        {
            get { return SoDienThoai.ApplyFormatPhone(); }
        }

        public string BHYTMaSoThe { get; set; }
        public DateTime? BHYTNgayHieuLuc { get; set; }
        public DateTime? BHYTNgayHetHan { get; set; }
        public int? BHYTMucHuong { get; set; }
        public bool? CoBHYT { get; set; }
        public string BHYTThoiGianHieuLucDisplay => BHYTNgayHieuLuc?.ApplyFormatDate() + (BHYTNgayHieuLuc != null && BHYTNgayHetHan != null ? " - " : "") + BHYTNgayHetHan?.ApplyFormatDate();
        public bool IsBHYTHetHan => CoBHYT == true && BHYTNgayHieuLuc != null && BHYTNgayHetHan != null && (DateTime.Now.Date < BHYTNgayHieuLuc.Value.Date || DateTime.Now.Date > BHYTNgayHetHan.Value.Date);
        public string TenLyDoTiepNhan
        {
            get { return LyDoTiepNhan?.Ten; }
        }
        public DateTime? ThoiDiemTiepNhan { get; set; }
        public string ThoiDiemTiepNhanDisplay
        {
            get { return ThoiDiemTiepNhan?.ApplyFormatDateTimeSACH(); }
        }
        public Enums.EnumLyDoVaoVien? LyDoVaoVien { get; set; }
        public string TenLyDoVaoVien
        {
            get { return LyDoVaoVien == null ? null : LyDoVaoVien.GetDescription(); }
        }
        public decimal SoDuTaiKhoan { get; set; }
        public string SoDuTaiKhoanDisplay => SoDuTaiKhoan.ApplyFormatMoneyVND();
        public decimal SoDuTaiKhoanConLai { get; set; }
        public string SoDuTaiKhoanConLaiDisplay => SoDuTaiKhoanConLai.ApplyFormatMoneyVND();

        public long? HopDongKhamSucKhoeNhanVienId { get; set; }
        public long? HopDongKhamSucKhoeId { get; set; }

        //BVHD-3800
        public bool? LaCapCuu { get; set; }

        //BVHD-3941
        public bool? CoBaoHiemTuNhan { get; set; }
        public string TenCongTyBaoHiemTuNhan { get; set; }

        public virtual DonViHanhChinhViewModel PhuongXa { get; set; }
        public virtual DonViHanhChinhViewModel QuanHuyen { get; set; }
        public virtual DonViHanhChinhViewModel TinhThanh { get; set; }
        public virtual DanTocViewModel DanToc { get; set; }
        public virtual NgheNghiepViewModel NgheNghiep { get; set; }
        public virtual LyDoTiepNhanViewModel LyDoTiepNhan { get; set; }
        public virtual TiemChungBenhNhanViewModel BenhNhan { get; set; }

        //public List<TiemChungKetQuaSinhHieuViewModel> KetQuaSinhHieus { get; set; }
        public List<YeuCauKhamTiemChungViewModel> YeuCauTiepNhanDichVuKyThuats { get; set; }
    }
}
