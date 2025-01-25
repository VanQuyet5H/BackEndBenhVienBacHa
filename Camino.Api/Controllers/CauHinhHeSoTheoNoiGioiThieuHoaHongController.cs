using Camino.Api.Auth;
using Camino.Api.Extensions;
using Camino.Api.Models.CauhinhHeSoTheoNoiGioiThieuHoaHong;
using Camino.Core.Domain;
using Camino.Core.Domain.Entities.NoiGioiThieu;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.CauHinhHeSoTheoNoiGioiThieuHoaHong;
using Camino.Core.Domain.ValueObject.ExportExcelItemVo;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Helpers;
using Camino.Services.CauHinhHeSoTheoNoiGioiThieuHoaHong;
using Camino.Services.ExportImport;
using Camino.Services.Helpers;
using Camino.Services.Localization;
using Camino.Services.TaiLieuDinhKem;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static Camino.Core.Domain.Enums;

namespace Camino.Api.Controllers
{
    public class CauHinhHeSoTheoNoiGioiThieuHoaHongController : CaminoBaseController
    {
        private readonly ICauHinhHeSoTheoNoiGioiThieuHoaHongService _cauHinhHeSoTheoNoiGioiThieuHoaHongService;
        private readonly ILocalizationService _localizationService;
        private readonly IExcelService _excelService;
        private readonly ITaiLieuDinhKemService _taiLieuDinhKemService;

        public CauHinhHeSoTheoNoiGioiThieuHoaHongController(ILocalizationService localizationService,
            IExcelService excelService,
            ICauHinhHeSoTheoNoiGioiThieuHoaHongService cauHinhHeSoTheoNoiGioiThieuHoaHongService,
            ITaiLieuDinhKemService taiLieuDinhKemService
            )
        {
            _localizationService = localizationService;
            _excelService = excelService;
            _cauHinhHeSoTheoNoiGioiThieuHoaHongService = cauHinhHeSoTheoNoiGioiThieuHoaHongService;
            _taiLieuDinhKemService = taiLieuDinhKemService;
        }
        [HttpPost("ExportNoiGioiThieu")]
        [ClaimRequirement(Enums.SecurityOperation.Process, Enums.DocumentType.DanhMucNoiGioiThieu)]
        public async Task<ActionResult> ExportNoiGioiThieu(QueryInfo queryInfo)
        {
            var gridData = await _cauHinhHeSoTheoNoiGioiThieuHoaHongService.GetDataForGridAsync(queryInfo, true);
            var noiGioiThieuData = gridData.Data.Select(p => (CauHinhHeSoTheoNoiGioiThieuHoaHongGridVo)p).ToList();
            var excelData = noiGioiThieuData.Map<List<CauHinhNoiGioiThieuVaHoaHongExportExcel>>();

            var lstValueObject = new List<(string, string)>();
            lstValueObject.Add((nameof(CauHinhNoiGioiThieuVaHoaHongExportExcel.Ten), "Tên"));
            lstValueObject.Add((nameof(CauHinhNoiGioiThieuVaHoaHongExportExcel.DonVi), "Đơn vị"));
            lstValueObject.Add((nameof(CauHinhNoiGioiThieuVaHoaHongExportExcel.Sdt), "Số điện thoại"));
            lstValueObject.Add((nameof(CauHinhNoiGioiThieuVaHoaHongExportExcel.MoTa), "Mô tả"));

            var bytes = _excelService.ExportManagermentView(excelData, lstValueObject, "Nơi giới thiệu");

            HttpContext.Response.Headers.Add("content-disposition", "attachment; filename=NoiGioiThieu" + DateTime.Now.Year + ".xls");
            Response.ContentType = "application/vnd.ms-excel";

            return new FileContentResult(bytes, "application/vnd.ms-excel");
        }
        [HttpPost("GetDataForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DanhMucCauHinhHeSoTheoNoiGioiThieuHoaHong)]
        public async Task<ActionResult<GridDataSource>> GetDataForGridAsync([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _cauHinhHeSoTheoNoiGioiThieuHoaHongService.GetDataForGridAsync(queryInfo,false);
            return Ok(gridData);
        }

        //[ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetTotalPageForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DanhMucCauHinhHeSoTheoNoiGioiThieuHoaHong)]
        public async Task<ActionResult<GridDataSource>> GetTotalPageForGridAsync([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _cauHinhHeSoTheoNoiGioiThieuHoaHongService.GetTotalPageForGridAsync(queryInfo);
            return Ok(gridData);
        }


        [HttpPost("GetNoiGioiThieu")]
        public async Task<ActionResult<ICollection<LookupItemCauHinhHeSoTheoNoiGioiThieuHoaHongVo>>> GetNoiGioiThieu([FromBody]DropDownListRequestModel queryInfo)
        {
            var lookup = await _cauHinhHeSoTheoNoiGioiThieuHoaHongService.GetNoiGioiThieu(queryInfo);
            return Ok(lookup);
        }
        [HttpPost("GetNoiGioiThieuHopDongs")]
        public async Task<ActionResult<ICollection<LookupItemVo>>> GetNoiGioiThieuHopDongs([FromBody]DropDownListRequestModel queryInfo,long? id)
        {
            // id laf noi gioi thieu id

            var lookup = await _cauHinhHeSoTheoNoiGioiThieuHoaHongService.GetNoiGioiThieuHopDong(queryInfo,id);
            return Ok(lookup);
        }
        [HttpPost("GetNoiGioiThieuHopDongAdds")]
        public async Task<ActionResult<ICollection<LookupItemVo>>> GetNoiGioiThieuHopDongAdds([FromBody]DropDownListRequestModel queryInfo, long? id)
        {
            // id laf noi gioi thieu id

            var lookup = await _cauHinhHeSoTheoNoiGioiThieuHoaHongService.GetNoiGioiThieuHopDongAdd(queryInfo, id);
            return Ok(lookup);
        }

        [HttpPost("GetNoiGioiThieuHopDongAddHoaHongs")]
        public async Task<ActionResult<ICollection<LookupItemVo>>> GetNoiGioiThieuHopDongAddHoaHongs([FromBody]DropDownListRequestModel queryInfo, long? id)
        {
            // id laf noi gioi thieu id

            var lookup = await _cauHinhHeSoTheoNoiGioiThieuHoaHongService.GetNoiGioiThieuHopDongAddHoHong(queryInfo, id);
            return Ok(lookup);
        }


        [HttpPost("GetDataNoiGioiThieuHopDongForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DanhMucCauHinhHeSoTheoNoiGioiThieuHoaHong)]
        public async Task<ActionResult<GridDataSource>> GetDataNoiGioiThieuHopDongForGridAsync([FromBody]NoiGioiThieuHopDongQueryInfo queryInfo)
        {
            var gridData = await _cauHinhHeSoTheoNoiGioiThieuHoaHongService.GetDataNoiGioiThieuHopDongForGridAsync(queryInfo);
            return Ok(gridData);
        }

        #region save--  tạo nơi giới thiệu hợp đồng
        [HttpPost("SaveNoiGioiThieuHopDong")]
        [ClaimRequirement(Enums.SecurityOperation.Add, Enums.DocumentType.DanhMucCauHinhHeSoTheoNoiGioiThieuHoaHong)]
        public async Task<ActionResult<NoiGioiThieuHopDongVo>> Post([FromBody] NoiGioiThieuHopDongViewModel noiGioiThieuHopDongViewModel)
        {
            var noiGioiThieuHopDong = noiGioiThieuHopDongViewModel.ToEntity<NoiGioiThieuHopDong>();

            var data = await _cauHinhHeSoTheoNoiGioiThieuHoaHongService.XuLyThemNoiGioiThieuHopDongAsync(noiGioiThieuHopDong);

            return Ok(data);
        }

        [HttpGet("GetNoiGioiThieuHopDong")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DanhMucCauHinhHeSoTheoNoiGioiThieuHoaHong)]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<ActionResult<NoiGioiThieuHopDongViewModel>> Get(long id)
        {
            var result = await _cauHinhHeSoTheoNoiGioiThieuHoaHongService.GetByIdNoiGioiThieuHopDongAsync(id);
            if (result == null)
            {
                return NotFound();
            }
            var resultData = result.ToModel<NoiGioiThieuHopDongViewModel>();

            return Ok(resultData);
        }
        //Update
        [HttpPut("CapNhatNoiGioiThieuHopDong")]
        [ClaimRequirement(Enums.SecurityOperation.Update, Enums.DocumentType.DanhMucCauHinhHeSoTheoNoiGioiThieuHoaHong)]
        public async Task<ActionResult> Put([FromBody] NoiGioiThieuHopDongViewModel noiGioiThieuHopDongViewModel)
        {
            var result = await _cauHinhHeSoTheoNoiGioiThieuHoaHongService.GetByIdNoiGioiThieuHopDongAsync(noiGioiThieuHopDongViewModel.Id);
            if (result == null)
            {
                return NotFound();
            }
            var entity = noiGioiThieuHopDongViewModel.ToEntity(result);

            var data = await _cauHinhHeSoTheoNoiGioiThieuHoaHongService.XuLyCapNhatNoiGioiThieuHopDongAsync(entity);

            return Ok(data);
        }
        ////Delete
        [HttpDelete("DeleteNoiGioiThieuHopDong")]
        [ClaimRequirement(Enums.SecurityOperation.Delete, Enums.DocumentType.DanhMucChucDanh)]
        public async Task<ActionResult> Delete(long id)
        {
            var noiDung = await _cauHinhHeSoTheoNoiGioiThieuHoaHongService.GetByIdNoiGioiThieuHopDongAsync(id);

            noiDung.WillDelete = true;

            await _cauHinhHeSoTheoNoiGioiThieuHoaHongService.XuLyDeleteNoiGioiThieuHopDongAsync(noiDung);
            return NoContent();
        }
        #endregion

        #region lookup
        [HttpPost("GetDichVuKyThuat")]
        public async Task<ActionResult> GetDichVuKyThuat([FromBody]DropDownListRequestModel queryInfo)
        {
            var lookup = await _cauHinhHeSoTheoNoiGioiThieuHoaHongService.GetDichVuKyThuat(queryInfo);
            return Ok(lookup);
        }

        [HttpPost("GetDichVuGiuong")]
        public async Task<ActionResult> GetDichVuGiuong([FromBody]DropDownListRequestModel queryInfo)
        {
            var lookup = await _cauHinhHeSoTheoNoiGioiThieuHoaHongService.GetDichVuGiuong(queryInfo);
            return Ok(lookup);
        }

        [HttpPost("GetDichVuKham")]
        public async Task<ActionResult> GetDichVuKham([FromBody]DropDownListRequestModel queryInfo)
        {
            var lookup = await _cauHinhHeSoTheoNoiGioiThieuHoaHongService.GetDichVuKham(queryInfo);
            return Ok(lookup);
        }

        [HttpPost("GetDuocPham")]
        public async Task<ActionResult> GetDuocPham([FromBody]DropDownListRequestModel queryInfo)
        {
            var lookup = await _cauHinhHeSoTheoNoiGioiThieuHoaHongService.GetDuocPham(queryInfo);
            return Ok(lookup);
        }

        [HttpPost("GetVatTu")]
        public async Task<ActionResult> GetVatTu([FromBody]DropDownListRequestModel queryInfo)
        {
            var lookup = await _cauHinhHeSoTheoNoiGioiThieuHoaHongService.GetVatTu(queryInfo);
            return Ok(lookup);
        }

        [HttpPost("LoaiGiaHieuLucTheoDichVuKham")]
        public async Task<ActionResult<ICollection<LookupItemVo>>> LoaiGiaHieuLucTheoDichVuKham(DropDownListRequestModel model)
        {
            var lookup = await _cauHinhHeSoTheoNoiGioiThieuHoaHongService.LoaiGiaHieuLucTheoDichVuKham(model);
            return Ok(lookup);
        }
        [HttpPost("LoaiGiaHieuLucTheoDichVuKyThuat")]
        public async Task<ActionResult<ICollection<LookupItemVo>>> LoaiGiaHieuLucTheoDichVuKyThuat(DropDownListRequestModel model)
        {
            var lookup = await _cauHinhHeSoTheoNoiGioiThieuHoaHongService.LoaiGiaHieuLucTheoDichVuKyThuat(model);
            return Ok(lookup);
        }

        [HttpPost("LoaiGiaHieuLucTheoDichVuGiuong")]
        public async Task<ActionResult<ICollection<LookupItemVo>>> LoaiGiaHieuLucTheoDichVuGiuong(DropDownListRequestModel model)
        {
            var lookup = await _cauHinhHeSoTheoNoiGioiThieuHoaHongService.LoaiGiaHieuLucTheoDichVuGiuong(model);
            return Ok(lookup);
        }

        [HttpPost("GetLoaiGiaVatTuThuoc")]
        public async Task<ActionResult<ICollection<LookupItemVo>>> GetLoaiGiaVatTuThuoc([FromBody]DropDownListRequestModel queryInfo)
        {
            
            var datas = Enum.GetValues(typeof(LoaiGiaNoiGioiThieuHopDong)).Cast<Enum>();
            var models = datas.Select(o => new LookupItemVo
            {
                DisplayName = o.GetDescription(),
                KeyId = Convert.ToInt32(o)
            });

            return Ok(models);

        }
        [HttpPost("GetLoaiGiaDichVuKhamBenhs")]
        public async Task<ActionResult<ICollection<LookupItemVo>>> GetLoaiGiaDichVuKhamBenhs([FromBody]DropDownListRequestModel model,long? id)
        {
            if(id != null)
            {
                model.ParameterDependencies = "{DichVuKhamBenhBenhVienId:" + id +"}";
            }
            var lookup = await _cauHinhHeSoTheoNoiGioiThieuHoaHongService.LoaiGiaHieuLucTheoDichVuKham(model);
            return Ok(lookup);

        }
        [HttpPost("GetLoaiGiaHieuLucTheoDichVuKyThuats")]
        public async Task<ActionResult<ICollection<LookupItemVo>>> GetLoaiGiaHieuLucTheoDichVuKyThuats(DropDownListRequestModel model,long? id)
        {
            if (id != null)
            {
                model.ParameterDependencies = "{DichVuKyThuatBenhVienId:" +id + "}";
            }
            var lookup = await _cauHinhHeSoTheoNoiGioiThieuHoaHongService.LoaiGiaHieuLucTheoDichVuKyThuat(model);
            return Ok(lookup);
        }

        [HttpPost("GetLoaiGiaHieuLucTheoDichVuGiuongs")]
        public async Task<ActionResult<ICollection<LookupItemVo>>> GetLoaiGiaHieuLucTheoDichVuGiuongs(DropDownListRequestModel model, long? id)
        {
            if (id != null)
            {
                model.ParameterDependencies = "{DichVuGiuongBenhVienId:" + id + "}";
            }
            var lookup = await _cauHinhHeSoTheoNoiGioiThieuHoaHongService.LoaiGiaHieuLucTheoDichVuGiuong(model);
            return Ok(lookup);
        }
        [HttpPost("GetDonGia")]
        public async Task<ActionResult<ThongTinGiaNoiGioiThieuVo>> GetDonGia(ThongTinGiaNoiGioiThieuVo thongTinDichVu)
        {
            await _cauHinhHeSoTheoNoiGioiThieuHoaHongService.GetDonGia(thongTinDichVu);
            return Ok(thongTinDichVu);
        }
        #endregion

        #region CRUD 
        [HttpPost("SaveCauHinhHeSoTheoNoiGioiThieuHoaHong")]
        [ClaimRequirement(Enums.SecurityOperation.Add, Enums.DocumentType.DanhMucCauHinhHeSoTheoNoiGioiThieuHoaHong)]
        public async Task<ActionResult> SaveCauHinhHeSoTheoNoiGioiThieuHoaHong(CauHinhHeSoTheoThoiGianHoaHong cauHinhHeSoTheoThoiGianViewModel)
         {
            var data = _cauHinhHeSoTheoNoiGioiThieuHoaHongService.XuLyThemCauHinhHeSoTheoNoiGioiThieuHoaHongAsync(cauHinhHeSoTheoThoiGianViewModel);
            return Ok(data);
        }

        [HttpPost("UpDateCauHinhHeSoTheoNoiGioiThieuHoaHong")]
        [ClaimRequirement(Enums.SecurityOperation.Update, Enums.DocumentType.DanhMucCauHinhHeSoTheoNoiGioiThieuHoaHong)]
        public async Task<ActionResult> UpDateCauHinhHeSoTheoNoiGioiThieuHoaHong(CauHinhHeSoTheoThoiGianHoaHong cauHinhHeSoTheoThoiGianViewModel)
        {
            var data = _cauHinhHeSoTheoNoiGioiThieuHoaHongService.XuLyCapNhatCauHinhHeSoTheoNoiGioiThieuHoaHongAsync(cauHinhHeSoTheoThoiGianViewModel);
            return Ok(data);
        }

        [HttpPost("DeleteCauHinhHeSoTheoNoiGioiThieuHoaHong")]
        [ClaimRequirement(Enums.SecurityOperation.Delete, Enums.DocumentType.DanhMucCauHinhHeSoTheoNoiGioiThieuHoaHong)]
        public async Task<ActionResult> DeleteCauHinhHeSoTheoNoiGioiThieuHoaHong(DeleteNoiGioiThieuHopDongVo deleteNoiGioiThieuHopDongVo)
        {
            var data = _cauHinhHeSoTheoNoiGioiThieuHoaHongService.XuLyDeleteCauHinhHeSoTheoNoiGioiThieuAsync(deleteNoiGioiThieuHopDongVo);
            return Ok(data);
        }

        [HttpPost("GetThongTinNoiGioiThieuHopDong")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DanhMucCauHinhHeSoTheoNoiGioiThieuHoaHong)]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<ActionResult<CauHinhHeSoTheoThoiGianHoaHong>> GetThongTinNoiGioiThieuHopDong(NoiGioiThieuVo vo)
        {
            var data = _cauHinhHeSoTheoNoiGioiThieuHoaHongService.XuLyGetDataCauHinhHeSoTheoNoiGioiThieuHoaHongAsync(vo.Id,vo.NoiGioiThieuId);
            return Ok(data.Result);
        }
        #endregion
        #region CRUD cấu hình hoa hồng
        [HttpPost("SaveCauHinhHoaHong")]
        [ClaimRequirement(Enums.SecurityOperation.Add, Enums.DocumentType.DanhMucCauHinhHeSoTheoNoiGioiThieuHoaHong)]
        public async Task<ActionResult> SaveCauHinhHeSoTheoNoiGioiThieuHoaHong(CauHinhChiTietHoaHong cauHinhHoaHong)
        {
            var data = _cauHinhHeSoTheoNoiGioiThieuHoaHongService.XuLyThemCauHinhHoaHongAsync(cauHinhHoaHong);
            return Ok(data);
        }
        #endregion
        [HttpPost("GetThongTinCauHinhHoaHong")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DanhMucCauHinhHeSoTheoNoiGioiThieuHoaHong)]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<ActionResult<CauHinhChiTietHoaHong>> GetThongTinCauHinhHoaHong(NoiGioiThieuVo vo)
        {
            var data = _cauHinhHeSoTheoNoiGioiThieuHoaHongService.XuLyGetDataCauHinhHoaHongAsync(vo.Id, vo.NoiGioiThieuId);
            return Ok(data.Result);
        }

        [HttpPost("UpDateCauHinhHoaHong")]
        [ClaimRequirement(Enums.SecurityOperation.Update, Enums.DocumentType.DanhMucCauHinhHeSoTheoNoiGioiThieuHoaHong)]
        public async Task<ActionResult> UpDateCauHinhHoaHong(CauHinhChiTietHoaHong cauHinhChiTietHoaHong)
        {
            var data = _cauHinhHeSoTheoNoiGioiThieuHoaHongService.XuLyCapNhatCauHinhHoaHongAsync(cauHinhChiTietHoaHong);
            return Ok(data);
        }
        [HttpPost("DeleteCauHinhHoaHong")]
        [ClaimRequirement(Enums.SecurityOperation.Delete, Enums.DocumentType.DanhMucCauHinhHeSoTheoNoiGioiThieuHoaHong)]
        public async Task<ActionResult> DeleteCauHinhHoaHong(DeleteNoiGioiThieuHopDongVo deleteNoiGioiThieuHopDongVo)
        {
            var data = _cauHinhHeSoTheoNoiGioiThieuHoaHongService.XuLyDeleteCauHinhHoaHong(deleteNoiGioiThieuHopDongVo);
            return Ok(data);
        }


        [HttpPost("ExportDanhSachCauHinhNoiGioiThieuDichVu")]
        [ClaimRequirement(Enums.SecurityOperation.Process, Enums.DocumentType.DanhMucCauHinhHeSoTheoNoiGioiThieuHoaHong)]
        public async Task<ActionResult> ExportDanhSachCauHinhNoiGioiThieuDichVu(ExportDanhSachCauHinhNoiGioiThieuDichVuQueryInfo queryInfo)
        {

            //ThongTinCauHinhHeSoTheoNoiGtHoaHongs
            var cauHinhHeSoTheoThoiGianHoaHong = new CauHinhHeSoTheoThoiGianHoaHong();
             if (queryInfo.NoiGioiThieuHopDongId != null && queryInfo.NoiGioiThieuId != null)
            {
                cauHinhHeSoTheoThoiGianHoaHong = await _cauHinhHeSoTheoNoiGioiThieuHoaHongService.XuLyGetDataCauHinhHeSoTheoNoiGioiThieuHoaHongAsync(queryInfo.NoiGioiThieuHopDongId.GetValueOrDefault(), queryInfo.NoiGioiThieuId.GetValueOrDefault());
            }
            var arr = new List<CauHinhNoiGioiThieuDichVu>();
            if (cauHinhHeSoTheoThoiGianHoaHong != null && cauHinhHeSoTheoThoiGianHoaHong.ThongTinCauHinhHeSoTheoNoiGtHoaHongs.Count() != 0)
            {
                foreach (var item in cauHinhHeSoTheoThoiGianHoaHong.ThongTinCauHinhHeSoTheoNoiGtHoaHongs.Where(d=>d.LaDichVuKham == true || d.LaDichVuKyThuat == true || d.LaDichVuGiuong == true))
                {
                   
                    var dv = new CauHinhNoiGioiThieuDichVu()
                    {
                        LaDichVu = item.LaDichVuKham == true ? 1:
                                   item.LaDichVuKyThuat == true ? 2 : 3,
                        Ma = item.MaDichVu,
                        TenDichVu = item.TenDichVu,
                        NhomGiaDichVu = item.TenNhomGia,
                    };
                    if (item.DonGia != null)
                    {
                        dv.DonGia = item.DonGia.GetValueOrDefault();
                    }
                    if (item.DonGiaNGTTuLan1 != null)
                    {
                        dv.DonGiaNGTTuLan1 = item.DonGiaNGTTuLan1.GetValueOrDefault();
                    }
                    if (item.HeSoLan1 != null)
                    {
                        dv.HeSoLan1 = item.HeSoLan1.GetValueOrDefault();
                    }

                    if (item.DonGiaNGTTuLan2 != null)
                    {
                        dv.DonGiaNGTTuLan2 = item.DonGiaNGTTuLan2.GetValueOrDefault();
                    }
                    if (item.HeSoLan2 != null)
                    {
                        dv.HeSoLan2 = item.HeSoLan2.GetValueOrDefault();
                    }

                    if (item.DonGiaNGTTuLan3 != null)
                    {
                        dv.DonGiaNGTTuLan3 = item.DonGiaNGTTuLan3.GetValueOrDefault();
                    }
                    if (item.HeSoLan3 != null)
                    {
                        dv.HeSoLan3 = item.HeSoLan3.GetValueOrDefault();
                    }

                    if (item.GhiChu != null)
                    {
                        dv.Ghichu = item.GhiChu;
                    }
                    arr.Add(dv);
                }
            }
            else
            {
                arr.Add(new CauHinhNoiGioiThieuDichVu());
            }
            
            var datas = arr.Select(p => (CauHinhNoiGioiThieuDichVu)p).ToList();
            var excelData = datas.Map<List<CauHinhNoiGioiThieuDichVuExportExcel>>();
            var lstValueObject = new List<(string, string)>
            {
                (nameof(CauHinhNoiGioiThieuDichVuExportExcel.LaDichVu), "Loại Dịch Vụ"),
                (nameof(CauHinhNoiGioiThieuDichVuExportExcel.Ma), "Mã"),
                (nameof(CauHinhNoiGioiThieuDichVuExportExcel.TenDichVu), "Tên DV"),
                (nameof(CauHinhNoiGioiThieuDichVuExportExcel.NhomGiaDichVu), "Nhóm Giá DV"),
              
                (nameof(CauHinhNoiGioiThieuDichVuExportExcel.DonGia), "ĐƠN GIÁ"),
                (nameof(CauHinhNoiGioiThieuDichVuExportExcel.DonGiaNGTTuLan1), "ĐƠN GIÁ NGT TỪ LẦN 1"),
                (nameof(CauHinhNoiGioiThieuDichVuExportExcel.HeSoLan1), "HỆ SỐ LẦN 1"),
                (nameof(CauHinhNoiGioiThieuDichVuExportExcel.DonGiaNGTTuLan2), "ĐƠN GIÁ NGT TỪ LẦN 2"),
                (nameof(CauHinhNoiGioiThieuDichVuExportExcel.HeSoLan2), "HỆ SỐ LẦN 2"),
                (nameof(CauHinhNoiGioiThieuDichVuExportExcel.DonGiaNGTTuLan3), "ĐƠN GIÁ NGT TỪ LẦN 3"),
                (nameof(CauHinhNoiGioiThieuDichVuExportExcel.HeSoLan3), "HỆ SỐ LẦN 3"),
               
                (nameof(CauHinhNoiGioiThieuDichVuExportExcel.Ghichu), "GHI CHÚ"),
            };
            var bytes = _excelService.ExportManagermentView(excelData, lstValueObject, "DS Cấu Hình Nơi Giới Thiệu", 2, "DS Cấu Hình Nơi Giới Thiệu");
            HttpContext.Response.Headers.Add("content-disposition", "attachment; filename=DsCauHinhNoiGioiThieu" + DateTime.Now.Day + DateTime.Now.Month + DateTime.Now.Year + ".xls");
            Response.ContentType = "application/vnd.ms-excel";
            return new FileContentResult(bytes, "application/vnd.ms-excel");
        }

        [HttpPost("ImportDSDICHVUCHUADUOCTAO")]
        [ClaimRequirement(Enums.SecurityOperation.Update, Enums.DocumentType.DanhMucCauHinhHeSoTheoNoiGioiThieuHoaHong)]
        public async Task<ActionResult> ImportDSDICHVUCHUADUOCTAO(DSChuaTaoImport model)
        {
            var path = _taiLieuDinhKemService.GetObjectStream(model.DuongDan, model.TenGuid);
            var listError = await _cauHinhHeSoTheoNoiGioiThieuHoaHongService.ImportDSDichVus(path, model.NoiGioiThieuHopDongId,model.NoiGioiThieuId);
            return Ok(listError);
        }

        [HttpPost("GetNoiGioiThieuHopDongUuTiens")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DanhMucCauHinhHeSoTheoNoiGioiThieuHoaHong)]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<ActionResult<long>> GetNoiGioiThieuHopDongUuTiens(long id)
        {
            var data = _cauHinhHeSoTheoNoiGioiThieuHoaHongService.GetNoiGioiThieuHopDongUuTiens(id);
            return Ok(data);
        }
        [HttpPost("GetTextNoiGioiThieu")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.DanhMucCauHinhHeSoTheoNoiGioiThieuHoaHong)]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<ActionResult<long>> GetTextNoiGioiThieu(long id)
        {
            var data = _cauHinhHeSoTheoNoiGioiThieuHoaHongService.GetNoiGioiThieu(id);
            return Ok(data);
        }
        #region
        [HttpPost("ExportDanhSachCauHinhNoiGioiThieuDPVTYT")]
        [ClaimRequirement(Enums.SecurityOperation.Process, Enums.DocumentType.DanhMucCauHinhHeSoTheoNoiGioiThieuHoaHong)]
        public async Task<ActionResult> ExportDanhSachCauHinhNoiGioiThieuDPVTYT(ExportDanhSachCauHinhNoiGioiThieuDichVuQueryInfo queryInfo)
        {
            //ThongTinCauHinhHeSoTheoNoiGtHoaHongs
            var cauHinhHeSoTheoThoiGianHoaHong = new CauHinhHeSoTheoThoiGianHoaHong();
            if (queryInfo.NoiGioiThieuHopDongId != null && queryInfo.NoiGioiThieuId != null)
            {
                cauHinhHeSoTheoThoiGianHoaHong = await _cauHinhHeSoTheoNoiGioiThieuHoaHongService.XuLyGetDataCauHinhHeSoTheoNoiGioiThieuHoaHongAsync(queryInfo.NoiGioiThieuHopDongId.GetValueOrDefault(), queryInfo.NoiGioiThieuId.GetValueOrDefault());
            }
            var arr = new List<CauHinhNoiGioiThieuDPVTYTVo>();
            if (cauHinhHeSoTheoThoiGianHoaHong != null && cauHinhHeSoTheoThoiGianHoaHong.ThongTinCauHinhHeSoTheoNoiGtHoaHongs.Count() != 0)
            {
                foreach (var item in cauHinhHeSoTheoThoiGianHoaHong.ThongTinCauHinhHeSoTheoNoiGtHoaHongs.Where(d => d.LaDuocPham == true || d.LaVatTu == true))
                {

                    var dv = new CauHinhNoiGioiThieuDPVTYTVo()
                    {
                        LaDichVu = item.LaDuocPham == true ? 1 :2
                                   ,
                        Ma = item.MaDichVu,
                        Ten = item.TenDichVu,
                        NhomGia = item.TenNhomGia,
                    };
                    if (item.HeSo != null)
                    {
                        dv.HeSo = item.HeSo.GetValueOrDefault();
                    }
                   
                    

                    if (item.GhiChu != null)
                    {
                        dv.Ghichu = item.GhiChu;
                    }
                    arr.Add(dv);
                }
            }
            else
            {
                arr.Add(new CauHinhNoiGioiThieuDPVTYTVo());
            }
            
            var datas = arr.Select(p => (CauHinhNoiGioiThieuDPVTYTVo)p).ToList();

            var excelData = datas.Map<List<CauHinhNoiGioiThieuDPVTYTExportExcel>>();
            var lstValueObject = new List<(string, string)>
            {
                (nameof(CauHinhNoiGioiThieuDPVTYTExportExcel.LaDichVu), "Loại Dịch Vụ"),
                (nameof(CauHinhNoiGioiThieuDPVTYTExportExcel.Ma), "Mã"),
                (nameof(CauHinhNoiGioiThieuDPVTYTExportExcel.Ten), "Tên"),
                (nameof(CauHinhNoiGioiThieuDPVTYTExportExcel.NhomGia), "Nhóm Giá "),
                (nameof(CauHinhNoiGioiThieuDPVTYTExportExcel.HeSo), "HỆ SỐ "),

                (nameof(CauHinhNoiGioiThieuDPVTYTExportExcel.Ghichu), "GHI CHÚ"),
            };
            var bytes = _excelService.ExportManagermentView(excelData, lstValueObject, "DS Cấu Hình Nơi Giới Thiệu", 2, "DS Cấu Hình Nơi Giới Thiệu");
            HttpContext.Response.Headers.Add("content-disposition", "attachment; filename=DsExportDuocPhamVTYT" + DateTime.Now.Day + DateTime.Now.Month + DateTime.Now.Year + ".xls");
            Response.ContentType = "application/vnd.ms-excel";
            return new FileContentResult(bytes, "application/vnd.ms-excel");
        }

        [HttpPost("ImportDsCauHinhNoiGioiThieuDuocPhamVTYT")]
        [ClaimRequirement(Enums.SecurityOperation.Update, Enums.DocumentType.DanhMucCauHinhHeSoTheoNoiGioiThieuHoaHong)]
        public async Task<ActionResult> ImportDsCauHinhNoiGioiThieuDuocPhamVTYT(DSChuaTaoImport model)
        {
            var path = _taiLieuDinhKemService.GetObjectStream(model.DuongDan, model.TenGuid);
            var listError = await _cauHinhHeSoTheoNoiGioiThieuHoaHongService.ImportNoiGioiThieuDuocPhamVTYTs(path, model.NoiGioiThieuHopDongId, model.NoiGioiThieuId);
            return Ok(listError);
        }
        #endregion

        #region cấu hình hoa hồng
        
        [HttpPost("ExportDanhSachCauHinhHoaHongDichVu")]
        [ClaimRequirement(Enums.SecurityOperation.Process, Enums.DocumentType.DanhMucCauHinhHeSoTheoNoiGioiThieuHoaHong)]
        public async Task<ActionResult> ExportDanhSachCauHinhHoaHongDichVu(ExportDanhSachCauHinhNoiGioiThieuDichVuQueryInfo queryInfo)
        {
            //ThongTinCauHinhHeSoTheoNoiGtHoaHongs
            var cauHinhHeSoTheoThoiGianHoaHong = new CauHinhChiTietHoaHong();
            if (queryInfo.NoiGioiThieuHopDongId != null && queryInfo.NoiGioiThieuId != null)
            {
                cauHinhHeSoTheoThoiGianHoaHong = await _cauHinhHeSoTheoNoiGioiThieuHoaHongService.XuLyGetDataCauHinhHoaHongAsync(queryInfo.NoiGioiThieuHopDongId.GetValueOrDefault(), queryInfo.NoiGioiThieuId.GetValueOrDefault());
            }
            var arr = new List<CauHinhHoaHongDichVuVo>();
            if (cauHinhHeSoTheoThoiGianHoaHong != null && cauHinhHeSoTheoThoiGianHoaHong.ThongTinCauHinhHoaHongs.Count() != 0)
            {
                foreach (var item in cauHinhHeSoTheoThoiGianHoaHong.ThongTinCauHinhHoaHongs.Where(d => d.LaDuocPham == false || d.LaVatTu == false))
                {

                    var dv = new CauHinhHoaHongDichVuVo()
                    {
                        LaDichVu = item.LaDichVuKham == true ? "1" :
                                   item.LaDichVuKyThuat == true ? "2" : "3",
                        Ma = item.MaDichVu,
                        Ten = item.TenDichVu,
                        NhomGia = item.TenNhomGia,
                    };
                    if (item.DonGia != null)
                    {
                        dv.DonGia = item.DonGia.GetValueOrDefault();
                    }
                    if (item.DonGiaHoaHongHoacTien != null)
                    {
                        dv.DonGiaHoaHong = item.DonGiaHoaHongHoacTien.GetValueOrDefault();
                    }
                    if (item.ChonTienHayHoaHong != null)
                    {
                        dv.LoaiHoaHong = item.ChonTienHayHoaHong == LoaiHoaHong.SoTien ? LoaiHoaHong.SoTien .GetDescription()
                             : LoaiHoaHong.TiLe.GetDescription();
                    }

                    if (item.ADDHHTuLan != null)
                    {
                        dv.ApDungHoaHongTuLan = item.ADDHHTuLan.GetValueOrDefault();
                    }
                    if (item.ADDHHDenLan != null)
                    {
                        dv.ApDungHoaHongDenLan = item.ADDHHDenLan.GetValueOrDefault();
                    }

                    if (item.GhiChu != null)
                    {
                        dv.Ghichu = item.GhiChu;
                    }
                    arr.Add(dv);
                }
            }
            else
            {
                arr.Add(new CauHinhHoaHongDichVuVo());
            }
            var datas = arr.Select(p => (CauHinhHoaHongDichVuVo)p).ToList();
            var excelData = datas.Map<List<CauHinhHoaHongDichVuExportExcel>>();
            var lstValueObject = new List<(string, string)>
            {
                (nameof(CauHinhHoaHongDichVuExportExcel.LaDichVu), "Loại Dịch Vụ"),
                (nameof(CauHinhHoaHongDichVuExportExcel.Ma), "Mã"),
                (nameof(CauHinhHoaHongDichVuExportExcel.Ten), "Tên"),
                (nameof(CauHinhHoaHongDichVuExportExcel.NhomGia), "Nhóm Giá "),
                (nameof(CauHinhHoaHongDichVuExportExcel.DonGia), "Đơn Giá BV "),
                (nameof(CauHinhHoaHongDichVuExportExcel.LoaiHoaHong), "Loại Đơn Giá"),
                (nameof(CauHinhHoaHongDichVuExportExcel.DonGiaHoaHong), "Đơn Giá Hoa Hồng"),
                (nameof(CauHinhHoaHongDichVuExportExcel.ApDungHoaHongTuLanDisplay), "Áp Dụng Hoa Hồng Từ Lần"),
                (nameof(CauHinhHoaHongDichVuExportExcel.ApDungHoaHongDenLanDisplay), "Áp Dụng Hoa Hồng Đến Lần"),

                (nameof(CauHinhHoaHongDichVuExportExcel.Ghichu), "Ghi Chú"),
            };
            var bytes = _excelService.ExportManagermentView(excelData, lstValueObject, "DS Cấu Hình Hoa Hồng", 2, "DS Cấu Hình Hoa Hồng");
            HttpContext.Response.Headers.Add("content-disposition", "attachment; filename=DsExportHoaHongDichVu" + DateTime.Now.Day + DateTime.Now.Month + DateTime.Now.Year + ".xls");
            Response.ContentType = "application/vnd.ms-excel";
            return new FileContentResult(bytes, "application/vnd.ms-excel");
        }

        [HttpPost("ImportDsCauHinhHoaHongDichVu")]
        [ClaimRequirement(Enums.SecurityOperation.Update, Enums.DocumentType.DanhMucCauHinhHeSoTheoNoiGioiThieuHoaHong)]
        public async Task<ActionResult> ImportDsCauHinhHoaHongDichVu(DSChuaTaoImport model)
        {
            var path = _taiLieuDinhKemService.GetObjectStream(model.DuongDan, model.TenGuid);
            var listError = await _cauHinhHeSoTheoNoiGioiThieuHoaHongService.ImportHoaHongDichVus(path, model.NoiGioiThieuHopDongId, model.NoiGioiThieuId);
            return Ok(listError);
        }

        #endregion


        #region cấu hình hoa hồng  DP/VTYT

        [HttpPost("ExportDanhSachCauHinhHoaHongDPVTYT")]
        [ClaimRequirement(Enums.SecurityOperation.Process, Enums.DocumentType.DanhMucCauHinhHeSoTheoNoiGioiThieuHoaHong)]
        public async Task<ActionResult> ExportDanhSachCauHinhHoaHongDPVTYT(ExportDanhSachCauHinhNoiGioiThieuDichVuQueryInfo queryInfo)
        {
            var cauHinhHeSoTheoThoiGianHoaHong = new CauHinhChiTietHoaHong();
            if (queryInfo.NoiGioiThieuHopDongId != null && queryInfo.NoiGioiThieuId != null)
            {
                cauHinhHeSoTheoThoiGianHoaHong = await _cauHinhHeSoTheoNoiGioiThieuHoaHongService.XuLyGetDataCauHinhHoaHongAsync(queryInfo.NoiGioiThieuHopDongId.GetValueOrDefault(), queryInfo.NoiGioiThieuId.GetValueOrDefault());
            }
            var arr = new List<CauHinhHoaHongDuocPhamVTYTVo>();
            if (cauHinhHeSoTheoThoiGianHoaHong != null && cauHinhHeSoTheoThoiGianHoaHong.ThongTinCauHinhHoaHongs.Count() != 0)
            {
                foreach (var item in cauHinhHeSoTheoThoiGianHoaHong.ThongTinCauHinhHoaHongs.Where(d => d.LaDuocPham == true || d.LaVatTu == true))
                {

                    var dv = new CauHinhHoaHongDuocPhamVTYTVo()
                    {
                        LaDichVu = item.LaDuocPham == true ? 1 :
                                   2,
                        Ma = item.MaDichVu,
                        Ten = item.TenDichVu,
                    };
                    if (item.DonGiaHoaHongHoacTien != null)
                    {
                        dv.DonGiaHoaHong = item.DonGiaHoaHongHoacTien.GetValueOrDefault();
                    }


                    if (item.GhiChu != null)
                    {
                        dv.Ghichu = item.GhiChu;
                    }
                    arr.Add(dv);
                }
            }
            else
            {
                arr.Add(new CauHinhHoaHongDuocPhamVTYTVo());
            }
            var datas = arr.Select(p => (CauHinhHoaHongDuocPhamVTYTVo)p).ToList();
            var excelData = datas.Map<List<CauHinhHoaHongDPVTYTExportExcel>>();
            var lstValueObject = new List<(string, string)>
            {
                (nameof(CauHinhHoaHongDPVTYTExportExcel.LaDichVu), "Loại Dịch Vụ"),
                (nameof(CauHinhHoaHongDPVTYTExportExcel.Ma), "Mã"),
                (nameof(CauHinhHoaHongDPVTYTExportExcel.Ten), "Tên"),
                (nameof(CauHinhHoaHongDichVuExportExcel.DonGiaHoaHong), "Đơn Giá Hoa Hồng"),
                (nameof(CauHinhHoaHongDichVuExportExcel.Ghichu), "Ghi Chú"),
            };
            var bytes = _excelService.ExportManagermentView(excelData, lstValueObject, "DS Cấu Hình Hoa Hồng", 2, "DS Cấu Hình Hoa Hồng");
            HttpContext.Response.Headers.Add("content-disposition", "attachment; filename=DsExportHoaHongDPVTYT" + DateTime.Now.Day + DateTime.Now.Month + DateTime.Now.Year + ".xls");
            Response.ContentType = "application/vnd.ms-excel";
            return new FileContentResult(bytes, "application/vnd.ms-excel");
        }

        [HttpPost("ImportDsCauHinhHoaHongDPVTYT")]
        [ClaimRequirement(Enums.SecurityOperation.Update, Enums.DocumentType.DanhMucCauHinhHeSoTheoNoiGioiThieuHoaHong)]
        public async Task<ActionResult> ImportDsCauHinhHoaHongDPVTYT(DSChuaTaoImport model)
        {
            var path = _taiLieuDinhKemService.GetObjectStream(model.DuongDan, model.TenGuid);
            var listError = await _cauHinhHeSoTheoNoiGioiThieuHoaHongService.ImportHoaHongDPVTYTs(path, model.NoiGioiThieuHopDongId, model.NoiGioiThieuId);
            return Ok(listError);
        }

        #endregion
    }
}
