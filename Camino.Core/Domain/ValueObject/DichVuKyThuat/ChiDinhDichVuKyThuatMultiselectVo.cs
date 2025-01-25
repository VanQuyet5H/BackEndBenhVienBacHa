using System;
using System.Collections.Generic;
using System.Text;

namespace Camino.Core.Domain.ValueObject.DichVuKyThuat
{
    public class ChiDinhDichVuKyThuatMultiselectVo
    {
        public ChiDinhDichVuKyThuatMultiselectVo()
        {
            NoiThucHienNgoaiVienTheoHopDongs = new List<LookupItemTemplateVo>();
        }
        public long? NhomDichVuBenhVienId { get; set; }
        public long YeuCauTiepNhanId { get; set; }
        public long? PhieuDieuTriId { get; set; }
        public long YeuCauKhamBenhId { get; set; }
        public bool? KhongTinhPhi { get; set; }
        public List<string> DichVuKyThuatBenhVienChiDinhs { get; set; }
        public bool ChuyenHangDoiSangLamChiDinh { get; set; }
        public long YeuCauDichVuKyThuatCuoiCungId { get; set; }
        public Enums.HinhThucKhamBenh? LoaiDangNhap { get; set; }

        public List<LookupItemTemplateVo> NoiThucHienNgoaiVienTheoHopDongs { get; set; }
    }
}
