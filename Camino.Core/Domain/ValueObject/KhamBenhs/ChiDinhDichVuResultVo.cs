using System;
using System.Collections.Generic;
using System.Text;
using Camino.Core.Helpers;

namespace Camino.Core.Domain.ValueObject.KhamBenhs
{
    public class ChiDinhDichVuResultVo
    {
        public Enums.EnumTrangThaiYeuCauKhamBenh? TrangThaiYeuCauKhamBenh { get; set; }
        public bool ChuyenHangDoiSangLamChiDinh { get; set; }
        public decimal SoDuTaiKhoan { get; set; }
        public string SoDuTaiKhoanDisplay
        {
            get { return SoDuTaiKhoan.ApplyFormatMoneyVND(); }
        }

        public decimal SoDuTaiKhoanConLai { get; set; }
        public string SoDuTaiKhoanConLaiDisplay
        {
            get { return SoDuTaiKhoanConLai.ApplyFormatMoneyVND(); }
        }
        public bool IsVuotQuaSoDuTaiKhoan { get; set; }
        public bool? IsVuotQuaBaoLanhGoi { get; set; }
        public byte[] LastModified { get; set; }

        //Cập nhật 04/07/2022: bật cờ trường hợp thêm loại lĩnh bù -> xử lý xuất luôn
        public bool? LaLinhBu { get; set; }
    }
}
