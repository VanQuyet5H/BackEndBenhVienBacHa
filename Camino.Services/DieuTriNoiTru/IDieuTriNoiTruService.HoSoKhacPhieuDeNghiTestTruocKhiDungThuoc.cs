using Camino.Core.Domain.ValueObject.PhieuDeNghiTestTruocKhiDungThuoc;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Camino.Services.DieuTriNoiTru
{
    public partial interface IDieuTriNoiTruService
    {
        PhieuDeNghiTestTruocKhiDungThuocGridVo GetThongTinPhieuDeNghiTestTruocKhiDung(long yeuCauTiepNhan);
        Task<bool> KiemTraNamSinhHople(int? ten, long id = 0);
        Task<string> InPhieuDeNghiTestTruocKhiDungThuoc(InPhieuDeNghiTestTruocKhiDungThuoc xacNhanInTrichBienBanHoiChan);
        string GetFormatDuocPham(string tenThuongMai);
        string GetFormatSoLuong(string tenThuongMai, double soLuong);
    }
}
