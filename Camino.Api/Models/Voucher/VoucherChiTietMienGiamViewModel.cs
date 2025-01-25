using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static Camino.Core.Domain.Enums;

namespace Camino.Api.Models.Voucher
{
    public class VoucherChiTietMienGiamViewModel : BaseViewModel
    {
        public long VoucherId { get; set; }
        public EnumDichVuTongHop LoaiDichVuBenhVien { get; set; }
        public long? DichVuId { get; set; }
        public long? LoaiGiaId { get; set; }
        public LoaiChietKhau LoaiChietKhau { get; set; }
        public int? TiLeChietKhau { get; set; }
        public decimal? SoTienChietKhau { get; set; }
        public string  GhiChu { get; set; }
        public long? NhomDichVuId { get; set; }
    }
}
