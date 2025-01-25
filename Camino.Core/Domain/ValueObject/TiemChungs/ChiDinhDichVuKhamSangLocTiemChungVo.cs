using Camino.Core.Helpers;

namespace Camino.Core.Domain.ValueObject.TiemChungs
{
    public class ChiDinhDichVuKhamSangLocTiemChungVo
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
        //public bool IsKhacKetLuanVoiBYT { get; set; }
        public byte[] LastModified { get; set; }
    }
}