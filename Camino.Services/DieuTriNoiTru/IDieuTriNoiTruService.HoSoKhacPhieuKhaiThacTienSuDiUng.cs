using Camino.Core.Domain.ValueObject.PhieuKhaiThacTienSuDiUng;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Camino.Services.DieuTriNoiTru
{
    public partial interface IDieuTriNoiTruService
    {
        PhieuKhaiThacTienSuDiUngConfig PhieuKhaiThacTienSuDiUngConfig();
        List<string> GetDanhSachDuocPhamQuocGia();
        List<string> GetDanhSachDuocPhamQuocGiaDeNghiTest();
        PhieuKhaiThacTienSuDiUngGridVo GetThongTinPhieuKhaiThacTienSuBenh(long yeuCauTiepNhanId);
        Task<string> PhieuKhaiThacTienSuBenh(XacNhanInTienSu xacNhanIn);
    }
}
