using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Helpers;

namespace Camino.Core.Domain.ValueObject.YeuCauTiepNhans
{
    public class ThongTinPhieuThu
    {
        public ThongTinPhieuThu()
        {
            ChiPhiKhamChuaBenhNoiTruVos = new List<ChiPhiKhamChuaBenhNoiTruVo>();
            ChiPhiKhamChuaBenhVos = new List<ChiPhiKhamChuaBenhVo>();
        }

        public long Id { get; set; }

        public string SoPhieu { get; set; }
        public bool LaPhieuChi { get; set; }
        public LoaiPhieuThuChiThuNgan? LoaiPhieuThuChiThuNgan { get; set; }
        public string LoaiPhieu => LoaiPhieuThuChiThuNgan?.GetDescription();
        public bool? DaHuy { get; set; }
        public bool? DaHoanUng { get; set; }
        public string PhieuHoanUng { get; set; }
        public string TinhTrang => DaHuy == true ? "Đã hủy" : "Đang hiệu lực";
        public decimal SoTien => TienMat.GetValueOrDefault(0) + ChuyenKhoan.GetValueOrDefault(0) + Pos.GetValueOrDefault(0);
        public string HinhThucThanhToan { get; set; }
        public decimal? TienMat { get; set; }
        public decimal? ChuyenKhoan { get; set; }
        public decimal? Pos { get; set; }
        public decimal? CongNo { get; set; }
        public decimal? TongChiPhi { get; set; }
        public decimal? BHYTThanhToan { get; set; }
        public decimal? MienGiam { get; set; }
        public decimal? BenhNhanThanhToan { get; set; }
        public decimal? TamUng { get; set; }
        public decimal? SoTienPhaiThuHoacChi { get; set; }
        public DateTime NgayThu { get; set; }
        public string NgayThuStr => NgayThu.ApplyFormatDateTimeSACH();
        public string NoiDungThu { get; set; }

        //biến này kiểm tra coi coi lúc hủy lấy lại phiếu thu chua
        public bool ConHanHuyPhieuThu => NgayThu.Date == DateTime.Now.Date;
        public bool DaThuHoi { get; set; }
        public long? NguoiThuHoiId { get; set; }
        public string NguoiThuHoi { get; set; }
        public DateTime? NgayThuHoi { get; set; }

        //Updated Date 19/03/2021
        public string NhanVienHuyPhieu { get; set; }
        public DateTime? NgayHuy { get; set; }
        public string NgayHuyStr => NgayHuy?.ApplyFormatDateTimeSACH();

        public bool? DaChuyenVoNoiTru { get; set; }
        public bool? ThuTienGoiDichVu { get; set; }

        public List<ChiPhiKhamChuaBenhNoiTruVo> ChiPhiKhamChuaBenhNoiTruVos { get; set; }
        public List<ChiPhiKhamChuaBenhVo> ChiPhiKhamChuaBenhVos { get; set; }
    }

    public enum LoaiPhieuThuChiThuNgan
    {

        [Description("Phiếu thu")]
        ThuTheoChiPhi = 1,
        [Description("Tạm ứng")]
        ThuTamUng = 2,
        [Description("Hoàn ứng")]
        HoanUng = 3,
        [Description("Hoàn thu")]
        HoanThu = 4,
        [Description("Phiếu chi")]
        PhieuChi = 5,
    }

    public class ThongTinPhieuThuVo
    {
        public long KeyId { get; set; }
        public string DisplayName { get; set; }
        public LoaiPhieuThuChiThuNgan? LoaiPhieuThuChiThuNgan { get; set; }
        public string LoaiPhieu => LoaiPhieuThuChiThuNgan?.GetDescription();
        public bool LaPhieuChi { get; set; }
        public DateTime? NgayLap { get; set; }
        public string NgayLapStr => NgayLap?.ApplyFormatDateTimeSACH();
        public string NguoiLap { get; set; }
        public bool? DaHuy { get; set; }
        public string TinhTrang => DaHuy == true ? "Đã hủy" : "Đang hiệu lực";
        public bool? DaHoanUng { get; set; }
        public string PhieuHoanUng { get; set; }
        public bool? DaThuHoi { get; set; }
        public bool TrongGoi { get; set; }
        public bool NgoaiTru { get; set; }
    }


}
