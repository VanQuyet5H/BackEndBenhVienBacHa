using System;
using System.Collections.Generic;
using System.Text;

namespace Camino.Core.Domain.ValueObject.KhamDoan
{
    public class DichVuTrongGoiKhamSucKhoeLookupInfoVo
    {
        public long HopDongKhamSucKhoeNhanVienId { get; set; }
        public long GoiKhamSucKhoeId { get; set; }
    }

    public class ThongTinGiaDichVuTrongGoiKhamSucKhoeVo
    {
        public Enums.ChuyenKhoaKhamSucKhoe? ChuyenKhoaKhamSucKhoe { get; set; }
        public long NhomGiaDichVuBenhVienId { get; set; }
        public decimal DonGiaBenhVien { get; set; }
        public decimal DonGiaUuDai { get; set; }
        public decimal DonGiaChuaUuDai { get; set; }
        public string TenGoiKhamSucKhoe { get; set; }
        public long NoiThucHienId { get; set; }
    }
}
