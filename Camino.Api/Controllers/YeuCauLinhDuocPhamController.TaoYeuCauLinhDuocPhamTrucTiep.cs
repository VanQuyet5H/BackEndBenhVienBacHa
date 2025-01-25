using Camino.Api.Auth;
using Camino.Api.Extensions;
using Camino.Api.Models.LinhDuocPhamTrucTiep;
using Camino.Core.Domain;
using Camino.Core.Domain.Entities.YeuCauLinhDuocPhams;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Domain.ValueObject.LinhDuocPham;
using Camino.Core.Domain.ValueObject.YeuCauLinhDuocPham;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static Camino.Core.Domain.Enums;

namespace Camino.Api.Controllers
{
    public partial class YeuCauLinhDuocPhamController
    {
        #region thong tin tạo linh duoc pham
        [HttpPost("GetListKhoLinh")]
        public async Task<ActionResult<ICollection<DanhSachLinhVeKhoGridVo>>> GetListNhomChucDanh([FromBody]DropDownListRequestModel queryInfo)
        {
            var lookup = await _yeuCauLinhDuocPhamService.GetListKhoLinhVe(queryInfo);
            return Ok(lookup);
        }
        [HttpPost("GetData")]
        public async Task<ActionResult<GridDataSource>> GetData([FromBody]LinhDuocPhamTrucTiepQueryInfo linhDuocPhamTrucTiepQueryInfo)
        {
            //long idKhoLinh,long phongDangNhapId,string dateSearchStart, string dateSearchEnd
            var lookup = _yeuCauLinhDuocPhamService.GetData(linhDuocPhamTrucTiepQueryInfo.idKhoLinh, linhDuocPhamTrucTiepQueryInfo.phongDangNhapId, linhDuocPhamTrucTiepQueryInfo.dateSearchStart, linhDuocPhamTrucTiepQueryInfo.dateSearchEnd);
            return Ok(lookup);
        }
        [HttpPost("GetDataDaTao")]
        public async Task<ActionResult<GridDataSource>> GetDataDaTao(long idKhoLinh, long idYeuCauLinhDuocPham, long phongDangNhapId, long trangThai)
        {
            var lookup = _yeuCauLinhDuocPhamService.GetDataDaTao(idKhoLinh, idYeuCauLinhDuocPham, phongDangNhapId,trangThai);
            return Ok(lookup);
        }
        [HttpPost("ThongTinLinhTuKho")]
        public async Task<ActionResult<GridDataSource>> ThongTinLinhTuKho(long idKhoLinh)
        {
            var lookup = _yeuCauLinhDuocPhamService.GetDataThongTin(idKhoLinh);
            return Ok(lookup);
        }
        // 
        [HttpPost("ThongTinDanhSachCanLinh")]
        public async Task<ActionResult<GridDataSource>> ThongTinDanhSachCanLinh(long idKhoLinh,long phongLinhVeId)
        {
            var lookup = _yeuCauLinhDuocPhamService.ThongTinDanhSachCanLinh(idKhoLinh,phongLinhVeId);
            return Ok(lookup);
        }
        [HttpPost("ThongTinLinhTuKhoDaTao")]
        public async Task<ActionResult<GridDataSource>> ThongTinLinhTuKhoDaTao(long idYeuCauLinhDP)
        {
            var lookup = _yeuCauLinhDuocPhamService.GetDataThongTinDaTao(idYeuCauLinhDP);
            return Ok(lookup);
        }
        [HttpPost("GetTrangThaiDuyet")]
        public bool? GetTrangThaiDuyet(long IdYeuCauLinh)
        {
            var lookup = _yeuCauLinhDuocPhamService.GetTrangThaiDuyet(IdYeuCauLinh);
            return lookup;
        }
        [HttpPost("GetDaDuyet")]
        public DaDuyet GetDaDuyet(long IdYeuCauLinh)
        {
            var lookup = _yeuCauLinhDuocPhamService.GetDaDuyet(IdYeuCauLinh);
            return lookup;
        }
        // create
        [HttpPost("GetAllYeuCauLinhThuocTuKho")]
        public async Task<ActionResult<GridDataSource>> GetDataForGridChildAsync([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _yeuCauLinhDuocPhamService.GetDataForGridChildAsync(queryInfo);
            return Ok(gridData);
        }
        // đã tạo
        [HttpPost("GetAllYeuCauLinhThuocTuKhoDaTao")]
        public async Task<ActionResult<GridDataSource>> GetAllYeuCauLinhThuocTuKhoDaTao([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _yeuCauLinhDuocPhamService.GetAllYeuCauLinhThuocTuKhoDaTao(queryInfo);
            return Ok(gridData);
        }
        [HttpPost("GetDataThuocAsync")]
        public async Task<ActionResult<GridDataSource>> GetDataThuocAsync(long yeuCauTiepNhanId, long phongBenhVienId, long nhanVienLogin, long khoLinhId)
        {
            var gridData = await _yeuCauLinhDuocPhamService.GetDataThuocAsync(yeuCauTiepNhanId, phongBenhVienId, nhanVienLogin, khoLinhId);
            return Ok(gridData);
        }
        #endregion
        #region CRUD
        [HttpPost("GuiPhieuLinhTrucTiep")]
        [ClaimRequirement(Enums.SecurityOperation.Add, Enums.DocumentType.TaoYeuCauLinhTrucTiepDuocPham)]
        public async Task<ActionResult> GuiPhieuLinhTrucTiep(LinhDuocPhamTrucTiepViewModel linhTrucTiepDuocPhamVM)
        {
            List<LinhTrucTiepDuocPhamChiTietGridVo> listDuocPhamDuocTao = new List<LinhTrucTiepDuocPhamChiTietGridVo>();
            var listYeuCauDuocPhamBenhVienDuocTao = new List<long>();
            if (linhTrucTiepDuocPhamVM.YeuCauDuocPhamBenhIds != null)  // kiểm tra những yêu cauad dược phẩm bệnh viện đủ số lượng tồn để tạo
            {
                // lấy hết những ycdp bệnh viện ids Ui truyền xuống
             
                foreach (var yeuCauDuocPhamBenhVienId in linhTrucTiepDuocPhamVM.YeuCauDuocPhamBenhIds.Distinct())
                {
                    var yeuCauDuocPhamBenhVien = _yeuCauLinhDuocPhamService.GetDataForGridChiTietChildCreateAsync(yeuCauDuocPhamBenhVienId);
                    listDuocPhamDuocTao.AddRange(yeuCauDuocPhamBenhVien);
                   
                }
                // group lại kiểm tra dược phẩm trùng sum số lượng nhỏ hơn số lượng tồn => list id cuối cùng được tạo
              
                if(listDuocPhamDuocTao.Any())
                {
                    var groupDuocPhamDuocTao = listDuocPhamDuocTao.GroupBy(s => new { s.DuocPhamBenhVienId, s.LaDuocPhamBHYT })
                                                                  .Select(d => new LinhTrucTiepDuocPhamChiTietGridVo()
                                                                  {
                                                                      LaDuocPhamBHYT = d.First().LaDuocPhamBHYT,
                                                                      DuocPhamBenhVienId = d.First().DuocPhamBenhVienId,
                                                                      SoLuong = d.Sum(f=>f.SoLuong),
                                                                      SLTon = d.First().SLTon,
                                                                      YeuCauDuocPhamBenhVienIds = d.Select(g=>g.Id).ToList()
                                                                  }).Where(s => s.SLTon >= s.SoLuong).ToList();
                    foreach(var itemIdDuoctao in groupDuocPhamDuocTao)
                    {
                        listYeuCauDuocPhamBenhVienDuocTao.AddRange(itemIdDuoctao.YeuCauDuocPhamBenhVienIds);
                    }
                   
                }
                // xử lý tạo yêu cầu lĩnh dược phẩm chi tiết được chẹck
                if(listYeuCauDuocPhamBenhVienDuocTao.Any())
                {
                    foreach (var yeuCauDuocPhamBenhVienId in listYeuCauDuocPhamBenhVienDuocTao)
                    {
                        var linhTrucTiepDuocPhamChiTietDuocTao = _yeuCauLinhDuocPhamService.GetDataForGridChiTietChildCreateAsync(yeuCauDuocPhamBenhVienId);
                        foreach (var linhttt in linhTrucTiepDuocPhamChiTietDuocTao)
                        {
                            var addnew = new LinhTrucTiepDuocPhamChiTietViewModel();
                            addnew.LaDuocPhamBHYT = linhttt.LaDuocPhamBHYT;
                            addnew.SoLuong = linhttt.SoLuong;
                            addnew.DuocPhamBenhVienId = linhttt.DuocPhamBenhVienId;
                            addnew.YeuCauDuocPhamBenhVienId = linhttt.Id;
                            linhTrucTiepDuocPhamVM.YeuCauLinhDuocPhamChiTiets.Add(addnew);
                        }
                    }
                }
                // YeuCauLinhDuocPhamChiTiets == null thì không có dược phẩm đủ điều kiện để tạo
                if (!linhTrucTiepDuocPhamVM.YeuCauLinhDuocPhamChiTiets.Any())
                {
                    throw new Models.Error.ApiException(_localizationService.GetResource("LinhThuongDuocPham.LinhDuocPhamChiTiets.Required"));
                }
            }

            // xử lý nếu kho nhập id = kho nhân vien quản lý  nếu tài khoản nhân viên chưa có kho nhân viên quản lý không cho tạo 
            if (!linhTrucTiepDuocPhamVM.YeuCauDuocPhamBenhIds.Any())
            {
                throw new Models.Error.ApiException(_localizationService.GetResource("LinhTrucTiepDuocPham.YeuCauDuocPhamBenhVien.Required"));
            }
            if (linhTrucTiepDuocPhamVM.KhoNhapId != null)
            {
                var linhNhapId = _yeuCauLinhDuocPhamService.GetIdKhoNhap(linhTrucTiepDuocPhamVM.KhoNhapId);
                if (linhNhapId != 0)
                {
                    linhTrucTiepDuocPhamVM.KhoNhapId = linhNhapId;
                }
                else
                {
                    var KhoLinhNhapTheoNhanVien = _yeuCauLinhDuocPhamService.GetAllIdKhoNhapNhanVien(linhTrucTiepDuocPhamVM.NhanVienYeuCauId);
                    if (KhoLinhNhapTheoNhanVien != 0)
                    {
                        linhTrucTiepDuocPhamVM.KhoNhapId = KhoLinhNhapTheoNhanVien;
                    }
                    else
                    {
                        throw new ArgumentException(_localizationService
                                                    .GetResource("Common.NotCreatePhieuLinhTrucTiep"));
                    }
                }
            }
            linhTrucTiepDuocPhamVM.NgayYeuCau = DateTime.Now;
            linhTrucTiepDuocPhamVM.LoaiPhieuLinh = EnumLoaiPhieuLinh.LinhChoBenhNhan;
            var linhtt = linhTrucTiepDuocPhamVM.ToEntity<YeuCauLinhDuocPham>();
            linhtt.NoiYeuCauId = _userAgentHelper.GetCurrentNoiLLamViecId();
            if(linhTrucTiepDuocPhamVM.Goi == true)
            {
                linhtt.DaGui = true;
            }
            else
            {
                linhtt.DaGui = false;
            }
            

            linhtt.ThoiDiemLinhTongHopTuNgay = linhTrucTiepDuocPhamVM.ThoiDiemLinhTongHopTuNgay;

            if(linhTrucTiepDuocPhamVM.ThoiDiemLinhTongHopDenNgay == null)
            {
                if (linhtt.ThoiDiemLinhTongHopTuNgay > DateTime.Now)
                {
                    linhtt.ThoiDiemLinhTongHopDenNgay = linhtt.ThoiDiemLinhTongHopTuNgay;
                }
                else
                {
                    linhtt.ThoiDiemLinhTongHopDenNgay = DateTime.Now;
                }
            }
            else
            {
                linhtt.ThoiDiemLinhTongHopDenNgay = linhTrucTiepDuocPhamVM.ThoiDiemLinhTongHopDenNgay;
            }
         
            
            linhtt.GhiChu = linhTrucTiepDuocPhamVM.GhiChu;
            List<long> lst = listYeuCauDuocPhamBenhVienDuocTao.ToList();
            await _yeuCauLinhDuocPhamService.XuLyThemYeuCauLinhDuocPhamTTAsync(linhtt, lst);
            await _yeuCauLinhDuocPhamService.AddAsync(linhtt);
            return Ok(linhtt.Id);
        }
        [HttpGet("GetAll")]
        public async Task<ActionResult<LinhDuocPhamTrucTiepViewModel>> GetALL(long id)
        {
            var result = await _yeuCauLinhDuocPhamService.GetByIdAsync(id, s => s.Include(k => k.YeuCauDuocPhamBenhViens).Include(f=>f.KhoXuat));
            if (result == null)
            {
                return NotFound();
            }
            var resultData = result.ToModel<LinhDuocPhamTrucTiepViewModel>();
            return Ok(resultData);
        }
        //  gui lai phieu linh truc tiep -- va save
        [HttpPost("GuiLaiPhieuLinhTrucTiep")]
        [ClaimRequirement(Enums.SecurityOperation.Add, Enums.DocumentType.TaoYeuCauLinhTrucTiepDuocPham)]
        public async Task<ActionResult> GuiLaiPhieuLinhTrucTiep(LinhDuocPhamTrucTiepViewModel linhTrucTiepDuocPhamVM)
        {
            var listIdCuCanXoa = new List<long>();
            List<LinhTrucTiepDuocPhamChiTietGridVo> listDuocPhamDuocTao = new List<LinhTrucTiepDuocPhamChiTietGridVo>();
            var listYeuCauDuocPhamBenhVienDuocTao = new List<long>();
            if (linhTrucTiepDuocPhamVM.YeuCauLinhDuocPhamId != null)
            {
              
                // gét những yêu cầu dược phẩm đã tạo theo YeuCauLinhDuocPhamId field DaGui = false
                // tạo mới yêu cầu lĩnh dược phẩm chi tiết theo yêu cầu dược phẩm cần update 
                // update DaGui = true
                //1.------------------------------------------//
                
                if(linhTrucTiepDuocPhamVM.YeuCauDuocPhamBenhIds.Any())
                {
                    // lấy hết những ycdp bệnh viện ids Ui truyền xuống
                  
                    foreach (var yeuCauDuocPhamBenhVienId in linhTrucTiepDuocPhamVM.YeuCauDuocPhamBenhIds.Distinct())
                    {
                        var yeuCauDuocPhamBenhVien = _yeuCauLinhDuocPhamService.GetDataForGridChiTietChildCreateAsync(yeuCauDuocPhamBenhVienId);
                        listDuocPhamDuocTao.AddRange(yeuCauDuocPhamBenhVien);

                    }
                    // group lại kiểm tra dược phẩm trùng sum số lượng nhỏ hơn số lượng tồn => list id cuối cùng được tạo

                    if (listDuocPhamDuocTao.Any())
                    {
                        var groupDuocPhamDuocTao = listDuocPhamDuocTao.GroupBy(s => new { s.DuocPhamBenhVienId, s.LaDuocPhamBHYT })
                                                                      .Select(d => new LinhTrucTiepDuocPhamChiTietGridVo()
                                                                      {
                                                                          LaDuocPhamBHYT = d.First().LaDuocPhamBHYT,
                                                                          DuocPhamBenhVienId = d.First().DuocPhamBenhVienId,
                                                                          SoLuong = d.Sum(f => f.SoLuong),
                                                                          SLTon = d.First().SLTon,
                                                                          YeuCauDuocPhamBenhVienIds = d.Select(g => g.Id).ToList()
                                                                      }).Where(s => s.SLTon >= s.SoLuong).ToList();
                        foreach (var itemIdDuoctao in groupDuocPhamDuocTao)
                        {
                            listYeuCauDuocPhamBenhVienDuocTao.AddRange(itemIdDuoctao.YeuCauDuocPhamBenhVienIds);
                        }

                    }
                    // xử lý tạo yêu cầu lĩnh dược phẩm chi tiết được chẹck
                    if (listYeuCauDuocPhamBenhVienDuocTao.Any())
                    {
                        foreach (var yeuCauDuocPhamBenhVienId in listYeuCauDuocPhamBenhVienDuocTao)
                        {
                            var linhTrucTiepDuocPhamChiTietDuocTao = _yeuCauLinhDuocPhamService.GetDataForGridChiTietChildCreateAsync(yeuCauDuocPhamBenhVienId);
                            foreach (var linhttt in linhTrucTiepDuocPhamChiTietDuocTao)
                            {
                                var addnew = new LinhTrucTiepDuocPhamChiTietViewModel();
                                addnew.LaDuocPhamBHYT = linhttt.LaDuocPhamBHYT;
                                addnew.SoLuong = linhttt.SoLuong;
                                addnew.DuocPhamBenhVienId = linhttt.DuocPhamBenhVienId;
                                addnew.YeuCauLinhDuocPhamId = linhTrucTiepDuocPhamVM.YeuCauLinhDuocPhamId;
                                linhTrucTiepDuocPhamVM.YeuCauLinhDuocPhamChiTiets.Add(addnew);
                            }
                        }
                    }
                    //// YeuCauLinhDuocPhamChiTiets == null thì không có dược phẩm đủ điều kiện để tạo
                    //if (!linhTrucTiepDuocPhamVM.YeuCauLinhDuocPhamChiTiets.Any())
                    //{
                    //    throw new Models.Error.ApiException(_localizationService.GetResource("LinhThuongDuocPham.LinhDuocPhamChiTiets.Required"));
                    //}
                }
                //// xử lý nếu kho nhập id = kho nhân vien quản lý  nếu tài khoản nhân viên chưa có kho nhân viên quản lý không cho tạo 
                //if (!linhTrucTiepDuocPhamVM.YeuCauDuocPhamBenhIds.Any())
                //{
                //    throw new Models.Error.ApiException(_localizationService.GetResource("LinhTrucTiepDuocPham.YeuCauDuocPhamBenhVien.Required"));
                //}
                var listYeuCauDuocPhamBenhVienChoGoi = new List<long>();
                listYeuCauDuocPhamBenhVienChoGoi = _yeuCauLinhDuocPhamService.GetYeuCauDuocPhamIdDaTao((long)linhTrucTiepDuocPhamVM.YeuCauLinhDuocPhamId);
                // kiem tra những dịch vụ mới 
                if (linhTrucTiepDuocPhamVM.YeuCauDuocPhamBenhIds.Any())
                {
                    foreach (var item in listYeuCauDuocPhamBenhVienChoGoi)
                    {
                        if (!linhTrucTiepDuocPhamVM.YeuCauDuocPhamBenhIds.Any(d => d == item))
                        {
                            listIdCuCanXoa.Add(item);
                        }

                    }
                }

                if (listIdCuCanXoa.Any())
                {
                    _yeuCauLinhDuocPhamService.XuLyHuyYeuCauDuocPhamTTAsync(listIdCuCanXoa);
                }
                if (linhTrucTiepDuocPhamVM.KhoNhapId != null)
                {
                    var linhNhapId = _yeuCauLinhDuocPhamService.GetIdKhoNhap(linhTrucTiepDuocPhamVM.KhoNhapId);
                    if (linhNhapId != 0)
                    {
                        linhTrucTiepDuocPhamVM.KhoNhapId = linhNhapId;
                    }
                    else
                    {
                        var KhoLinhNhapTheoNhanVien = _yeuCauLinhDuocPhamService.GetAllIdKhoNhapNhanVien(linhTrucTiepDuocPhamVM.NhanVienYeuCauId);
                        if (KhoLinhNhapTheoNhanVien != 0)
                        {
                            linhTrucTiepDuocPhamVM.KhoNhapId = KhoLinhNhapTheoNhanVien;
                        }
                        else
                        {
                            throw new ArgumentException(_localizationService
                                                        .GetResource("Common.NotCreatePhieuLinhTrucTiep"));
                        }
                    }
                }
                var yeuCauLinh = await _yeuCauLinhDuocPhamService.GetByIdAsync((long)linhTrucTiepDuocPhamVM.YeuCauLinhDuocPhamId,
              x => x.Include(d => d.YeuCauLinhDuocPhamChiTiets)
                  .Include(d => d.YeuCauDuocPhamBenhViens)
                  .Include(a => a.NhanVienYeuCau).ThenInclude(b => b.User)
                      .Include(a => a.NhanVienDuyet).ThenInclude(b => b.User)
                      .Include(a => a.KhoNhap)
                      .Include(a => a.KhoXuat).ThenInclude(b => b.NhapKhoDuocPhams).ThenInclude(c => c.NhapKhoDuocPhamChiTiets)
                      .Include(a => a.XuatKhoDuocPhams).ThenInclude(b => b.NguoiXuat).ThenInclude(c => c.User)
                      .Include(a => a.XuatKhoDuocPhams).ThenInclude(b => b.NguoiNhan).ThenInclude(c => c.User));

                linhTrucTiepDuocPhamVM.NgayYeuCau = DateTime.Now;
                linhTrucTiepDuocPhamVM.LoaiPhieuLinh = EnumLoaiPhieuLinh.LinhChoBenhNhan;
                var linhtt = linhTrucTiepDuocPhamVM.ToEntity<YeuCauLinhDuocPham>();
                linhtt.NoiYeuCauId = _userAgentHelper.GetCurrentNoiLLamViecId();

                // chọn từ ngày != null ? thời điểm lĩnh tổng hợp từ ngày bằng = từ ngày 
                // nếu từ ngày == null ? thời điểm lĩnh tổng hợp từ ngày bằng = yêu câu dược phẩm bện viện có ngày điều trị nhỏ nhất 
                linhtt.ThoiDiemLinhTongHopTuNgay = linhTrucTiepDuocPhamVM.ThoiDiemLinhTongHopTuNgay;

                // chọn đến ngày != null ? thời điểm lĩnh tổng hợp đến ngày bằng = đến ngày 
                // nếu từ ngày == null ? thời điểm lĩnh tổng hợp từ ngày bằng = yêu câu dược phẩm bện viện có ngày điều trị nhỏ nhất 
                // + linhtt.ThoiDiemLinhTongHopTuNgay > ngày hiện tại thì  => đến ngày bằng từ ngày
                // + linhtt.ThoiDiemLinhTongHopTuNgay < ngày hiện tại thì  => đến ngày bằng ngày hiện tại
                if (linhTrucTiepDuocPhamVM.ThoiDiemLinhTongHopDenNgay == null)
                {
                    if (linhtt.ThoiDiemLinhTongHopTuNgay > DateTime.Now)
                    {
                        linhtt.ThoiDiemLinhTongHopDenNgay = linhtt.ThoiDiemLinhTongHopTuNgay;
                    }
                    else
                    {
                        linhtt.ThoiDiemLinhTongHopDenNgay = DateTime.Now;
                    }
                }
                else
                {
                    linhtt.ThoiDiemLinhTongHopDenNgay = linhTrucTiepDuocPhamVM.ThoiDiemLinhTongHopDenNgay;
                }

                List<long> lst = listYeuCauDuocPhamBenhVienDuocTao.ToList();
                await _yeuCauLinhDuocPhamService.XuLyThemYeuCauLinhDuocPhamTTAsync(linhtt, lst);

                #region  có thời gian viết lại -- nam
                foreach (var item in linhtt.YeuCauDuocPhamBenhViens)
                {
                    item.YeuCauLinhDuocPhamId = linhTrucTiepDuocPhamVM.YeuCauLinhDuocPhamId;
                    yeuCauLinh.YeuCauDuocPhamBenhViens.Add(item);
                }
                foreach (var item in linhtt.YeuCauLinhDuocPhamChiTiets)
                {
                    item.YeuCauLinhDuocPhamId = (long)linhTrucTiepDuocPhamVM.YeuCauLinhDuocPhamId;
                    yeuCauLinh.YeuCauLinhDuocPhamChiTiets.Add(item);
                }
                if(linhTrucTiepDuocPhamVM.SaveOrUpDate == true)
                {
                    yeuCauLinh.DaGui = false;
                }
                else
                {
                    yeuCauLinh.DaGui = true;
                }
                
                yeuCauLinh.GhiChu = linhTrucTiepDuocPhamVM.GhiChu;

                // khi gửi cập nhật thời gian gửi tại thời điểm hiện tại
                yeuCauLinh.NgayYeuCau = DateTime.Now;

                #endregion

                await _yeuCauLinhDuocPhamService.UpdateAsync(yeuCauLinh);
            }
            return Ok(linhTrucTiepDuocPhamVM.YeuCauLinhDuocPhamId);
        }
        #region In lĩnh duoc pham TT đã  tạo
        [HttpPost("InPhieuLinhTrucTiepDuocPham")]
        public string InPhieuLinhTrucTiepDuocPham([FromBody]XacNhanInLinhDuocPham xacNhanInLinhDuocPham)
        {
            var htmlLinhDuocPham = _yeuCauLinhDuocPhamService.InPhieuLinhTrucTiepDuocPham(xacNhanInLinhDuocPham);
            return htmlLinhDuocPham;
        }
        #endregion
        [HttpPost("GuiLaiPhieuLinhTrucTiepVaDuyet")]
        [ClaimRequirement(Enums.SecurityOperation.Add, Enums.DocumentType.TaoYeuCauLinhTrucTiepDuocPham)]
        public async Task<ActionResult> GuiLaiPhieuLinhTrucTiepVaDuyet(LinhDuocPhamTrucTiepViewModel linhTrucTiepDuocPhamVM)
        {
            var lstYCDPBVCu = await _yeuCauLinhDuocPhamService.GetByIdAsync(linhTrucTiepDuocPhamVM.Id, s => s.Include(x => x.YeuCauDuocPhamBenhViens));
            foreach(var itemx in lstYCDPBVCu.YeuCauDuocPhamBenhViens)
            {
                var entity = _yeuCauDuocPhamBenhVienService.GetById(itemx.Id);
                entity.YeuCauLinhDuocPhamId = null;
            }
            if (linhTrucTiepDuocPhamVM.KhoNhapId != null)
            {
                var linhNhapId = _yeuCauLinhDuocPhamService.GetIdKhoNhap(linhTrucTiepDuocPhamVM.KhoNhapId);
                if (linhNhapId != 0)
                {
                    linhTrucTiepDuocPhamVM.KhoNhapId = linhNhapId;
                }
                else
                {
                    var KhoLinhNhapTheoNhanVien = _yeuCauLinhDuocPhamService.GetAllIdKhoNhapNhanVien(linhTrucTiepDuocPhamVM.NhanVienYeuCauId);
                    if (KhoLinhNhapTheoNhanVien != 0)
                    {
                        linhTrucTiepDuocPhamVM.KhoNhapId = KhoLinhNhapTheoNhanVien;
                    }
                    else
                    {
                        throw new ArgumentException(_localizationService
                                                    .GetResource("Common.NotCreatePhieuLinhTrucTiep"));
                    }
                }
            }
            linhTrucTiepDuocPhamVM.NgayYeuCau = lstYCDPBVCu.NgayYeuCau;
            linhTrucTiepDuocPhamVM.LoaiPhieuLinh = EnumLoaiPhieuLinh.LinhChoBenhNhan;
            linhTrucTiepDuocPhamVM.ToEntity(lstYCDPBVCu);
            lstYCDPBVCu.NoiYeuCauId = _userAgentHelper.GetCurrentNoiLLamViecId();
            List<long> lst = linhTrucTiepDuocPhamVM.YeuCauDuocPhamBenhViensTT.Select(s => s.Id).ToList();
            await _yeuCauLinhDuocPhamService.XuLyThemYeuCauLinhDuocPhamTTAsync(lstYCDPBVCu, lst);
            await _yeuCauLinhDuocPhamService.UpdateAsync(lstYCDPBVCu);
            return Ok(lstYCDPBVCu.Id);
        }
        #endregion
        #region In lĩnh duoc pham chưa tạo
        [HttpPost("XemTruocInPhieuLinhTrucTiepDuocPham")]
        public string XemTruocInPhieuLinhTrucTiepDuocPham(XacNhanInLinhDuocPhamXemTruoc xacNhanInLinhDuocPhamXemTruoc)
        {
            var htmlLinhDuocPham = _yeuCauLinhDuocPhamService.InPhieuLinhTrucTiepDuocPhamXemTruoc(xacNhanInLinhDuocPhamXemTruoc);
            return htmlLinhDuocPham;
        }
        #endregion
        #region  get danh sach cho goi linh tt 30072021
        [HttpPost("GetDataGridYeuCauLinhDuocPhamLuuTamThoi")]
        public async Task<ActionResult<GridDataSource>> GetData(VoQueryDSChoGoi vo)
        {
            // gio không lấy theo phòng = > theo khoa phongDangNhapId = 0 
            var listDanhSachTheoKhoaChuaTao = _yeuCauLinhDuocPhamService.GetData(vo.KhoLinhId, 0, vo.TuNgay, vo.DenNgay);
            var listDanhSachTheoKhoaDatao = _yeuCauLinhDuocPhamService.GetGridChoGoi(vo.YeuCauLinhDuocPhamId, vo.TuNgay, vo.DenNgay);
            var query = listDanhSachTheoKhoaChuaTao.Union(listDanhSachTheoKhoaDatao);
            // group những bệnh nhân cùng yêu cầu tiêp nhận id
            query = query.GroupBy(d => d.YeuCauTiepNhanId).Select(s => new ThongTinLinhTuKhoGridVo
            {
                Id = s.First().Id,
                TenDuocPham = s.First().TenDuocPham,
                NongDoVaHamLuong = s.First().NongDoVaHamLuong,
                HoatChat = s.First().HoatChat,
                DuongDung = s.First().DuongDung,
                DonViTinh = s.First().DonViTinh,
                HangSX = s.First().HangSX,
                NuocSanXuat = s.First().NuocSanXuat,
                SLYeuCau = s.Sum(x => x.SLYeuCau),
                LoaiThuoc = s.First().LoaiThuoc,
                DuocPhamId = s.First().DuocPhamId,
                KhoLinhId = s.First().KhoLinhId,
                MaTN = s.First().MaTN,
                MaBN = s.First().MaBN,
                HoTen = s.First().HoTen,
                YeuCauTiepNhanId = s.First().YeuCauTiepNhanId,
                LoaiDuocPham = s.First().LoaiDuocPham,
                SoLuongTon = s.First().SoLuongTon,
                NgayYeuCau = s.First().NgayYeuCau,
                NgayDieuTri = s.First().NgayDieuTri,
                IsCheckRowItem = s.Where(d => d.IsCheckRowItem == true).Select(d => d.IsCheckRowItem).Any() ? true : false,
                YeuCauLinhDuocPhamId = s.First().YeuCauLinhDuocPhamId,
                ListYeuCauDuocPhamBenhViens = s.SelectMany(d =>
                d.ListYeuCauDuocPhamBenhViens.Select(p => new ThongTinLanKhamKho
                {
                    Id = p.Id,
                    MaTN = p.MaTN,
                    MaBN = p.MaBN,
                    HoTen = p.HoTen,
                    DVKham = p.DVKham,
                    BacSyKeToa = p.BacSyKeToa,
                    SLKe = p.SLKe,
                    NgayYeuCau = p.NgayYeuCau,
                    NgayKe = p.NgayKe,
                    NgayDieuTri = p.NgayDieuTri,
                    DuocDuyet = p.DuocDuyet,
                    DuocPhamId = p.DuocPhamId,
                    TenDuocPham = p.TenDuocPham,
                    NongDoVaHamLuong = p.NongDoVaHamLuong,
                    HoatChat = p.HoatChat,
                    DuongDung = p.DuongDung,
                    DonViTinh = p.DonViTinh,
                    HangSX = p.HangSX,
                    NuocSanXuat = p.NuocSanXuat,
                    SLYeuCau = p.SLYeuCau,
                    LoaiThuoc = p.LoaiThuoc,
                    SoLuongTon = p.SoLuongTon,
                    IsCheckRowItem = p.IsCheckRowItem,
                    YeuCauTiepNhanId = p.YeuCauTiepNhanId
                }).OrderBy(g => g.MaTN).ThenBy(g => g.LaDuocPhamBHYT).ThenBy(g => g.TenDuocPham).ToList()).ToList()
            }).OrderBy(d => d.MaTN).ThenBy(d => d.LaDuocPhamBHYT).ThenBy(d => d.TenDuocPham);
            return Ok(query);
        }
        #endregion
    }
}
