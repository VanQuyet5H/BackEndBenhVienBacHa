using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Camino.Core.Domain.ValueObject.GoiDvMarketings
{
    public class NoiDungQuyetToanGoiMarketing
    {
        public LoaiDichVuYeuCau LoaiDichVuYeuCau { get; set; }
        public Enums.LoaiDichVuKyThuat? LoaiDichVuKyThuat { get; set; }
        public long? NhomDichVuBenhVienId { get; set; }
        public string NoiDung { get; set; }
        public decimal ThanhTien { get; set; }
        //BVHD-3961
        public decimal? DonGia { get; set; }
        public long? DichVuBenhVienId { get; set; }
        public long? NhomGiaDichVuId { get; set; }
        public double? SoLuong { get; set; }
    }
    public enum LoaiDichVuYeuCau
    {
        YeuCauKhamBenh = 1,
        YeuCauDichVuKyThuat = 2,
        YeuCauDichVuGiuong = 3,
    }
}
