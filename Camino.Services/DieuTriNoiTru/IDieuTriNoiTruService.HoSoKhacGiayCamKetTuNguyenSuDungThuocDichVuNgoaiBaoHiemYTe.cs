using Camino.Core.Domain;
using Camino.Core.Domain.Entities.YeuCauKhamBenhs;
using Camino.Core.Domain.ValueObject.GiayCamKetTuNguyenSuDungThuocDichVuNgoaiBaoHiemYTe;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Camino.Services.DieuTriNoiTru
{
    public partial interface IDieuTriNoiTruService
    {
        List<DanhSachDichVuKyThuatThuocVatTuGrid> GetListGiayCamKetTuNguyenSuDungThuocDichVuNgoaiBaoHiemYTeGridAsync(long yeuCauTiepNhanId);
        GiayCamKetTuNguyenSuDungThuocDichVuNgoaiBaoHiemYTeGridVo GetThongTinGiayCamKetTuNguyenSuDungThuocDVNgoaiBHYT(long yeuCauTiepNhanId);
        Task<string> InPhieuGiayCamKetTuNguyenSuDungThuoc(XacNhanInPhieuGiayCamKetTuNguyenSuDungThuocDichVuNgoaiBaoHiemYTe xacNhanIn);
        string FormatTenDuocPhamNoiTru(YeuCauDuocPhamBenhVien p, long? duocPhamBenhVienPhanNhomId);
        string FormatSoLuongNoiTru(double soLuong, Enums.LoaiThuocTheoQuanLy? loaiThuoc);
    }
}
