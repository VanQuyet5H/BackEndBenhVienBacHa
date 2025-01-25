using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Camino.Api.Models.KetQuaSinhHieu;
using Camino.Core.Domain;

namespace Camino.Api.Models.KhamBenh.ViewModelCheckValidators
{
    public class YeuCauTiepNhanKhamBenhViewModel : BaseViewModel
    {
        public YeuCauTiepNhanKhamBenhViewModel()
        {
            KetQuaSinhHieus = new List<KetQuaSinhHieuViewModel>();
            ChuyenKhoaKhamSucKhoeChinhs = new List<Enums.ChuyenKhoaKhamSucKhoe>();
        }
        public string TrieuChungTiepNhan { get; set; }
        public Enums.PhanLoaiSucKhoe? KSKPhanLoaiTheLuc { get; set; }
        public KhamBenhBenhNhanViewModel BenhNhan { get; set; }
        public List<KetQuaSinhHieuViewModel> KetQuaSinhHieus { get; set; }

        // khám đoàn tất cả
        public string KSKKetQuaCanLamSang { get; set; }
        public string KSKDanhGiaCanLamSang { get; set; }
        public long? KSKNhanVienDanhGiaCanLamSangId { get; set; }
        public Enums.PhanLoaiSucKhoe? PhanLoaiSucKhoeId { get; set; }
        public string KSKKetLuanPhanLoaiSucKhoe { get; set; }
        public string KSKKetLuanGhiChu { get; set; }
        public string KSKKetLuanCacBenhTat { get; set; }
        public long? KSKNhanVienKetLuanId { get; set; }
        public DateTime? KSKThoiDiemKetLuan { get; set; }
        public string KetQuaKhamSucKhoeData { get; set; }
        public bool? IsKhamDoanTatCa { get; set; }

        // update mới: kiểm tra nếu đủ 5 chuyên khoa mới bắt valdation kết luận khám đoàn
        public bool? IsDuChuyenKhoaKhamSucKhoeChinh { get; set; }
        public List<Enums.ChuyenKhoaKhamSucKhoe> ChuyenKhoaKhamSucKhoeChinhs { get; set; }
    }
}
