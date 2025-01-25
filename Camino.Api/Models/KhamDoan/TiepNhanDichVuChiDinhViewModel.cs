using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Camino.Core.Domain;
using Camino.Core.Domain.ValueObject.KhamDoan;
using Camino.Core.Helpers;

namespace Camino.Api.Models.KhamDoan
{
    public class TiepNhanDichVuChiDinhViewModel : BaseViewModel
    {
        public Enums.NhomDichVuChiDinhKhamSucKhoe? LoaiDichVu { get; set; }
        public string TenNhomDichVu => LoaiDichVuKyThuat == null ? Enums.NhomDichVuChiDinhKhamSucKhoe.KhamBenh.GetDescription() : LoaiDichVuKyThuat.GetDescription(); //LoaiDichVu.GetDescription();
        private Enums.LoaiDichVuKyThuat? _loaiDichVuKyThuat { get; set; }
        public Enums.LoaiDichVuKyThuat? LoaiDichVuKyThuat
        {
            get { return _loaiDichVuKyThuat; }
            set
            {
                _loaiDichVuKyThuat = value;
                if (_loaiDichVuKyThuat != null)
                {
                    if (_loaiDichVuKyThuat == Enums.LoaiDichVuKyThuat.ChuanDoanHinhAnh)
                    {
                        LoaiDichVu = Enums.NhomDichVuChiDinhKhamSucKhoe.ChuanDoanHinhAnh;
                    }
                    else if (_loaiDichVuKyThuat == Enums.LoaiDichVuKyThuat.XetNghiem)
                    {
                        LoaiDichVu = Enums.NhomDichVuChiDinhKhamSucKhoe.XetNghiem;
                    }
                    else if (_loaiDichVuKyThuat == Enums.LoaiDichVuKyThuat.ThamDoChucNang)
                    {
                        LoaiDichVu = Enums.NhomDichVuChiDinhKhamSucKhoe.ThamDoChucNang;
                    }
                    else
                    {
                        LoaiDichVu = Enums.NhomDichVuChiDinhKhamSucKhoe.KhamBenh;
                    }
                }
            }
        }
        public long? DichVuBenhVienId { get; set; }
        public string Ma { get; set; }
        public string Ten { get; set; }
        public long? LoaiGiaId { get; set; }
        public string TenLoaiGia { get; set; }
        public int? SoLan { get; set; }
        public decimal? DonGiaBenhVien { get; set; }
        public decimal? DonGiaMoi { get; set; }
        public decimal? DonGiaUuDai { get; set; }
        public decimal? DonGiaChuaUuDai { get; set; }
        public decimal ThanhTien => (decimal) ((GoiKhamSucKhoeId == null && DonGiaMoi != null && SoLan != null) ?  DonGiaMoi * SoLan : 0);
        public long? NoiThucHienId { get; set; }
        public string TenNoiThucHien { get; set; }
        public long? GoiKhamSucKhoeId { get; set; }
        public bool LaDichVuKham => LoaiDichVuKyThuat == null; // LoaiDichVu == Enums.NhomDichVuChiDinhKhamSucKhoe.KhamBenh;
        public int? TinhTrang { get; set; }
        public string TenTinhTrang { get; set; }
        public Enums.ChuyenKhoaKhamSucKhoe? ChuyenKhoaKhamSucKhoe { get; set; }
        public long? YeuCauTiepNhanId { get; set; }

        public string TenGoiKhamSucKhoe { get; set; }
        public int? SoLanChuaLuu { get; set; }

        //BVHD-3618
        public bool? LaGoiChung { get; set; }
        public long? GoiKhamSucKhoeChungDichVuKhamBenhNhanVienId { get; set; }
        public long? GoiKhamSucKhoeChungDichVuKyThuatNhanVienId { get; set; }

        // BVHD -3668

        public long? GoiKhamSucKhoeDichVuPhatSinhId { get; set; }
    }

    public class TiepNhanDichVuChiDinhViewModelMultiselect : BaseViewModel
    {
        public TiepNhanDichVuChiDinhViewModelMultiselect()
        {
            DichVus = new List<string>();
            DichVuThems = new List<TiepNhanDichVuChiDinhVo>();
            DichVuGois = new List<TiepNhanDichVuChiDinhVo>();
        }
        public long? YeuCauTiepNhanId { get; set; }
        public long? LoaiNhomDichVuId { get; set; }
        public long? GoiKhamSucKhoeId { get; set; }
        public List<string> DichVus { get; set; }
        public List<TiepNhanDichVuChiDinhVo> DichVuThems { get; set; }
        public List<TiepNhanDichVuChiDinhVo> DichVuGois { get; set; }

        public Enums.HinhThucKhamBenh HinhThucKhamBenh { get; set; }
        public long HopDongKhamSucKhoeId { get; set; }

        //BVHD-3618
        public long? HopDongKhamSucKhoeNhanVienId { get; set; }
        public string BieuHienLamSang { get; set; }
        public string DichTeSarsCoV2 { get; set; }
    }
}
