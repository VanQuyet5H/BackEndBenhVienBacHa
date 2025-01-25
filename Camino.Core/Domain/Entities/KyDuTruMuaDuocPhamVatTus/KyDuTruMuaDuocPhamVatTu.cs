using Camino.Core.Domain.Entities.DuTruMuaDuocPhamKhoDuocs;
using Camino.Core.Domain.Entities.DuTruMuaDuocPhams;
using Camino.Core.Domain.Entities.DuTruMuaDuocPhamTheoKhoas;
using Camino.Core.Domain.Entities.DuTruVatTus;
using Camino.Core.Domain.Entities.DuyetBaoHiems;
using Camino.Core.Domain.Entities.NhanViens;
using System;
using System.Collections.Generic;
using System.Text;

namespace Camino.Core.Domain.Entities.KyDuTruMuaDuocPhamVatTus
{
    public class KyDuTruMuaDuocPhamVatTu : BaseEntity
    {
        public DateTime TuNgay { get; set; }
        public DateTime DenNgay { get; set; }
        public long NhanVienTaoId { get; set; }
        public bool? MuaDuocPham { get; set; }
        public bool? MuaVatTu { get; set; }
        public string MoTa { get; set; }
        public bool HieuLuc { get; set; }
        public DateTime NgayBatDauLap { get; set; }
        public DateTime NgayKetThucLap { get; set; }
        public virtual NhanVien NhanVien { get; set; }

        private ICollection<DuTruMuaDuocPham> _duTruMuaDuocPhams { get; set; }
        public virtual ICollection<DuTruMuaDuocPham> DuTruMuaDuocPhams
        {
            get => _duTruMuaDuocPhams ?? (_duTruMuaDuocPhams = new List<DuTruMuaDuocPham>());
            protected set => _duTruMuaDuocPhams = value;
        }

        private ICollection<DuTruMuaDuocPhamTheoKhoa> _duTruMuaDuocPhamTheoKhoas { get; set; }
        public virtual ICollection<DuTruMuaDuocPhamTheoKhoa> DuTruMuaDuocPhamTheoKhoas
        {
            get => _duTruMuaDuocPhamTheoKhoas ?? (_duTruMuaDuocPhamTheoKhoas = new List<DuTruMuaDuocPhamTheoKhoa>());
            protected set => _duTruMuaDuocPhamTheoKhoas = value;
        }

        private ICollection<DuTruMuaDuocPhamKhoDuoc> _duTruMuaDuocPhamKhoDuocs { get; set; }
        public virtual ICollection<DuTruMuaDuocPhamKhoDuoc> DuTruMuaDuocPhamKhoDuocs
        {
            get => _duTruMuaDuocPhamKhoDuocs ?? (_duTruMuaDuocPhamKhoDuocs = new List<DuTruMuaDuocPhamKhoDuoc>());
            protected set => _duTruMuaDuocPhamKhoDuocs = value;
        }

        private ICollection<DuTruMuaVatTu> _duTruMuaVatTus { get; set; }
        public virtual ICollection<DuTruMuaVatTu> DuTruMuaVatTus
        {
            get => _duTruMuaVatTus ?? (_duTruMuaVatTus = new List<DuTruMuaVatTu>());
            protected set => _duTruMuaVatTus = value;
        }

        private ICollection<DuTruMuaVatTuTheoKhoa> _duTruMuaVatTuTheoKhoas { get; set; }
        public virtual ICollection<DuTruMuaVatTuTheoKhoa> DuTruMuaVatTuTheoKhoas
        {
            get => _duTruMuaVatTuTheoKhoas ?? (_duTruMuaVatTuTheoKhoas = new List<DuTruMuaVatTuTheoKhoa>());
            protected set => _duTruMuaVatTuTheoKhoas = value;
        }

        private ICollection<DuTruMuaVatTuKhoDuoc> _duTruMuaVatTuKhoDuocs { get; set; }
        public virtual ICollection<DuTruMuaVatTuKhoDuoc> DuTruMuaVatTuKhoDuocs
        {
            get => _duTruMuaVatTuKhoDuocs ?? (_duTruMuaVatTuKhoDuocs = new List<DuTruMuaVatTuKhoDuoc>());
            protected set => _duTruMuaVatTuKhoDuocs = value;
        }


    }
}
