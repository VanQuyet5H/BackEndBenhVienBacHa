using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Camino.Api.Auth;
using Camino.Api.Extensions;
using Camino.Api.Models.DichVuKyThuatBenhVien;
using Camino.Api.Models.Error;
using Camino.Core.Domain;
using Camino.Core.Domain.Entities.DichVuKyThuats;
using Camino.Core.Domain.Entities.DichVuXetNghiems;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.DichVuKyThuat;
using Camino.Core.Domain.ValueObject.ExportExcelItemVo;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Domain.ValueObject.KhamBenhs;
using Camino.Core.Domain.ValueObject.QuayThuoc;
using Camino.Core.Domain.ValueObject.YeuCauKhamBenh;
using Camino.Core.Helpers;
using Camino.Services.CauHinh;
using Camino.Services.DichVuKyThuat;
using Camino.Services.DichVuKyThuatBenhVien;
using Camino.Services.ExportImport;
using Camino.Services.Helpers;
using Camino.Services.KhamBenhs;
using Camino.Services.KhoaPhong;
using Camino.Services.Localization;
using Camino.Services.TaiLieuDinhKem;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using static Camino.Core.Domain.Enums;

namespace Camino.Api.Controllers
{
    public class DichVuKyThuatBenhVienController : CaminoBaseController
    {
        private readonly IDichVuKyThuatBenhVienService _dichVuKyThuatBenhVienService;
        private readonly IDichVuKyThuatService _dichVuKyThuatService;
        private readonly ILocalizationService _localizationService;
        private readonly IKhoaPhongService _khoaPhongService;
        private readonly IExcelService _excelService;
        private readonly IKhamBenhService _khamBenhService;
        private readonly ICauHinhService _cauHinhService;
        private readonly ITaiLieuDinhKemService _taiLieuDinhKemService;
        public DichVuKyThuatBenhVienController(IKhoaPhongService khoaPhongService, IDichVuKyThuatBenhVienService dichVuKyThuatBenhVienService, ILocalizationService localizationService
        , IDichVuKyThuatService dichVuKyThuatService, IExcelService excelService
        , IKhamBenhService khamBenhService
        , ICauHinhService cauHinhService
        , ITaiLieuDinhKemService taiLieuDinhKemService
        )
        {
            _khoaPhongService = khoaPhongService;
            _localizationService = localizationService;
            _dichVuKyThuatBenhVienService = dichVuKyThuatBenhVienService;
            _dichVuKyThuatService = dichVuKyThuatService;
            _excelService = excelService;
            _khamBenhService = khamBenhService;
            _cauHinhService = cauHinhService;
            _taiLieuDinhKemService = taiLieuDinhKemService;
        }
        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetDataForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DanhMucDichVuKyThuatTaiBenhVien)]
        public async Task<ActionResult<GridDataSource>> GetDataForGridAsync([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _dichVuKyThuatBenhVienService.GetDataForGridAsync(queryInfo);
            return Ok(gridData);
        }
        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetTotalPageForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DanhMucDichVuKyThuatTaiBenhVien)]
        public async Task<ActionResult<GridDataSource>> GetTotalPageForGridAsync([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _dichVuKyThuatBenhVienService.GetTotalPageForGridAsync(queryInfo);
            return Ok(gridData);
        }


        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetDataForGridChildAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DanhMucDichVuKyThuatTaiBenhVien)]
        public async Task<ActionResult<GridDataSource>> GetDataForGridChildAsync([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _dichVuKyThuatBenhVienService.GetDataForGridChildAsync(queryInfo);
            return Ok(gridData);
        }

        //
        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetTotalPageForGridGiaBenhVienAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DanhMucDichVuKyThuatTaiBenhVien)]
        public async Task<ActionResult<GridDataSource>> GetTotalPageForGridGiaBenhVienAsync([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _dichVuKyThuatBenhVienService.GetTotalPageForGridChildBenhVienAsync(queryInfo);
            return Ok(gridData);
        }

        [HttpPost("GetNhomDichVu")]
        public async Task<ActionResult> GetNhomDichVu()
        {
            var lookup = await _dichVuKyThuatBenhVienService.GetNhomDichVu();
            return Ok(lookup);
        }
        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetDataForGridChildGiaBenhVienAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DanhMucDichVuKyThuatTaiBenhVien)]
        public async Task<ActionResult<GridDataSource>> GetDataForGridChildGiaBenhVienAsync([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _dichVuKyThuatBenhVienService.GetDataForGridChildBenhVienAsync(queryInfo);
            return Ok(gridData);
        }
        [HttpPost("GetKhoaKham")]
        public async Task<ActionResult> GetKhoaKham([FromBody]DropDownListRequestModel queryInfo)
        {
            var lookup = await _dichVuKyThuatBenhVienService.GetKhoaKham(queryInfo);
            return Ok(lookup);
        }
        [HttpPost("GetPhongKhamTheoDichVuKyThuatBenhVien")]
        public async Task<ActionResult> GetPhongKhamTheoDichVuKyThuatBenhVien([FromBody]DropDownListRequestModel queryInfo)
        {
            var lookup = await _dichVuKyThuatBenhVienService.GetPhongKhamTheoDichVuKyThuatBenhVien(queryInfo);
            return Ok(lookup);
        }
        //
        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetTotalPageForGridChildAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DanhMucDichVuKyThuatTaiBenhVien)]
        public async Task<ActionResult<GridDataSource>> GetTotalPageForGridChildAsync([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _dichVuKyThuatBenhVienService.GetTotalPageForGridChildAsync(queryInfo);
            return Ok(gridData);
        }

        #region CRUD
        private bool ValidateCungDong(DateTime? TuNgay, DateTime? denNgay)
        {
            if (TuNgay != null && denNgay != null)
            {
                if (TuNgay > denNgay)
                {
                    //throw new ApiException(
                    //                    _localizationService.GetResource("DichVuKhamBenhBenhVien.DichVuKhamBenhBenhVienGiaBenhViens.TuNgay"));
                    return true;
                }
            }

            return false;
        }
        private bool ValidateDongTren(DateTime? TuNgayDuoi, DateTime? denNgayTren)
        {
            //throw new ApiException(
            //                             _localizationService.GetResource("DichVuKhamBenhBenhVien.DichVuKhamBenhBenhVienGiaBenhViens.TuNgayLonHonDenNgayDongTruoc"));
            if (TuNgayDuoi != null && denNgayTren != null)
            {
                if (TuNgayDuoi < denNgayTren)
                {
                    return true;
                }
            }
            return false;
        }
        [HttpGet("{id}")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DanhMucDichVuKyThuatTaiBenhVien)]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<ActionResult<DichVuKyThuatBenhVienViewModel>> Get(long id)
        {
            var result = await _dichVuKyThuatBenhVienService.GetByIdAsync(id, p => p.Include(s => s.DichVuKyThuat).ThenInclude(t => t.NhomDichVuKyThuat)
                .Include(x => x.DichVuKyThuatBenhVienGiaBaoHiems)
                .Include(x => x.DichVuKyThuatVuBenhVienGiaBenhViens).ThenInclude(x => x.NhomGiaDichVuKyThuatBenhVien)
                .Include(x => x.DichVuKyThuatBenhVienNoiThucHiens).ThenInclude(y => y.PhongBenhVien)
                .Include(x => x.NhomDichVuBenhVien)
                .Include(x => x.ChuyenKhoaChuyenNganh)
                .Include(x => x.DichVuKyThuatBenhVienNoiThucHienUuTiens).ThenInclude(y => y.PhongBenhVien)
                .Include(x => x.DichVuKyThuatBenhVienTiemChungs).ThenInclude(x => x.DuocPhamBenhVien).ThenInclude(x => x.DuocPham)
                .Include(x => x.DichVuKyThuatBenhVienDinhMucDuocPhamVatTus));
            if (result == null)
                return NotFound();
            var resultData = result.ToModel<DichVuKyThuatBenhVienViewModel>();

            foreach (var item in resultData.DichVuKyThuatVuBenhVienGiaBenhViens)
            {
                item.NhomGiaDichVuKyThuatBenhVienText = item.NhomGiaDichVuKyThuatBenhVien.Ten;
            }

            var nhomTiemChungId = GetNhomBenhVienTiemChung();
            resultData.LaVacxin = nhomTiemChungId != null && resultData.NhomDichVuBenhVienId == nhomTiemChungId;
            resultData.NhomDichVuVacxinId = nhomTiemChungId;
            if (resultData.DichVuKyThuatVuBenhVienGiaBenhViens != null && resultData.DichVuKyThuatVuBenhVienGiaBenhViens.Count > 1)
            {
                var nhomdichVu = await _dichVuKyThuatBenhVienService.GetNhomGiaDichVuKyThuatBenhVien();
                for (int j = 0; j < nhomdichVu.Count; j++)
                {
                    var giaCungLoais = resultData.DichVuKyThuatVuBenhVienGiaBenhViens.Where(x => x.NhomGiaDichVuKyThuatBenhVienId == nhomdichVu[j].Id).ToList();
                    if (giaCungLoais.Count > 1)
                    {
                        for (var i = 0; i < giaCungLoais.Count - 1; i++)
                        {
                            giaCungLoais[i].DenNgayRequired = true;
                        }
                    }
                }
            }
            if (resultData.DichVuKyThuatBenhVienGiaBaoHiems != null && resultData.DichVuKyThuatBenhVienGiaBaoHiems.Count > 1)
            {
                var giaCungLoais = resultData.DichVuKyThuatBenhVienGiaBaoHiems.ToList();
                if (giaCungLoais.Count > 1)
                {
                    // bỏ vị trí cuối cùng
                    for (var i = 0; i < giaCungLoais.Count - 1; i++)
                    {
                        giaCungLoais[i].DenNgayRequired = true;
                    }
                }
            }
            // định mức dịch vụ
            if (result.DichVuKyThuatBenhVienDinhMucDuocPhamVatTus.Any())
            {

            }
            resultData.DichVuChuyenGoi = result.DichVuChuyenGoi;
            resultData.ChuyenKhoaChuyenNganhText = result?.ChuyenKhoaChuyenNganh?.Ten;
            return Ok(resultData);
        }

        [HttpPut]
        [ClaimRequirement(Enums.SecurityOperation.Update, Enums.DocumentType.DanhMucDichVuKyThuatTaiBenhVien)]
        public async Task<ActionResult> Put([FromBody] DichVuKyThuatBenhVienViewModel viewModel)
        {

            if (viewModel.DichVuKyThuatBenhVienGiaBaoHiems.Count > 0)
            {
                //for (int i = 1; i < viewModel.DichVuKyThuatBenhVienGiaBaoHiems.Count; i++)
                //{
                //    viewModel.DichVuKyThuatBenhVienGiaBaoHiems.ToList()[i - 1].DenNgay = Convert.ToDateTime(viewModel.DichVuKyThuatBenhVienGiaBaoHiems.ToList()[i].TuNgay).AddDays(-1);
                //}
                for (int i = 1; i < viewModel.DichVuKyThuatBenhVienGiaBaoHiems.Count; i++)
                {

                    //viewModel.DichVuKyThuatBenhVienGiaBaoHiems[i - 1].DenNgay = Convert.ToDateTime(viewModel.DichVuKyThuatBenhVienGiaBaoHiems[i].TuNgay).Date.AddDays(-1);

                    if (
                         viewModel.DichVuKyThuatBenhVienGiaBaoHiems[i - 1].TuNgay > viewModel.DichVuKyThuatBenhVienGiaBaoHiems[i - 1].DenNgay ||
                         viewModel.DichVuKyThuatBenhVienGiaBaoHiems[i].TuNgay > viewModel.DichVuKyThuatBenhVienGiaBaoHiems[i].DenNgay)
                    {
                        throw new ApiException(
                                            _localizationService.GetResource("DichVuKyThuatBenhVien.DichVuKyThuatBenhVienGiaBaoHiems.TuNgay"));
                    }
                    if (viewModel.DichVuKyThuatBenhVienGiaBaoHiems[i].TuNgay < Convert.ToDateTime(viewModel.DichVuKyThuatBenhVienGiaBaoHiems[i - 1].DenNgay))
                    {
                        throw new ApiException(
                                                  _localizationService.GetResource("DichVuKyThuatBenhVien.DichVuKyThuatVuBenhVienGiaBenhViens.TuNgayLonHonDenNgayDongTruoc"));
                    }
                }
            }

            //if (viewModel.DichVuKyThuatVuBenhVienGiaBenhViens.Count > 0)
            //{
            //    var nhomdichVu = await _dichVuKyThuatBenhVienService.GetNhomGiaDichVuKyThuatBenhVien();
            //    for (int j = 0; j < nhomdichVu.Count; j++)
            //    {
            //        var benhvien = viewModel.DichVuKyThuatVuBenhVienGiaBenhViens.Where(x => x.NhomGiaDichVuKyThuatBenhVienId == nhomdichVu[j].Id).ToList();
            //        for (int i = 1; i < benhvien.Count; i++)
            //        {

            //            benhvien[i - 1].DenNgay = Convert.ToDateTime(benhvien[i].TuNgay).Date.AddDays(-1);
            //            //if (benhvien[i - 1].DenNgay != null)
            //            //{
            //            //    if (benhvien[i - 1].TuNgay >= benhvien[i - 1].DenNgay)
            //            //    {
            //            //        throw new ApiException(
            //            //                        _localizationService.GetResource("DichVuKyThuatBenhVien.DichVuKyThuatVuBenhVienGiaBenhViens.TuNgay"));
            //            //    }
            //            //}
            //            if (benhvien[i].TuNgay <= Convert.ToDateTime(benhvien[i - 1].TuNgay).AddDays(1) ||
            //                 benhvien[i - 1].TuNgay >= benhvien[i - 1].DenNgay ||
            //                 benhvien[i].TuNgay >= benhvien[i].DenNgay)
            //            {
            //                throw new ApiException(
            //                                    _localizationService.GetResource("DichVuKyThuatBenhVien.DichVuKyThuatVuBenhVienGiaBenhViens.TuNgay"));
            //            }

            //        }
            //    }

            //}
            #region nam update 24062021
            //Hiện tại mình đã có field đến ngày nhưng field này chỉ hiển thị khi có 2 dòng cùng loại giá.
            //Giờ mình cho nó hiển thị ra luôn và ko disable để cho nó có thể sửa 
            //nhưng mình vẫn phải validate là từ ngày của dòng sau > đến ngày của dòng trước +1(cùng loại giá)
            if (viewModel.DichVuKyThuatVuBenhVienGiaBenhViens.Count > 0)
            {
                var nhomdichVu = await _dichVuKyThuatBenhVienService.GetNhomGiaDichVuKyThuatBenhVien();
                for (int j = 0; j < nhomdichVu.Count; j++)
                {
                    var benhvien = viewModel.DichVuKyThuatVuBenhVienGiaBenhViens.Where(x => x.NhomGiaDichVuKyThuatBenhVienId == nhomdichVu[j].Id).ToList();
                    for (int i = 0; i < benhvien.Count; i++)
                    {
                        var value = ValidateCungDong(benhvien[i].TuNgay, benhvien[i].DenNgay);
                        if (value == true)
                        {
                            throw new ApiException(
                                                 _localizationService.GetResource("DichVuKyThuatBenhVien.DichVuKyThuatVuBenhVienGiaBenhViens.TuNgay"));
                        }
                        if (i != 0)
                        {
                            var valueKhacDong = ValidateDongTren(benhvien[i].TuNgay, benhvien[i - 1].DenNgay);
                            if (valueKhacDong == true)
                            {
                                throw new ApiException(
                                                      _localizationService.GetResource("DichVuKyThuatBenhVien.DichVuKyThuatVuBenhVienGiaBenhViens.TuNgayLonHonDenNgayDongTruoc"));
                            }

                        }
                        //if(i <benhvien.Count)
                        //{
                        //    if (benhvien[i].TuNgay <= Convert.ToDateTime(benhvien[i - 1].DenNgay))
                        //    {
                        //        throw new ApiException(
                        //                            _localizationService.GetResource("DichVuKyThuatBenhVien.DichVuKyThuatVuBenhVienGiaBenhViens.TuNgayLonHonDenNgayDongTruoc"));
                        //    }

                        //    if (
                        //         benhvien[i - 1].TuNgay > benhvien[i - 1].DenNgay ||
                        //         benhvien[i].TuNgay > benhvien[i].DenNgay)
                        //    {
                        //        throw new ApiException(
                        //                            _localizationService.GetResource("DichVuKyThuatBenhVien.DichVuKyThuatVuBenhVienGiaBenhViens.TuNgay"));
                        //    }
                        //}
                    }
                }

            }
            //Hiện tại cho giá bệnh viện nhâp 0 nên không cho nhập giá bhyt
            var kiemTraGiaBenhVien = viewModel.DichVuKyThuatVuBenhVienGiaBenhViens.Any(c => c.TuNgay.Value.Date <= DateTime.Now.Date && c.DenNgay?.Date >= DateTime.Now.Date && c.Gia == 0);
            var kiemTraGiaBHYT = viewModel.DichVuKyThuatBenhVienGiaBaoHiems.Any();
            if (kiemTraGiaBenhVien && kiemTraGiaBHYT)
            {
                throw new ApiException(_localizationService.GetResource("Common.KhongNhapBHYT"));
            }

            //Hiện tại cho giá bệnh viện không được nhỏ hơn giá BHYT
            var giabenhViens = viewModel.DichVuKyThuatVuBenhVienGiaBenhViens.Where(c => c.TuNgay.Value.Date <= DateTime.Now.Date && c.DenNgay?.Date >= DateTime.Now.Date && c.Gia != 0);
            var giaBHYTs = viewModel.DichVuKyThuatBenhVienGiaBaoHiems.Where(c => c.TuNgay.Value.Date <= DateTime.Now.Date && c.DenNgay?.Date >= DateTime.Now.Date && c.Gia != 0);

            if (giabenhViens.Any() && giaBHYTs.Any())
            {
                foreach (var giabenhVien in giabenhViens)
                {
                    foreach (var giaBHYT in giaBHYTs)
                    {
                        if (giabenhVien.Gia < giaBHYT.Gia)
                        {
                            throw new ApiException(_localizationService.GetResource("Common.GiaBVLonGiaBHYT"));
                        }
                    }
                }
            }

            #endregion
            var entity = await _dichVuKyThuatBenhVienService.GetByIdAsync(viewModel.Id, p => p.Include(s => s.DichVuKyThuat)
                .Include(x => x.DichVuKyThuatBenhVienGiaBaoHiems)
                .Include(x => x.DichVuXetNghiem).ThenInclude(z => z.DichVuXetNghiems).ThenInclude(z => z.DichVuXetNghiems)
                .Include(x => x.DichVuKyThuatVuBenhVienGiaBenhViens).ThenInclude(x => x.NhomGiaDichVuKyThuatBenhVien)
                .Include(x => x.DichVuKyThuatBenhVienNoiThucHiens)
                .Include(x => x.DichVuKyThuatBenhVienNoiThucHienUuTiens).ThenInclude(y => y.PhongBenhVien)
                .Include(x => x.DichVuKyThuatBenhVienTiemChungs)
                .Include(x => x.DichVuKyThuatBenhVienDinhMucDuocPhamVatTus));
            //if (_dichVuKyThuatBenhVienService.CheckExistDichVuKyThuatBenhVien(Convert.ToInt64(viewModel.DichVuKyThuatId)) != null && viewModel.DichVuKyThuatId!=entity.DichVuKyThuatId)
            //{
            //    throw new ApiException(
            //                               _localizationService.GetResource("DichVuKyThuatBenhVien.HaveExist"));
            //}
            if (entity == null)
                return NotFound();

            viewModel.ToEntity(entity);
            //await _dichVuKyThuatBenhVienService.UpdateAsync(entity);
            //for (int i = 1; i < entity.DichVuKyThuatBenhVienGiaBaoHiems.Count; i++)
            //{
            //    entity.DichVuKyThuatBenhVienGiaBaoHiems.ToList()[i - 1].DenNgay = entity.DichVuKyThuatBenhVienGiaBaoHiems.ToList()[i].TuNgay.AddDays(-1);
            //}
            //await _dichVuKyThuatBenhVienService.UpdateDayGiaBenhVienEntity(entity.DichVuKyThuatVuBenhVienGiaBenhViens);

            // xử lý kiểm tra giá BH so với giá BV
            var lstGiaBaoHiem = entity.DichVuKyThuatBenhVienGiaBaoHiems.Where(x => x.WillDelete != true).ToList();
            var lstGiaBenhVien = entity.DichVuKyThuatVuBenhVienGiaBenhViens.Where(x => x.WillDelete != true).ToList();
            CheckValidGiaBaoHiem(lstGiaBaoHiem, lstGiaBenhVien);

            // xử lý xóa nơi thực hiện ưu tiên nếu xóa nơi thực hiện cũ
            await _dichVuKyThuatBenhVienService.XuLyCapNhatNoiThucHienUuTienKhiCapNhatDichVuKyThuatAsync(entity);
            var nhomDichVuBenhViens = await _dichVuKyThuatBenhVienService.NhomDichVuBenhViens();
            await _dichVuKyThuatBenhVienService.XuLyChuyenNhomXetNghiem(entity, nhomDichVuBenhViens, viewModel.NhomDichVuBenhVienId.GetValueOrDefault(), viewModel.Ma, viewModel.Ten);

            //update 20/06/2022: set về cuối ngày của đến ngày
            foreach (var giaBaoHiem in entity.DichVuKyThuatBenhVienGiaBaoHiems)
            {
                if (giaBaoHiem.DenNgay != null)
                {
                    giaBaoHiem.DenNgay = giaBaoHiem.DenNgay.Value.Date.AddHours(23).AddMinutes(59).AddSeconds(59);
                }
            }
            foreach (var giaBenhVien in entity.DichVuKyThuatVuBenhVienGiaBenhViens)
            {
                if (giaBenhVien.DenNgay != null)
                {
                    giaBenhVien.DenNgay = giaBenhVien.DenNgay.Value.Date.AddHours(23).AddMinutes(59).AddSeconds(59);
                }
            }

            await _dichVuKyThuatBenhVienService.UpdateAsync(entity);
            return NoContent();
        }

        [HttpPost]
        [ClaimRequirement(Enums.SecurityOperation.Add, Enums.DocumentType.DanhMucDichVuKyThuatTaiBenhVien)]
        public async Task<ActionResult> Post([FromBody] DichVuKyThuatBenhVienViewModel chucVuViewModel)
        {


            //if (_dichVuKyThuatBenhVienService.CheckExistDichVuKyThuatBenhVien(Convert.ToInt64(chucVuViewModel.DichVuKyThuatId)) != null)
            //{
            //    throw new ApiException(
            //                               _localizationService.GetResource("DichVuKyThuatBenhVien.HaveExist"));
            //}
            if (chucVuViewModel.DichVuKyThuatBenhVienGiaBaoHiems.Count > 0)
            {
                for (int i = 1; i < chucVuViewModel.DichVuKyThuatBenhVienGiaBaoHiems.Count; i++)
                {

                    //chucVuViewModel.DichVuKyThuatBenhVienGiaBaoHiems[i - 1].DenNgay = Convert.ToDateTime(chucVuViewModel.DichVuKyThuatBenhVienGiaBaoHiems[i].TuNgay).Date.AddDays(-1);

                    if (
                         chucVuViewModel.DichVuKyThuatBenhVienGiaBaoHiems[i - 1].TuNgay > chucVuViewModel.DichVuKyThuatBenhVienGiaBaoHiems[i - 1].DenNgay ||
                         chucVuViewModel.DichVuKyThuatBenhVienGiaBaoHiems[i].TuNgay > chucVuViewModel.DichVuKyThuatBenhVienGiaBaoHiems[i].DenNgay)
                    {
                        throw new ApiException(
                                            _localizationService.GetResource("DichVuKyThuatBenhVien.DichVuKyThuatBenhVienGiaBaoHiems.TuNgay"));
                    }
                    if (chucVuViewModel.DichVuKyThuatBenhVienGiaBaoHiems[i].TuNgay < Convert.ToDateTime(chucVuViewModel.DichVuKyThuatBenhVienGiaBaoHiems[i - 1].TuNgay).AddDays(1))
                    {
                        throw new ApiException(
                                                  _localizationService.GetResource("DichVuKyThuatBenhVien.DichVuKyThuatVuBenhVienGiaBenhViens.TuNgayLonHonDenNgayDongTruoc"));
                    }
                }
            }
            //if (chucVuViewModel.DichVuKyThuatVuBenhVienGiaBenhViens.Count > 0)
            //{
            //    var nhomdichVu = await _dichVuKyThuatBenhVienService.GetNhomGiaDichVuKyThuatBenhVien();
            //    for (int j = 0; j < nhomdichVu.Count; j++)
            //    {
            //        var benhvien = chucVuViewModel.DichVuKyThuatVuBenhVienGiaBenhViens.Where(x => x.NhomGiaDichVuKyThuatBenhVienId == nhomdichVu[j].Id).ToList();
            //        for (int i = 1; i < benhvien.Count; i++)
            //        {

            //            benhvien[i - 1].DenNgay = Convert.ToDateTime(benhvien[i].TuNgay).Date.AddDays(-1);
            //            //if (benhvien[i - 1].DenNgay != null)
            //            //{
            //            //    if (benhvien[i - 1].TuNgay >= benhvien[i - 1].DenNgay)
            //            //    {
            //            //        throw new ApiException(
            //            //                        _localizationService.GetResource("DichVuKyThuatBenhVien.DichVuKyThuatVuBenhVienGiaBenhViens.TuNgay"));
            //            //    }
            //            //}
            //            if (benhvien[i].TuNgay <= Convert.ToDateTime(benhvien[i - 1].TuNgay).AddDays(1) ||
            //                 benhvien[i - 1].TuNgay >= benhvien[i - 1].DenNgay ||
            //                 benhvien[i].TuNgay >= benhvien[i].DenNgay)
            //            {
            //                throw new ApiException(
            //                                    _localizationService.GetResource("DichVuKyThuatBenhVien.DichVuKyThuatVuBenhVienGiaBenhViens.TuNgay"));
            //            }

            //        }
            //    }

            //}
            #region nam update 24062021
            //Hiện tại mình đã có field đến ngày nhưng field này chỉ hiển thị khi có 2 dòng cùng loại giá.
            //Giờ mình cho nó hiển thị ra luôn và ko disable để cho nó có thể sửa 
            //nhưng mình vẫn phải validate là từ ngày của dòng sau > đến ngày của dòng trước +1(cùng loại giá)
            if (chucVuViewModel.DichVuKyThuatVuBenhVienGiaBenhViens.Count > 0)
            {
                var nhomdichVu = await _dichVuKyThuatBenhVienService.GetNhomGiaDichVuKyThuatBenhVien();
                for (int j = 0; j < nhomdichVu.Count; j++)
                {
                    var benhvien = chucVuViewModel.DichVuKyThuatVuBenhVienGiaBenhViens.Where(x => x.NhomGiaDichVuKyThuatBenhVienId == nhomdichVu[j].Id && x.DenNgayRequired == true).ToList();
                    for (int i = 0; i < benhvien.Count; i++)
                    {
                        var value = ValidateCungDong(benhvien[i].TuNgay, benhvien[i].DenNgay);
                        if (value == true)
                        {
                            throw new ApiException(
                                                 _localizationService.GetResource("DichVuKyThuatBenhVien.DichVuKyThuatVuBenhVienGiaBenhViens.TuNgay"));
                        }
                        if (i != 0)
                        {
                            var valueKhacDong = ValidateDongTren(benhvien[i].TuNgay, benhvien[i - 1].DenNgay);
                            if (valueKhacDong == true)
                            {
                                throw new ApiException(
                                                      _localizationService.GetResource("DichVuKyThuatBenhVien.DichVuKyThuatVuBenhVienGiaBenhViens.TuNgayLonHonDenNgayDongTruoc"));
                            }

                        }
                        //if (i < benhvien.Count)
                        //{
                        //    if (benhvien[i].TuNgay <= Convert.ToDateTime(benhvien[i - 1].DenNgay))
                        //    {
                        //        throw new ApiException(
                        //                            _localizationService.GetResource("DichVuKyThuatBenhVien.DichVuKyThuatVuBenhVienGiaBenhViens.TuNgayLonHonDenNgayDongTruoc"));
                        //    }

                        //    if (
                        //         benhvien[i - 1].TuNgay > benhvien[i - 1].DenNgay ||
                        //         benhvien[i].TuNgay > benhvien[i].DenNgay)
                        //    {
                        //        throw new ApiException(
                        //                            _localizationService.GetResource("DichVuKyThuatBenhVien.DichVuKyThuatVuBenhVienGiaBenhViens.TuNgay"));
                        //    }
                        //}

                    }
                }

            }

            //Hiện tại cho giá bệnh viện nhâp 0 nên không cho nhập giá bhyt
            var kiemTraGiaBenhVien = chucVuViewModel.DichVuKyThuatVuBenhVienGiaBenhViens.Any(c => c.TuNgay.Value.Date <= DateTime.Now.Date && c.DenNgay?.Date >= DateTime.Now.Date && c.Gia == 0);
            var kiemTraGiaBHYT = chucVuViewModel.DichVuKyThuatBenhVienGiaBaoHiems.Any();
            if (kiemTraGiaBenhVien && kiemTraGiaBHYT)
            {
                throw new ApiException(_localizationService.GetResource("Common.KhongNhapBHYT"));
            }

            //Hiện tại cho giá bệnh viện không được nhỏ hơn giá BHYT
            var giabenhViens = chucVuViewModel.DichVuKyThuatVuBenhVienGiaBenhViens.Where(c => c.TuNgay.Value.Date <= DateTime.Now.Date && c.DenNgay?.Date >= DateTime.Now.Date && c.Gia != 0);
            var giaBHYTs = chucVuViewModel.DichVuKyThuatBenhVienGiaBaoHiems.Where(c => c.TuNgay.Value.Date <= DateTime.Now.Date && c.DenNgay?.Date >= DateTime.Now.Date && c.Gia != 0);

            if (giabenhViens.Any() && giaBHYTs.Any())
            {
                foreach (var giabenhVien in giabenhViens)
                {
                    foreach (var giaBHYT in giaBHYTs)
                    {
                        if (giabenhVien.Gia < giaBHYT.Gia)
                        {
                            throw new ApiException(_localizationService.GetResource("Common.GiaBVLonGiaBHYT"));
                        }
                    }
                }
            }
            #endregion

            var entity = chucVuViewModel.ToEntity<DichVuKyThuatBenhVien>();
            //await _dichVuKyThuatBenhVienService.AddAndUpdateLastEntity(Convert.ToDateTime(chucVuViewModel.DichVuKyThuatBenhVienGiaBaoHiems.FirstOrDefault().TuNgay), entity.DichVuKyThuatVuBenhVienGiaBenhViens);

            // xử lý kiểm tra giá BH so với giá BV
            var lstGiaBaoHiem = entity.DichVuKyThuatBenhVienGiaBaoHiems.Where(x => x.WillDelete != true).ToList();
            var lstGiaBenhVien = entity.DichVuKyThuatVuBenhVienGiaBenhViens.Where(x => x.WillDelete != true).ToList();
            CheckValidGiaBaoHiem(lstGiaBaoHiem, lstGiaBenhVien);
            if (chucVuViewModel.NhomDichVuBenhVienId == (long)LoaiDichVuKyThuat.XetNghiem)
            {
                var dichVuXetNghiem = new DichVuXetNghiem
                {
                    NhomDichVuBenhVienId = chucVuViewModel.NhomDichVuBenhVienId.GetValueOrDefault(),
                    Ma = chucVuViewModel.Ma,
                    Ten = chucVuViewModel.Ten,
                    CapDichVu = 1,
                    HieuLuc = true,
                };
                entity.DichVuXetNghiem = dichVuXetNghiem;
            }
            else
            {
                var nhomDichVuBenhViens = await _dichVuKyThuatBenhVienService.NhomDichVuBenhViens();
                LoaiDichVuKyThuat loaiDichVuKyThuat = CalculateHelper.GetLoaiDichVuKyThuat(chucVuViewModel.NhomDichVuBenhVienId.GetValueOrDefault(), nhomDichVuBenhViens);
                if (loaiDichVuKyThuat == LoaiDichVuKyThuat.XetNghiem)
                {
                    var dichVuXetNghiem = new DichVuXetNghiem
                    {
                        NhomDichVuBenhVienId = chucVuViewModel.NhomDichVuBenhVienId.GetValueOrDefault(),
                        Ma = chucVuViewModel.Ma,
                        Ten = chucVuViewModel.Ten,
                        CapDichVu = 1,
                        HieuLuc = true,
                    };
                    entity.DichVuXetNghiem = dichVuXetNghiem;
                }
            }
            await _dichVuKyThuatBenhVienService.AddAsync(entity);
            return NoContent();
        }
        //Delete
        [HttpDelete("{id}")]
        [ClaimRequirement(Enums.SecurityOperation.Delete, Enums.DocumentType.DanhMucDichVuKyThuatTaiBenhVien)]
        public async Task<ActionResult> Delete(long id)
        {
            var dichVuKyThuat = await _dichVuKyThuatBenhVienService.GetByIdAsync(id, p => p.Include(x => x.DichVuKyThuatBenhVienGiaBaoHiems)
                .Include(x => x.DichVuKyThuatVuBenhVienGiaBenhViens)
                .Include(x => x.DichVuKyThuatBenhVienNoiThucHiens)
                .Include(x => x.DichVuXetNghiem).ThenInclude(z => z.DichVuXetNghiems).ThenInclude(z => z.DichVuXetNghiems)
                .Include(x => x.DichVuKyThuatBenhVienNoiThucHienUuTiens)
                .Include(x => x.DichVuKyThuatBenhVienTiemChungs)
                .Include(x => x.DichVuKyThuatBenhVienDinhMucDuocPhamVatTus));
            if (dichVuKyThuat == null)
            {
                return NotFound();
            }
            if (dichVuKyThuat.DichVuXetNghiem != null)
            {
                dichVuKyThuat.DichVuXetNghiem.HieuLuc = false;
                if (dichVuKyThuat.DichVuXetNghiem.DichVuXetNghiems.Any())
                {
                    foreach (var item in dichVuKyThuat.DichVuXetNghiem.DichVuXetNghiems)
                    {
                        item.HieuLuc = false;
                    }
                }
            }
            await _dichVuKyThuatBenhVienService.DeleteAsync(dichVuKyThuat);
            return NoContent();
        }
        #endregion CRUD

        [HttpPost("GetDichVuKyThuat")]
        public async Task<ActionResult> GetDichVuKyThuat([FromBody]DropDownListRequestModel queryInfo)
        {
            var lookup = await _dichVuKyThuatBenhVienService.GetDichVuKyThuat(queryInfo);
            return Ok(lookup);
        }

        [HttpPost("GetDichVuKyThuatBenhVien")]
        public async Task<ActionResult> GetDichVuKyThuatBenhVien([FromBody]DropDownListRequestModel queryInfo)
        {
            var lookup = await _dichVuKyThuatBenhVienService.GetDichVuKyThuatBenhVien(queryInfo);
            return Ok(lookup);
        }

        [HttpPost("GetDichVuKyThuatById")]
        public async Task<ActionResult> GetDichVuKyThuatById([FromBody]DropDownListRequestModel queryInfo, long id)
        {
            var lookup = await _dichVuKyThuatBenhVienService.GetDichVuKyThuatById(queryInfo, id);
            return Ok(lookup);
        }

        [HttpGet("GetThongTinDichVuKyThuat")]
        public async Task<ActionResult<DichVuKyThuatViewModel>> GetThongTinDichVuKyThuatAsync(long dichVuKyThuatId)
        {
            var result = await _dichVuKyThuatService.GetByIdAsync(dichVuKyThuatId, x => x.Include(y => y.NhomDichVuKyThuat));
            if (result == null)
            {
                return NotFound();
            }

            var viewModel = result.ToModel<DichVuKyThuatViewModel>();
            return Ok(viewModel);
        }

        private void CheckValidGiaBaoHiem(ICollection<DichVuKyThuatBenhVienGiaBaoHiem> lstGiaBaoHiem, ICollection<DichVuKyThuatBenhVienGiaBenhVien> lstGiaBenhVien)
        {
            if (lstGiaBaoHiem.Any() && lstGiaBenhVien.Any())
            {
                foreach (var giaBaoHiem in lstGiaBaoHiem)
                {
                    var lstGiaBenhVienTheoKhoangThoiGian = lstGiaBenhVien.Where(x =>
                            // trường hợp từ ngày nằm trong khoảng thời gian
                            (giaBaoHiem.TuNgay.Date >= x.TuNgay.Date && (x.DenNgay == null || giaBaoHiem.TuNgay.Date <= x.DenNgay))

                            // trường hợp đến ngày nằm trong khoảng thời gian
                            || (giaBaoHiem.DenNgay != null && giaBaoHiem.DenNgay.Value.Date >= x.TuNgay.Date && (x.DenNgay == null || giaBaoHiem.DenNgay.Value.Date <= x.DenNgay.Value.Date))

                            // trường hợp ko có đến ngày
                            || (giaBaoHiem.DenNgay == null && (giaBaoHiem.TuNgay.Date <= x.TuNgay.Date || (x.DenNgay != null && giaBaoHiem.TuNgay.Date <= x.DenNgay.Value.Date))))
                        .ToList();
                    //                    foreach (var giaBenhVienTheoThoiGian in lstGiaBenhVienTheoKhoangThoiGian)
                    //                    {
                    //                        if (giaBenhVienTheoThoiGian.Gia < giaBaoHiem.Gia)
                    //                        {
                    //                            throw new ApiException(_localizationService.GetResource("DichVuKyThuatBenhVien.GiaBaoHiem.RangeValueByTime"));
                    //                        }
                    //                    }
                }
            }
        }

        [HttpPost("ExportDichVuKyThuatbenhVien")]
        [ClaimRequirement(Enums.SecurityOperation.Process, Enums.DocumentType.DanhMucDichVuKyThuatTaiBenhVien)]
        public async Task<ActionResult> ExportDichVuKyThuatbenhVienAsync(QueryInfo queryInfo)
        {
            // todo: hardcode max row export excel
            queryInfo.Skip = 0;
            queryInfo.Take = 20000;

            var gridData = await _dichVuKyThuatBenhVienService.GetDataForGridAsync(queryInfo);
            var dichVuKhamBenhBenhVienData = gridData.Data.Select(p => (DichVuKyThuatBenhVienGridVo)p).ToList();
            var dataExcel = dichVuKhamBenhBenhVienData.Map<List<DichVuKyThuatBenhVienExportExcel>>();

            var lstValueObject = new List<(string, string)>();
            lstValueObject.Add((nameof(DichVuKyThuatBenhVienExportExcel.Ma), "Mã"));
            lstValueObject.Add((nameof(DichVuKyThuatBenhVienExportExcel.Ma4350), "Mã 4350"));
            lstValueObject.Add((nameof(DichVuKyThuatBenhVienExportExcel.Ten), "Tên"));
            lstValueObject.Add((nameof(DichVuKyThuatBenhVienExportExcel.TenNoiThucHien), "Nơi Thực Hiện"));
            lstValueObject.Add((nameof(DichVuKyThuatBenhVienExportExcel.NgayBatDauHienThi), "Ngày bắt đầu"));
            lstValueObject.Add((nameof(DichVuKyThuatBenhVienExportExcel.ThongTu), "Thông tư"));
            lstValueObject.Add((nameof(DichVuKyThuatBenhVienExportExcel.NghiDinh), "Nghị định"));
            lstValueObject.Add((nameof(DichVuKyThuatBenhVienExportExcel.NoiBanHanh), "Nơi ban hành"));
            lstValueObject.Add((nameof(DichVuKyThuatBenhVienExportExcel.SoMayTT), "Số máy TT"));
            lstValueObject.Add((nameof(DichVuKyThuatBenhVienExportExcel.SoMayCBCM), "Số CBCM"));
            lstValueObject.Add((nameof(DichVuKyThuatBenhVienExportExcel.ThoiGianThucHien), "Thời gian thực hiện"));
            lstValueObject.Add((nameof(DichVuKyThuatBenhVienExportExcel.SoCaCP), "Số ca CP"));
            lstValueObject.Add((nameof(DichVuKyThuatBenhVienExportExcel.MoTa), "Mô Tả"));
            lstValueObject.Add((nameof(DichVuKyThuatBenhVienExportExcel.HieuLucHienThi), "Hiệu lực"));

            var bytes = _excelService.ExportManagermentView(dataExcel, lstValueObject, "Dịch vụ kỹ thuật bệnh viện");

            HttpContext.Response.Headers.Add("content-disposition", "attachment; filename=DichVuKyThuatbenhVien" + DateTime.Now.Year + ".xls");
            Response.ContentType = "application/vnd.ms-excel";

            return new FileContentResult(bytes, "application/vnd.ms-excel");
        }

        [HttpPost("GetListNoiThucHienUuTienTheoNoiThucHienDangChon")]
        public async Task<ActionResult> GetListNoiThucHienUuTienTheoNoiThucHienDangChonAsync([FromBody]DropDownListRequestModel queryInfo)
        {
            var lookup = await _dichVuKyThuatBenhVienService.GetListNoiThucHienUuTienTheoNoiThucHienDangChonAsync(queryInfo);
            return Ok(lookup);
        }

        //Update phân loại PTTT (BVHD-3180)
        [HttpPost("GetDanhSachPhanLoaiPTTT")]
        public ActionResult<ICollection<LookupItemVo>> GetDanhSachPhanLoaiPTTT([FromBody]DropDownListRequestModel model)
        {
            var lookup = _dichVuKyThuatBenhVienService.GetDanhSachPhanLoaiPTTT(model);
            return Ok(lookup);
        }

        [HttpPost("GetListNhomDichVuBenhVien")]
        public async Task<ActionResult<ICollection<NhomDichVuBenhVienTreeViewVo>>> GetListNhomDichVuBenhVienAsync([FromBody]DropDownListRequestModel model)
        {
            var lookup = await _khamBenhService.GetListNhomDichVuBenhVienAsync(model, true);
            return Ok(lookup);
        }

        [HttpPost("GetListNhomDichVuBenhVienCombobox")]
        public async Task<ActionResult<ICollection<LookupItemVo>>> 
            GetListNhomDichVuBenhVienCombobox([FromBody]DropDownListRequestModel model)
        {
            var lookup = await _khamBenhService.GetListNhomDichVuBenhVienAsync(model, true);
            return Ok(lookup);
        }

        [HttpPost("GetListDuocPhamBenhVien")]
        public async Task<ActionResult> GetListDuocPhamBenhVien([FromBody]DropDownListRequestModel queryInfo)
        {
            var lookup = await _dichVuKyThuatBenhVienService.GetListDuocPhamBenhVien(queryInfo);
            return Ok(lookup);
        }

        [HttpPost("GetListChuyenKhoaChuyenNganh")]
        public async Task<ActionResult> GetListChuyenKhoaChuyenNganh([FromBody]DropDownListRequestModel queryInfo)
        {
            var lookup = await _dichVuKyThuatBenhVienService.GetListChuyenKhoaChuyenNganh(queryInfo);
            return Ok(lookup);
        }

        [HttpGet("GetNhomBenhVienTiemChung")]
        public long? GetNhomBenhVienTiemChung()
        {
            var cauHinhNhomTiemChung = _cauHinhService.GetSetting("CauHinhTiemChung.NhomDichVuTiemChung");
            var nhomTiemChungId = cauHinhNhomTiemChung != null ? long.Parse(cauHinhNhomTiemChung.Value) : (long?)null;
            return nhomTiemChungId;
        }
        [HttpPost("GetDuocPhamVaVatTuDinhMucAsync")]
        public async Task<List<DuocPhamVaVatTuTNhaThuocTemplateVo>> GetDuocPhamVaVatTuDinhMucAsync(DropDownListRequestModel queryInfo)
        {
            var listEnum = await _dichVuKyThuatBenhVienService.GetDuocPhamVaVatTuDinhMucAsync(queryInfo);
            return listEnum;
        }

        [HttpPost("GetDuocPhamVaVatTuDinhMucDVKTAsync")]
        public async Task<List<DuocPhamVaVatTuTNhaThuocTemplateVo>> GetDuocPhamVaVatTuDinhMucDVKTAsync(DropDownListRequestModel queryInfo, long? DuocPhamVTYTId)
        {
            var listEnum = await _dichVuKyThuatBenhVienService.GetDuocPhamVaVatTuDinhMucDVKTAsync(queryInfo, DuocPhamVTYTId);
            return listEnum;
        }

        [HttpPost("GetThongTinDuocPham")]
        public async Task<List<ThongTinDuocPhamQuayThuocVo>> GetThongTinDuocPham(long duocPhamId, long loaiDuocPhamHoacVatTu)
        {
            var result = await _dichVuKyThuatBenhVienService.GetThongTinDuocPham(duocPhamId, loaiDuocPhamHoacVatTu);
            return result;
        }

        [HttpPost("ExportDichVuKyThuatbenhVienCustom")]
        [ClaimRequirement(Enums.SecurityOperation.Process, Enums.DocumentType.DanhMucDichVuKyThuatTaiBenhVien)]
        public async Task<ActionResult> ExportDichVuKyThuatbenhVienCustom(QueryInfo queryInfo)
        {           
            queryInfo.Skip = 0;
            queryInfo.Take = int.MaxValue;
            var gridData = await _dichVuKyThuatBenhVienService.GetDataForGridAsync(queryInfo);
            var bytes = _dichVuKyThuatBenhVienService.ExportDichVuKyThuatBenhVien(gridData);

            HttpContext.Response.Headers.Add("content-disposition", "attachment; filename=DichVuKyThuatbenhVien" + DateTime.Now.Year + ".xls");
            Response.ContentType = "application/vnd.ms-excel";

            return new FileContentResult(bytes, "application/vnd.ms-excel");
        }
        [HttpPost("GetNhomDichVuTheoDichVuKyThuat")]
        public async Task<ActionResult> GetNhomDichVuTheoDichVuKyThuat([FromBody]DropDownListRequestModel queryInfo)
        {
            var lookup = await _dichVuKyThuatBenhVienService.GetNhomDichVuTheoDichVuKyThuat(queryInfo);
            return Ok(lookup);
        }

        #region BVHD-3937
        [HttpPost("DownloadFileExcelTemplateGiaDichVuBenhVien")]
        public ActionResult DownloadFileExcelTemplateGiaDichVuBenhVien()
        {
            var path = @"Resource\\GiaDichVuBenhVien.xlsx";
            System.IO.FileStream fs = new System.IO.FileStream(path, System.IO.FileMode.Open, System.IO.FileAccess.Read);
            System.IO.BinaryReader binaryReader = new System.IO.BinaryReader(fs);

            long byteLength = new System.IO.FileInfo(path).Length;
            var fileContent = binaryReader.ReadBytes((Int32)byteLength);

            HttpContext.Response.Headers.Add("content-disposition", "attachment; filename=GiaDichVuBenhVien.xlsx");
            Response.ContentType = "application/vnd.ms-excel";
            return new FileContentResult(fileContent, "application/vnd.ms-excel");
        }

        [HttpPost("ImportGiaDichVuBenhVien")]
        [ClaimRequirement(Enums.SecurityOperation.Add, Enums.DocumentType.DanhMucDichVuKyThuatTaiBenhVien)]
        public async Task<ActionResult> ImportGiaDichVuBenhVien(GiaDichVuBenhVienFileImportVo model)
        {
            var path = _taiLieuDinhKemService.GetObjectStream(model.DuongDan, model.TenGuid);
            model.Path = path;
            var result = await _dichVuKyThuatBenhVienService.XuLyKiemTraDataGiaDichVuBenhVienImportAsync(model);
            return Ok(result);
        }

        [HttpPost("KiemTraGiaDichVuBenhVienLoi")]
        [ClaimRequirement(Enums.SecurityOperation.Add, Enums.DocumentType.DanhMucDichVuKyThuatTaiBenhVien)]
        public async Task<ActionResult> KiemTraGiaDichVuBenhVienLoi(GiaDichVuCanKiemTraVo info)
        {
            var result = new GiaDichVuBenhVieDataImportVo();
            await _dichVuKyThuatBenhVienService.KiemTraDataGiaDichVuBenhVienImportAsync(info.datas, result);
            return Ok(result);
        }

        [HttpPost("XuLyLuuGiaDichVuImport")]
        [ClaimRequirement(Enums.SecurityOperation.Add, Enums.DocumentType.DanhMucDichVuKyThuatTaiBenhVien)]
        public async Task<ActionResult> XuLyLuuGiaDichVuImport(GiaDichVuCanKiemTraVo info)
        {
            await _dichVuKyThuatBenhVienService.XuLyLuuGiaDichVuImportAsync(info.datas);
            return Ok();
        }
        #endregion

        #region BVHD-3961
        [HttpPost("GetListKhoaPhong")]
        public async Task<ActionResult<ICollection<LookupItemVo>>> GetListKhoaPhong([FromBody]DropDownListRequestModel model)
        {
            var lookup = await _dichVuKyThuatBenhVienService.GetListKhoaPhongAll(model);
            return Ok(lookup);
        }
        #endregion
    }
}