using Camino.Core.DependencyInjection.Attributes;
using Camino.Core.Domain;
using Camino.Core.Domain.Entities.Thuocs;
using Camino.Core.Domain.Entities.XuatKhos;
using Camino.Core.Domain.Entities.YeuCauKhamBenhs;
using Camino.Core.Domain.Entities.YeuCauTiepNhans;
using Camino.Core.Helpers;
using Camino.Data;
using Camino.Services.Helpers;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Camino.Services.KhamBenhs
{

    [ScopedDependency(ServiceType = typeof(IYeuCauDuocPhamBenhVienService))]
    public class YeuCauDuocPhamBenhVienService : MasterFileService<YeuCauDuocPhamBenhVien>, IYeuCauDuocPhamBenhVienService
    {
        private IRepository<DuocPham> _duocPhamRepository;
        private IRepository<YeuCauTiepNhan> _yeuCauTiepNhanRepository;
        private IRepository<YeuCauDuocPhamBenhVien> _yeuCauDuocPhamBenhVienRepository;
        private readonly IUserAgentHelper _userAgentHelper;
        public YeuCauDuocPhamBenhVienService(IRepository<YeuCauDuocPhamBenhVien> repository
            , IRepository<DuocPham> duocPhamRepository
            , IRepository<YeuCauTiepNhan> yeuCauTiepNhanRepository
            , IRepository<YeuCauDuocPhamBenhVien> yeuCauDuocPhamBenhVienRepository
            , IUserAgentHelper userAgentHelper) : base(repository)
        {
            _duocPhamRepository = duocPhamRepository;
            _yeuCauTiepNhanRepository = yeuCauTiepNhanRepository;
            _userAgentHelper = userAgentHelper;
            _yeuCauDuocPhamBenhVienRepository = yeuCauDuocPhamBenhVienRepository;
        }
        public async Task ThemYeuCauDuocPhamBenhVien(YeuCauDuocPhamBenhVien yeuCauDuocPhamBenhVien)
        {
            var ycTiepNhan = _yeuCauTiepNhanRepository.GetById(yeuCauDuocPhamBenhVien.YeuCauTiepNhanId,
                  x => x.Include(o => o.YeuCauDuocPhamBenhViens));

            var duocPham = _duocPhamRepository.GetById(yeuCauDuocPhamBenhVien.DuocPhamBenhVienId, //DuocPhamBenhVienId create to DuocPhamId
                  x => x.Include(o => o.DuocPhamBenhVien).ThenInclude(dpbv => dpbv.NhapKhoDuocPhamChiTiets).ThenInclude(nkct => nkct.HopDongThauDuocPhams)
                        .Include(o => o.DuocPhamBenhVien).ThenInclude(dpbv => dpbv.NhapKhoDuocPhamChiTiets).ThenInclude(nkct => nkct.NhapKhoDuocPhams).ThenInclude(nk => nk.KhoDuocPhams));

            //TODO update entity kho on 9/9/2020
            //if (duocPham.DuocPhamBenhVien == null || duocPham.DuocPhamBenhVien.NhapKhoDuocPhamChiTiets
            //      .Where(o => o.NhapKhoDuocPhams.KhoDuocPhams.LoaiKho == Enums.EnumLoaiKhoDuocPham.KhoNgoai && o.SoLuongNhap > o.SoLuongDaXuat)
            //      .Sum(o => o.SoLuongNhap - o.SoLuongDaXuat) < yeuCauDuocPhamBenhVien.SoLuong)
            //{
            //    //return "Dược phẩm không có trong kho";
            //    throw new Exception("Dược phẩm không có trong kho");
            //}
            //else
            //{

            //    double soLuongCanXuat = yeuCauDuocPhamBenhVien.SoLuong;
            //    int? mucHuongBHYT = ycTiepNhan.BHYTMucHuong ?? 100;

            //    while (!soLuongCanXuat.Equals(0))
            //    {
            //        // tinh so luong xuat
            //        var nhapKhoDuocPhamChiTiet = duocPham.DuocPhamBenhVien.NhapKhoDuocPhamChiTiets.Where(o => o.NhapKhoDuocPhams.KhoDuocPhams.LoaiKho == Enums.EnumLoaiKhoDuocPham.KhoNgoai && o.SoLuongNhap > o.SoLuongDaXuat).OrderBy(o => o.HanSuDung).First();
            //        var soLuongTon = Convert.ToSingle((nhapKhoDuocPhamChiTiet.SoLuongNhap - nhapKhoDuocPhamChiTiet.SoLuongDaXuat));
            //        var soLuongXuat = soLuongTon > soLuongCanXuat ? soLuongCanXuat : soLuongTon;
            //        nhapKhoDuocPhamChiTiet.SoLuongDaXuat += soLuongXuat;
            //        var xuatKhoChiTiet = new XuatKhoDuocPhamChiTietViTri
            //        {
            //            SoLuongXuat = soLuongXuat,
            //            NgayXuat = DateTime.Now,
            //            NhapKhoDuocPhamChiTiet = nhapKhoDuocPhamChiTiet,
            //            XuatKhoDuocPhamChiTiet = new XuatKhoDuocPhamChiTiet
            //            {
            //                DuocPhamBenhVien = duocPham.DuocPhamBenhVien,
            //                //TODO update entity kho on 9/9/2020
            //                //DatChatLuong = true,
            //                NgayXuat = DateTime.Now,
            //                XuatKhoDuocPham = new XuatKhoDuocPham
            //                {
            //                    KhoDuocPhamXuat = nhapKhoDuocPhamChiTiet.NhapKhoDuocPhams.KhoDuocPhams,
            //                    LoaiXuatKho = Enums.XuatKhoDuocPham.XuatChoBenhNhan,
            //                    LyDoXuatKho = Enums.XuatKhoDuocPham.XuatChoBenhNhan.GetDescription(),
            //                    LoaiNguoiNhan = Enums.LoaiNguoiGiaoNhan.NgoaiHeThong,
            //                    TenNguoiNhan = ycTiepNhan.HoTen,
            //                    NgayXuat = DateTime.Now,
            //                    NguoiXuatId = _userAgentHelper.GetCurrentUserId(),
            //                    SoPhieu = ycTiepNhan.MaYeuCauTiepNhan
            //                }
            //            }
            //        };
            //        var ttGiaBh = duocPham.DuocPhamBenhVien.DuocPhamBenhVienGiaBaoHiems.FirstOrDefault(o => o.TuNgay < DateTime.Now && (o.DenNgay == null || DateTime.Now < o.DenNgay));

            //        yeuCauDuocPhamBenhVien.DuocPhamBenhVienId = duocPham.Id;
            //        //TODO update entity kho on 9/9/2020
            //        //yeuCauDuocPhamBenhVien.XuatKhoDuocPhamChiTietViTri = xuatKhoChiTiet;
            //        //yeuCauDuocPhamBenhVien.Gia = nhapKhoDuocPhamChiTiet.DonGiaBan + (nhapKhoDuocPhamChiTiet.DonGiaBan * nhapKhoDuocPhamChiTiet.VAT);
            //        yeuCauDuocPhamBenhVien.SoLuong = soLuongXuat;
            //        yeuCauDuocPhamBenhVien.DonGiaBaoHiem = ttGiaBh?.Gia;
            //        yeuCauDuocPhamBenhVien.SoTienBenhNhanDaChi = 0;
            //        yeuCauDuocPhamBenhVien.TenTiengAnh = duocPham.TenTiengAnh;
            //        yeuCauDuocPhamBenhVien.Ten = duocPham.Ten;
            //        yeuCauDuocPhamBenhVien.STTHoatChat = duocPham.STTHoatChat;
            //        yeuCauDuocPhamBenhVien.MaHoatChat = duocPham.MaHoatChat;

            //        // cần cập nhật lại chỗ này
            //        yeuCauDuocPhamBenhVien.DuocHuongBaoHiem = false; //(ycTiepNhan.CoBHYT == true && ttGiaBh != null);
            //        yeuCauDuocPhamBenhVien.BaoHiemChiTra = null;

            //        yeuCauDuocPhamBenhVien.LoaiThuocHoacHoatChat = duocPham.LoaiThuocHoacHoatChat;
            //        yeuCauDuocPhamBenhVien.SoDangKy = duocPham.SoDangKy;
            //        yeuCauDuocPhamBenhVien.MaHoatChat = duocPham.MaHoatChat;
            //        yeuCauDuocPhamBenhVien.HoatChat = duocPham.HoatChat;
            //        yeuCauDuocPhamBenhVien.LoaiThuocHoacHoatChat = duocPham.LoaiThuocHoacHoatChat;
            //        yeuCauDuocPhamBenhVien.NhaSanXuat = duocPham.NhaSanXuat;
            //        yeuCauDuocPhamBenhVien.NuocSanXuat = duocPham.NuocSanXuat;
            //        yeuCauDuocPhamBenhVien.DuongDungId = duocPham.DuongDungId;
            //        yeuCauDuocPhamBenhVien.HamLuong = duocPham.HamLuong;
            //        yeuCauDuocPhamBenhVien.QuyCach = duocPham.QuyCach;
            //        yeuCauDuocPhamBenhVien.TieuChuan = duocPham.TieuChuan;
            //        yeuCauDuocPhamBenhVien.DangBaoChe = duocPham.DangBaoChe;
            //        yeuCauDuocPhamBenhVien.DonViTinhId = duocPham.DonViTinhId;
            //        yeuCauDuocPhamBenhVien.HuongDan = duocPham.HuongDan;
            //        yeuCauDuocPhamBenhVien.MoTa = duocPham.MoTa;
            //        yeuCauDuocPhamBenhVien.ChiDinh = duocPham.ChiDinh;
            //        yeuCauDuocPhamBenhVien.ChongChiDinh = duocPham.ChongChiDinh;
            //        yeuCauDuocPhamBenhVien.LieuLuongCachDung = duocPham.LieuLuongCachDung;
            //        yeuCauDuocPhamBenhVien.TacDungPhu = duocPham.TacDungPhu;
            //        yeuCauDuocPhamBenhVien.HopDongThauDuocPhamId = nhapKhoDuocPhamChiTiet.HopDongThauDuocPhamId;
            //        yeuCauDuocPhamBenhVien.NhaThauId = nhapKhoDuocPhamChiTiet.HopDongThauDuocPhams.NhaThauId;
            //        yeuCauDuocPhamBenhVien.SoHopDongThau = nhapKhoDuocPhamChiTiet.HopDongThauDuocPhams.SoHopDong;
            //        yeuCauDuocPhamBenhVien.SoQuyetDinhThau = nhapKhoDuocPhamChiTiet.HopDongThauDuocPhams.SoQuyetDinh;
            //        yeuCauDuocPhamBenhVien.LoaiThau = nhapKhoDuocPhamChiTiet.HopDongThauDuocPhams.LoaiThau;
            //        yeuCauDuocPhamBenhVien.LoaiThuocThau = nhapKhoDuocPhamChiTiet.HopDongThauDuocPhams.LoaiThuocThau;
            //        yeuCauDuocPhamBenhVien.NhomThau = nhapKhoDuocPhamChiTiet.HopDongThauDuocPhams.NhomThau;
            //        yeuCauDuocPhamBenhVien.GoiThau = nhapKhoDuocPhamChiTiet.HopDongThauDuocPhams.GoiThau;
            //        yeuCauDuocPhamBenhVien.NamThau = nhapKhoDuocPhamChiTiet.HopDongThauDuocPhams.Nam;
            //        yeuCauDuocPhamBenhVien.NhanVienChiDinhId = _userAgentHelper.GetCurrentUserId();
            //        yeuCauDuocPhamBenhVien.NoiChiDinhId = _userAgentHelper.GetCurrentNoiLLamViecId();
            //        yeuCauDuocPhamBenhVien.TrangThaiThanhToan = yeuCauDuocPhamBenhVien.KhongTinhPhi == true ? Enums.TrangThaiThanhToan.DaThanhToan : Enums.TrangThaiThanhToan.ChuaThanhToan;
            //        yeuCauDuocPhamBenhVien.TrangThai = Enums.EnumYeuCauDuocPhamBenhVien.ChuaThucHien;
            //        ycTiepNhan.YeuCauDuocPhamBenhViens.Add(yeuCauDuocPhamBenhVien);
            //        soLuongCanXuat = soLuongCanXuat - soLuongXuat;
            //    }
            //}
            //await _yeuCauTiepNhanRepository.UpdateAsync(ycTiepNhan);
            //return null;
        }

        //todo: cân nhắc có thể xóa, hiện tại ko sử dụng
        public async Task<string> XoaYeuCauDuocPhamBenhVien(long IdYeuCauDuocPham)
        {
            var ycDuocPham = _yeuCauDuocPhamBenhVienRepository.GetById(IdYeuCauDuocPham
                 //TODO update entity kho on 9/9/2020 x.Include(o => o.XuatKhoDuocPhamChiTietViTri).ThenInclude(o => o.XuatKhoDuocPhamChiTiet).ThenInclude(o => o.XuatKhoDuocPham)x => x.Include(o => o.XuatKhoDuocPhamChiTietViTri).ThenInclude(o => o.NhapKhoDuocPhamChiTiet)
                 );
            //kiem tra truoc khi cap nhat
            if (ycDuocPham == null || (ycDuocPham.KhongTinhPhi != true && (ycDuocPham.TrangThaiThanhToan == Enums.TrangThaiThanhToan.DaThanhToan || ycDuocPham.TrangThaiThanhToan == Enums.TrangThaiThanhToan.CapNhatThanhToan)))
            {
                return "KhamBenhChiDinh.DuocPham.DaThanhToan";
            }
            else if(ycDuocPham.KhongTinhPhi == true && ycDuocPham.TrangThai == Enums.EnumYeuCauDuocPhamBenhVien.DaThucHien)
            {
                return "KhamBenhChiDinh.DuocPham.DaThucHien";
            }


            //hoan lai duoc pham da book trong kho
            ycDuocPham.WillDelete = true;
            //TODO update entity kho on 9/9/2020
            //ycDuocPham.XuatKhoDuocPhamChiTietViTri.NhapKhoDuocPhamChiTiet.SoLuongDaXuat -= ycDuocPham.XuatKhoDuocPhamChiTietViTri.SoLuongXuat;
            //ycDuocPham.XuatKhoDuocPhamChiTietViTri.WillDelete = true;
            //ycDuocPham.XuatKhoDuocPhamChiTietViTri.XuatKhoDuocPhamChiTiet.WillDelete = true;
            //ycDuocPham.XuatKhoDuocPhamChiTietViTri.XuatKhoDuocPhamChiTiet.XuatKhoDuocPham.WillDelete = true;

            await _yeuCauDuocPhamBenhVienRepository.UpdateAsync(ycDuocPham);
            return null;
        }




    }

}
