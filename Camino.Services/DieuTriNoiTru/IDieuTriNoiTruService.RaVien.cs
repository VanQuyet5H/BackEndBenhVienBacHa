using System;
using System.Collections.Generic;
using Camino.Core.Domain.ValueObject.RaVienNoiTru;
using Newtonsoft.Json;
using System.Linq;
using Camino.Core.Domain;
using Camino.Core.Domain.ValueObject.DieuTriNoiTru;
using System.Threading.Tasks;

namespace Camino.Services.DieuTriNoiTru
{
    public partial interface IDieuTriNoiTruService
    {
        RaVien GetRaVienNoiTruKhoaNoiKhoaNhi(long yeuCauTiepNhanId);
        void LuuHoacCapNhatRaVienNoiTruKhoaNoiKhoaNhi(RaVien thongTinBenhAnNoiKhoaNhi);

        RaVien GetRaVienNoiTruNgoaiKhoaThamMy(long yeuCauTiepNhanId);
        void LuuHoacCapNhatRaVienNoiKhoaThamMy(RaVien thongTinBenhAnNoiKhoaNhi);

        RaVien GetRaVienNoiTruPhuKhoa(long yeuCauTiepNhanId);
        void LuuHoacCapNhatRaVienNoiTruPhuKhoa(RaVien thongTinBenhAnNoiKhoaNhi);

        RaVien GetRaVienNoiTruSanKhoaMo(long yeuCauTiepNhanId);
        void LuuHoacCapNhatRaVienNoiTruSanKhoaMo(RaVien thongTinBenhAnNoiKhoaNhi);

        RaVien GetRaVienNoiTruSanKhoaThuong(long yeuCauTiepNhanId);
        void LuuHoacCapNhatRaVienNoiTruSanKhoaThuong(RaVien thongTinBenhAnNoiKhoaNhi);

        void KetThucBenhAn(KetThucBenhAnVo ketThucBenhAnVo);
        void MoLaiBenhAn(long yeuCauTiepNhanId);

        string KiemTraKhoaKhongChoRaVien(long yeuCauTiepNhanId);
        Task<bool> KiemTraNgayRaVien(long yeuCauTiepNhanId, DateTime? ngayRaVien);
        List<KiemTraThongTinKetThucBenhAnError> KiemTraThongTinKetThucBenhAn(long yeuCauTiepNhanId);
    }
}
