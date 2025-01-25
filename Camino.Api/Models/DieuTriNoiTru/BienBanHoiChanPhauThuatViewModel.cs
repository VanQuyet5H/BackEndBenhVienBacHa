using System;
using System.Collections.Generic;
using System.Linq;
using Camino.Core.Domain;
using Camino.Core.Helpers;

namespace Camino.Api.Models.DieuTriNoiTru
{
    public class BienBanHoiChanPhauThuatViewModel
    {
        public BienBanHoiChanPhauThuatViewModel()
        {
            ThanhVienThamGia = new List<string>();
            NhomPhauThuat = new List<string>();
            NhomGayMe = new List<string>();
            ListPhieu = new List<PhieuHoiChanViewModel>();
            FileChuKy = new List<FileChuKyViewModel>();
        }

        public string ThongTinHoSo { get; set; }

        public DateTime? ThoiGianHoiChan { get; set; }

        public List<string> ThanhVienThamGia { get; set; }

        public string ChanDoan { get; set; }

        public string Summary { get; set; }

        public string DongMau { get; set; }
        
        public string NhomMau { get; set; }
        
        public string Ure { get; set; }
        
        public string Creatinin { get; set; }
        
        public string Got { get; set; }
        
        public string Gpt { get; set; }
        
        public string DienTim { get; set; }
        
        public string XnKhac { get; set; }

        public string KetLuan { get; set; }

        public string DuKienPhauThuat { get; set; }

        public List<string> NhomPhauThuat { get; set; }

        public List<string> NhomGayMe { get; set; }

        public string PhuongPhapGayMe { get; set; }

        public DateTime? ThoiGianPhauThuat { get; set; }

        public string DuKienKhac { get; set; }

        public DateTime? NgayThucHien { get; set; }

        public string BacSiDieuTri { get; set; }

        public string BacSiGayMe { get; set; }

        public string BacSiTruongKhoa { get; set; }

        public string BacSiDuyetMo { get; set; }

        public string BacSiThucHien { get; set; }

        public string NgayThucHienReadonly { get; set; }

        public string NguoiThucHienReadonly { get; set; }

        public long? IdNoiTruHoSo { get; set; }

        public List<PhieuHoiChanViewModel> ListPhieu { get; set; }

        public List<FileChuKyViewModel> FileChuKy { get; set; }
    }

    public class PhieuHoiChanViewModel : BaseViewModel
    {
        public DateTime NgayHoiChan { get; set; }

        public string NgayHoiChanDisplay => NgayHoiChan.ApplyFormatDateTime();

        public List<string> ThanhVienThamGia { get; set; }

        public string ThanhVienThamGiaDisplay => GetThanhVienThamGia(ThanhVienThamGia);

        private string GetThanhVienThamGia(List<string> thanhVienThamGias)
        {
            var res = string.Empty;
            if (thanhVienThamGias != null)
            {
                var thanhVienLast = thanhVienThamGias.LastOrDefault();
                foreach (var thanhVienThamGia in thanhVienThamGias)
                {
                    if (thanhVienThamGia == thanhVienLast)
                    {
                        res += thanhVienThamGia;
                    }
                    else
                    {
                        res += thanhVienThamGia;
                        res += ", ";
                    }
                }
                return res;
            }

            return string.Empty;
        }

        public string ChanDoan { get; set; }
    }

    public class FileChuKyViewModel : BaseViewModel
    {
        public string Ten { get; set; }

        public string Uid { get; set; }

        public string TenGuid { get; set; }

        public long KichThuoc { get; set; }

        public string DuongDan { get; set; }

        public string MoTa { get; set; }

        public Enums.LoaiTapTin? LoaiTapTin { get; set; }

        public string DuongDanTmp { get; set; }
    }

    public class BienBanHoiChanPhauThuatModel : BaseViewModel
    {
        public BienBanHoiChanPhauThuatModel()
        {
            FileChuKy = new List<FileChuKyBienBanHoiChanModel>();
        }
        public long YeuCauTiepNhanId { get; set; }
        public DateTime? ThoiDiemThucHien { get; set; }
        public Enums.LoaiHoSoDieuTriNoiTru LoaiHoSoDieuTriNoiTru { get; set; }
        public string ThongTinHoSo { get; set; }
        public long? NhanVienThucHienId { get; set; }
        public long? NoiThucHienId { get; set; }
        public DateTime? ThoiGianHoiChan { get; set; }
        public List<FileChuKyBienBanHoiChanModel> FileChuKy { get; set; }
      
    }
    public class FileChuKyBienBanHoiChanModel
    {
        public long NoiTruHoSoKhacId { get; set; }
        public string Ma { get; set; }
        public string Uid { get; set; }
        public string Ten { get; set; }
        public string TenGuid { get; set; }
        public long KichThuoc { get; set; }
        public string DuongDan { get; set; }
        public Enums.LoaiTapTin LoaiTapTin { get; set; }
        public string MoTa { get; set; }
    }
   
}
