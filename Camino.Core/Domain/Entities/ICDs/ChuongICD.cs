using System;
using System.Collections.Generic;
using System.Text;

namespace Camino.Core.Domain.Entities.ICDs
{
    public class ChuongICD : BaseICDEntity
    {
        public string SoChuong { get; set; }
        public string Stt { get; set; }
        public string TenTiengVietTheoBenhVien { get; set; }
        public string TenTiengAnhTheoBenhVien { get; set; }
        public bool? CoBaoCao { get; set; }

        public ICollection<NhomICD> _nhomICDs;
        public virtual ICollection<NhomICD> NhomICDs
        {
            get => _nhomICDs ?? (_nhomICDs = new List<NhomICD>());
            protected set => _nhomICDs = value;
        }

        public ICollection<NhomICDTheoBenhVien> _nhomICDTheoBenhViens;
        public virtual ICollection<NhomICDTheoBenhVien> NhomICDTheoBenhViens
        {
            get => _nhomICDTheoBenhViens ?? (_nhomICDTheoBenhViens = new List<NhomICDTheoBenhVien>());
            protected set => _nhomICDTheoBenhViens = value;
        }
    }
}
