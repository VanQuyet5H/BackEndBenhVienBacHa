using Camino.Core.Domain.ValueObject.Voucher;
using System;
using System.Collections.Generic;
using static Camino.Core.Domain.Enums;

namespace Camino.Api.Models.Voucher
{
    public class VoucherMarketingViewModel : BaseViewModel
    {
        public VoucherMarketingViewModel()
        {
            lstVoucherChiTietMienGiam = new List<VoucherChiTietMienGiamViewModel>();
            lstVoucherChiTietMienGiamNhomDichVu = new List<VoucherChiTietMienGiamViewModel>();
        }

        public string Ma { get; set; }
        public string Ten { get; set; }
        public int? SoLuongPhatHanh { get; set; }
        public DateTime? TuNgay { get; set; }
        public DateTime? DenNgay { get; set; }
        public List<VoucherChiTietMienGiamViewModel> lstVoucherChiTietMienGiam { get; set; }
        public List<VoucherChiTietMienGiamViewModel> lstVoucherChiTietMienGiamNhomDichVu { get; set; }
        public EnumLoaiDichVuVoucherMarketing LoaiDichVuVoucherMarketing { get; set; }
        public LoaiChietKhau? LoaiChietKhau { get; set; }
        public int? TiLeChietKhau { get; set; }
        public decimal? SoTienChietKhau { get; set; }
        public bool? ChietKhauTatCaDichVu => LoaiDichVuVoucherMarketing == EnumLoaiDichVuVoucherMarketing.TatCaDichVu ? true : false;
        public string GhiChu { get; set; }
        public bool IsThemHoacLuuDichVu { get; set; }

        //public EnumDichVuTongHop LoaiDichVu { get; set; }
        //public long LoaiGiaId { get; set; }

        //public string GhiChu { get; set; }
    }
}
