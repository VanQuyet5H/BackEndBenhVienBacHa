using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Camino.Api.Models.YeuCauKhamBenhChanDoanPhanBiet;
using Camino.Api.Models.YeuCauKhamBenhKhamBoPhanKhac;
using Camino.Core.Domain;
using Camino.Core.Helpers;

namespace Camino.Api.Models.KhamBenh.ViewModelCheckValidators
{
    public class YeuCauKhamBenhKhamBenhViewModel: BaseViewModel
    {
        public YeuCauKhamBenhKhamBenhViewModel()
        {
            YeuCauKhamBenhKhamBoPhanKhacs = new List<YeuCauKhamBenhKhamBoPhanKhacViewModel>();
            YeuCauKhamBenhChanDoanPhanBiets = new List<YeuCauKhamBenhChanDoanPhanBietViewModel>();
            YeuCauKhamBenhBoPhanTonThuongs = new List<YeuCauKhamBenhBoPhanTonThuongViewModel>();
            YeuCauKhamBenhICDKhacs = new List<YeuCauKhamBenhICDKhacViewModel>();
            TemplateDichVuKhamSucKhoes = new List<KhamBenhTemplateDichVuKhamSucKhoeViewModel>();
        }

        public string ThongTinKhamTheoDichVuTemplate { get; set; }
        public string ThongTinKhamTheoDichVuData { get; set; }

        public string BenhSu { get; set; }

        public string KhamToanThan { get; set; }
        public string TuanHoan { get; set; }
        public string HoHap { get; set; }
        public string TieuHoa { get; set; }
        public string ThanTietNieuSinhDuc { get; set; }
        public string ThanKinh { get; set; }
        public string CoXuongKhop { get; set; }
        public string TaiMuiHong { get; set; }
        public string RangHamMat { get; set; }
        public string NoiTietDinhDuong { get; set; }
        public string SanPhuKhoa { get; set; }
        public string DaLieu { get; set; }
        public long? ChanDoanSoBoICDId { get; set; }
        public string TenChanDoanSoBoICD { get; set; }
        public string ChanDoanSoBoGhiChu { get; set; }
        public string ChanDoanCuaNoiGioiThieu { get; set; }
        
        //public long? IcdchinhId { get; set; }
        //public string GhiChuICDChinh { get; set; }

        public bool? IsHoanThanhKham { get; set; }
        public bool IsKhamBenhDangKham { get; set; }

        //khám đoàn
        public bool IsKhamDoan { get; set; }

        //BVHD-3574
        public string NoiDungKhamBenh { get; set; }

        //BVHD-3706
        public string TrieuChungTiepNhan { get; set; }

        public List<YeuCauKhamBenhKhamBoPhanKhacViewModel> YeuCauKhamBenhKhamBoPhanKhacs { get; set; }
        public List<YeuCauKhamBenhChanDoanPhanBietViewModel> YeuCauKhamBenhChanDoanPhanBiets { get; set; }
        public List<YeuCauKhamBenhBoPhanTonThuongViewModel> YeuCauKhamBenhBoPhanTonThuongs { get; set; }
        public List<YeuCauKhamBenhICDKhacViewModel> YeuCauKhamBenhICDKhacs { get; set; }
        public List<KhamBenhTemplateDichVuKhamSucKhoeViewModel> TemplateDichVuKhamSucKhoes { get; set; }
    }
}
