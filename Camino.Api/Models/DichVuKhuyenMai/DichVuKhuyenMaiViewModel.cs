using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Camino.Core.Domain;
using Camino.Core.Domain.ValueObject.YeuCauKhamBenh;

namespace Camino.Api.Models.DichVuKhuyenMai
{
    public class ChiDinhGoiDichVuKhuyenMaiTheoBenhNhanViewModel : BaseViewModel
    {
        public ChiDinhGoiDichVuKhuyenMaiTheoBenhNhanViewModel()
        {
            DichVus = new List<ChiTietGoiDichVuKhuyenMaiChiDinhTheoBenhNhanViewModel>();
            DichVuKhongThems = new List<ChiDinhGoiDichVuKhuyenMaiTheoBenhNhanDichVuLoiViewModel>();
            ChiDinhDichVuGridVos = new List<ChiDinhDichVuGridVo>();
        }
        public long YeuCauTiepNhanId { get; set; }
        public long? YeuCauKhamBenhId { get; set; }
        public long? NoiTruPhieuDieuTriId { get; set; }
        public bool IsKhamBenhDangKham { get; set; }
        public bool ISPTTT { get; set; }
        public bool IsVacxin { get; set; }
        public List<ChiTietGoiDichVuKhuyenMaiChiDinhTheoBenhNhanViewModel> DichVus { get; set; }
        public List<ChiDinhGoiDichVuKhuyenMaiTheoBenhNhanDichVuLoiViewModel> DichVuKhongThems { get; set; }

        // dùng cho tiếp nhận
        public List<ChiDinhDichVuGridVo> ChiDinhDichVuGridVos { get; set; }
        public bool? DuocHuongBaoHiem { get; set; }
    }

    public class ChiTietGoiDichVuKhuyenMaiChiDinhTheoBenhNhanViewModel
    {
        public string Id { get; set; }
        public long YeuCauGoiDichVuId { get; set; }
        public long ChuongTrinhGoiDichVuId { get; set; }
        public long ChuongTrinhGoiDichVuChiTietId { get; set; }
        public long DichVuBenhVienId { get; set; }
        public string TenDichVu { get; set; }
        public int NhomGoiDichVu { get; set; }
        public int SoLuongSuDung { get; set; }
        public bool IsActive { get; set; }

        public Enums.ViTriTiem? ViTriTiem { get; set; }
        public int? MuiSo { get; set; }
        public long? NoiThucHienId { get; set; }
        public string LieuLuong { get; set; }
    }

    public class ChiDinhGoiDichVuKhuyenMaiTheoBenhNhanDichVuLoiViewModel
    {
        public long YeuCauGoiDichVuId { get; set; }
        public long ChuongTrinhGoiDichVuId { get; set; }
        public long ChuongTrinhGoiDichVuChiTietId { get; set; }
        public int NhomGoiDichVuValue { get; set; }
        public long GoiDichVuId { get; set; }
        public Enums.EnumNhomGoiDichVu NhomGoiDichVu { get; set; }
        public long DichVuId { get; set; }
        public Enums.LoaiLoiGoiDichVu LoaiLoi { get; set; }
        public string TenLoi { get; set; }
        public bool KhongThem { get; set; }
    }
}
