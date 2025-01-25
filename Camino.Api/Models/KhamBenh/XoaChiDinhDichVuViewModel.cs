using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Camino.Api.Models.KhamBenh
{
    public class XoaChiDinhDichVuViewModel
    {
        public long DichVuId { get; set; }
        public long YeuCauKhamBenhId { get; set; }
        public bool IsKhamBenhDangKham { get; set; }
        public bool IsKhamDoanTatCa { get; set; }
        public byte[] LastModifiedYeuCauKhamBenh { get; set; }
        public string LyDoHuyDichVu { get; set; }
    }

    //BVHD-3745
    // dùng cho trường hợp xóa nhiều
    public class XoaNhieuDichVuViewModel
    {
        public XoaNhieuDichVuViewModel()
        {
            XoaNhieuChiDinhDichVuChiTiets = new List<XoaNhieuChiDinhDichVuChiTietViewModel>();
        }
        public List<XoaNhieuChiDinhDichVuChiTietViewModel> XoaNhieuChiDinhDichVuChiTiets { get; set; }
    }
    public class XoaNhieuChiDinhDichVuChiTietViewModel : XoaChiDinhDichVuViewModel
    {
        public long YeuCauTiepNhanId { get; set; }
        public bool LaDichVuKham { get; set; }
    }
}
