using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Camino.Core.Domain;
using static Camino.Core.Domain.Enums;

namespace Camino.Api.Models.TiemChung
{
    public class TiemChungChiDinhGoiDichVuTheoBenhNhanViewModel : BaseViewModel
    {
        public TiemChungChiDinhGoiDichVuTheoBenhNhanViewModel()
        {
            DichVus = new List<TiemChungChiTietGoiDichVuChiDinhTheoBenhNhanViewModel>();
            DichVuKhongThems = new List<TiemChungChiDinhGoiDichVuTheoBenhNhanDichVuLoiViewModel>();
        }
        public long YeuCauTiepNhanId { get; set; }
        public long? YeuCauKhamBenhId { get; set; }
        public long? NoiTruPhieuDieuTriId { get; set; }
        public long? YeuCauDichVuKyThuatKhamSangLocTiemChungId { get; set; }
        public bool IsKhamBenhDangKham { get; set; }
        public bool ISPTTT { get; set; }
        public List<TiemChungChiTietGoiDichVuChiDinhTheoBenhNhanViewModel> DichVus { get; set; }
        public List<TiemChungChiDinhGoiDichVuTheoBenhNhanDichVuLoiViewModel> DichVuKhongThems { get; set; }
    }

    public class TiemChungChiTietGoiDichVuChiDinhTheoBenhNhanViewModel
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
        public ViTriTiem? ViTriTiem { get; set; }
        public int? MuiSo { get; set; }
        public long? NoiThucHienId { get; set; }
        public string LieuLuong { get; set; }
    }

    public class TiemChungChiDinhGoiDichVuTheoBenhNhanDichVuLoiViewModel
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
