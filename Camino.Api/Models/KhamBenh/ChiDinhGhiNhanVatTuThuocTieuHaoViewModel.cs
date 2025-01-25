using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using static Camino.Core.Domain.Enums;

namespace Camino.Api.Models.KhamBenh
{
    public class ChiDinhGhiNhanVatTuThuocTieuHaoViewModel: BaseViewModel
    {
        public long YeuCauTiepNhanId { get; set; }
        public long YeuCauKhamBenhId { get; set; }
        public string DichVuChiDinhId { get; set; }
        public long? KhoId { get; set; }
        public string DichVuGhiNhanId { get; set; }
        public double? SoLuong { get; set; }
        public bool? TinhPhi { get; set; }
		public bool LaDuocPhamBHYT { get; set; }
        [NotMapped]
        public string strDonViTinh { get; set; }
        public bool IsKhamBenhDangKham { get; set; }
        public LoaiNoiChiDinh LoaiNoiChiDinh { get; set; }
    }
}
