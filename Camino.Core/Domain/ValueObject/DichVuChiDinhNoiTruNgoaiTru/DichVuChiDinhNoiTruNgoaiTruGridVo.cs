using Camino.Core.Domain.Entities.YeuCauKhamBenhs;
using Camino.Core.Domain.ValueObject.Grid;
using System;
using System.Collections.Generic;
using System.Text;

namespace Camino.Core.Domain.ValueObject.DichVuChiDinhNoiTruNgoaiTru
{
    public class DichVuChiDinhNoiTruNgoaiTruGridVo :GridItem
    {
        public DichVuChiDinhNoiTruNgoaiTruGridVo() {
            YeuCauDichVuKyThuats = new List<YeuCauDichVuKyThuat>();
        }

        public List<YeuCauDichVuKyThuat> YeuCauDichVuKyThuats { get; set; }
    }
    public class DichVuChiDinhTheoNguoiChiDinhGridVo
    {
        public long NhomChiDinhId { get; set; }
        public long DichVuChiDinhId { get; set; }
        public string TenNhom { get; set; }
        public int ThuTuIn { get; set; }
        public long NhanVienChiDinhId { get; set; }
        public DateTime ThoiDiemChiDinh { get; set; }
    }
}
