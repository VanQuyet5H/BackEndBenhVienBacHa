using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Camino.Api.Auth;
using Camino.Api.Extensions;
using Camino.Api.Infrastructure.Auth;
using Camino.Api.Models.DichVuKhamBenh;
using Camino.Api.Models.DichVuKhamBenhBenhViens;
using Camino.Api.Models.Error;
using Camino.Core.Domain;
using Camino.Core.Domain.Entities.DichVuKhamBenhBenhViens;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.DichVuGiuong;
using Camino.Core.Domain.ValueObject.DichVuKhamBenh;
using Camino.Core.Domain.ValueObject.ExportExcelItemVo;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Services.DichVuKhamBenh;
using Camino.Services.DichVuKhamBenhBenhViens;
using Camino.Services.ExportImport;
using Camino.Services.Helpers;
using Camino.Services.KhoaPhong;
using Camino.Services.Localization;
using Camino.Services.TaiLieuDinhKem;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Camino.Api.Controllers
{
    public class DichVuKhamBenhBenhVienController : CaminoBaseController
    {
        private readonly IDichVuKhamBenhBenhVienService _dichVuKhamBenhService;
        private readonly ILocalizationService _localizationService;
        private readonly IDichVuKhamBenhService _dichVuKhamBenhKhacService;
        private readonly IKhoaPhongService _khoaPhongService;
        private readonly IExcelService _excelService;
        private readonly ITaiLieuDinhKemService _taiLieuDinhKemService;
        public DichVuKhamBenhBenhVienController(IKhoaPhongService khoaPhongService, ILocalizationService localizationService, IJwtFactory iJwtFactory,
            IDichVuKhamBenhBenhVienService dichVuKhamBenhService, IDichVuKhamBenhService dichVuKhamBenhKhacService, IExcelService excelService,
            ITaiLieuDinhKemService taiLieuDinhKemService)
        {
            _dichVuKhamBenhKhacService = dichVuKhamBenhKhacService;
            _localizationService = localizationService;
            _dichVuKhamBenhService = dichVuKhamBenhService;
            _khoaPhongService = khoaPhongService;
            _excelService = excelService;
            _taiLieuDinhKemService = taiLieuDinhKemService;
        }
        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetDataForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DanhMucDichVuKhamBenhTaiBenhVien)]
        public async Task<ActionResult<GridDataSource>> GetDataForGridAsync([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _dichVuKhamBenhService.GetDataForGridAsync(queryInfo);
            return Ok(gridData);
        }
        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetTotalPageForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DanhMucDichVuKhamBenhTaiBenhVien)]
        public async Task<ActionResult<GridDataSource>> GetTotalPageForGridAsync([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _dichVuKhamBenhService.GetTotalPageForGridAsync(queryInfo);
            return Ok(gridData);
        }
        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetDataForGridChildAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DanhMucDichVuKhamBenhTaiBenhVien)]
        public async Task<ActionResult<GridDataSource>> GetDataForGridChildAsync([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _dichVuKhamBenhService.GetDataForGridChildAsync(queryInfo);
            return Ok(gridData);
        }
        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetTotalPageForGridChildAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DanhMucDichVuKhamBenhTaiBenhVien)]
        public async Task<ActionResult<GridDataSource>> GetTotalPageForGridChildAsync([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _dichVuKhamBenhService.GetTotalPageForGridChildAsync(queryInfo);
            return Ok(gridData);
        }
        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetDataForGridChildGiaBenhVienAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DanhMucDichVuKhamBenhTaiBenhVien)]
        public async Task<ActionResult<GridDataSource>> GetDataForGridChildGiaBenhVienAsync([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _dichVuKhamBenhService.GetDataForGridChildGiaBenhVienAsync(queryInfo);
            return Ok(gridData);
        }
        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetTotalPageForGridChildGiaBenhVienAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DanhMucDichVuKhamBenhTaiBenhVien)]
        public async Task<ActionResult<GridDataSource>> GetTotalPageForGridChildGiaBenhVienAsync([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _dichVuKhamBenhService.GetTotalPageForGridChildGiaBenhVienAsync(queryInfo);
            return Ok(gridData);
        }
        [HttpDelete("{id}")]
        [ClaimRequirement(Enums.SecurityOperation.Delete, Enums.DocumentType.DanhMucDichVuKhamBenhTaiBenhVien)]
        public async Task<ActionResult> Delete(long id)
        {
            var dichVuKhamBenh = await _dichVuKhamBenhService.GetByIdAsync(id, p => p.Include(x => x.DichVuKhamBenhBenhVienGiaBaoHiems)
                .Include(x => x.DichVuKhamBenhBenhVienGiaBenhViens)
                .Include(x => x.DichVuKhamBenhBenhVienNoiThucHiens));
            if (dichVuKhamBenh == null)
            {
                return NotFound();
            }
            await _dichVuKhamBenhService.DeleteByIdAsync(id);
            return NoContent();
        }
        [HttpPost("GetDichVuKhamBenh")]
        public async Task<ActionResult> GetDichVuKhamBenh([FromBody]DropDownListRequestModel queryInfo)
        {
            var lookup = await _dichVuKhamBenhService.GetDichVuKhamBenh(queryInfo);
            return Ok(lookup);
        }
        [HttpPost("GetKhoaPhong")]
        public async Task<ActionResult> GetKhoaPhong([FromBody]DropDownListRequestModel queryInfo)
        {
            var lookup = await _dichVuKhamBenhService.GetKhoaPhong(queryInfo);
            return Ok(lookup);
        }

        [HttpPost("GetPhongBenhVienDichVuKhamBenh")]
        public async Task<ActionResult> GetPhongBenhVienDichVuKhamBenh([FromBody]DropDownListRequestModel queryInfo)
        {
            var lookup = await _dichVuKhamBenhService.GetPhongBenhVienDichVuKhamBenh(queryInfo);
            return Ok(lookup);
        }

        [HttpPost("GetDichVuKhamBenhById")]
        public async Task<ActionResult> GetDichVuKhamBenhById([FromBody]DropDownListRequestModel queryInfo, long id)
        {
            if (id != 0)
            {
                var khoaPhong = await _dichVuKhamBenhService.GetIdKhoaPhongFromRequestDropDownList(queryInfo);
                var lookup = await _dichVuKhamBenhService.GetDichVuKhamBenhById(queryInfo, id, khoaPhong);
                return Ok(lookup);
            }
            return Ok();
        }
        [HttpPost("GetNhomDichVu")]
        public async Task<ActionResult> GetNhomDichVu()
        {
            var lookup = await _dichVuKhamBenhService.GetNhomDichVu();
            return Ok(lookup);
        }

        [HttpPost("GetNhomDichVuTheoDichVuKhamBenh")]
        public async Task<ActionResult> GetNhomDichVuTheoDichVuKhamBenh([FromBody]DropDownListRequestModel queryInfo)
        {
            var lookup = await _dichVuKhamBenhService.GetNhomDichVuTheoDichVuKhamBenh(queryInfo);
            return Ok(lookup);
        }

        [HttpGet("{id}")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DanhMucDichVuKhamBenhTaiBenhVien)]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<ActionResult<DichVuKhamBenhBenhVienViewModel>> Get(long id)
        {
            var result = await _dichVuKhamBenhService.GetByIdAsync(id, p => p.Include(s => s.DichVuKhamBenh)
                .Include(x => x.DichVuKhamBenhBenhVienGiaBaoHiems)
                .Include(x => x.DichVuKhamBenhBenhVienGiaBenhViens).ThenInclude(x => x.NhomGiaDichVuKhamBenhBenhVien)
                .Include(x => x.DichVuKhamBenhBenhVienNoiThucHiens).ThenInclude(y => y.PhongBenhVien));
            if (result == null)
                return NotFound();
            var resultData = result.ToModel<DichVuKhamBenhBenhVienViewModel>();

            foreach (var item in resultData.DichVuKhamBenhBenhVienGiaBenhViens)
            {
                item.NhomGiaDichVuKhamBenhBenhVienText = item.NhomGiaDichVuKhamBenhBenhVien.Ten;
            }

            if (resultData.DichVuKhamBenhBenhVienGiaBenhViens != null && resultData.DichVuKhamBenhBenhVienGiaBenhViens.Count > 1)
            {
                var nhomdichVu = await _dichVuKhamBenhService.GetNhomGiaDichVuKhamBenhBenhVien();
                for (int j = 0; j < nhomdichVu.Count; j++)
                {
                    var giaCungLoais = resultData.DichVuKhamBenhBenhVienGiaBenhViens.Where(x => x.NhomGiaDichVuKhamBenhBenhVienId == nhomdichVu[j].Id).ToList();
                    if (giaCungLoais.Count > 1)
                    {
                        for (var i = 0; i < giaCungLoais.Count-1; i++)
                        {
                            giaCungLoais[i].DenNgayRequired = true;
                        }
                    }
                }
            }
            if (resultData.DichVuKhamBenhBenhVienGiaBaoHiems != null && resultData.DichVuKhamBenhBenhVienGiaBaoHiems.Count > 1)
            {
                var giaCungLoais = resultData.DichVuKhamBenhBenhVienGiaBaoHiems.ToList();
                if (giaCungLoais.Count > 1)
                {
                    // bỏ vị trí cuối cùng
                    for (var i = 0; i < giaCungLoais.Count - 1; i++)
                    {
                        giaCungLoais[i].DenNgayRequired = true;
                    }
                }
            }

            return Ok(resultData);
        }
        [HttpPost]
        [ClaimRequirement(Enums.SecurityOperation.Add, Enums.DocumentType.DanhMucDichVuKhamBenhTaiBenhVien)]
        public async Task<ActionResult> Post([FromBody] DichVuKhamBenhBenhVienViewModel chucVuViewModel)
        {

            //if (_dichVuKhamBenhService.CheckExistDichVuKhamBenhBenhVien(Convert.ToInt64(chucVuViewModel.DichVuKhamBenhId)) != null)
            //{
            //    throw new ApiException(
            //                               _localizationService.GetResource("DichVuKhamBenhBenhVien.HaveExist"));
            //}
           
            if (chucVuViewModel.DichVuKhamBenhBenhVienGiaBaoHiems.Count > 0)
            {
                for (int i = 1; i < chucVuViewModel.DichVuKhamBenhBenhVienGiaBaoHiems.Count; i++)
                {

                   // chucVuViewModel.DichVuKhamBenhBenhVienGiaBaoHiems[i - 1].DenNgay = Convert.ToDateTime(chucVuViewModel.DichVuKhamBenhBenhVienGiaBaoHiems[i].TuNgay).Date.AddDays(-1);

                    //if (chucVuViewModel.DichVuKhamBenhBenhVienGiaBaoHiems[i - 1].DenNgay != null) {
                    //    if (chucVuViewModel.DichVuKhamBenhBenhVienGiaBaoHiems[i - 1].TuNgay == chucVuViewModel.DichVuKhamBenhBenhVienGiaBaoHiems[i - 1].DenNgay)
                    //    {
                    //        throw new ApiException(
                    //                        _localizationService.GetResource("DichVuKhamBenhBenhVien.DichVuKhamBenhBenhVienGiaBaoHiems.TuNgay"));
                    //    }
                    //}
                    if (
                         chucVuViewModel.DichVuKhamBenhBenhVienGiaBaoHiems[i - 1].TuNgay > chucVuViewModel.DichVuKhamBenhBenhVienGiaBaoHiems[i - 1].DenNgay ||
                         chucVuViewModel.DichVuKhamBenhBenhVienGiaBaoHiems[i].TuNgay > chucVuViewModel.DichVuKhamBenhBenhVienGiaBaoHiems[i].DenNgay)
                    {
                        throw new ApiException(
                                            _localizationService.GetResource("DichVuKhamBenhBenhVien.DichVuKhamBenhBenhVienGiaBaoHiems.TuNgay"));
                    }
                    if(chucVuViewModel.DichVuKhamBenhBenhVienGiaBaoHiems[i].TuNgay < Convert.ToDateTime(chucVuViewModel.DichVuKhamBenhBenhVienGiaBaoHiems[i - 1].TuNgay).AddDays(1))
                    {
                        throw new ApiException(
                                                  _localizationService.GetResource("DichVuKhamBenhBenhVien.DichVuKhamBenhBenhVienGiaBenhViens.TuNgayLonHonDenNgayDongTruoc"));
                    }

                }
            }
            #region nam update 24062021
            //Hiện tại mình đã có field đến ngày nhưng field này chỉ hiển thị khi có 2 dòng cùng loại giá.
            //Giờ mình cho nó hiển thị ra luôn và ko disable để cho nó có thể sửa 
            //nhưng mình vẫn phải validate là từ ngày của dòng sau > đến ngày của dòng trước +1(cùng loại giá)
            // bổ sung 13102021
            // nếu giá bệnh viện  chỉ có 1 dòng  thì đến ngày dc null => hsd đến ngày là vô thời hạn 
            // nếu giá bệnh viện  có 2 dòng trở lên thì validate đến ngày dòng 1
            // + lấy từ ngày dòng 2 đem lên đến ngày dòng 1 đối với trường hợp  đến ngày dòng 1 null , còn nếu  trường hợp đến ngày dòng 1 có kiểm tra từ ngày dòng 2 lớn hơn <= đến ngày dòng 1
            // + dòng 2 đến ngày k validate
            // dòng 3 thì tương tự
            if (chucVuViewModel.DichVuKhamBenhBenhVienGiaBenhViens.Count > 0)
            {
                var nhomdichVu = await _dichVuKhamBenhService.GetNhomGiaDichVuKhamBenhBenhVien();
                for (int j = 0; j < nhomdichVu.Count; j++)
                {
                    var benhvien = chucVuViewModel.DichVuKhamBenhBenhVienGiaBenhViens.Where(x => x.NhomGiaDichVuKhamBenhBenhVienId == nhomdichVu[j].Id ).ToList();
                    for (int i = 0; i < benhvien.Count; i++)
                    {
                        var value = ValidateCungDong(benhvien[i].TuNgay, benhvien[i].DenNgay);
                        if (value == true)
                        {
                            throw new ApiException(
                                                 _localizationService.GetResource("DichVuKhamBenhBenhVien.DichVuKhamBenhBenhVienGiaBenhViens.TuNgay"));
                        }
                        if (i != 0)
                        {
                            var valueKhacDong = ValidateDongTren(benhvien[i].TuNgay, benhvien[i - 1].DenNgay);
                            if (valueKhacDong == true)
                            {
                                throw new ApiException(
                                                      _localizationService.GetResource("DichVuKhamBenhBenhVien.DichVuKhamBenhBenhVienGiaBenhViens.TuNgayLonHonDenNgayDongTruoc"));
                            }

                        }
                        //if (i < benhvien.Count)
                        //{
                        //    if (benhvien[i].TuNgay <= Convert.ToDateTime(benhvien[i - 1].DenNgay))
                        //    {
                        //        throw new ApiException(
                        //                            _localizationService.GetResource("DichVuKhamBenhBenhVien.DichVuKhamBenhBenhVienGiaBenhViens.TuNgayLonHonDenNgayDongTruoc"));
                        //    }
                        //    if (benhvien[i - 1].TuNgay >= benhvien[i - 1].DenNgay ||
                        //   benhvien[i].TuNgay >= benhvien[i].DenNgay)
                        //    {
                        //        throw new ApiException(
                        //                            _localizationService.GetResource("DichVuKhamBenhBenhVien.DichVuKhamBenhBenhVienGiaBenhViens.TuNgay"));
                        //    }
                        //}

                    }
                }

            }
            //Hiện tại cho giá bệnh viện nhâp 0 nên không cho nhập giá bhyt
            var kiemTraGiaBenhVien = chucVuViewModel.DichVuKhamBenhBenhVienGiaBenhViens.Any(c => c.TuNgay.Value.Date <= DateTime.Now.Date && c.DenNgay?.Date >= DateTime.Now.Date && c.Gia == 0);
            var kiemTraGiaBHYT = chucVuViewModel.DichVuKhamBenhBenhVienGiaBaoHiems.Any();
            if (kiemTraGiaBenhVien && kiemTraGiaBHYT)
            {
                throw new ApiException(_localizationService.GetResource("Common.KhongNhapBHYT"));
            }


            //Hiện tại cho giá bệnh viện không được nhỏ hơn giá BHYT
            var giabenhViens = chucVuViewModel.DichVuKhamBenhBenhVienGiaBenhViens.Where(c => c.TuNgay.Value.Date <= DateTime.Now.Date && c.DenNgay?.Date >= DateTime.Now.Date && c.Gia != 0);
            var giaBHYTs = chucVuViewModel.DichVuKhamBenhBenhVienGiaBaoHiems.Where(c => c.TuNgay.Value.Date <= DateTime.Now.Date && c.DenNgay?.Date >= DateTime.Now.Date && c.Gia != 0);

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
            var entity = chucVuViewModel.ToEntity<DichVuKhamBenhBenhVien>();
            //await _dichVuKhamBenhService.AddAndUpdateLastEntity(Convert.ToDateTime(chucVuViewModel.DichVuKhamBenhBenhVienGiaBaoHiems.FirstOrDefault().TuNgay),entity.DichVuKhamBenhBenhVienGiaBenhViens);


            // xử lý kiểm tra giá BH so với giá BV
            var lstGiaBaoHiem = entity.DichVuKhamBenhBenhVienGiaBaoHiems.Where(x => x.WillDelete != true).ToList();
            var lstGiaBenhVien = entity.DichVuKhamBenhBenhVienGiaBenhViens.Where(x => x.WillDelete != true).ToList();
            CheckValidGiaBaoHiem(lstGiaBaoHiem, lstGiaBenhVien);
            await _dichVuKhamBenhService.AddAsync(entity);

            //var result = entity.ToModel<DichVuKhamBenhBenhVienViewModel>();
            //return CreatedAtAction(nameof(Get), new { id = entity.Id }, result);
            return NoContent();
        }

        [HttpPut]
        [ClaimRequirement(Enums.SecurityOperation.Update, Enums.DocumentType.DanhMucDichVuKhamBenhTaiBenhVien)]
        public async Task<ActionResult> Put([FromBody] DichVuKhamBenhBenhVienViewModel viewModel)
        {
            if (viewModel.DichVuKhamBenhBenhVienGiaBaoHiems.Count > 0)
            {
                //for (int i = 1; i < viewModel.DichVuKhamBenhBenhVienGiaBaoHiems.Count; i++)
                //{
                //    viewModel.DichVuKhamBenhBenhVienGiaBaoHiems.ToList()[i - 1].DenNgay = Convert.ToDateTime(viewModel.DichVuKhamBenhBenhVienGiaBaoHiems.ToList()[i].TuNgay).AddDays(-1);
                //}
                for (int i = 1; i < viewModel.DichVuKhamBenhBenhVienGiaBaoHiems.Count; i++)
                {

                   // viewModel.DichVuKhamBenhBenhVienGiaBaoHiems[i - 1].DenNgay = Convert.ToDateTime(viewModel.DichVuKhamBenhBenhVienGiaBaoHiems[i].TuNgay).Date.AddDays(-1);

                    if (
                         viewModel.DichVuKhamBenhBenhVienGiaBaoHiems[i - 1].TuNgay > viewModel.DichVuKhamBenhBenhVienGiaBaoHiems[i - 1].DenNgay ||
                         viewModel.DichVuKhamBenhBenhVienGiaBaoHiems[i].TuNgay > viewModel.DichVuKhamBenhBenhVienGiaBaoHiems[i].DenNgay)
                    {
                        throw new ApiException(
                                            _localizationService.GetResource("DichVuKhamBenhBenhVien.DichVuKhamBenhBenhVienGiaBaoHiems.TuNgay"));
                    }
                    if (viewModel.DichVuKhamBenhBenhVienGiaBaoHiems[i].TuNgay < Convert.ToDateTime(viewModel.DichVuKhamBenhBenhVienGiaBaoHiems[i - 1].DenNgay))
                    {
                        throw new ApiException(
                                                  _localizationService.GetResource("DichVuKhamBenhBenhVien.DichVuKhamBenhBenhVienGiaBenhViens.TuNgayLonHonDenNgayDongTruoc"));
                    }
                }
            }
           
            #region nam update 24062021
            //Hiện tại mình đã có field đến ngày nhưng field này chỉ hiển thị khi có 2 dòng cùng loại giá.
            //Giờ mình cho nó hiển thị ra luôn và ko disable để cho nó có thể sửa 
            //nhưng mình vẫn phải validate là từ ngày của dòng sau > đến ngày của dòng trước +1(cùng loại giá)
            // bổ sung 13102021
            // nếu giá bệnh viện  chỉ có 1 dòng  thì đến ngày dc null => hsd đến ngày là vô thời hạn 
            // nếu giá bệnh viện  có 2 dòng trở lên thì validate đến ngày dòng 1
            // + lấy từ ngày dòng 2 đem lên đến ngày dòng 1 đối với trường hợp  đến ngày dòng 1 null , còn nếu  trường hợp đến ngày dòng 1 có kiểm tra từ ngày dòng 2 lớn hơn <= đến ngày dòng 1
            // + dòng 2 đến ngày k validate
            // dòng 3 thì tương tự
            if (viewModel.DichVuKhamBenhBenhVienGiaBenhViens.Count > 0)
            {
                var nhomdichVu = await _dichVuKhamBenhService.GetNhomGiaDichVuKhamBenhBenhVien();
                for (int j = 0; j < nhomdichVu.Count; j++)
                {
                    var benhvien = viewModel.DichVuKhamBenhBenhVienGiaBenhViens.Where(x => x.NhomGiaDichVuKhamBenhBenhVienId == nhomdichVu[j].Id ).ToList();
                    for (int i = 0; i < benhvien.Count; i++)
                    {

                        var value = ValidateCungDong(benhvien[i].TuNgay,benhvien[i].DenNgay);
                        if(value == true)
                        {
                            throw new ApiException(
                                                 _localizationService.GetResource("DichVuKhamBenhBenhVien.DichVuKhamBenhBenhVienGiaBenhViens.TuNgay"));
                        }
                        if( i != 0)
                        {
                            var valueKhacDong = ValidateDongTren(benhvien[i].TuNgay, benhvien[i - 1].DenNgay);
                            if (valueKhacDong == true)
                            {
                                throw new ApiException(
                                                      _localizationService.GetResource("DichVuKhamBenhBenhVien.DichVuKhamBenhBenhVienGiaBenhViens.TuNgayLonHonDenNgayDongTruoc"));
                            }

                        }
                        
                        //if (i < benhvien.Count)
                        //{
                        //    if (benhvien[i].TuNgay <= Convert.ToDateTime(benhvien[i - 1].DenNgay))
                        //    {
                        //        throw new ApiException(
                        //                            _localizationService.GetResource("DichVuKhamBenhBenhVien.DichVuKhamBenhBenhVienGiaBenhViens.TuNgayLonHonDenNgayDongTruoc"));
                        //    }
                        //    if (benhvien[i - 1].TuNgay >= benhvien[i - 1].DenNgay ||
                        //   benhvien[i].TuNgay >= benhvien[i].DenNgay)
                        //    {
                        //        throw new ApiException(
                        //                            _localizationService.GetResource("DichVuKhamBenhBenhVien.DichVuKhamBenhBenhVienGiaBenhViens.TuNgay"));
                        //    }
                        //}

                    }
                }

            }
            #endregion
            var entity = await _dichVuKhamBenhService.GetByIdAsync(viewModel.Id, p => p.Include(x => x.DichVuKhamBenhBenhVienGiaBaoHiems)
                .Include(x => x.DichVuKhamBenhBenhVienGiaBenhViens)
                .Include(x => x.DichVuKhamBenhBenhVienNoiThucHiens));
            //if (_dichVuKhamBenhService.CheckExistDichVuKhamBenhBenhVien(Convert.ToInt64(viewModel.DichVuKhamBenhId)) != null && entity.DichVuKhamBenhId!= viewModel.DichVuKhamBenhId)
            //{
            //    throw new ApiException(
            //                               _localizationService.GetResource("DichVuKhamBenhBenhVien.HaveExist"));
            //}
            if (entity == null)
                return NotFound();

            viewModel.ToEntity(entity);
            //for (int i = 1; i < entity.DichVuKhamBenhBenhVienGiaBaoHiems.Count; i++)
            //{
            //    if (entity.DichVuKhamBenhBenhVienGiaBaoHiems.ToList()[i - 1].DenNgay == null)
            //    {
            //        entity.DichVuKhamBenhBenhVienGiaBaoHiems.ToList()[i - 1].DenNgay = entity.DichVuKhamBenhBenhVienGiaBaoHiems.ToList()[i].TuNgay.AddDays(-1);
            //    }
            //}
            //await _dichVuKhamBenhService.UpdateDayGiaBenhVienEntity(entity.DichVuKhamBenhBenhVienGiaBenhViens);

            // xử lý kiểm tra giá BH so với giá BV
            var lstGiaBaoHiem = entity.DichVuKhamBenhBenhVienGiaBaoHiems.Where(x => x.WillDelete != true).ToList();
            var lstGiaBenhVien = entity.DichVuKhamBenhBenhVienGiaBenhViens.Where(x => x.WillDelete != true).ToList();
            CheckValidGiaBaoHiem(lstGiaBaoHiem, lstGiaBenhVien);

            //update 20/06/2022: set về cuối ngày của đến ngày
            foreach(var giaBaoHiem in entity.DichVuKhamBenhBenhVienGiaBaoHiems)
            {
                if(giaBaoHiem.DenNgay != null)
                {
                    giaBaoHiem.DenNgay = giaBaoHiem.DenNgay.Value.Date.AddHours(23).AddMinutes(59).AddSeconds(59);
                }
            }
            foreach (var giaBenhVien in entity.DichVuKhamBenhBenhVienGiaBenhViens)
            {
                if (giaBenhVien.DenNgay != null)
                {
                    giaBenhVien.DenNgay = giaBenhVien.DenNgay.Value.Date.AddHours(23).AddMinutes(59).AddSeconds(59);
                }
            }

            await _dichVuKhamBenhService.UpdateAsync(entity);

            return NoContent();
        }

        private bool ValidateCungDong(DateTime? TuNgay , DateTime? denNgay)
        {
            if(TuNgay != null && denNgay != null)
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
        [HttpGet("GetThongTinDichVuKhamBenh")]
        public async Task<ActionResult<DichVuKhamBenhViewModel>> GetThongTinDichVuKhamBenhAsync(long dichVuKhamBenhId)
        {
            var result = await _dichVuKhamBenhKhacService.GetByIdAsync(dichVuKhamBenhId);
            if (result == null)
            {
                return NotFound();
            }

            var viewModel = result.ToModel<DichVuKhamBenhViewModel>();
            return Ok(viewModel);
        }


        private void CheckValidGiaBaoHiem(ICollection<DichVuKhamBenhBenhVienGiaBaoHiem> lstGiaBaoHiem, ICollection<DichVuKhamBenhBenhVienGiaBenhVien> lstGiaBenhVien)
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
                    //                            throw new ApiException(_localizationService.GetResource("DichVuKhamBenhBenhVien.GiaBaoHiem.RangeValueByTime"));
                    //                        }
                    //                    }
                }
            }
        }

        [HttpPost("ExportDichVuKhamBenhBenhVien")]
        [ClaimRequirement(Enums.SecurityOperation.Process, Enums.DocumentType.DanhMucDichVuKhamBenhTaiBenhVien)]
        public async Task<ActionResult> ExportDichVuKhamBenhBenhVienAsync(QueryInfo queryInfo)
        {
            // todo: hardcode max row export excel
            queryInfo.Skip = 0;
            queryInfo.Take = 20000;

            var gridData = await _dichVuKhamBenhService.GetDataForGridAsync(queryInfo);
            var dichVuKhamBenhBenhVienData = gridData.Data.Select(p => (DichVuKhamBenhBenhVienVO)p).ToList();
            var dataExcel = dichVuKhamBenhBenhVienData.Map<List<DichVuKhamBenhBenhVienExportExcel>>();

            var lstValueObject = new List<(string, string)>();
            lstValueObject.Add((nameof(DichVuKhamBenhBenhVienExportExcel.Ma), "Mã"));
            lstValueObject.Add((nameof(DichVuKhamBenhBenhVienExportExcel.MaTT37), "Mã TT37"));
            lstValueObject.Add((nameof(DichVuKhamBenhBenhVienExportExcel.HangBenhVien), "Hạng Bệnh Viện"));
            lstValueObject.Add((nameof(DichVuKhamBenhBenhVienExportExcel.Ten), "Tên"));
            lstValueObject.Add((nameof(DichVuKhamBenhBenhVienExportExcel.TenNoiThucHien), "Nơi Thực Hiện"));
            lstValueObject.Add((nameof(DichVuKhamBenhBenhVienExportExcel.MoTa), "Mô Tả"));
            lstValueObject.Add((nameof(DichVuKhamBenhBenhVienExportExcel.HieuLucHienThi), "Hiệu lực"));

            var bytes = _excelService.ExportManagermentView(dataExcel, lstValueObject, "Dịch vụ khám bệnh bệnh viện");

            HttpContext.Response.Headers.Add("content-disposition", "attachment; filename=DichVuKhamBenhBenhVien" + DateTime.Now.Year + ".xls");
            Response.ContentType = "application/vnd.ms-excel";

            return new FileContentResult(bytes, "application/vnd.ms-excel");
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
        [ClaimRequirement(Enums.SecurityOperation.Add, Enums.DocumentType.DanhMucDichVuKhamBenhBenhVien)]
        public async Task<ActionResult> ImportGiaDichVuBenhVien(GiaDichVuBenhVienFileImportVo model)
        {
            var path = _taiLieuDinhKemService.GetObjectStream(model.DuongDan, model.TenGuid);
            model.Path = path;
            var result = await _dichVuKhamBenhService.XuLyKiemTraDataGiaDichVuBenhVienImportAsync(model);
            return Ok(result);
        }

        [HttpPost("KiemTraGiaDichVuBenhVienLoi")]
        [ClaimRequirement(Enums.SecurityOperation.Add, Enums.DocumentType.DanhMucDichVuKhamBenhBenhVien)]
        public async Task<ActionResult> KiemTraGiaDichVuBenhVienLoi(GiaDichVuCanKiemTraVo info)
        {
            var result = new GiaDichVuBenhVieDataImportVo();
            await _dichVuKhamBenhService.KiemTraDataGiaDichVuBenhVienImportAsync(info.datas, result);
            return Ok(result);
        }

        [HttpPost("XuLyLuuGiaDichVuImport")]
        [ClaimRequirement(Enums.SecurityOperation.Add, Enums.DocumentType.DanhMucDichVuKhamBenhBenhVien)]
        public async Task<ActionResult> XuLyLuuGiaDichVuImport(GiaDichVuCanKiemTraVo info)
        {
            await _dichVuKhamBenhService.XuLyLuuGiaDichVuImportAsync(info.datas);
            return Ok();
        }
        #endregion
    }
}