using Camino.Api.Auth;
using Camino.Api.Models.Error;
using Camino.Api.Models.PhauThuatThuThuat;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Domain.ValueObject.KhamBenhs;
using Camino.Core.Domain.ValueObject.PhauThuatThuThuat;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static Camino.Core.Domain.Enums;

namespace Camino.Api.Controllers
{
    public partial class PhauThuatThuThuatController
    {
        [HttpGet("GetGridDataGhiNhanVTTHThuoc")]
        public async Task<List<GhiNhanVatTuTieuHaoThuocGroupParentGridVo>> GetGridDataGhiNhanVTTHThuocAsync(long yeuCauDichVuKyThuatId)
        {
            //var gridData = await _phauThuatThuThuatService.GetGridDataGhiNhanVTTHThuocAsync(yeuCauDichVuKyThuatId);
            var gridData = await _phauThuatThuThuatService.GetGridDataGhiNhanVTTHThuocAsyncVer2(yeuCauDichVuKyThuatId);
            return gridData;
        }

        [HttpPost("ThemGhiNhanVatTuBenhVien")]
        [ClaimRequirement(SecurityOperation.Add, DocumentType.PhauThuatThuThuatTheoNgay)]
        public async Task<ActionResult<ChiDinhDichVuResultVo>> ThemGhiNhanVatTuBenhVien([FromBody] ChiDinhGhiNhanVatTuThuocTieuHaoPTTTViewModel yeuCauViewModel)
        {
            //var ycdvkt = _yeuCauDichVuKyThuatService.GetById(yeuCauViewModel.DichVuChiDinhId);
            //yeuCauViewModel.YeuCauTiepNhanId = ycdvkt.YeuCauTiepNhanId;
            
            yeuCauViewModel.YeuCauTiepNhanId = await _phauThuatThuThuatService.GetYeuCauTiepNhanIdTheoYeuCauKyThuatIdAsync(yeuCauViewModel.DichVuChiDinhId);

            var yeuCauVo = new ChiDinhGhiNhanVatTuThuocPTTTVo
            {
                YeuCauTiepNhanId = yeuCauViewModel.YeuCauTiepNhanId,
                DichVuChiDinhId = yeuCauViewModel.DichVuChiDinhId,
                DichVuGhiNhanId = yeuCauViewModel.DichVuGhiNhanId,
                KhoId = yeuCauViewModel.KhoId,
                SoLuong = yeuCauViewModel.SoLuong,
                TinhPhi = yeuCauViewModel.TinhPhi,
                LaDuocPhamBHYT = yeuCauViewModel.LaDuocPhamBHYT,
                GiaiDoanPhauThuat = yeuCauViewModel.GiaiDoanPhauThuat,
                LoaiNoiChiDinh = yeuCauViewModel.LoaiNoiChiDinh,
            };

            var yeuCauTiepNhanChiTiet = _phauThuatThuThuatService.GetYeuCauTiepNhanForGhiNhanVatTuThuoc(yeuCauViewModel.YeuCauTiepNhanId);
            //var yeuCauTiepNhanChiTiet = await _khamBenhService.GetYeuCauTiepNhanByIdAsync(yeuCauViewModel.YeuCauTiepNhanId);

            await _phauThuatThuThuatService.XuLyThemGhiNhanVatTuBenhVienAsync(yeuCauVo, yeuCauTiepNhanChiTiet);

            // gọi hàm chung
            await _yeuCauTiepNhanService.PrepareForAddDichVuAndUpdateAsync(yeuCauTiepNhanChiTiet);

            // cập nhật lại số lượng tồn
            if (yeuCauVo.NhapKhoDuocPhamChiTiets.Any() || yeuCauVo.NhapKhoVatTuChiTiets.Any())
            {
                await _khamBenhService.CapNhatSoLuongTonKhiGhiNhanVTTHThuocAsync(yeuCauVo.NhapKhoDuocPhamChiTiets, yeuCauVo.NhapKhoVatTuChiTiets);
            }
            //update 10/08/2022: tách get SoDuTaiKhoan khi thêm thuốc/VT
            //var chiDinhDichVuResultVo = new ChiDinhDichVuResultVo();

            //decimal soDuTk = 0;
            //decimal soDuUocLuongConLai = 0;

            //if (yeuCauTiepNhanChiTiet.NoiTruBenhAn != null || yeuCauTiepNhanChiTiet.QuyetToanTheoNoiTru == true)
            //{
            //    var chiPhiKhamChuaBenh = _thuNganNoiTruService.GetDanhSachChiPhiKhamChuaBenhChuaThu(yeuCauTiepNhanChiTiet.Id).Result.Select(o => o.BNConPhaiThanhToan).DefaultIfEmpty(0).Sum();

            //    soDuTk = await _taiKhoanBenhNhanService.GetSoTienDaTamUngAsync(yeuCauTiepNhanChiTiet.Id);
            //    soDuUocLuongConLai = soDuTk - chiPhiKhamChuaBenh;
            //}
            //else
            //{
            //    soDuTk = await _taiKhoanBenhNhanService.GetSoTienDaTamUngAsync(yeuCauTiepNhanChiTiet.Id);
            //    soDuUocLuongConLai = await _taiKhoanBenhNhanService.GetSoTienUocLuongConLai(yeuCauTiepNhanChiTiet.Id);
            //}

            //chiDinhDichVuResultVo.SoDuTaiKhoan = soDuTk;
            //chiDinhDichVuResultVo.SoDuTaiKhoanConLai = soDuUocLuongConLai;

            return Ok();
        }

        [HttpPost("XuLyXoaYeuCauGhiNhanVTTHThuoc")]
        [ClaimRequirement(SecurityOperation.Delete, DocumentType.PhauThuatThuThuatTheoNgay)]
        public async Task<ActionResult<ChiDinhDichVuResultVo>> XuLyXoaYeuCauGhiNhanVTTHThuocAsync(GridItemGhiNhanVatTuThuocPTTTViewModel xoaYeuCauViewModel)
        {
            var yeuCauTiepNhanChiTiet = _phauThuatThuThuatService.GetYeuCauTiepNhanForGhiNhanVatTuThuoc(xoaYeuCauViewModel.YeuCauTiepNhanId);
            //var yeuCauTiepNhanChiTiet2 = await _khamBenhService.GetYeuCauTiepNhanByIdAsync(xoaYeuCauViewModel.YeuCauTiepNhanId);

            // xử lý xóa yeu cau duoc pham/ vat tu
            _phauThuatThuThuatService.XuLyXoaYeuCauGhiNhanVTTHThuocAsync(yeuCauTiepNhanChiTiet, xoaYeuCauViewModel.YeuCauGhiNhanVTTHThuocId);

            await _tiepNhanBenhNhanService.PrepareForDeleteDichVuAndUpdateAsync(yeuCauTiepNhanChiTiet);
            //await _tiepNhanBenhNhanServiceService.UpdateAsync(yeuCauTiepNhanChiTiet);

            // get lại thông tin số dư tài khoản người bệnh
            var chiDinhDichVuResultVo = new ChiDinhDichVuResultVo();
            //chiDinhDichVuResultVo.SoDuTaiKhoan = await _taiKhoanBenhNhanService.GetSoTienDaTamUngAsync(yeuCauTiepNhanChiTiet.Id);
            //chiDinhDichVuResultVo.SoDuTaiKhoanConLai = await _taiKhoanBenhNhanService.GetSoTienUocLuongConLai(yeuCauTiepNhanChiTiet.Id);

            decimal soDuTk = 0;
            decimal soDuUocLuongConLai = 0;

            if (yeuCauTiepNhanChiTiet.NoiTruBenhAn != null || yeuCauTiepNhanChiTiet.QuyetToanTheoNoiTru == true)
            {
                //var chiPhiKhamChuaBenh = _thuNganNoiTruService.GetDanhSachChiPhiKhamChuaBenhChuaThu(yeuCauTiepNhanChiTiet.Id).Result.Select(o => o.BNConPhaiThanhToan).DefaultIfEmpty(0).Sum();
                var chiPhiKhamChuaBenh = _thuNganNoiTruService.GetSoTienBNConPhaiThanhToan(yeuCauTiepNhanChiTiet.Id).Result;

                soDuTk = await _taiKhoanBenhNhanService.GetSoTienDaTamUngAsync(yeuCauTiepNhanChiTiet.Id);
                soDuUocLuongConLai = soDuTk - chiPhiKhamChuaBenh;
            }
            else
            {
                soDuTk = await _taiKhoanBenhNhanService.GetSoTienDaTamUngAsync(yeuCauTiepNhanChiTiet.Id);
                soDuUocLuongConLai = await _taiKhoanBenhNhanService.GetSoTienUocLuongConLai(yeuCauTiepNhanChiTiet.Id);
            }

            chiDinhDichVuResultVo.SoDuTaiKhoan = soDuTk;
            chiDinhDichVuResultVo.SoDuTaiKhoanConLai = soDuUocLuongConLai;

            return chiDinhDichVuResultVo;
        }

        /// <summary>
        /// Function này viết ra để xuất những yêu cầu người dùng quên nhấn xuất thuốc và VT nhưng đã chuyển giao Bn rồi
        /// </summary>
        private void XuLyXuatYeuCauGhiNhanVTTHThuocChoNhungYeuCauQuenNhanXuat()
        {
            var arr = new List<ChiDinhGhiNhanVatTuThuocPTTTVo>
            {
                new ChiDinhGhiNhanVatTuThuocPTTTVo(){YeuCauTiepNhanId = 2098,YeuCauDichVuKyThuatId = 7547},
                new ChiDinhGhiNhanVatTuThuocPTTTVo(){YeuCauTiepNhanId = 2093,YeuCauDichVuKyThuatId = 7582},
                new ChiDinhGhiNhanVatTuThuocPTTTVo(){YeuCauTiepNhanId = 2106,YeuCauDichVuKyThuatId = 7642},
                new ChiDinhGhiNhanVatTuThuocPTTTVo(){YeuCauTiepNhanId = 2105,YeuCauDichVuKyThuatId = 7727},
                new ChiDinhGhiNhanVatTuThuocPTTTVo(){YeuCauTiepNhanId = 2107,YeuCauDichVuKyThuatId = 7728},
                new ChiDinhGhiNhanVatTuThuocPTTTVo(){YeuCauTiepNhanId = 2107,YeuCauDichVuKyThuatId = 7733},
                new ChiDinhGhiNhanVatTuThuocPTTTVo(){YeuCauTiepNhanId = 2107,YeuCauDichVuKyThuatId = 7740},
                new ChiDinhGhiNhanVatTuThuocPTTTVo(){YeuCauTiepNhanId = 2107,YeuCauDichVuKyThuatId = 7762},
                new ChiDinhGhiNhanVatTuThuocPTTTVo(){YeuCauTiepNhanId = 2126,YeuCauDichVuKyThuatId = 7793},
                new ChiDinhGhiNhanVatTuThuocPTTTVo(){YeuCauTiepNhanId = 2151,YeuCauDichVuKyThuatId = 7839},
                new ChiDinhGhiNhanVatTuThuocPTTTVo(){YeuCauTiepNhanId = 2138,YeuCauDichVuKyThuatId = 7971},
                new ChiDinhGhiNhanVatTuThuocPTTTVo(){YeuCauTiepNhanId = 2165,YeuCauDichVuKyThuatId = 8028},
                new ChiDinhGhiNhanVatTuThuocPTTTVo(){YeuCauTiepNhanId = 2165,YeuCauDichVuKyThuatId = 8047},
                new ChiDinhGhiNhanVatTuThuocPTTTVo(){YeuCauTiepNhanId = 2211,YeuCauDichVuKyThuatId = 8199},
                new ChiDinhGhiNhanVatTuThuocPTTTVo(){YeuCauTiepNhanId = 2221,YeuCauDichVuKyThuatId = 8211},
                new ChiDinhGhiNhanVatTuThuocPTTTVo(){YeuCauTiepNhanId = 2468,YeuCauDichVuKyThuatId = 8507},
                new ChiDinhGhiNhanVatTuThuocPTTTVo(){YeuCauTiepNhanId = 2468,YeuCauDichVuKyThuatId = 8508},
                new ChiDinhGhiNhanVatTuThuocPTTTVo(){YeuCauTiepNhanId = 2482,YeuCauDichVuKyThuatId = 8509},
                new ChiDinhGhiNhanVatTuThuocPTTTVo(){YeuCauTiepNhanId = 2520,YeuCauDichVuKyThuatId = 8679},
                new ChiDinhGhiNhanVatTuThuocPTTTVo(){YeuCauTiepNhanId = 2522,YeuCauDichVuKyThuatId = 8737},
                new ChiDinhGhiNhanVatTuThuocPTTTVo(){YeuCauTiepNhanId = 2541,YeuCauDichVuKyThuatId = 8841},
                new ChiDinhGhiNhanVatTuThuocPTTTVo(){YeuCauTiepNhanId = 2513,YeuCauDichVuKyThuatId = 8842},
                new ChiDinhGhiNhanVatTuThuocPTTTVo(){YeuCauTiepNhanId = 2513,YeuCauDichVuKyThuatId = 8844},
                new ChiDinhGhiNhanVatTuThuocPTTTVo(){YeuCauTiepNhanId = 2575,YeuCauDichVuKyThuatId = 8863},
                new ChiDinhGhiNhanVatTuThuocPTTTVo(){YeuCauTiepNhanId = 2536,YeuCauDichVuKyThuatId = 8864},
                new ChiDinhGhiNhanVatTuThuocPTTTVo(){YeuCauTiepNhanId = 2535,YeuCauDichVuKyThuatId = 8937},
                new ChiDinhGhiNhanVatTuThuocPTTTVo(){YeuCauTiepNhanId = 2595,YeuCauDichVuKyThuatId = 8947},
                new ChiDinhGhiNhanVatTuThuocPTTTVo(){YeuCauTiepNhanId = 2591,YeuCauDichVuKyThuatId = 8973},
                new ChiDinhGhiNhanVatTuThuocPTTTVo(){YeuCauTiepNhanId = 2518,YeuCauDichVuKyThuatId = 8976},
                new ChiDinhGhiNhanVatTuThuocPTTTVo(){YeuCauTiepNhanId = 2602,YeuCauDichVuKyThuatId = 8979},
                new ChiDinhGhiNhanVatTuThuocPTTTVo(){YeuCauTiepNhanId = 2538,YeuCauDichVuKyThuatId = 8980},
                new ChiDinhGhiNhanVatTuThuocPTTTVo(){YeuCauTiepNhanId = 2542,YeuCauDichVuKyThuatId = 8983},
                new ChiDinhGhiNhanVatTuThuocPTTTVo(){YeuCauTiepNhanId = 2603,YeuCauDichVuKyThuatId = 8996},
                new ChiDinhGhiNhanVatTuThuocPTTTVo(){YeuCauTiepNhanId = 2603,YeuCauDichVuKyThuatId = 8998},
                new ChiDinhGhiNhanVatTuThuocPTTTVo(){YeuCauTiepNhanId = 2590,YeuCauDichVuKyThuatId = 9144},
                new ChiDinhGhiNhanVatTuThuocPTTTVo(){YeuCauTiepNhanId = 2556,YeuCauDichVuKyThuatId = 9171},
                new ChiDinhGhiNhanVatTuThuocPTTTVo(){YeuCauTiepNhanId = 2670,YeuCauDichVuKyThuatId = 9210},
                new ChiDinhGhiNhanVatTuThuocPTTTVo(){YeuCauTiepNhanId = 2675,YeuCauDichVuKyThuatId = 9266},
                new ChiDinhGhiNhanVatTuThuocPTTTVo(){YeuCauTiepNhanId = 2675,YeuCauDichVuKyThuatId = 9267},
                new ChiDinhGhiNhanVatTuThuocPTTTVo(){YeuCauTiepNhanId = 2675,YeuCauDichVuKyThuatId = 9268},
                new ChiDinhGhiNhanVatTuThuocPTTTVo(){YeuCauTiepNhanId = 2698,YeuCauDichVuKyThuatId = 9293},
                new ChiDinhGhiNhanVatTuThuocPTTTVo(){YeuCauTiepNhanId = 2541,YeuCauDichVuKyThuatId = 9312},
                new ChiDinhGhiNhanVatTuThuocPTTTVo(){YeuCauTiepNhanId = 2471,YeuCauDichVuKyThuatId = 9314},
                new ChiDinhGhiNhanVatTuThuocPTTTVo(){YeuCauTiepNhanId = 2729,YeuCauDichVuKyThuatId = 9409},
                new ChiDinhGhiNhanVatTuThuocPTTTVo(){YeuCauTiepNhanId = 2740,YeuCauDichVuKyThuatId = 9534},
                new ChiDinhGhiNhanVatTuThuocPTTTVo(){YeuCauTiepNhanId = 2671,YeuCauDichVuKyThuatId = 9643},
                new ChiDinhGhiNhanVatTuThuocPTTTVo(){YeuCauTiepNhanId = 2752,YeuCauDichVuKyThuatId = 9647},
                new ChiDinhGhiNhanVatTuThuocPTTTVo(){YeuCauTiepNhanId = 2751,YeuCauDichVuKyThuatId = 9736},
                new ChiDinhGhiNhanVatTuThuocPTTTVo(){YeuCauTiepNhanId = 2712,YeuCauDichVuKyThuatId = 9739},
                new ChiDinhGhiNhanVatTuThuocPTTTVo(){YeuCauTiepNhanId = 2518,YeuCauDichVuKyThuatId = 9742},
                new ChiDinhGhiNhanVatTuThuocPTTTVo(){YeuCauTiepNhanId = 2779,YeuCauDichVuKyThuatId = 9750},
                new ChiDinhGhiNhanVatTuThuocPTTTVo(){YeuCauTiepNhanId = 2747,YeuCauDichVuKyThuatId = 9788},
                new ChiDinhGhiNhanVatTuThuocPTTTVo(){YeuCauTiepNhanId = 2756,YeuCauDichVuKyThuatId = 9791},
                new ChiDinhGhiNhanVatTuThuocPTTTVo(){YeuCauTiepNhanId = 2744,YeuCauDichVuKyThuatId = 9792},
                new ChiDinhGhiNhanVatTuThuocPTTTVo(){YeuCauTiepNhanId = 2789,YeuCauDichVuKyThuatId = 9817},
                new ChiDinhGhiNhanVatTuThuocPTTTVo(){YeuCauTiepNhanId = 2790,YeuCauDichVuKyThuatId = 9822},
                new ChiDinhGhiNhanVatTuThuocPTTTVo(){YeuCauTiepNhanId = 2709,YeuCauDichVuKyThuatId = 9829},
                new ChiDinhGhiNhanVatTuThuocPTTTVo(){YeuCauTiepNhanId = 2709,YeuCauDichVuKyThuatId = 9830},
                new ChiDinhGhiNhanVatTuThuocPTTTVo(){YeuCauTiepNhanId = 2793,YeuCauDichVuKyThuatId = 9832},
                new ChiDinhGhiNhanVatTuThuocPTTTVo(){YeuCauTiepNhanId = 2572,YeuCauDichVuKyThuatId = 9844},
                new ChiDinhGhiNhanVatTuThuocPTTTVo(){YeuCauTiepNhanId = 2804,YeuCauDichVuKyThuatId = 9861},
                new ChiDinhGhiNhanVatTuThuocPTTTVo(){YeuCauTiepNhanId = 2802,YeuCauDichVuKyThuatId = 10023},
                new ChiDinhGhiNhanVatTuThuocPTTTVo(){YeuCauTiepNhanId = 2806,YeuCauDichVuKyThuatId = 10026},
                new ChiDinhGhiNhanVatTuThuocPTTTVo(){YeuCauTiepNhanId = 2806,YeuCauDichVuKyThuatId = 10028},
                new ChiDinhGhiNhanVatTuThuocPTTTVo(){YeuCauTiepNhanId = 2842,YeuCauDichVuKyThuatId = 10151},
                new ChiDinhGhiNhanVatTuThuocPTTTVo(){YeuCauTiepNhanId = 2851,YeuCauDichVuKyThuatId = 10167},
                new ChiDinhGhiNhanVatTuThuocPTTTVo(){YeuCauTiepNhanId = 2844,YeuCauDichVuKyThuatId = 10176},
                new ChiDinhGhiNhanVatTuThuocPTTTVo(){YeuCauTiepNhanId = 2844,YeuCauDichVuKyThuatId = 10177}
            };
            foreach (var yeuCauVo in arr)
            {
                _phauThuatThuThuatService.XuLyXuatYeuCauGhiNhanVTTHThuocChoNhungYeuCauQuenNhanXuat(yeuCauVo);
            }
        }

        [HttpPost("XuLyXuatYeuCauGhiNhanVTTHThuoc")]
        [ClaimRequirement(SecurityOperation.Update, DocumentType.PhauThuatThuThuatTheoNgay)]
        public async Task XuLyXuatYeuCauGhiNhanVTTHThuocAsync(GridItemGhiNhanVatTuThuocPTTTViewModel yeuCauViewModel)
        {
            var yeuCauVo = new ChiDinhGhiNhanVatTuThuocPTTTVo()
            {
                YeuCauTiepNhanId = yeuCauViewModel.YeuCauTiepNhanId,
                YeuCauDichVuKyThuatId = yeuCauViewModel.YeuCauDichVuKyThuatId
            };

            await _phauThuatThuThuatService.XuLyXuatYeuCauGhiNhanVTTHThuocAsync(yeuCauVo);
        }

        [HttpPost("CapNhatGridItemGhiNhanVTTHThuoc")]
        [ClaimRequirement(SecurityOperation.Update, DocumentType.PhauThuatThuThuatTheoNgay)]
        public async Task<ActionResult<ChiDinhDichVuResultVo>> CapNhatGridItemGhiNhanVTTHThuoc(GridItemGhiNhanVatTuThuocPTTTViewModel viewModel)
        {
            //await _yeuCauKhamBenhService.KiemTraDatayeuCauKhamBenhAsync(viewModel.YeuCauKhamBenhId);

            //if (viewModel.SoLuong == null || viewModel.SoLuong.Value == 0)
            if (viewModel.IsCapNhatSoLuong && (viewModel.SoLuong == null || viewModel.SoLuong.Value == 0))
            {
                throw new ApiException(_localizationService.GetResource("CapNhatGhiNhatVTTHThuoc.SoLuong.Required"));
            }

            var yeuCauTiepNhanChiTiet = _phauThuatThuThuatService.GetYeuCauTiepNhanForGhiNhanVatTuThuoc(viewModel.YeuCauTiepNhanId);
            //var yeuCauTiepNhanChiTiet2 = await _khamBenhService.GetYeuCauTiepNhanByIdAsync(viewModel.YeuCauTiepNhanId);

            var yeuCauVo = new ChiDinhGhiNhanVatTuThuocPTTTVo
            {
                YeuCauTiepNhanId = viewModel.YeuCauTiepNhanId,
                YeuCauDichVuKyThuatId = viewModel.YeuCauDichVuKyThuatId,
                YeuCauGhiNhanVTTHThuocId = viewModel.YeuCauGhiNhanVTTHThuocId,
                SoLuongCapNhat = viewModel.SoLuong,
                IsCapNhatSoLuong = viewModel.IsCapNhatSoLuong,
                IsCapNhatTinhPhi = viewModel.IsCapNhatTinhPhi,
                TinhPhi = viewModel.TinhPhi
                //DichVuChiDinhId = viewModel.DichVuChiDinhId,
                //DichVuGhiNhanId = viewModel.DichVuGhiNhanId,
                //KhoId = viewModel.KhoId,
                //SoLuong = viewModel.SoLuong,
                //TinhPhi = viewModel.TinhPhi,
                //LaDuocPhamBHYT = viewModel.LaDuocPhamBHYT,
                //GiaiDoanPhauThuat = viewModel.GiaiDoanPhauThuat
            };

            // xử lý cập nhật số luonjg, số lượng xuất
            await _phauThuatThuThuatService.CapNhatGridItemGhiNhanVTTHThuocAsync(yeuCauTiepNhanChiTiet, yeuCauVo);

            await _tiepNhanBenhNhanService.PrepareForEditDichVuAndUpdateAsync(yeuCauTiepNhanChiTiet);

            if (yeuCauVo.NhapKhoDuocPhamChiTiets.Any() || yeuCauVo.NhapKhoVatTuChiTiets.Any())
            {
                await _khamBenhService.CapNhatSoLuongTonKhiGhiNhanVTTHThuocAsync(yeuCauVo.NhapKhoDuocPhamChiTiets, yeuCauVo.NhapKhoVatTuChiTiets);
            }

            // get lại thông tin số dư tài khoản người bệnh
            var chiDinhDichVuResultVo = new ChiDinhDichVuResultVo();
            //chiDinhDichVuResultVo.SoDuTaiKhoan = await _taiKhoanBenhNhanService.GetSoTienDaTamUngAsync(yeuCauTiepNhanChiTiet.Id);
            //chiDinhDichVuResultVo.SoDuTaiKhoanConLai = await _taiKhoanBenhNhanService.GetSoTienUocLuongConLai(yeuCauTiepNhanChiTiet.Id);

            decimal soDuTk = 0;
            decimal soDuUocLuongConLai = 0;

            if (yeuCauTiepNhanChiTiet.NoiTruBenhAn != null || yeuCauTiepNhanChiTiet.QuyetToanTheoNoiTru == true)
            {
                //var chiPhiKhamChuaBenh = _thuNganNoiTruService.GetDanhSachChiPhiKhamChuaBenhChuaThu(yeuCauTiepNhanChiTiet.Id).Result.Select(o => o.BNConPhaiThanhToan).DefaultIfEmpty(0).Sum();
                var chiPhiKhamChuaBenh = _thuNganNoiTruService.GetSoTienBNConPhaiThanhToan(yeuCauTiepNhanChiTiet.Id).Result;

                soDuTk = await _taiKhoanBenhNhanService.GetSoTienDaTamUngAsync(yeuCauTiepNhanChiTiet.Id);
                soDuUocLuongConLai = soDuTk - chiPhiKhamChuaBenh;
            }
            else
            {
                soDuTk = await _taiKhoanBenhNhanService.GetSoTienDaTamUngAsync(yeuCauTiepNhanChiTiet.Id);
                soDuUocLuongConLai = await _taiKhoanBenhNhanService.GetSoTienUocLuongConLai(yeuCauTiepNhanChiTiet.Id);
            }

            chiDinhDichVuResultVo.SoDuTaiKhoan = soDuTk;
            chiDinhDichVuResultVo.SoDuTaiKhoanConLai = soDuUocLuongConLai;

            return chiDinhDichVuResultVo;
        }

        [HttpPost("GetThongTinHoanTraVatTuThuocPTTT")]
        [ClaimRequirement(SecurityOperation.View, DocumentType.PhauThuatThuThuatTheoNgay, DocumentType.KhamBenh, DocumentType.KhamBenhDangKham, DocumentType.KhamDoanKhamBenh)]
        public async Task<ActionResult> GetThongTinHoanTraVatTuThuocPTTT(HoanTraVatTuThuocVo hoanTraVatTuThuocVo)
        {
            var res = await _phauThuatThuThuatService.GetThongTinHoanTraVatTuThuocPTTT(hoanTraVatTuThuocVo);
            return Ok(res);
        }

        [HttpPost("GetDataForGridDanhSachVatTuThuocHoanTraPTTT")]
        [ClaimRequirement(SecurityOperation.View, DocumentType.PhauThuatThuThuatTheoNgay, DocumentType.KhamBenh, DocumentType.KhamBenhDangKham, DocumentType.KhamDoanKhamBenh)]
        public async Task<ActionResult<GridDataSource>> GetDataForGridDanhSachVatTuThuocHoanTraPTTT([FromBody] QueryInfo queryInfo)
        {
            var gridData = await _phauThuatThuThuatService.GetDataForGridDanhSachVatTuThuocHoanTraPTTT(queryInfo);
            return Ok(gridData);
        }

        [HttpPost("HoanTraVatTuThuocTuBenhNhanPTTT")]
        [ClaimRequirement(SecurityOperation.Update, DocumentType.PhauThuatThuThuatTheoNgay, DocumentType.KhamBenh, DocumentType.KhamBenhDangKham, DocumentType.KhamDoanKhamBenh)]
        public async Task<ActionResult> HoanTraVatTuThuocTuBenhNhanPTTT(PhauThuatThuThuatHoanTraVatTuThuocViewModel hoanTraViewModel)
        {
            var model = new YeuCauHoanTraVatTuThuocPTTTVo
            {
                Id = hoanTraViewModel.Id,
                VatTuThuocBenhVienId = hoanTraViewModel.VatTuThuocBenhVienId,
                DuocHuongBaoHiem = hoanTraViewModel.DuocHuongBaoHiem,
                NgayYeuCau = hoanTraViewModel.NgayYeuCau.Value,
                NhanVienYeuCauId = hoanTraViewModel.NhanVienYeuCauId.Value,
                //SoLuong = hoanTraViewModel.SoLuong,
                //SoLuongDaTra = hoanTraViewModel.SoLuongDaTra,
                //SoLuongTra = hoanTraViewModel.SoLuongTra,
                //NhomYeuCauId = hoanTraViewModel.NhomYeuCauId,
                YeuCauDuocPhamVatTuBenhViens = hoanTraViewModel.YeuCauDuocPhamVatTuBenhViens.Select(p => new YeuCauHoanTraVatTuThuocChiTietPTTTVo
                {
                    Id = p.Id,
                    SoLuong = p.SoLuong,
                    SoLuongDaTra = p.SoLuongDaTra,
                    SoLuongTra = p.SoLuongTra,
                    NhomYeuCauId = p.NhomYeuCauId
                }).ToList()
            };

            await _phauThuatThuThuatService.XuLyHoanTraYeuCauGhiNhanVTTHThuocAsync(model);

            return NoContent();
        }

        [HttpPost("GetListKhoGhiNhanPTTT")]
        public async Task<ActionResult<ICollection<KhoSapXepUuTienLookupItemVo>>> GetListKhoGhiNhanPTTT([FromBody]DropDownListRequestModel queryInfo)
        {
            var lookup = await _phauThuatThuThuatService.GetListKhoGhiNhanPTTTAsync(queryInfo);
            return Ok(lookup);
        }

        [HttpPost("GetGridDataGoiDuocPhamVatTuTrongDichVu")]
        [ClaimRequirement(SecurityOperation.View, DocumentType.PhauThuatThuThuatTheoNgay)]
        public async Task<ActionResult<GridDataSource>> GetGridDataGoiDuocPhamVatTuTrongDichVu(QueryInfo queryInfo)
        {
            //var gridData = await _phauThuatThuThuatService.GetGridDataGhiNhanVTTHThuocAsync(yeuCauDichVuKyThuatId);
            var gridData = await _phauThuatThuThuatService.GetGridDataGoiDuocPhamVatTuTrongDichVu(queryInfo);
            return gridData;
        }

        [HttpPost("GetTotalPagesGoiDuocPhamVatTuTrongDichVu")]
        [ClaimRequirement(SecurityOperation.View, DocumentType.PhauThuatThuThuatTheoNgay)]
        public async Task<ActionResult<GridDataSource>> GetTotalPagesGoiDuocPhamVatTuTrongDichVu(QueryInfo queryInfo)
        {
            //var gridData = await _phauThuatThuThuatService.GetGridDataGhiNhanVTTHThuocAsync(yeuCauDichVuKyThuatId);
            var gridData = await _phauThuatThuThuatService.GetTotalPagesGoiDuocPhamVatTuTrongDichVu(queryInfo);
            return gridData;
        }

        [HttpPost("GetKhoLeNhanVienQuanLy")]
        public async Task<ActionResult<ICollection<LookupItemVo>>> GetKhoLeNhanVienQuanLy([FromBody] DropDownListRequestModel queryInfo)
        {
            var lookup = await _phauThuatThuThuatService.GetKhoLeNhanVienQuanLy(queryInfo);
            return Ok(lookup);
        }

        [HttpPost("ThemGhiNhanThuocVatTus")]
        [ClaimRequirement(SecurityOperation.Add, DocumentType.PhauThuatThuThuatTheoNgay , DocumentType.CanLamSang)]
        public async Task<ActionResult<ChiDinhDichVuResultVo>> ThemGhiNhanThuocVatTus(ChiDinhGhiNhanVatTuThuocPTTTTheoGoiDVKTVo model)
        {
            var yeuCauTiepNhan = await _yeuCauTiepNhanService.GetByIdAsync(model.YeuCauTiepNhanId,
                s => s.Include(z => z.YeuCauDuocPhamBenhViens).ThenInclude(z => z.DuocPhamBenhVien)
                      .Include(z => z.YeuCauVatTuBenhViens).ThenInclude(z => z.VatTuBenhVien)
                      .Include(z => z.YeuCauDichVuKyThuats)
                      .Include(z => z.NoiTruBenhAn)
                );
            await _phauThuatThuThuatService.XuLyThemGhiNhanThuocVatTusAsync(model, yeuCauTiepNhan);

            // gọi hàm chung
            await _yeuCauTiepNhanService.PrepareForAddDichVuAndUpdateAsync(yeuCauTiepNhan);

            var chiDinhDichVuResultVo = new ChiDinhDichVuResultVo();
            decimal soDuTk = 0;
            decimal soDuUocLuongConLai = 0;

            if (yeuCauTiepNhan.NoiTruBenhAn != null || yeuCauTiepNhan.QuyetToanTheoNoiTru == true)
            {
                //var chiPhiKhamChuaBenh = _thuNganNoiTruService.GetDanhSachChiPhiKhamChuaBenhChuaThu(yeuCauTiepNhan.Id).Result.Select(o => o.BNConPhaiThanhToan).DefaultIfEmpty(0).Sum();
                var chiPhiKhamChuaBenh = _thuNganNoiTruService.GetSoTienBNConPhaiThanhToan(yeuCauTiepNhan.Id).Result;

                soDuTk = await _taiKhoanBenhNhanService.GetSoTienDaTamUngAsync(yeuCauTiepNhan.Id);
                soDuUocLuongConLai = soDuTk - chiPhiKhamChuaBenh;
            }
            else
            {
                soDuTk = await _taiKhoanBenhNhanService.GetSoTienDaTamUngAsync(yeuCauTiepNhan.Id);
                soDuUocLuongConLai = await _taiKhoanBenhNhanService.GetSoTienUocLuongConLai(yeuCauTiepNhan.Id);
            }

            chiDinhDichVuResultVo.SoDuTaiKhoan = soDuTk;
            chiDinhDichVuResultVo.SoDuTaiKhoanConLai = soDuUocLuongConLai;

            return chiDinhDichVuResultVo;
        }
    }
}
