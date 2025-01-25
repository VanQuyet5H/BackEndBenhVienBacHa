using System;
using System.Collections.Generic;
using Camino.Core.Domain;
using iText.Layout.Element;
using static Camino.Core.Domain.Enums;

namespace Camino.Api.Models.KhamBenh
{
    public class KhamBenhYeuCauGoiDichVuViewModel : BaseViewModel
    {
        public long? YeuCauTiepNhanId { get; set; }
        public long? YeuCauKhamBenhId { get; set; }
        public long? GoiDichVuId { get; set; }
        public EnumLoaiGoiDichVu? LoaiGoiDichVu { get; set; }
        public string Ten { get; set; }
        public bool? CoChietKhau { get; set; }
        public long? ChiPhiGoiDichVu { get; set; }
        public string MoTa { get; set; }
        public long? NhanVienChiDinhId { get; set; }
        public long? NoiChiDinhId { get; set; }
        public DateTime? ThoiDiemChiDinh { get; set; }
        public bool? DaThanhToan { get; set; }
        public long? NoiThanhToanId { get; set; }
        public long? NhanVienThanhToanId { get; set; }
        public DateTime? ThoiDiemThanhToan { get; set; }
        public EnumTrangThaiYeuCauDichVuKyThuat? TrangThai { get; set; }
        public string GhiChu { get; set; }
    }


    public class YeuCauThemGoi:BaseViewModel
    {
        public YeuCauThemGoi()
        {
            ListNoiThucHiens = new List<GoiDichVuChiTietNoiThucHienViewModel>();
            DichVuChiDinhTheoGois = new List<string>();
        }
        public long? YeuCauTiepNhanId { get; set; }
        public long? YeuCauKhamBenhId { get; set; }
        public long? GoiDichVuId { get; set; }
        public byte[] LastModifiedYeuCauKhamBenh { get; set; }
        public List<string> DichVuChiDinhTheoGois { get; set; }
        public List<GoiDichVuChiTietNoiThucHienViewModel> ListNoiThucHiens { get; set; }
    }

    public class DichVuChiDinhTheoGoiViewModel
    {
        public long Id { get; set; }
        public int NhomDichVu { get; set; }
    }
}
