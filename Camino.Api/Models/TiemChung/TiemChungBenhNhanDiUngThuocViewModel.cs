using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Camino.Core.Domain;
using Camino.Core.Helpers;

namespace Camino.Api.Models.TiemChung
{
    public class TiemChungBenhNhanDiUngThuocViewModel : BaseViewModel
    {
        public string TenDiUng { get; set; }
        public string Ma { get; set; }
        public long BenhNhanId { get; set; }
        public string BieuHienDiUng { get; set; }
        public Enums.LoaiDiUng? LoaiDiUng { get; set; }

        public string TenLoaiDiUng
        {
            get { return LoaiDiUng != null ? LoaiDiUng.GetDescription() : null; }
        }

        public Enums.EnumMucDoDiUng? MucDo { get; set; }

        public string TenMucDo
        {
            get { return MucDo != null ? MucDo.GetDescription() : null; }
        }

        public TiemChungBenhNhanViewModel BenhNhan { get; set; }
    }
}
