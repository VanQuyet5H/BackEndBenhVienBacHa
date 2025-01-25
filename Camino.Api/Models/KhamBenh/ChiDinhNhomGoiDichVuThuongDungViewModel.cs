using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Camino.Core.Domain;
using Camino.Core.Domain.Entities.YeuCauKhamBenhs;
using Camino.Core.Domain.ValueObject.KhamBenhs;
using Camino.Core.Domain.ValueObject.YeuCauKhamBenh;

namespace Camino.Api.Models.KhamBenh
{
    public class ChiDinhNhomGoiDichVuThuongDungViewModel : BaseViewModel
    {
        public ChiDinhNhomGoiDichVuThuongDungViewModel()
        {
            GoiDichVuIds = new List<long>();
            DichVuKhongThems = new List<ChiDinhGoiDichVuThuongDungDichVuLoiViewModel>();
        }
        public long YeuCauTiepNhanId { get; set; }
        public long YeuCauKhamBenhId { get; set; }
        public bool IsKhamBenhDangKham { get; set; }
        public bool? LaPhauThuatThuThuat { get; set; }
        public long? PhieuDieuTriId { get; set; }
        public List<long> GoiDichVuIds { get; set; }
        public List<ChiDinhGoiDichVuThuongDungDichVuLoiViewModel> DichVuKhongThems { get; set; }
        public List<ChiDinhDichVuGridVo> ChiDinhDichVuGridVos { get; set; }
    }

    public class ChiDinhGoiDichVuThuongDungDichVuLoiViewModel
    {
        public long GoiDichVuId { get; set; }
        public Enums.EnumNhomGoiDichVu NhomGoiDichVu { get; set; }
        public long DichVuId { get; set; }
        public Enums.LoaiLoiGoiDichVu LoaiLoi { get; set; }
        public string TenLoi { get; set; }
        public bool KhongThem { get; set; }
    }

    public class ChiDinhNhomGoiDichVuThuongDungTaoYCTNViewModel : BaseViewModel
    {
        public ChiDinhNhomGoiDichVuThuongDungTaoYCTNViewModel()
        {
            GoiDichVuIds = new List<long>();
            DanhSachDichVuChons= new List<DichVuDaChonYCTNVo>();
            DichVuKhongThems = new List<ChiDinhGoiDichVuThuongDungDichVuLoiViewModel>();
        }
        public bool? DuocHuongBaoHiem { get; set; }
        public List<long> GoiDichVuIds { get; set; }
        public List<DichVuDaChonYCTNVo> DanhSachDichVuChons { get; set; }
        public List<ChiDinhGoiDichVuThuongDungDichVuLoiViewModel> DichVuKhongThems { get; set; }
    }
    public class DichVuChiDinhYCTNViewModel 
    {
        public DichVuChiDinhYCTNViewModel()
        {
            YeuCauDichVuKyThuatIds = new List<long>();
        }
        public List<long> YeuCauDichVuKyThuatIds { get; set; }
        
    }
    public class DichVuChiDinhBacSyKhacChiDinhViewModel
    {
        public DichVuChiDinhBacSyKhacChiDinhViewModel()
        {
            YeuCauDichVuKyThuatIds = new List<long>();
        }
        public List<long> YeuCauDichVuKyThuatIds { get; set; }
        public string Hosting { get; set; }
        public long YeuCauTiepNhanId { get; set; }

    }
}
