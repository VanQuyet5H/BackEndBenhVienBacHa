using System;
using System.Collections.Generic;
using Camino.Core.Domain;

namespace Camino.Api.Models.PhauThuatThuThuat
{
    public class EkipFormViewModel : BaseViewModel
    {
        public long? ICDTruocId { get; set; }

        public DateTime? ThoiGianPt { get; set; }

        public List<EkipViewModel> Ekips { get; set; }

        public long YcdvktId { get; set; }

        public long YctnId { get; set; }

        public long NoiThucHienId { get; set; }

        //BVHD-3877
        public string GhiChuCaPTTT { get; set; }
    }

    public class EkipViewModel
    {
        public Enums.EnumNhomChucDanh? NhomChucDanh { get; set; }

        public long? BacSiId { get; set; }

        public Enums.EnumVaiTroBacSi? VaiTroBacSi { get; set; }

        public Enums.EnumVaiTroDieuDuong? VaiTroDieuDuong { get; set; }

        public long IdDb { get; set; }
    }
}
