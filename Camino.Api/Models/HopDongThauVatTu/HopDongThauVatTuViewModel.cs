using System;
using System.Collections.Generic;
using Camino.Core.Domain;
using Camino.Core.Helpers;

namespace Camino.Api.Models.HopDongThauVatTu
{
    public class HopDongThauVatTuViewModel : BaseViewModel
    {
        public HopDongThauVatTuViewModel()
        {
            HopDongThauVatTuChiTiets = new List<HopDongThauVatTuChiTietViewModel>();
        }

        public long? NhaThauId { get; set; }

        public string NhaThau { get; set; }

        public string SoHopDong { get; set; }

        public string SoQuyetDinh { get; set; }

        public DateTime? CongBo { get; set; }

        public DateTime? NgayKy { get; set; }

        public DateTime? NgayHieuLuc { get; set; }

        public DateTime? NgayHetHan { get; set; }

        public string CongBoDisplay => CongBo?.ApplyFormatDateTimeSACH();

        public string NgayKyDisplay => NgayKy?.ApplyFormatDateTimeSACH();

        public string NgayHieuLucDisplay => NgayHieuLuc?.ApplyFormatDateTimeSACH();

        public string NgayHetHanDisplay => NgayHetHan?.ApplyFormatDateTimeSACH();

        public Enums.EnumLoaiThau? LoaiThau { get; set; }

        public string TenLoaiThau => LoaiThau.GetValueOrDefault().GetDescription();

        public string NhomThau { get; set; }

        public string GoiThau { get; set; }

        public int? Nam { get; set; }

        public bool? CoNhapKho { get; set; }

        public List<HopDongThauVatTuChiTietViewModel> HopDongThauVatTuChiTiets { get; set; }
    }
}
