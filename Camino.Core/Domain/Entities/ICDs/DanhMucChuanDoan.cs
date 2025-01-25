using Camino.Core.Domain.Entities.TrieuChungDanhMucChuanDoans;
using System;
using System.Collections.Generic;
using System.Text;

namespace Camino.Core.Domain.Entities.ICDs
{
    public class DanhMucChuanDoan: BaseEntity
    {
        public string TenTiengViet { get; set; }
        public string TenTiengAnh { get; set; }
        public string GhiChu { get; set; }

        private ICollection<ChuanDoan> _chuanDoans { get; set; }

        public virtual ICollection<ChuanDoan> ChuanDoans
        {
            get => _chuanDoans ?? (_chuanDoans = new List<ChuanDoan>());
            protected set => _chuanDoans = value;
        }
        private ICollection<TrieuChungDanhMucChuanDoan> _trieuChungDanhMucChuanDoan;
        public virtual ICollection<TrieuChungDanhMucChuanDoan> TrieuChungDanhMucChuanDoans
        {
            get => _trieuChungDanhMucChuanDoan ?? (_trieuChungDanhMucChuanDoan = new List<TrieuChungDanhMucChuanDoan>());
            protected set => _trieuChungDanhMucChuanDoan = value;
        }
    }
}
