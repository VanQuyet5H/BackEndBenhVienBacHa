using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Camino.Api.Models.KhamBenh.ViewModelCheckValidators;
using Camino.Core.Domain;

namespace Camino.Api.Models.KhamBenh
{
    public class KhamBenhChiDinhDichVuKyThuatMultiselectViewModel
    {
        public KhamBenhChiDinhDichVuKyThuatMultiselectViewModel()
        {
            DichVuKyThuatTuGois = new List<ChiDinhDichVuTrungTuGoiViewModel>();
        }
        public long? NhomDichVuBenhVienId { get; set; }
        public long YeuCauTiepNhanId { get; set; }
        public long YeuCauKhamBenhId { get; set; }
        public long? PhieuDieuTriId { get; set; }
        public List<string> DichVuKyThuatBenhVienChiDinhs { get; set; }
        public bool IsKhamBenhDangKham { get; set; }
        public bool IsKhamDoanTatCa { get; set; }
        public Enums.HinhThucKhamBenh? LoaiDangNhap { get; set; }
        public List<ChiDinhDichVuTrungTuGoiViewModel> DichVuKyThuatTuGois { get; set; }
        public string BieuHienLamSang { get; set; }
        public string DichTeSarsCoV2 { get; set; }
    }
}
