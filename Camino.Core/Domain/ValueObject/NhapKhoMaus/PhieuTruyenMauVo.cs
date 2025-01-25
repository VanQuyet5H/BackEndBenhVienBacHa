using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Camino.Core.Helpers;
using Microsoft.EntityFrameworkCore.Internal;

namespace Camino.Core.Domain.ValueObject.NhapKhoMaus
{
    public class PhieuTruyenMauVo
    {
        public PhieuTruyenMauVo()
        {
            KetQuaXetNghiemKhacs = new List<KetQuaXetNghiemKhacVo>();
        }
        public string HeaderPhieuTruyenMau { get; set; }
        public string SoVaoVien { get; set; }

        public string BenhNhanHoTen { get; set; }
        public string BenhNhanMaSo { get; set; }
        public int? NgaySinh { get; set; }
        public int? ThangSinh { get; set; }
        public int? NamSinh { get; set; }
        public string NguoiLamXetNghiemHoaHop { get; set; }
        
        public string BenhNhanTuoi
        {
            get
            {
                var start = DateTime.Now;
                var ngaySinhDayDu = NgaySinh != null && ThangSinh != null && NgaySinh != 0 && ThangSinh != 0 ? new DateTime(NamSinh.Value, ThangSinh.Value, NgaySinh.Value) : new DateTime(NamSinh.Value, 1, 1);
                var thangTuoi = ((start.Year * 12 + start.Month) - (ngaySinhDayDu.Year * 12 + ngaySinhDayDu.Month));
                //var chenhLechNgayTuoi = start.Day - ngaySinhDayDu.Day;
                if (thangTuoi > 72)// && chenhLechNgayTuoi >= 0) // 6 tuổi
                {
                    return (DateTime.Now.Year - NamSinh.Value).ToString();
                }
                else
                {
                    return thangTuoi + " tháng";
                }
            }
        }
        public string BenhNhanGioiTinh { get; set; }
        public string BenhNhanNhomMau { get; set; }
        public string BenhNhanYeuToRh { get; set; }

        public string ChanDoan { get; set; }
        public string KhoaPhong { get; set; }
        public string Giuong { get; set; }

        public string ChePhamMau { get; set; }
        public string ChePhamMauNhomMau { get; set; }
        public string ChePhamMauYeuToRh { get; set; }
        public string SoLuong { get; set; }
        public string MaSoDonVi { get; set; }
        public string NgayNhanMau { get; set; }
        public string HanSuDung { get; set; }
        public string NgayXetNghiemHoaHop { get; set; }
        public string NoiXetNghiemHoaHop { get; set; }


        public List<KetQuaXetNghiemKhacVo> KetQuaXetNghiemKhacs { get; set; }
        public bool IsKetQuaMoiTruongMuoiAmTinh { get; set; }
        public Enums.EnumKetQuaXetNghiem? KetQuaMoiTruongMuoi => KetQuaXetNghiemKhacs.Where(x => x.LoaiXetNghiem == Enums.LoaiXetNghiemMauNhapThem.KetQuaXetNghiemMoiTruongMuoi).Select(x => x.KetQua).FirstOrDefault();
        public string KetQuaMoiTruongMuoiAmTinh => KetQuaMoiTruongMuoi == Enums.EnumKetQuaXetNghiem.AmTinh ? "Âm tính" : null; //IsKetQuaMoiTruongMuoiAmTinh ? "Âm tính" : null;
        public string KetQuaMoiTruongMuoiDuongTinh => KetQuaMoiTruongMuoi == Enums.EnumKetQuaXetNghiem.DuongTinh ? "Dương tính" : null; /*!IsKetQuaMoiTruongMuoiAmTinh ? "Dương tính" : null;*/
        public bool IsKetQua37oCKhangGlobulinAmTinh { get; set; }
        public Enums.EnumKetQuaXetNghiem? KetQua37oCKhangGlobulin => KetQuaXetNghiemKhacs.Where(x => x.LoaiXetNghiem == Enums.LoaiXetNghiemMauNhapThem.KetQuaXetNghiem37oCKhangGlubulin).Select(x => x.KetQua).FirstOrDefault();
        public string KetQua37oCKhangGlobulinAmTinh => KetQua37oCKhangGlobulin == Enums.EnumKetQuaXetNghiem.AmTinh ? "Âm tính" : null;  //IsKetQua37oCKhangGlobulinAmTinh ? "Âm tính" : null;
        public string KetQua37oCKhangGlobulinDuongTinh => KetQua37oCKhangGlobulin == Enums.EnumKetQuaXetNghiem.DuongTinh ? "Dương tính" : null; /*!IsKetQua37oCKhangGlobulinAmTinh ? "Dương tính" : null;*/

        public string DiaDiem { get; set; }
        public string Gio { get; set; }
        public string Phut { get; set; }
        public string Ngay { get; set; }
        public string Thang { get; set; }
        public string Nam { get; set; }
        public string HoTenNguoiPhatMau { get; set; }
    }

    public class KetQuaXetNghiemKhacVo
    {
        public Enums.LoaiXetNghiemMauNhapThem? LoaiXetNghiem { get; set; }
        public string TenLoaiXetNghiem { get; set; }
        public Enums.EnumKetQuaXetNghiem? KetQua { get; set; }
        public string HienThiKetQuaKhac => LoaiXetNghiem.GetDescription() + " (" + KetQua.GetDescription() + ")";
    }
}
