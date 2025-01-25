using Camino.Api.Auth;
using Camino.Api.Extensions;
using Camino.Api.Models.Error;
using Camino.Api.Models.KhamBenh;
using Camino.Api.Models.KhamBenh.ViewModelCheckValidators;
using Camino.Api.Models.KhamDoan;
using Camino.Core.Domain;
using Camino.Core.Domain.Entities.YeuCauKhamBenhs;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Domain.ValueObject.KhamDoan;
using Camino.Core.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using static Camino.Core.Domain.Enums;

namespace Camino.Api.Controllers
{
    public partial class KhamDoanController
    {
        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetDataForGridAsyncDanhSachInKetQuaKhamSucKhoe")]
        [ClaimRequirement(SecurityOperation.View, DocumentType.KhamDoanKetLuanKhamSucKhoeDoan)]
        public async Task<ActionResult<GridDataSource>> GetDataForGridAsyncDanhSachInKetQuaKhamSucKhoe([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _khamDoanService.GetDataForGridAsyncDanhSachInKetQuaKhamSucKhoe(queryInfo);
            return Ok(gridData);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetTotalPageForGridAsyncDanhSachInKetQuaKhamSucKhoe")]
        [ClaimRequirement(SecurityOperation.View, DocumentType.KhamDoanKetLuanKhamSucKhoeDoan)]
        public async Task<ActionResult<GridDataSource>> GetTotalPageForGridAsyncDanhSachInKetQuaKhamSucKhoe([FromBody]QueryInfo queryInfo)
        {
            //bo lazy load
            var gridData = await _khamDoanService.GetDataForGridAsyncDanhSachInKetQuaKhamSucKhoe(queryInfo);
            return Ok(gridData);
        }
        [HttpPost("GetCongTyInKetQuaKSKs")]
        public async Task<ActionResult> GetCongTyInKetQuaKSKs([FromBody]DropDownListRequestModel queryInfo)
        {
            var lookup = await _khamDoanService.GetCongTyInKetQuaKSKs(queryInfo);
            return Ok(lookup);
        }
        [HttpPost("GetHopDongKhamSucKhoeInKetQuaKSKs")]
        public async Task<ActionResult> GetHopDongKhamSucKhoeInKetQuaKSKs([FromBody]DropDownListRequestModel queryInfo, bool LaHopDongKetThuc = false)
        {
            var lookup = await _khamDoanService.GetHopDongKhamSucKhoeInKetQuaKSKs(queryInfo, LaHopDongKetThuc);
            return Ok(lookup);
        }
        [HttpGet("GetThongTinHanhChinhInKetQuaKSK")]
        public async Task<ActionResult<KhamDoanThongTinHanhChinhInKetQuaKhamSucKhoeVo>> GetThongTinHanhChinhInKetQuaKSK(long yeuCauTiepNhanId)
        {
            var thongTinHanhChinh = await _khamDoanService.GetThongTinHanhChinhInKetQuaKSKAsync(yeuCauTiepNhanId);
            return thongTinHanhChinh;
        }

        [HttpPost("GetInFoBarCode")]
        public async Task<ActionResult<List<InFoBarCodeKSK>>> GetInFoBarCode(InFoBarCodeKSKVo vo)
        {
            var ycTiepNhan =  _khamDoanService.ComPareBarCode(vo);
            return ycTiepNhan;
        }


        [HttpPost("LuuVaCapNhatTrangThaiKhamDoan")]
        [ClaimRequirement(Enums.SecurityOperation.Update, Enums.DocumentType.KhamDoanKhamBenh, Enums.DocumentType.KhamDoanKetLuanKhamSucKhoeDoan)]
        public async Task<ActionResult<KhamBenhPhongBenhVienHangDoiViewModel>> LuuVaCapNhatTrangThaiKhamDoan([FromBody]LuuVaCapNhatTrangThaiViewModel info)
        {
            //chỗ này nó muốn như vầy, nếu chỗ DV khám bệnh cột cập nhật kết quả DV của DV khám nào có nhập thì khi nhấn Lưu và đổi trạng thái DV nó sẽ tương đương với 
            //chức năng LƯu và hoàn thành khám trong phòng khám của DV đó(nếu DV chưa hoàn thành khám), nhưng hiện tại trong phòng khám ban đầu mình đang fill thông tin
            //mặc định lên, nếu nhấn Lưu và đổi trạng thái DV thì những thông tin mặc định đó mình ko lưu, sẽ để trắng

            long yctnId = 0;
            
            var listDichVuKhamBenhUpdateIds = info.ListDichVuKSKCanCapNhatTrangThaiDVKs.Select(d => d.Id).ToList();

            var yctn = await _khamDoanService.GetYeuCauTiepNhanDoanCantAsync(info.YeuCauTiepNhanId); 

            #region cập nhật xem người khám   kết luận, Thời điểm kết luận
                var objJsonKetLuan = new KetQuaKhamSucKhoeDaTa
                {
                    NhanVienKetLuanId = _userAgentHelper.GetCurrentUserId(),
                    ThoiDiemKetLuan = DateTime.Now,
                    KetQuaKhamSucKhoe = info.JsonKetQuaKSK
                };


            var json = JsonConvert.SerializeObject(objJsonKetLuan);

            yctn.KetQuaKhamSucKhoeData = json;
            yctn.KSKKetLuanData = info.JsonKetLuan;
            yctn.LoaiLuuInKetQuaKSK = true;

            #endregion end cập nhật xem người khám   kết luận, Thời điểm kết luận


            #region cập nhật trạng thái


            // yêu cầu khám bệnh
            foreach (var item in yctn.YeuCauKhamBenhs.Where( d=> listDichVuKhamBenhUpdateIds.Contains(d.Id)).ToList())
            {

                #region Xử lý trường hợp hoàn thành khám dv  và xóa phòng PhongBenhVienHangDoi theo yêu cầu khám 


                // cập nhật dịch vụ khám cần lưu


                if (item.TrangThai != Enums.EnumTrangThaiYeuCauKhamBenh.DaKham &&
                   item.TrangThai != Enums.EnumTrangThaiYeuCauKhamBenh.HuyKham)
                {
                    // nếu dịch vụ khám chưa khám => set []
                    if (item.TrangThai == Enums.EnumTrangThaiYeuCauKhamBenh.ChuaKham)
                    {
                        var dataEmptyByTemplate = "{\"DataKhamTheoTemplate\": []}";
                        item.ThongTinKhamTheoDichVuData = dataEmptyByTemplate;
                    }

                    item.TrangThai = Enums.EnumTrangThaiYeuCauKhamBenh.DaKham;

                    //

                    //item.NoiThucHienId = _userAgentHelper.GetCurrentNoiLLamViecId();
                    item.BacSiThucHienId = _userAgentHelper.GetCurrentUserId();
                    item.ThoiDiemThucHien = DateTime.Now;
                    item.ThoiDiemHoanThanh = DateTime.Now;
                    item.BacSiKetLuanId = _userAgentHelper.GetCurrentUserId();




                    YeuCauKhamBenhLichSuTrangThai trangThaiMoi = new YeuCauKhamBenhLichSuTrangThai
                    {
                        TrangThaiYeuCauKhamBenh = item.TrangThai,
                        MoTa = item.TrangThai.GetDescription() + " cập nhật trạng thái từ màn hình in kết quả"
                    };
                    item.YeuCauKhamBenhLichSuTrangThais.Add(trangThaiMoi);

                    if (string.IsNullOrEmpty(item.ThongTinKhamTheoDichVuData))
                    {
                        var dataEmptyByTemplate = "{\"DataKhamTheoTemplate\": []}";
                        item.ThongTinKhamTheoDichVuData = dataEmptyByTemplate;
                    }

                }
                #endregion
            }
            #endregion

            #region xóa hàng đợi theo từng yêu cầu khám và yêu cầu dịch vụ kỹ thuật
            if (yctn.YeuCauKhamBenhs != null)
            {
                var phongBenhVienHangDois = yctn.YeuCauKhamBenhs.Where(d=> listDichVuKhamBenhUpdateIds.Contains(d.Id)).SelectMany(d=>d.PhongBenhVienHangDois).ToList();

                foreach (var item in phongBenhVienHangDois)
                {
                    item.WillDelete = true;
                }
            }
            #endregion

            await _yeuCauTiepNhanService.PrepareForDeleteDichVuAndUpdateAsync(yctn);
            yctnId = yctn.Id;

            return Ok(yctnId);
        }

        [HttpPost("GetHopDongId")]
        public async Task<ActionResult<long>> GetHopDongId(long hopDongKhamSucKhoeNVId)
        {
            var hopDongId = _khamDoanService.GetHopDongId(hopDongKhamSucKhoeNVId);
            return Ok(hopDongId.Result);
        }

    }
}
