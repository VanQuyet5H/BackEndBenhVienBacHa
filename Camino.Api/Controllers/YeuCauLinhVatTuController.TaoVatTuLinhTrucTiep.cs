using Camino.Api.Auth;
using Camino.Api.Extensions;
using Camino.Api.Models.LinhVatTuTrucTiep;
using Camino.Core.Domain;
using Camino.Core.Domain.Entities.YeuCauLinhVatTus;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Domain.ValueObject.LinhVatTu;
using Camino.Core.Domain.ValueObject.YeuCauLinhVatTu;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static Camino.Core.Domain.Enums;

namespace Camino.Api.Controllers
{
    public partial class YeuCauLinhVatTuController
    {
        [HttpPost("GetData")]
        public async Task<ActionResult<GridDataSource>> GetData([FromBody] LinhVatTuTrucTiepQueryInfo linhVatTuTrucTiepQueryInfo)
        {
            //long idKhoLinh, long idYeuCauLinhVatTu, long phongDangNhapId, long trangThai
            var lookup = _yeuCauLinhVatTuService.GetData(linhVatTuTrucTiepQueryInfo.idKhoLinh, linhVatTuTrucTiepQueryInfo.phongDangNhapId, linhVatTuTrucTiepQueryInfo.dateSearchStart, linhVatTuTrucTiepQueryInfo.dateSearchEnd);
            return Ok(lookup);
        }
        [HttpPost("GetDataDaTao")]
        public async Task<ActionResult<GridDataSource>> GetDataDaTao(long idKhoLinh, long idYeuCauLinhVatTu, long phongDangNhapId, long trangThai)
        {
            var lookup = _yeuCauLinhVatTuService.GetDataDaTao(idKhoLinh, idYeuCauLinhVatTu, phongDangNhapId,trangThai);
            return Ok(lookup);
        }
        [HttpPost("ThongTinLinhTuKho")]
        public async Task<ActionResult<GridDataSource>> ThongTinLinhTuKho(long idKhoLinh)
        {
            var lookup = _yeuCauLinhVatTuService.GetDataThongTin(idKhoLinh);
            return Ok(lookup);
        }
        [HttpPost("ThongTinDanhSachCanLinh")]
        public async Task<ActionResult<GridDataSource>> ThongTinDanhSachCanLinh(long idKhoLinh, long phongLinhVeId)
        {
            var lookup = _yeuCauLinhVatTuService.ThongTinDanhSachCanLinh(idKhoLinh, phongLinhVeId);
            return Ok(lookup);
        }
        [HttpPost("ThongTinLinhTuKhoDaTao")]
        public async Task<ActionResult<GridDataSource>> ThongTinLinhTuKhoDaTao(long idYeuCauLinhDP)
        {
            var lookup = _yeuCauLinhVatTuService.GetDataThongTinDaTao(idYeuCauLinhDP);
            return Ok(lookup);
        }
        [HttpPost("GetTrangThaiDuyet")]
        public bool? GetTrangThaiDuyet(long IdYeuCauLinh)
        {
            var lookup = _yeuCauLinhVatTuService.GetTrangThaiDuyet(IdYeuCauLinh);
            return lookup;
        }
        [HttpPost("GetDaDuyet")]
        public DaDuyetVatTu GetDaDuyet(long IdYeuCauLinh)
        {
            var lookup = _yeuCauLinhVatTuService.GetDaDuyet(IdYeuCauLinh);
            return lookup;
        }
        // create
        [HttpPost("GetAllYeuCauLinhThuocTuKho")]
        public async Task<ActionResult<GridDataSource>> GetDataForGridChildAsync([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _yeuCauLinhVatTuService.GetDataForGridChildAsync(queryInfo);
            return Ok(gridData);
        }
        // đã tạo
        [HttpPost("GetAllYeuCauLinhThuocTuKhoDaTao")]
        public async Task<ActionResult<GridDataSource>> GetAllYeuCauLinhThuocTuKhoDaTao([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _yeuCauLinhVatTuService.GetAllYeuCauLinhVatTuTuKhoDaTao(queryInfo);
            return Ok(gridData);
        }
        [HttpPost("GuiPhieuLinhTrucTiep")]
        [ClaimRequirement(Enums.SecurityOperation.Add, Enums.DocumentType.TaoYeuCauLinhTrucTiepVatTu)]
        public async Task<ActionResult> GuiPhieuLinhTrucTiep(LinhVatTuTrucTiepViewModel linhTrucTiepVatTuVM)
        {
            List<LinhTrucTiepVatTuChiTietGridVo> listvatTuDuocTao = new List<LinhTrucTiepVatTuChiTietGridVo>();
            var listYeuCauVatTuBenhVienDuocTao = new List<long>();
            if (linhTrucTiepVatTuVM.YeuCauVatTuBenhVienIds.Any())
            {
                foreach (var idYCVatTuBenhVien in linhTrucTiepVatTuVM.YeuCauVatTuBenhVienIds.Distinct())
                {
                    var linhTrucTiepVatTuChiTiet = _yeuCauLinhVatTuService.GetDataForGridChiTietChildCreateAsync(idYCVatTuBenhVien);
                    listvatTuDuocTao.AddRange(linhTrucTiepVatTuChiTiet);
                   
                }
                if(listvatTuDuocTao.Any())
                {
                    var groupvatTu = listvatTuDuocTao.GroupBy(s => new { s.VatTuBenhVienId, s.LaVatTuBHYT })
                                                        .Select(d => new LinhTrucTiepVatTuChiTietGridVo() {
                                                            YeuCauVatTuBenhVienIds = d.Select(f=>f.Id).ToList(),
                                                            LaVatTuBHYT = d.First().LaVatTuBHYT,
                                                            VatTuBenhVienId = d.First().VatTuBenhVienId,
                                                            SoLuong = d.Sum(g=>g.SoLuong),
                                                            SLTon = d.First().SLTon
                                                        }).Where(s => s.SLTon >= s.SoLuong).ToList();
                    foreach(var item in groupvatTu)
                    {
                        listYeuCauVatTuBenhVienDuocTao.AddRange(item.YeuCauVatTuBenhVienIds);
                    }
                }
                if(listYeuCauVatTuBenhVienDuocTao.Any())
                {
                    foreach(var id in listYeuCauVatTuBenhVienDuocTao)
                    {
                        var linhTrucTiepVatTuChiTiet = _yeuCauLinhVatTuService.GetDataForGridChiTietChildCreateAsync(id);
                        foreach (var linhttt in linhTrucTiepVatTuChiTiet)
                        {
                            var addnew = new LinhTrucTiepVatTuChiTietViewModel();
                            addnew.LaVatTuBHYT = linhttt.LaVatTuBHYT;
                            addnew.SoLuong = linhttt.SoLuong;
                            addnew.VatTuBenhVienId = linhttt.VatTuBenhVienId;
                            addnew.YeuCauVatTuBenhVienId = linhttt.Id;
                            linhTrucTiepVatTuVM.YeuCauLinhVatTuChiTiets.Add(addnew);
                        }
                    }
                }
                if (!linhTrucTiepVatTuVM.YeuCauLinhVatTuChiTiets.Any())
                {
                    throw new Models.Error.ApiException(_localizationService.GetResource("LinhThuongDuocPham.LinhDuocPhamChiTiets.Required"));
                }
            }
         
            if (!linhTrucTiepVatTuVM.YeuCauVatTuBenhVienIds.Any())
            {
                throw new Models.Error.ApiException(_localizationService.GetResource("LinhTrucTiepDuocPham.YeuCauDuocPhamBenhVien.Required"));
            }

            // kho xuat == kho linh && kho nhập == lấy 1 phòng bất kỳ trong cái khoa đó
            // nếu trường hợp không có khoa thì lấy theo phòng nhân viên quản lý
            if (linhTrucTiepVatTuVM.KhoNhapId != null)
            {
                var linhNhapId = _yeuCauLinhVatTuService.GetIdKhoNhap(linhTrucTiepVatTuVM.KhoNhapId);
                if (linhNhapId != 0)
                {
                    linhTrucTiepVatTuVM.KhoNhapId = linhNhapId;
                }
                else
                {
                    var KhoLinhNhapTheoNhanVien = _yeuCauLinhDuocPhamService.GetAllIdKhoNhapNhanVien(linhTrucTiepVatTuVM.NhanVienYeuCauId);
                    if (KhoLinhNhapTheoNhanVien != 0)
                    {
                        linhTrucTiepVatTuVM.KhoNhapId = KhoLinhNhapTheoNhanVien;
                    }
                    else
                    {
                        throw new ArgumentException(_localizationService
                                                    .GetResource("Common.NotCreatePhieuLinhTrucTiep"));
                    }
                }
            }
            linhTrucTiepVatTuVM.NgayYeuCau = DateTime.Now;
            linhTrucTiepVatTuVM.LoaiPhieuLinh = EnumLoaiPhieuLinh.LinhChoBenhNhan;
            var linhtt = linhTrucTiepVatTuVM.ToEntity<YeuCauLinhVatTu>();
            linhtt.NoiYeuCauId = _userAgentHelper.GetCurrentNoiLLamViecId();

            linhtt.ThoiDiemLinhTongHopTuNgay = linhTrucTiepVatTuVM.ThoiDiemLinhTongHopTuNgay;

            if (linhTrucTiepVatTuVM.ThoiDiemLinhTongHopDenNgay == null)
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
                linhtt.ThoiDiemLinhTongHopDenNgay = linhTrucTiepVatTuVM.ThoiDiemLinhTongHopDenNgay;
            }

            if (linhTrucTiepVatTuVM.Goi == true)
            {
                linhtt.DaGui = true;
            }
            else
            {
                linhtt.DaGui = false;
            }
            linhtt.GhiChu = linhTrucTiepVatTuVM.GhiChu;
            List<long> lst = listYeuCauVatTuBenhVienDuocTao.ToList();
            await _yeuCauLinhVatTuService.XuLyThemYeuCauLinhVatTuTTAsync(linhtt, lst);
            await _yeuCauLinhVatTuService.AddAsync(linhtt);
            return Ok(linhtt.Id);
        }
        [HttpGet("GetAll")]
        //[ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DanhMucChucDanh)]
        public async Task<ActionResult<LinhVatTuTrucTiepViewModel>> GetALL(long id)
        {
            var result = await _yeuCauLinhVatTuService.GetByIdAsync(id, s => s.Include(k => k.YeuCauVatTuBenhViens).Include(f => f.KhoXuat).Include(h=>h.YeuCauLinhVatTuChiTiets));
            if (result == null)
            {
                return NotFound();
            }
            var resultData = result.ToModel<LinhVatTuTrucTiepViewModel>();
            return Ok(resultData);
        }
        //  gui lai phieu linh truc tiep -- va save
        [HttpPost("GuiLaiPhieuLinhTrucTiep")]
        //[ClaimRequirement(Enums.SecurityOperation.Add, Enums.DocumentType.LinhBuDuocPham)]
        public async Task<ActionResult> GuiLaiPhieuLinhTrucTiep(LinhVatTuTrucTiepViewModel linhTrucTiepVatTuVM)
        {
            var listIdCuCanXoa = new List<long>();
            List<LinhTrucTiepVatTuChiTietGridVo> listvatTuDuocTao = new List<LinhTrucTiepVatTuChiTietGridVo>();
            var listYeuCauVatTuBenhVienDuocTao = new List<long>();
            if (linhTrucTiepVatTuVM.YeuCauLinhVatTuId != null)
            {

                // gét những yêu cầu vật tư đã tạo theo YeuCauLinhVatTuId field DaGui = false
                // tạo mới yêu cầu lĩnh dược phẩm chi tiết theo yêu cầu dược phẩm cần update 
                // update DaGui = true
                //1.------------------------------------------//

                if (linhTrucTiepVatTuVM.YeuCauVatTuBenhVienIds.Any())
                {
                    foreach (var idYCVatTuBenhVien in linhTrucTiepVatTuVM.YeuCauVatTuBenhVienIds.Distinct())
                    {
                        var linhTrucTiepVatTuChiTiet = _yeuCauLinhVatTuService.GetDataForGridChiTietChildCreateAsync(idYCVatTuBenhVien);
                        listvatTuDuocTao.AddRange(linhTrucTiepVatTuChiTiet);

                    }
                    if (listvatTuDuocTao.Any())
                    {
                        var groupvatTu = listvatTuDuocTao.GroupBy(s => new { s.VatTuBenhVienId, s.LaVatTuBHYT })
                                                            .Select(d => new LinhTrucTiepVatTuChiTietGridVo()
                                                            {
                                                                YeuCauVatTuBenhVienIds = d.Select(f => f.Id).ToList(),
                                                                LaVatTuBHYT = d.First().LaVatTuBHYT,
                                                                VatTuBenhVienId = d.First().VatTuBenhVienId,
                                                                SoLuong = d.Sum(g => g.SoLuong),
                                                                SLTon = d.First().SLTon
                                                            }).Where(s => s.SLTon >= s.SoLuong).ToList();
                        foreach (var item in groupvatTu)
                        {
                            listYeuCauVatTuBenhVienDuocTao.AddRange(item.YeuCauVatTuBenhVienIds);
                        }
                    }
                    if (listYeuCauVatTuBenhVienDuocTao.Any())
                    {
                        foreach (var id in listYeuCauVatTuBenhVienDuocTao)
                        {
                            var linhTrucTiepVatTuChiTiet = _yeuCauLinhVatTuService.GetDataForGridChiTietChildCreateAsync(id);
                            foreach (var linhttt in linhTrucTiepVatTuChiTiet)
                            {
                                var addnew = new LinhTrucTiepVatTuChiTietViewModel();
                                addnew.LaVatTuBHYT = linhttt.LaVatTuBHYT;
                                addnew.SoLuong = linhttt.SoLuong;
                                addnew.VatTuBenhVienId = linhttt.VatTuBenhVienId;
                                addnew.YeuCauVatTuBenhVienId = linhttt.Id;
                                linhTrucTiepVatTuVM.YeuCauLinhVatTuChiTiets.Add(addnew);
                            }
                        }
                    }
                    //if (!linhTrucTiepVatTuVM.YeuCauLinhVatTuChiTiets.Any())
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
                listYeuCauDuocPhamBenhVienChoGoi = _yeuCauLinhDuocPhamService.GetYeuCauDuocPhamIdDaTao((long)linhTrucTiepVatTuVM.YeuCauLinhVatTuId);
                // kiem tra những dịch vụ mới 
                if (linhTrucTiepVatTuVM.YeuCauVatTuBenhVienIds.Any())
                {
                    foreach (var item in listYeuCauDuocPhamBenhVienChoGoi)
                    {
                        if (!linhTrucTiepVatTuVM.YeuCauVatTuBenhVienIds.Any(d => d == item))
                        {
                            listIdCuCanXoa.Add(item);
                        }

                    }
                }

                if (listIdCuCanXoa.Any())
                {
                    _yeuCauLinhDuocPhamService.XuLyHuyYeuCauDuocPhamTTAsync(listIdCuCanXoa);
                }
                if (linhTrucTiepVatTuVM.KhoNhapId != null)
                {
                    var linhNhapId = _yeuCauLinhDuocPhamService.GetIdKhoNhap(linhTrucTiepVatTuVM.KhoNhapId);
                    if (linhNhapId != 0)
                    {
                        linhTrucTiepVatTuVM.KhoNhapId = linhNhapId;
                    }
                    else
                    {
                        var KhoLinhNhapTheoNhanVien = _yeuCauLinhDuocPhamService.GetAllIdKhoNhapNhanVien(linhTrucTiepVatTuVM.NhanVienYeuCauId);
                        if (KhoLinhNhapTheoNhanVien != 0)
                        {
                            linhTrucTiepVatTuVM.KhoNhapId = KhoLinhNhapTheoNhanVien;
                        }
                        else
                        {
                            throw new ArgumentException(_localizationService
                                                        .GetResource("Common.NotCreatePhieuLinhTrucTiep"));
                        }
                    }
                }
                var yeuCauLinh = await _yeuCauLinhVatTuService.GetByIdAsync((long)linhTrucTiepVatTuVM.YeuCauLinhVatTuId,
              x => x.Include(d => d.YeuCauLinhVatTuChiTiets)
                  .Include(d => d.YeuCauVatTuBenhViens)
                  .Include(a => a.NhanVienYeuCau).ThenInclude(b => b.User)
                      .Include(a => a.NhanVienDuyet).ThenInclude(b => b.User)
                      .Include(a => a.KhoNhap)
                      .Include(a => a.KhoXuat).ThenInclude(b => b.NhapKhoDuocPhams).ThenInclude(c => c.NhapKhoDuocPhamChiTiets)
                      .Include(a => a.XuatKhoVatTus).ThenInclude(b => b.NguoiXuat).ThenInclude(c => c.User)
                      .Include(a => a.XuatKhoVatTus).ThenInclude(b => b.NguoiNhan).ThenInclude(c => c.User));

                linhTrucTiepVatTuVM.NgayYeuCau = DateTime.Now;
                linhTrucTiepVatTuVM.LoaiPhieuLinh = EnumLoaiPhieuLinh.LinhChoBenhNhan;
                var linhtt = linhTrucTiepVatTuVM.ToEntity<YeuCauLinhVatTu>();
                linhtt.NoiYeuCauId = _userAgentHelper.GetCurrentNoiLLamViecId();

                linhtt.ThoiDiemLinhTongHopTuNgay = linhTrucTiepVatTuVM.ThoiDiemLinhTongHopTuNgay;

                if (linhTrucTiepVatTuVM.ThoiDiemLinhTongHopDenNgay == null)
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
                    linhtt.ThoiDiemLinhTongHopDenNgay = linhTrucTiepVatTuVM.ThoiDiemLinhTongHopDenNgay;
                }

                List<long> lst = listYeuCauVatTuBenhVienDuocTao.ToList();
                await _yeuCauLinhVatTuService.XuLyThemYeuCauLinhVatTuTTAsync(linhtt, lst);
                foreach (var item in linhtt.YeuCauVatTuBenhViens)
                {
                    item.YeuCauLinhVatTuId = linhTrucTiepVatTuVM.YeuCauLinhVatTuId;
                    yeuCauLinh.YeuCauVatTuBenhViens.Add(item);
                }
                foreach (var item in linhtt.YeuCauLinhVatTuChiTiets)
                {
                    item.YeuCauLinhVatTuId = (long)linhTrucTiepVatTuVM.YeuCauLinhVatTuId;
                    yeuCauLinh.YeuCauLinhVatTuChiTiets.Add(item);
                }

                if (linhTrucTiepVatTuVM.SaveOrUpDate == true)
                {
                    yeuCauLinh.DaGui = false;
                }
                else
                {
                    yeuCauLinh.DaGui = true;
                }

                yeuCauLinh.GhiChu = linhTrucTiepVatTuVM.GhiChu;
                yeuCauLinh.NgayYeuCau = DateTime.Now;
               await _yeuCauLinhVatTuService.UpdateAsync(yeuCauLinh);
            }
            return Ok(linhTrucTiepVatTuVM.YeuCauLinhVatTuId);
        }
        #region In lĩnh duoc pham TT đã  tạo
        [HttpPost("InPhieuLinhTrucTiepVatTu")]
        public async Task<ActionResult<string>> InPhieuLinhTrucTiepDuocPham([FromBody]XacNhanInLinhVatTu xacNhanInLinhDuocPham)
        {
            var htmlLinhDuocPham = await _yeuCauLinhVatTuService.InPhieuLinhTrucTiepVatTu(xacNhanInLinhDuocPham);
            return htmlLinhDuocPham;
        }
        #endregion
        [HttpPost("GuiLaiPhieuLinhTrucTiepVaDuyet")]
        //[ClaimRequirement(Enums.SecurityOperation.Add, Enums.DocumentType.LinhBuDuocPham)]
        public async Task<ActionResult> GuiLaiPhieuLinhTrucTiepVaDuyet(LinhVatTuTrucTiepViewModel linhTrucTiepDuocPhamVM)
        {
            var lstYCDPBVCu = await _yeuCauLinhVatTuService.GetByIdAsync(linhTrucTiepDuocPhamVM.Id, s => s.Include(x => x.YeuCauVatTuBenhViens));
            foreach (var itemx in lstYCDPBVCu.YeuCauVatTuBenhViens)
            {
                var entity = _yeuCauVatTuBenhVienService.GetById(itemx.Id);
                entity.YeuCauLinhVatTuId = null;
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
            List<long> lst = linhTrucTiepDuocPhamVM.YeuCauVatTuBenhViensTT.Select(s => s.Id).ToList();
            await _yeuCauLinhVatTuService.XuLyThemYeuCauLinhVatTuTTAsync(lstYCDPBVCu, lst);
            await _yeuCauLinhVatTuService.UpdateAsync(lstYCDPBVCu);
            return Ok(lstYCDPBVCu.Id);
        }

        #region In lĩnh duoc pham xem trước
        [HttpPost("InXemTruocPhieuLinhTrucTiepVatTu")]
        public async Task<ActionResult<string>> InXemTruocPhieuLinhTrucTiepVatTu(XacNhanInLinhVatTuXemTruoc xacNhanInLinhVatTuXemTruoc)
        {
            var htmlLinhDuocPham = await _yeuCauLinhVatTuService.InXemtruocPhieuLinhTrucTiepVatTu(xacNhanInLinhVatTuXemTruoc);
            return htmlLinhDuocPham;
        }
        #endregion
        #region  get danh sach cho goi linh tt 30072021
        [HttpPost("GetDataGridYeuCauLinhVatTuLuuTamThoi")]
        public async Task<ActionResult<GridDataSource>> GetData(VoQueryDSVTChoGoi vo)
        {
            // gio không lấy theo phòng = > theo khoa phongDangNhapId = 0 
            var listDanhSachTheoKhoaChuaTao = _yeuCauLinhVatTuService.GetData(vo.KhoLinhId, 0, vo.TuNgay, vo.DenNgay);
            var listDanhSachTheoKhoaDatao = _yeuCauLinhVatTuService.GetGridChoGoi(vo.YeuCauLinhVatTuId, vo.TuNgay, vo.DenNgay);
            var query = listDanhSachTheoKhoaChuaTao.Union(listDanhSachTheoKhoaDatao);
            // group những bệnh nhân cùng yêu cầu tiêp nhận id
            query = query.GroupBy(d => d.YeuCauTiepNhanId).Select(s => new ThongTinLinhVatTuTuKhoGridVo
            {
                Id = s.First().Id,
                TenVatTu = s.First().TenVatTu,
                NongDoVaHamLuong = s.First().NongDoVaHamLuong,
                HoatChat = s.First().HoatChat,
                DuongDung = s.First().DuongDung,
                DonViTinh = s.First().DonViTinh,
                HangSX = s.First().HangSX,
                NuocSanXuat = s.First().NuocSanXuat,
                SLYeuCau = s.Sum(x => x.SLYeuCau),
                LoaiThuoc = s.First().LoaiThuoc,
                VatTuId = s.First().VatTuId,
                KhoLinhId = s.First().KhoLinhId,
                MaTN = s.First().MaTN,
                MaBN = s.First().MaBN,
                HoTen = s.First().HoTen,
                YeuCauTiepNhanId = s.First().YeuCauTiepNhanId,
                LoaiVatTu = s.First().LoaiVatTu,
                SoLuongTon = s.First().SoLuongTon,
                NgayYeuCau = s.First().NgayYeuCau,
                NgayDieuTri = s.First().NgayDieuTri,
                IsCheckRowItem = s.Where(d => d.IsCheckRowItem == true).Select(d => d.IsCheckRowItem).Any() ? true : false,
                YeuCauLinhVatTuId = s.First().YeuCauLinhVatTuId,
                ListYeuCauVatTuBenhViens = s.SelectMany(d => d.ListYeuCauVatTuBenhViens.Select(p => new ThongTinLanKhamKho
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
                    VatTuId = p.VatTuId,
                    TenVatTu = p.TenVatTu,
                    HoatChat = p.HoatChat,
                    DonViTinh = p.DonViTinh,
                    HangSX = p.HangSX,
                    NuocSanXuat = p.NuocSanXuat,
                    SLYeuCau = p.SLYeuCau,
                    LoaiThuoc = p.LoaiThuoc,
                    SoLuongTon = p.SoLuongTon,
                    IsCheckRowItem = p.IsCheckRowItem,
                    YeuCauTiepNhanId = p.YeuCauTiepNhanId
                }).ToList()).ToList()
            });
            return Ok(query);
        }
        #endregion
    }
}
