using System;
using System.Collections.Generic;
using System.Text;

namespace Camino.Core.Domain.ValueObject.KhamBenhs
{
    public class UpdateGhiChuCanLamSangVo
    {
        public long YeuCauKhamBenhId { get; set; }
        public string GhiChuCanLamSang { get; set; }
    }
    

    public class ThongTinDichVuTrongGoi
    {
        public long BenhNhanId { get; set; }
        public long DichVuId { get; set; }
        public int SoLuong { get; set; }
        public Enums.EnumNhomGoiDichVu NhomGoiDichVu { get; set; }
        public decimal DonGia { get; set; }
        public decimal? DonGiaTruocChietKhau { get; set; }
        public decimal? DonGiaSauChietKhau { get; set; }
        public long? YeuCauGoiDichVuId { get; set; }

        //BVHD-3825
        public decimal? DonGiaKhuyenMai { get; set; }
        public long NhomGiaId { get; set; }

        //chỉ dùng cho trường hợp cập nhật số lượng hoặc loại giá thôi
        public long? YeuCauDichVuCapNhatSoLuongLoaiGiaId { get; set; }
        public long? YeucauGoiDichVuKhuyenMaiId { get; set; }
    }
}
