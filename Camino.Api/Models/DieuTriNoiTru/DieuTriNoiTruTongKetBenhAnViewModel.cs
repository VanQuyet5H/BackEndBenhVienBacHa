using Camino.Core.Domain;
using Camino.Core.Domain.ValueObject.NoiTruBenhAn;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static Camino.Core.Domain.Enums;
using Camino.Core.Helpers;

namespace Camino.Api.Models.DieuTriNoiTru
{
    public class DieuTriNoiTruTongKetBenhAnViewModel : BaseViewModel
    {
        public DieuTriNoiTruTongKetBenhAnViewModel()
        {
            gridPhauThuatThuThuat = new List<GridPhauThuatThuThuatViewModel>();
            DacDiemTreSoSinhs = new List<DacDiemTreSoSinh>();
        }
        public long? YeuCauTiepNhanId { get; set; }
        public string QuaTrinhBenhLy { get; set; }
        public string XNMau { get; set; }
        public string XQuang { get; set; }
        public string XNTeBao { get; set; }
        public string SieuAm { get; set; }
        public string XNBLGP { get; set; }
        public string XNKhac { get; set; }
        public string PhuongPhapDieuTri { get; set; }
        public string TinhTrangNguoiBenhKhiRaVien { get; set; }
        public string HuongDieuTri { get; set; }

        public int? PhauThuatThuThuat { get; set; }

        #region grid phau thuat / thu thuat
        public List<GridPhauThuatThuThuatViewModel> gridPhauThuatThuThuat { get; set; }
        #endregion grid phau thuat / thu thuat

        #region grid
        public int? SoToXQuang { get; set; }
        public int? SoToCTScanner { get; set; }
        public int? SoToSieuAm { get; set; }
        public int? SoToXetNghiem { get; set; }
        public int? SoToKhac { get; set; }
        #endregion grid


        public DateTime? VaoBuongDeLuc { get; set; }
        public int? NhanVienTheoDoiId { get; set; }
        public string TenNhanVienTheoDoi { get; set; }
        public int? ChucDanhId { get; set; }
        public string TenChucDanh { get; set; }

        public string TinhTrangSauKhiDe { get; set; }
        public string XuLyKetQua { get; set; }
        public bool? Boc { get; set; }
        public bool? So { get; set; }
        public DateTime? RauSoLuc { get; set; }
        public string CachSoRau { get; set; }
        public string MatMang { get; set; }
        public string MatMui { get; set; }
        public string BanhRau { get; set; }
        public double? CanNang { get; set; }
        public bool? RauCuonCo { get; set; }
        public double? CuonRauDai { get; set; }
        public bool? CoChayMauSauSo { get; set; }
        public double? LuongMauMat { get; set; }
        public bool? KiemSoatTuCung { get; set; }
        public string XuLyKetQuaSoRau { get; set; }

        public string DaMienMac { get; set; }
        public Enums.PhuongPhapDe? PhuongPhapDeId { get; set; }
        public string TenPhuongPhapDe { get; set; }
        public string LyDoCanThiep { get; set; }

        public Enums.TangSinhMon? TangSinhMonId { get; set; }
        public string TenTangSinhMon { get; set; }
        public string PhuongPhapKhauVaLoaiChi { get; set; }
        public int? SoMuiKhau { get; set; }
        public Enums.CoTuCung? CoTuCungId { get; set; }
        public string TenCoTuCung { get; set; }
        public string ChanDoanTruocPhauThuat { get; set; }
        public string ChanDoanSauPhauThuat { get; set; }

        public List<GridPhauThuatThuThuatViewModel> LanPhauThuats { get; set; }

        public bool? TrieuChung { get; set; }
        public bool? TaiBien { get; set; }
        public bool? BienChung { get; set; }
        public bool? DoPhauThuat { get; set; }
        public bool? DoGayMe { get; set; }
        public bool? DoViKhuan { get; set; }
        public bool? DoKhac { get; set; }
        public string HuongDieuTriCacCheDoTiepTheo { get; set; }

        public List<ChiSoSinhTon> ChiSoSinhTons { get; set; }
        public List<DacDiemTreSoSinh> DacDiemTreSoSinhs { get; set; }
    }

    public class GridPhauThuatThuThuatViewModel : BaseViewModel
    {
        public long IdView { get; set; }
        public DateTime? PTTTNgayGio { get; set; }
        public string PTTTPhuongPhap { get; set; }
        public string PTTT { get; set; }
        public string VoCam { get; set; }
        public long? PTTTPhauThuatVien { get; set; }
        public long? PTTTBacSyGayMe { get; set; }
    }

    public class TaoDacDiemTreSoSinhChoMe
    {
        public long YeuCauTiepNhanMeId { get; set; }       
        public DacDiemTreSoSinh DacDiemTreSoSinh { get; set; }
    }

    public class ThongTinTheoDoiSoSinhDuocChon
    {
        public long YeuCauTiepNhanMeId { get; set; }
        public long? YeuCauTiepNhanConId { get; set; }
        public List<DacDiemTreSoSinh> DacDiemTreSoSinhs { get; set; }
    }

    public class DacDiemTreSoSinh : BaseViewModel
    {
        public DateTime? DeLuc { get; set; }
        public string DeLucDisplayName => DeLuc?.ToString("dd/MM/yyy hh:mm:ss");

        public EnumGioiTinh? GioiTinhId { get; set; }
        public string GioiTinh { get; set; }
        public EnumTrangThaiSong? TinhTrangId { get; set; }
        public string TenTinhTrang { get; set; }
        public string DiTat { get; set; }
        public double? CanNang { get; set; }
        public double? Cao { get; set; }
        public int? VongDau { get; set; }
        public bool? CoHauMon { get; set; }

        //khách hàng muốn nhập 1 lúc 3 chỉ số => 1,5,10 phut 
        public int? ApGar => 1;
        public int? ChiSoApGar { get; set; }

        public int? ApGar5 => 2;
        public int? ChiSoApGar5 { get; set; }

        public int? ApGar10 => 3;
        public int? ChiSoApGar10 { get; set; }

        public string TinhTrang { get; set; }
        public string KetQuaXuLy { get; set; }
      
        public long? YeuCauTiepNhanConId { get; set; }
    }

    public class PhuongPhapPTTTVoCamViewModel
    {
        public bool IsPhuongPhapPTTT { get; set; }
        public string Ten { get; set; }
    }
}
