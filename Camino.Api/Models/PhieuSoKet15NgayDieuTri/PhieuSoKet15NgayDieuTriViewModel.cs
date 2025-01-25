using Camino.Api.Models.DieuTriNoiTru;
using Camino.Core.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Camino.Api.Models.PhieuSoKet15NgayDieuTri
{
    //public class FileChuKyViewModel : BaseViewModel
    //{
    //    public string Ten { get; set; }

    //    public string Uid { get; set; }

    //    public string TenGuid { get; set; }

    //    public long KichThuoc { get; set; }

    //    public string DuongDan { get; set; }

    //    public string MoTa { get; set; }

    //    public Enums.LoaiTapTin? LoaiTapTin { get; set; }

    //    public string DuongDanTmp { get; set; }
    //}

    public class PhieuSoKet15NgayDieuTriNewViewModel : BaseViewModel
    {
        public PhieuSoKet15NgayDieuTriNewViewModel()
        {
            FileChuKy = new List<FileChuKyViewModel>();
        }

        public DateTime? TuNgay { get; set; }
        public string TuNgayString { get; set; }
        public DateTime? DenNgay { get; set; }

        public string DenNgayString { get; set; }
        public string DienBienLS { get; set; }

        public string XetNghiemCLS { get; set; }
        public string QuaTrinhDieuTri { get; set; }

        public string DanhGiaKQ { get; set; }
        public string HuongDieuTriTiep { get; set; }
        public DateTime? NgayThucHien { get; set; }
        public string NgayThucHienString { get; set; }

        public string BSDieuTri { get; set; }
        public string TruongKhoa { get; set; }
        public string NguoiThucHienText { get; set; }
        public string NgayThucHienText { get; set; }
        public long? INoiTruHoSoKhacId { get; set; }

        public long YeuCauTiepNhanId { get; set; }
        public List<FileChuKyViewModel> FileChuKy { get; set; }

        public bool CheckCreateOrCapNhat { get; set; }
        public string ChanDoan { get; set; }

        public bool? NhanVienTrongBVHayNgoaiBV { get; set; }
        public bool? NhanVienTrongBVHayNgoaiBVTruongKhoa { get; set; }

        public long?  BSDieuTriId { get; set; }

        public long?  TruongKhoaId { get; set; }

        public string HocHamHocViBsDieuTri{ get; set; }

        public string HocHamHocViTruongKhoa{ get; set; }

    }
    public class DsPhieu15Ngay
    {
        public string TuNgay { get; set; }
        public string DenNgay { get; set; }
        public long NoiTruHoSoKhacId { get; set; }
    }
}
