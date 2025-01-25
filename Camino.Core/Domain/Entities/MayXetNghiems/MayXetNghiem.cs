using System;
using System.Collections.Generic;
using Camino.Core.Domain.Entities.DuocPhamBenhViens;
using Camino.Core.Domain.Entities.MauMayXetNghiems;
using Camino.Core.Domain.Entities.XetNghiems;
using Camino.Core.Domain.Entities.XuatKhos;
using Camino.Core.Domain.Entities.YeuCauKhamBenhs;

namespace Camino.Core.Domain.Entities.MayXetNghiems
{
    public class MayXetNghiem : BaseEntity
    {
        public string Ma { get; set; }

        public string Ten { get; set; }

        public long MauMayXetNghiemID { get; set; }

        public string NhaCungCap { get; set; }

        public bool HieuLuc { get; set; }

        public string HostName { get; set; }

        public string PortName { get; set; }

        public int? BaudRate { get; set; }

        public byte? DataBits { get; set; }

        public string StopBits { get; set; }

        public string Parity { get; set; }

        public string Handshake { get; set; }

        public string Encoding { get; set; }

        public int? ReadBufferSize { get; set; }

        public bool RtsEnable { get; set; }

        public bool DtrEnable { get; set; }

        public bool DiscardNull { get; set; }

        public string ConnectionMode { get; set; }

        public string ConnectionProtocol { get; set; }
        
        public bool AutoOpenPort { get; set; }
        
        public bool AutoOpenForm { get; set; }

        public Enums.EnumConnectionStatus? ConnectionStatus { get; set; }

        public long? OpenById { get; set; }

        public DateTime? OpenDateTime { get; set; }

        public DateTime? CloseDateTime { get; set; }

        public bool LogDataEnabled { get; set; }

        public virtual MauMayXetNghiem MauMayXetNghiem { get; set; }

        private ICollection<KetQuaXetNghiemChiTiet> _ketQuaXetNghiemChiTiets;
        public virtual ICollection<KetQuaXetNghiemChiTiet> KetQuaXetNghiemChiTiets
        {
            get => _ketQuaXetNghiemChiTiets ?? (_ketQuaXetNghiemChiTiets = new List<KetQuaXetNghiemChiTiet>());
            protected set => _ketQuaXetNghiemChiTiets = value;
        }

        private ICollection<YeuCauDichVuKyThuat> _yeuCauDichVuKyThuats;
        public virtual ICollection<YeuCauDichVuKyThuat> YeuCauDichVuKyThuats
        {
            get => _yeuCauDichVuKyThuats ?? (_yeuCauDichVuKyThuats = new List<YeuCauDichVuKyThuat>());
            protected set => _yeuCauDichVuKyThuats = value;
        }

        private ICollection<DuocPhamBenhVienMayXetNghiem> _duocPhamBenhVienMayXetNghiems;
        public virtual ICollection<DuocPhamBenhVienMayXetNghiem> DuocPhamBenhVienMayXetNghiems
        {
            get => _duocPhamBenhVienMayXetNghiems ?? (_duocPhamBenhVienMayXetNghiems = new List<DuocPhamBenhVienMayXetNghiem>());
            protected set => _duocPhamBenhVienMayXetNghiems = value;
        }

        private ICollection<XuatKhoDuocPhamChiTietViTri> _xuatKhoDuocPhamChiTietViTris;
        public virtual ICollection<XuatKhoDuocPhamChiTietViTri> XuatKhoDuocPhamChiTietViTris
        {
            get => _xuatKhoDuocPhamChiTietViTris ?? (_xuatKhoDuocPhamChiTietViTris = new List<XuatKhoDuocPhamChiTietViTri>());
            protected set => _xuatKhoDuocPhamChiTietViTris = value;
        }

        private ICollection<YeuCauXuatKhoDuocPhamChiTiet> _yeuCauXuatKhoDuocPhamChiTiets;
        public virtual ICollection<YeuCauXuatKhoDuocPhamChiTiet> YeuCauXuatKhoDuocPhamChiTiets
        {
            get => _yeuCauXuatKhoDuocPhamChiTiets ?? (_yeuCauXuatKhoDuocPhamChiTiets = new List<YeuCauXuatKhoDuocPhamChiTiet>());
            protected set => _yeuCauXuatKhoDuocPhamChiTiets = value;
        }
    }
}
