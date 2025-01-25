using Camino.Core.Domain.Entities.NhapKhoQuaTangs;
using Camino.Core.Domain.Entities.QuaTangs;
using System;
using System.Collections.Generic;
using System.Text;

namespace Camino.Core.Domain.Entities.XuatKhoQuaTangs
{
    public class XuatKhoQuaTangChiTiet : BaseEntity
    {
        public long XuatKhoQuaTangId { get; set; }
        public long QuaTangId { get; set; }
        public long NhapKhoQuaTangChiTietId { get; set; }
        public int SoLuongXuat { get; set; }

        public virtual XuatKhoQuaTang XuatKhoQuaTang { get; set; }
        public virtual QuaTang QuaTang { get; set; }
        public virtual NhapKhoQuaTangChiTiet NhapKhoQuaTangChiTiet { get; set; }
    }
}
