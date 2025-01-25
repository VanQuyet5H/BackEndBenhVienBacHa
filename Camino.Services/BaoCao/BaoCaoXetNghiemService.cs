using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using Camino.Core.DependencyInjection.Attributes;
using Camino.Core.Domain;
using Camino.Core.Domain.Entities.BenhNhans;
using Camino.Core.Domain.Entities.DichVuXetNghiems;
using Camino.Core.Domain.Entities.KhoDuocPhams;
using Camino.Core.Domain.Entities.XetNghiems;
using Camino.Core.Domain.Entities.YeuCauTiepNhans;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.BaoCao;
using Camino.Core.Domain.ValueObject.BaoCaoLuuketQuaXeNghiemTrongNgay;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Data;
using Camino.Services.Helpers;
using DotLiquid.Tags;
using Microsoft.EntityFrameworkCore;
using Camino.Core.Helpers;

namespace Camino.Services.BaoCao
{
    [ScopedDependency(ServiceType = typeof(IBaoCaoXetNghiemService))]
    public partial class BaoCaoXetNghiemService : IBaoCaoXetNghiemService
    {
        IRepository<YeuCauTiepNhan> _yeuCauTiepNhanRepository;
        IRepository<PhienXetNghiem> _phienXetNghiemRepository;
        IRepository<DichVuXetNghiemKetNoiChiSo> _dichVuXetNghiemKetNoiChiSoRepository;
        IRepository<Core.Domain.Entities.NhanViens.NhanVien> _nhanVienRepository;
        IRepository<Core.Domain.Entities.PhongBenhViens.PhongBenhVien> _phongBenhVienRepository;
        IRepository<Core.Domain.Entities.KhoaPhongs.KhoaPhong> _khoaPhongRepository;
        IRepository<Template> _templateRepository;
        IUserAgentHelper _userAgentHelper;
        public BaoCaoXetNghiemService(IRepository<YeuCauTiepNhan> yeuCauTiepNhanRepository, IRepository<PhienXetNghiem> phienXetNghiemRepository, IRepository<Core.Domain.Entities.NhanViens.NhanVien> nhanVienRepository,
            IRepository<Core.Domain.Entities.PhongBenhViens.PhongBenhVien> phongBenhVienRepository, IRepository<DichVuXetNghiemKetNoiChiSo> dichVuXetNghiemKetNoiChiSoRepository,
        IRepository<Core.Domain.Entities.KhoaPhongs.KhoaPhong> khoaPhongRepository,
             IRepository<Template> templateRepository,
             IUserAgentHelper userAgentHelper)
        {
            _yeuCauTiepNhanRepository = yeuCauTiepNhanRepository;
            _nhanVienRepository = nhanVienRepository;
            _phongBenhVienRepository = phongBenhVienRepository;
            _khoaPhongRepository = khoaPhongRepository;
            _templateRepository = templateRepository;
            _userAgentHelper = userAgentHelper;
            _phienXetNghiemRepository = phienXetNghiemRepository;
            _dichVuXetNghiemKetNoiChiSoRepository = dichVuXetNghiemKetNoiChiSoRepository;
        }

        #region báo cáo Lưu kết quả xe nghiệm người bệnh hàng ngày
        public async Task<GridDataSource> GetDataBaoCaoLuuKetQuaXetNghiemHangNgayForGridAsync(BaoCaoKetQuaXetNghiemQueryInfo queryInfo, bool exportExcel)
        {
            var tuNgay = queryInfo.TuNgay ?? DateTime.MinValue;
            var denNgay = queryInfo.DenNgay ?? DateTime.Now;

            var ketQuaXetNghiemQuery = _phienXetNghiemRepository.TableNoTracking
                .Where(o => o.PhienXetNghiemChiTiets.Any(ct => ct.ThoiDiemKetLuan != null && ct.ThoiDiemKetLuan >= tuNgay && ct.ThoiDiemKetLuan <= denNgay) &&
                        (queryInfo.NoiChiDinhId == null || o.PhienXetNghiemChiTiets.Any(ct=>ct.YeuCauDichVuKyThuat.NoiChiDinhId == queryInfo.NoiChiDinhId)) &&
                        (queryInfo.BHYT == null || (queryInfo.BHYT == false && (o.YeuCauTiepNhan.CoBHYT == null || o.YeuCauTiepNhan.CoBHYT == false)) || (queryInfo.BHYT == true && o.YeuCauTiepNhan.CoBHYT != null && o.YeuCauTiepNhan.CoBHYT == true)) &&
                        (queryInfo.KhamSucKhoe == null || (o.YeuCauTiepNhan.LoaiYeuCauTiepNhan == Enums.EnumLoaiYeuCauTiepNhan.KhamSucKhoe) == queryInfo.KhamSucKhoe));

            //IQueryable<PhienXetNghiem> ketQuaXetNghiemDataQuery;
            //if (exportExcel)
            //{
            //    ketQuaXetNghiemDataQuery = ketQuaXetNghiemQuery.OrderBy(o => o.BarCodeId);
            //}
            //else
            //{
            //    var ketQuaXetNghiemGroup = ketQuaXetNghiemQuery.Select(o=>new {o.BarCodeId, o.Id}).GroupBy(o=>o.BarCodeId).Skip(queryInfo.Skip).Take(queryInfo.Take).ToList();
            //    var phienXetNghiemIds = ketQuaXetNghiemGroup.SelectMany(o => o.Select(i => i.Id)).ToList();

            //    ketQuaXetNghiemDataQuery = _phienXetNghiemRepository.TableNoTracking.Where(o => phienXetNghiemIds.Contains(o.Id)).OrderBy(o => o.BarCodeId);
            //}


            var ketQuaXetNghiemData = ketQuaXetNghiemQuery
                .Include(o => o.YeuCauTiepNhan)
                .Include(o => o.PhienXetNghiemChiTiets).ThenInclude(o => o.KetQuaXetNghiemChiTiets)
                .Include(o => o.PhienXetNghiemChiTiets).ThenInclude(o => o.NhomDichVuBenhVien)
                .Include(o => o.PhienXetNghiemChiTiets).ThenInclude(o => o.YeuCauDichVuKyThuat).ThenInclude(o => o.NoiChiDinh)
                .Include(o => o.PhienXetNghiemChiTiets).ThenInclude(o => o.YeuCauDichVuKyThuat).ThenInclude(o => o.NhanVienChiDinh).ThenInclude(o => o.User)
                .Include(o => o.PhienXetNghiemChiTiets).ThenInclude(o => o.YeuCauDichVuKyThuat).ThenInclude(o => o.YeuCauKhamBenh)
                .Include(o => o.PhienXetNghiemChiTiets).ThenInclude(o => o.YeuCauDichVuKyThuat).ThenInclude(o => o.NoiTruPhieuDieuTri).ToList();

            var gridData = ketQuaXetNghiemData.GroupBy(o => o.BarCodeId)
                .Select((o, i) => new BaoCaoLuuketQuaXeNghiemTrongNgayGridVo
                {
                    STT = queryInfo.Skip + i + 1,
                    Sid = o.Key,
                    HoVaTen = o.First().YeuCauTiepNhan.HoTen,
                    NamSinh = o.First().YeuCauTiepNhan.NamSinh,
                    GioiTinh = o.First().YeuCauTiepNhan.GioiTinh,
                    BHYT = o.First().YeuCauTiepNhan.CoBHYT,
                    KhamSucKhoe =
                        o.First().YeuCauTiepNhan.LoaiYeuCauTiepNhan == Enums.EnumLoaiYeuCauTiepNhan.KhamSucKhoe,
                    NoiChiDinhs = o.SelectMany(p =>
                        p.PhienXetNghiemChiTiets.Select(ct => ct.YeuCauDichVuKyThuat.NoiChiDinh?.Ma)),
                    HoTenBacSis = o.SelectMany(p =>
                        p.PhienXetNghiemChiTiets.Select(ct => ct.YeuCauDichVuKyThuat.NhanVienChiDinh?.User.HoTen)),
                    ChanDoans = o.SelectMany(p => p.PhienXetNghiemChiTiets
                        .Select(ct => ct.YeuCauDichVuKyThuat.NoiTruPhieuDieuTri != null
                            ? ct.YeuCauDichVuKyThuat.NoiTruPhieuDieuTri.ChanDoanChinhGhiChu
                            : (ct.YeuCauDichVuKyThuat.YeuCauKhamBenh != null ? ct.YeuCauDichVuKyThuat.YeuCauKhamBenh.ChanDoanSoBoGhiChu : string.Empty)))
                }).ToArray();
            var ketnoichiso = _dichVuXetNghiemKetNoiChiSoRepository.TableNoTracking.Where(o => o.HieuLuc).ToList();

            foreach (var groupByBarcode in ketQuaXetNghiemData.GroupBy(o => o.BarCodeId))
            {
                var ketQuaPhienXetNghiemChiTiets = groupByBarcode.SelectMany(p => p.PhienXetNghiemChiTiets
                    .Where(ct => ct.ThoiDiemKetLuan != null && ct.ThoiDiemKetLuan >= tuNgay && ct.ThoiDiemKetLuan <= denNgay &&
                          (queryInfo.NoiChiDinhId == null || ct.YeuCauDichVuKyThuat.NoiChiDinhId == queryInfo.NoiChiDinhId))
                    //.Where(ct => ct.ThoiDiemKetLuan != null && ct.ThoiDiemKetLuan <= denNgay)
                    .Select(ct =>
                        new KetQuaPhienXetNghiemChiTiet
                        {
                            YeuCauDichVuKyThuatId = ct.YeuCauDichVuKyThuatId,
                            YeuCauDichVuKyThuatTen = ct.YeuCauDichVuKyThuat.TenDichVu,
                            LanThucHien = ct.LanThucHien,
                            NhomDichVuBenhVienId = ct.NhomDichVuBenhVienId,
                            NhomDichVuBenhVienTen = ct.NhomDichVuBenhVien.Ten,
                            KetQuaXetNghiemChiTiets = ct.KetQuaXetNghiemChiTiets
                        }));
                gridData.First(o => o.Sid == groupByBarcode.Key).KetQua = LISHelper.GetKetQuaCuaPhienXetNghiem(ketQuaPhienXetNghiemChiTiets, ketnoichiso);
            }

            //var totalRowCount = queryInfo.LazyLoadPage == true ? 0 : ketQuaXetNghiemQuery.GroupBy(o => o.BarCodeId).Count();
            return new GridDataSource { Data = exportExcel ? gridData.OrderBy(o=>o.Sid).ToArray() : gridData.OrderBy(o => o.Sid).Skip(queryInfo.Skip).Take(queryInfo.Take).ToArray(), TotalRowCount = gridData.Count() };
        }

        public async Task<GridDataSource> GetDataBaoCaoLuuKetQuaXetNghiemHangNgayTotalPageForGridAsync(BaoCaoKetQuaXetNghiemQueryInfo queryInfo)
        {
            var tuNgay = queryInfo.TuNgay ?? DateTime.MinValue;
            var denNgay = queryInfo.DenNgay ?? DateTime.Now;

            var ketQuaXetNghiem = _phienXetNghiemRepository.TableNoTracking
                .Where(o =>  (queryInfo.NoiChiDinhId == null || o.PhienXetNghiemChiTiets.Any(ct => ct.YeuCauDichVuKyThuat.NoiChiDinhId == queryInfo.NoiChiDinhId)) &&
                            (queryInfo.BHYT == null || (queryInfo.BHYT == false && (o.YeuCauTiepNhan.CoBHYT == null || o.YeuCauTiepNhan.CoBHYT == false)) || (queryInfo.BHYT == true && o.YeuCauTiepNhan.CoBHYT != null && o.YeuCauTiepNhan.CoBHYT == true)) &&
                            (queryInfo.KhamSucKhoe == null || (o.YeuCauTiepNhan.LoaiYeuCauTiepNhan == Enums.EnumLoaiYeuCauTiepNhan.KhamSucKhoe) == queryInfo.KhamSucKhoe))
                .Select(o=>new { o.BarCodeId, PhienXetNghiemChiTietIds = o.PhienXetNghiemChiTiets.Where(ct => ct.ThoiDiemKetLuan != null && ct.ThoiDiemKetLuan >= tuNgay && ct.ThoiDiemKetLuan <= denNgay).Select(ct=>ct.Id).ToList() })
                .ToList();

            var totalRowCount = ketQuaXetNghiem.Where(o => o.PhienXetNghiemChiTietIds.Any()).GroupBy(o => o.BarCodeId).Count();

            //var ketQuaXetNghiemQuery = _phienXetNghiemRepository.TableNoTracking
            //    .Where(o => o.PhienXetNghiemChiTiets.Any(ct => ct.ThoiDiemKetLuan != null && ct.ThoiDiemKetLuan >= tuNgay && ct.ThoiDiemKetLuan <= denNgay) &&
            //                (queryInfo.NoiChiDinhId == null || o.PhienXetNghiemChiTiets.Any(ct => ct.YeuCauDichVuKyThuat.NoiChiDinhId == queryInfo.NoiChiDinhId)) &&
            //                (queryInfo.BHYT == null || o.YeuCauTiepNhan.CoBHYT.GetValueOrDefault() == queryInfo.BHYT) &&
            //                (queryInfo.KhamSucKhoe == null || (o.YeuCauTiepNhan.LoaiYeuCauTiepNhan == Enums.EnumLoaiYeuCauTiepNhan.KhamSucKhoe) == queryInfo.KhamSucKhoe));

            //var totalRowCount = ketQuaXetNghiemQuery.GroupBy(o => o.BarCodeId).Count();
            return new GridDataSource { TotalRowCount = totalRowCount };
        }
        #endregion

        //public GridDataSource GetBaoCaoKetQuaXetNghiemForGrid(BaoCaoKetQuaXetNghiemQueryInfo queryInfo)
        //{




        //    var gridData = new List<BaoCaoThuPhiVienPhiGridVo>();
        //    var num = 0;
        //    foreach (var thuNgan in groupThuNgan)
        //    {
        //        num++;
        //        foreach (var baoCaoThuPhiVienPhiGridVo in thuNgan.ToList())
        //        {
        //            baoCaoThuPhiVienPhiGridVo.STT = num;
        //            gridData.Add(baoCaoThuPhiVienPhiGridVo);
        //        }
        //    }

        //    return new GridDataSource { Data = gridData.ToArray(), TotalRowCount = groupThuNgan.Count() };
        //}
        public async Task<List<LookupItemVo>> GetListBHYT(DropDownListRequestModel model)
        {
            List<LookupItemVo> listDataBHYT = new List<LookupItemVo>();
            var itemBHYT = new LookupItemVo { KeyId = 1, DisplayName = "BHYT" };
            var itemKhongBHYT = new LookupItemVo { KeyId = 2, DisplayName = "Không BHYT" };
            var itemAll = new LookupItemVo { KeyId = 0, DisplayName = "Tất cả" };
            listDataBHYT.Add(itemAll);
            listDataBHYT.Add(itemBHYT);
            listDataBHYT.Add(itemKhongBHYT);

            var query = listDataBHYT.OrderBy(s => s.KeyId).Select(item => new LookupItemVo()
            {
                DisplayName = item.DisplayName,
                KeyId = item.KeyId,
            }).ToList();
            return query;
        }
        public async Task<List<LookupItemVo>> GetListKSK(DropDownListRequestModel model)
        {
            List<LookupItemVo> listBHYT = new List<LookupItemVo>();
            var itemKSK= new LookupItemVo { KeyId = 1, DisplayName = "Có KSK" };
            var itemKhongKSK = new LookupItemVo { KeyId = 2, DisplayName = "Không KSK" };
            var itemAll = new LookupItemVo { KeyId = 0, DisplayName = "Tất cả" };
            listBHYT.Add(itemAll);
            listBHYT.Add(itemKSK);
            listBHYT.Add(itemKhongKSK);

            var query = listBHYT.OrderBy(s => s.KeyId).Select(item => new LookupItemVo()
            {
                DisplayName = item.DisplayName,
                KeyId = item.KeyId,
            }).ToList();
            return query;
        }
        public async Task<string> XuLyInBaoCaoLuuKetQuaXetNghiemHangNgayAsync(BaoCaoKetQuaXetNghiemQueryInfo phieuInNhanVienKhamSucKhoeInfoVo, ICollection<BaoCaoLuuketQuaXeNghiemTrongNgayGridVo> data)
        {
            var content = "";
            var dataRow = "";
            var result = _templateRepository.TableNoTracking
            .FirstOrDefault(x => x.Name.Equals("BaoCaoLuuKetQuaXetNghiemHangNgay"));

            long userId = _userAgentHelper.GetCurrentUserId();
            string nguoiLogin = _nhanVienRepository.TableNoTracking.Where(x => x.Id == userId).Select(s => s.User.HoTen).FirstOrDefault();
            BaoCaoKetQuaXetNghiemIn dataBind = new BaoCaoKetQuaXetNghiemIn();

            dataBind.TuNgay = phieuInNhanVienKhamSucKhoeInfoVo.TuNgay?.ApplyFormatDate();
            dataBind.DenNgay = phieuInNhanVienKhamSucKhoeInfoVo.DenNgay?.ApplyFormatDate();
            dataBind.GioTuNgay = phieuInNhanVienKhamSucKhoeInfoVo.DenNgay?.ApplyFormatTime();
            dataBind.GioTuNgay = phieuInNhanVienKhamSucKhoeInfoVo.TuNgay?.ApplyFormatTime();

            dataBind.NguoiLap = nguoiLogin;


            int stt = 1;
            foreach (var item in data)
            {
                dataRow += "<tr>" +
                           "<td style ='width:3%;' class='borderCot'>" + stt + "</td>" +
                           "<td style ='width:6%;' class='borderCot'>" + item.Sid + "</td>" +
                           "<td style ='width:10%;' class='borderCot'>" + item.HoVaTen + "</td>" +
                           "<td style ='width:4%;' class='borderCot'>" + item.NamSinhDisplay + "</td>" +
                           "<td style ='width:4%;' class='borderCot'>" + item.LoaiGioiTinhDisplay + "</td>" +
                           "<td style ='width:10%;' class='borderCot'>" + item.NoiChiDinh + "</td>" +
                           "<td style ='width:3%;' class='borderCot'>" + item.BHYTDisplay + "</td>" +
                           "<td style ='width:3%;' class='borderCot'>" + item.KhamSucKhoeDisplay + "</td>" +
                           "<td style ='width:10%;' class='borderCot'>" + item.HoTenBacSi + "</td>" +
                           "<td style ='width:15%;' class='borderCot'>" + item.ChanDoan + "</td>" +
                           "<td style ='width:32%;' class='borderCot'>" + item.KetQua + "</td>" + "</tr>";
                stt++;
            }
            dataBind.Data = dataRow;
            content = TemplateHelpper.FormatTemplateWithContentTemplate(result.Body, dataBind);
            return content; ;
        }
    }
   
}
