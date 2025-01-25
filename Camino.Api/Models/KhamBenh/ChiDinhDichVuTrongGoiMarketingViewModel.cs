using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Camino.Core.Domain;

namespace Camino.Api.Models.KhamBenh
{
    public class ChiDinhGoiDichVuTheoBenhNhanViewModel : BaseViewModel
    {
        public ChiDinhGoiDichVuTheoBenhNhanViewModel()
        {
            DichVus = new List<ChiTietGoiDichVuChiDinhTheoBenhNhanViewModel>();
            DichVuKhongThems = new List<ChiDinhGoiDichVuTheoBenhNhanDichVuLoiViewModel>();
        }
        public long YeuCauTiepNhanId { get; set; }
        public long? YeuCauKhamBenhId { get; set; }
        public long? NoiTruPhieuDieuTriId { get; set; }
        public bool IsKhamBenhDangKham { get; set; }
        public bool ISPTTT { get; set; }
        public List<ChiTietGoiDichVuChiDinhTheoBenhNhanViewModel> DichVus { get; set; }
        public List<ChiDinhGoiDichVuTheoBenhNhanDichVuLoiViewModel> DichVuKhongThems { get; set; }
    }

    public class ChiTietGoiDichVuChiDinhTheoBenhNhanViewModel
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
    }

    public class ChiDinhGoiDichVuTheoBenhNhanDichVuLoiViewModel
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
