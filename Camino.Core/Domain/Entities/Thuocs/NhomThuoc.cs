using System;
using System.Collections.Generic;
using System.Text;

namespace Camino.Core.Domain.Entities.Thuocs
{
    public class NhomThuoc : BaseEntity
    {
        public string Ten { get; set; }
        public long? NhomChaId { get; set; }
        public int CapNhom { get; set; }
        public Enums.LoaiThuocHoacHoatChat LoaiThuocHoacHoatChat { get; set; }

        public virtual NhomThuoc NhomCha { get; set; }
      

        private ICollection<ThuocHoacHoatChat> _thuocHoacHoatChat;
        public virtual ICollection<ThuocHoacHoatChat> ThuocHoacHoatChats
        {
            get => _thuocHoacHoatChat ?? (_thuocHoacHoatChat = new List<ThuocHoacHoatChat>());
            protected set => _thuocHoacHoatChat = value;
        }

        private ICollection<NhomThuoc> _nhomThuocs;
        public virtual ICollection<NhomThuoc> NhomThuocs
        {
            get => _nhomThuocs ?? (_nhomThuocs = new List<NhomThuoc>());
            protected set => _nhomThuocs = value;
        }
       
    }
}
