using Camino.Core.Helpers;
using System;
using System.Collections.Generic;
using static Camino.Core.Domain.Enums;

namespace Camino.Api.Models.TiemChung
{
    public class TiemChungYeuCauDichVuKyThuatKhamSangLocViewModel : BaseViewModel
    {
        public TiemChungYeuCauDichVuKyThuatKhamSangLocViewModel()
        {
            TiemChungTheoDoiSauTiem = new TiemChungTheoDoiSauTiemViewModel();
            YeuCauDichVuKyThuats = new List<YeuCauKhamTiemChungViewModel>();
            KetQuaSinhHieus = new List<TiemChungKetQuaSinhHieuViewModel>();
            //TiemChungYeuCauDichVuKyThuatTiemChungs = new List<TiemChungYeuCauDichVuKyThuatTiemChungViewModel>();
        }

        public string ThongTinKhamSangLocTiemChungTemplate { get; set; }
        public string ThongTinKhamSangLocTiemChungData { get; set; }
        public bool BenhNhanDeNghi { get; set; }
        public string LyDoDeNghi { get; set; }
        public LoaiKetLuanKhamSangLocTiemChung? KetLuan { get; set; }
        public string KetLuanDisplay => KetLuan?.GetDescription();
        public string GhiChuKetLuan { get; set; }
        public int? SoNgayHenTiemMuiTiepTheo { get; set; }
        public DateTime? NgayHenTiemMuiTiepTheo { get; set; }
        public string GhiChuHenTiemMuiTiepTheo { get; set; }
        public long? NhanVienHoanThanhKhamSangLocId { get; set; }
        public string NhanVienHoanThanhKhamSangLocDisplay { get; set; }
        public DateTime? ThoiDiemHoanThanhKhamSangLoc { get; set; }
        public long? NoiTheoDoiSauTiemId { get; set; }

        public TiemChungTheoDoiSauTiemViewModel TiemChungTheoDoiSauTiem { get; set; }
        //public List<TiemChungYeuCauDichVuKyThuatTiemChungViewModel> TiemChungYeuCauDichVuKyThuatTiemChungs { get; set; }
        public List<YeuCauKhamTiemChungViewModel> YeuCauDichVuKyThuats { get; set; }
        public List<TiemChungKetQuaSinhHieuViewModel> KetQuaSinhHieus { get; set; }
    }
}