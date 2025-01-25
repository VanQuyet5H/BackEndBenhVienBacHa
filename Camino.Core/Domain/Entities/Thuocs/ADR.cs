using System;
using System.Collections.Generic;
using System.Text;

namespace Camino.Core.Domain.Entities.Thuocs
{
    public class ADR : BaseEntity
    {
        public long ThuocHoacHoatChat1Id { get; set; }
        public long ThuocHoacHoatChat2Id { get; set; }
        public Enums.MucDoChuYKhiChiDinh MucDoChuYKhiChiDinh { get; set; }
        public Enums.MucDoTuongTac MucDoTuongTac { get; set; }
        public bool? DuocDongHoc { get; set; }
        public bool? DuocLucHoc { get; set; }
        public bool? ThuocThucAn { get; set; }
        public bool? QuyTac { get; set; }
        public string TuongTacHauQua { get; set; }
        public string CachXuLy { get; set; }
        public string GhiChu { get; set; }

        public virtual ThuocHoacHoatChat ThuocHoacHoatChat1 { get; set; }
        public virtual ThuocHoacHoatChat ThuocHoacHoatChat2 { get; set; }
    }
}
