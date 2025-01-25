using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Helpers;

namespace Camino.Core.Domain.ValueObject.DieuTriNoiTru
{
    public class TongHopYLenhPhieuDieuTriVo
    {
        public TongHopYLenhPhieuDieuTriVo()
        {
            TongHopYLenhDienBiens = new List<TongHopYLenhDienBienVo>();
        }
        //public long? NoiTruPhieuDieuTriId { get; set; }
        public long? NoiTruBenhAnId { get; set; }
        public DateTime? NgayYLenh { get; set; }
        public bool IsBenhAnDaKetThuc { get; set; }
        public bool IsNgayDieuTriKhongHopLe { get; set; }
        public bool IsDisableEdit => IsBenhAnDaKetThuc || IsNgayDieuTriKhongHopLe;//false; cập nhật kho cho sửa y lệnh của bệnh án đã kết thúc //NgayYLenh == null || NgayYLenh.Value.Date < DateTime.Now.Date; // cập chật cho phép sửa y lệnh trong quá khứ
        public bool CoYLenhThemMoi => TongHopYLenhDienBiens.Any(x => x.CoYLenhThemMoi); // là y lệnh y tá hoặc điều dưỡng thêm mới
        public List<TongHopYLenhDienBienVo> TongHopYLenhDienBiens { get; set; }
    }

    public class TongHopYLenhDienBienVo: GridItem
    {
        public TongHopYLenhDienBienVo()
        {
            TongHopYLenhDienBienChiTiets = new List<TongHopYLenhDienBienChiTietVo>();
        }
        
        public int GioYLenh { get; set; }
        public string GioYLenhDisplay => GioYLenh.ConvertIntSecondsToTime12h();
        public string DienBien { get; set; }
        public bool CoYLenhThemMoi => TongHopYLenhDienBienChiTiets.Any(x => x.IsYLenhThemMoi); // là y lệnh y tá hoặc điều dưỡng thêm mới

        public List<TongHopYLenhDienBienChiTietVo> TongHopYLenhDienBienChiTiets { get; set; }
    }

    public class TongHopYLenhDienBienChiTietVo : GridItem
    {
        //public long? NoiTruPhieuDieuTriId { get; set; }
        public long? NoiTruBenhAnId { get; set; }
        public string MoTaYLenh { get; set; }
        public int? GioYLenh { get; set; }
        public long? NhanVienChiDinhId { get; set; }
        public string NhanVienChiDinhDisplay { get; set; }
        public long? NoiChiDinhId { get; set; }
        public long? NhanVienXacNhanThucHienId { get; set; }
        public string NhanVienXacNhanThucHienDisplay { get; set; }
        public DateTime? ThoiDiemXacNhanThucHien { get; set; }
        public int? GioThucHien { get; set; }
        public bool? XacNhanThucHien { get; set; }
        public long? NhanVienCapNhatId { get; set; }
        public string NhanVienCapNhatDisplay { get; set; }
        public DateTime? ThoiDiemCapNhat { get; set; }
        public string ThoiDiemCapNhatDisplay { get; set; }



        public long? YeuCauDichVuKyThuatId { get; set; }
        public bool LaSuatAn { get; set; }
        public long? YeuCauVatTuBenhVienId { get; set; }
        public long? YeuCauTruyenMauId { get; set; }
        public long? NoiTruChiDinhDuocPhamId { get; set; }

        //BVHD-3575: cập nhật cho phép chỉ định dv khám từ nội trú
        public long? YeuCauKhamBenhId { get; set; }

        public bool IsDieuTriNoiTru { get; set; }

        public DateTime? NgayThucHien { get; set; }
        public bool IsQuaNgayThucHien => false; //DateTime.Now.Date > NgayThucHien.Value.Date; // cho phép sửa cả những y lệnh trọng quá khứ
        public bool IsQuaThoiGianYLenh => DateTime.Now.Date > NgayThucHien.Value.Date 
                                          || (DateTime.Now.Date == NgayThucHien.Value.Date 
                                              && GioYLenh != null
                                              && (DateTime.Now.Hour * 3600 +
                                                DateTime.Now.Minute * 60 +
                                                DateTime.Now.Second) > GioYLenh.Value);

        public bool IsDisabledYeuCauDichVuKyThuat { get; set; }
        //todo: cần bổ sung điều kiện
        public bool IsDisabled => (IsDisabledYeuCauDichVuKyThuat || YeuCauVatTuBenhVienId != null || YeuCauTruyenMauId != null || YeuCauKhamBenhId != null);
        public bool IsEdit => ((YeuCauDichVuKyThuatId == null || LaSuatAn) && YeuCauVatTuBenhVienId == null && YeuCauTruyenMauId == null && YeuCauKhamBenhId == null);

        public bool IsYLenhThemMoi => YeuCauDichVuKyThuatId == null && YeuCauVatTuBenhVienId == null && YeuCauTruyenMauId == null && NoiTruChiDinhDuocPhamId == null && YeuCauKhamBenhId == null;
    }
}
