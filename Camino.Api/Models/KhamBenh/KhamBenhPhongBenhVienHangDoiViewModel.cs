using Camino.Api.Models.BenhVien;
using Camino.Api.Models.PhongBenhVien;
using Camino.Core.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Camino.Core.Domain.ValueObject.KhamBenhs;

namespace Camino.Api.Models.KhamBenh
{
    public class KhamBenhPhongBenhVienHangDoiViewModel : BaseViewModel
    {
        public KhamBenhPhongBenhVienHangDoiViewModel()
        {
            GoiDichVus = new List<GoiDichVuTheoBenhNhanGridVo>();
        }

        public long PhongBenhVienId { get; set; }
        public long YeuCauTiepNhanId { get; set; }
        public long? YeuCauKhamBenhId { get; set; }
        public long? YeuCauDichVuKyThuatId { get; set; }
        public Enums.EnumLoaiHangDoi LoaiHangDoi { get; set; }
        public Enums.EnumTrangThaiHangDoi TrangThai { get; set; }
        public int? SoThuTu { get; set; }
        public bool? CoDichVuKhuyenMai { get; set; }

        public bool isShowPanelItemKhamBenh
        {
            get { return true; }
        }

        public bool isExpandPanelItemKhamBenh
        {
            get { return true; }
        }

        public bool isShowPanelItemKetLuan
        {
            get
            {
                return YeuCauKhamBenh != null && YeuCauKhamBenh.IcdchinhId != null;
            }
        }

        public bool isExpandPanelItemKetLuan
        {
            get { return isShowPanelItemKetLuan; }
        }

        public bool LaChuyenKhoaKhamNhieuKhamBenhDangKham { get; set; }
        public bool LaDichVuKhamNhieu { get; set; }

        //BVHD-3895
        public bool? LaDichVuKhamVietTat { get; set; }

        public List<GoiDichVuTheoBenhNhanGridVo> GoiDichVus { get; set; }

        public virtual PhongBenhVienViewModel PhongBenhVien { get; set; }
        public virtual KhamBenhYeuCauDichVuKyThuatViewModel YeuCauDichVuKyThuat { get; set; }

        public virtual KhamBenhYeuCauTiepNhanViewModel YeuCauTiepNhan { get; set; }
        public virtual KhamBenhYeuCauKhamBenhViewModel YeuCauKhamBenh { get; set; }
    }
}
