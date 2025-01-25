using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Camino.Core.Domain
{
    public partial class Enums
    {
        public enum TrangThaiThanhToan
        {
            [Description("Chưa thanh toán")]
            ChuaThanhToan = 1,
            [Description("Đã thanh toán")]
            DaThanhToan = 2,
            [Description("Cập nhật thanh toán")]
            CapNhatThanhToan = 3,
            [Description("Hủy thanh toán")]
            HuyThanhToan = 4,
            [Description("Bảo lãnh thanh toán")]
            BaoLanhThanhToan = 5,
            //[Description("Đang chờ nhận tiền")]
            //DangChoNhanTien = 6,
        }
        public enum TrangThaiThuNgan
        {
            [Description("Chưa thu")]
            ChuaThu = 1,
            [Description("Công nợ")]
            CongNo = 2,
            [Description("Hoàn ứng")]
            HoanUng = 3,
            [Description("Đã thu")]
            DaThu = 4
        }
        public enum LoaiThuTienBenhNhan
        {
            [Description("Thu theo chi phí")]
            ThuTheoChiPhi = 1,
            [Description("Thu tạm ứng")]
            ThuTamUng = 2,
            //[Description("Phiếu Chi")]
            //PhieuChi = 3,
            [Description("Thu nợ")]
            ThuNo = 4,
        }

        public enum LoaiNoiThu
        {
            [Description("Thu Ngân")]
            ThuNgan = 1,
            [Description("Nhà Thuốc")]
            NhaThuoc = 2
        }

        public enum LoaiChiTienBenhNhan
        {
            [Description("Thanh toán chi phí")]
            ThanhToanChiPhi = 1,
            [Description("Trả lại người bệnh")]
            TraLaiBenhNhan = 2,//khong dung nua
            [Description("Hoàn ứng")]
            HoanUng = 3,
            [Description("Hoàn thu")]
            HoanThu = 4,
        }
        public enum LoaiNoiDungChiTien
        {
            QuyetToanGoiMarketing = 1
        }
        public enum LoaiMienGiam
        {
            [Description("Ưu đãi")]
            UuDai = 1,
            [Description("Voucher")]
            Voucher = 2,
            [Description("Miễn giảm thêm")]
            MienGiamThem = 3,
        }
        public enum LoaiChietKhau
        {
            [Description("Chiết khấu theo tỉ lệ")]
            ChietKhauTheoTiLe = 1,
            [Description("Chiết khấu theo số tiền")]
            ChietKhauTheoSoTien = 2,
        }
        //TODO: need remove
        public enum LoaiVoucher
        {
            [Description("Dùng một lần")]
            DungMotLan = 1,
            [Description("Dùng nhiều lần")]
            DungNhieuLan = 2,
        }
        public enum LoaiPhieuThuChi
        {
            [Description("Phiếu thu")]
            PhieuThu = 1,
            [Description("Phiếu chi")]
            PhieuChi = 2,
        }
        public enum LyDoChuyenTuyen
        {
            [Description("Đủ điều kiện chuyển tuyến")]
            DuDieuKien = 1,
            [Description("Theo yêu cầu của người bệnh hoặc người đại diện hợp pháp của người bệnh")]
            TheoYeuCauBenhNhan = 2,
        }
        public enum LoaiGiaNoiGioiThieuHopDong
        {
            [Description("Giá bán")]
            GiaBan = 1,
            [Description("Giá nhập")]
            GiaNhap = 2
            
        }
        public enum LoaiHoaHong
        {
            [Description("Số tiền")]
            SoTien = 1,
            [Description("Tỉ lệ")]
            TiLe = 2

        }
    }
}
