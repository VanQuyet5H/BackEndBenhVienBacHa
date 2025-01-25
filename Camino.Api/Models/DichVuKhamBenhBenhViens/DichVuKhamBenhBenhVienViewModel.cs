using Camino.Api.Models.DichVuKhamBenh;
using Camino.Api.Models.KhoaPhong;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Camino.Api.Models.DichVuKhamBenhBenhViens
{
    public class DichVuKhamBenhBenhVienViewModel: BaseViewModel
    {
        public DichVuKhamBenhBenhVienViewModel()
        {
            DichVuKhamBenhBenhVienGiaBaoHiems = new List<DichVuKhamBenhBenhVienGiaBaoHiemViewModel>();
            DichVuKhamBenhBenhVienGiaBenhViens = new List<DichVuKhamBenhBenhVienGiaBenhVienViewModel>();
            KhoaPhongIds = new List<long>();
            NoiThucHienIds = new List<string>();
        }

        public string Ten { get; set; }
        public string Ma { get; set; }
        public bool? AnhXa { get; set; }
        public long? DichVuKhamBenhId { get; set; }
        public string DichVuKhamBenhModelText { get; set; }
        public string KhoaPhongModelText { get; set; }
        public long? KhoaPhongId { get; set; }
        public List<long> KhoaPhongIds { get; set; }
        public List<string> NoiThucHienIds { get; set; }
        public string MoTa { get; set; }
        public bool HieuLuc { get; set; }
        public KhoaPhongViewModel KhoaPhong { get; set; }
        public DichVuKhamBenhViewModel DichVuKhamBenh { get; set; }
        public List<DichVuKhamBenhBenhVienGiaBaoHiemViewModel> DichVuKhamBenhBenhVienGiaBaoHiems { get; set; }
        public List<DichVuKhamBenhBenhVienGiaBenhVienViewModel> DichVuKhamBenhBenhVienGiaBenhViens { get; set; }
    }
}
