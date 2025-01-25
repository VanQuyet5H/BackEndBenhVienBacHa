using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Camino.Api.Auth;
using Camino.Api.Extensions;
using Camino.Api.Models.Error;
using Camino.Api.Models.KhamDoanChiSoSinhTons;
using Camino.Core.Domain;
using Camino.Core.Domain.Entities.KetQuaSinhHieus;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Domain.ValueObject.KhamDoan;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Camino.Api.Controllers
{
    public partial class KhamDoanController
    {
        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpGet("GetDataListForChiSoSinhTon")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.KhamDoanChiSoSinhTon, Enums.DocumentType.KhamDoanKhamBenhTatCaPhong)]
        public async Task<ActionResult<GridDataSource>> GetDataListForChiSoSinhTon
            (long id)
        {
            var gridData = await _khamDoanService.GetDataListForChiSoSinhTon(id);
            return Ok(gridData);
        }

        [HttpGet("GetHopDongKhamSucKhoe")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.KhamDoanChiSoSinhTon)]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<ActionResult<KhamDoanHopDongKhamSucKhoeNhanVienViewModel>> GetHopDongKhamSucKhoe(long id)
        {
            var hopDongKhamSucKhoe = await _khamDoanService.GetByHopDongKhamSucKhoeIdAsync(id, w => w.Include(q => q.NgheNghiep)
                .Include(q => q.QuocTich)
                .Include(q => q.DanToc)
                .Include(q => q.TinhThanh)
                .Include(q => q.QuanHuyen)
                .Include(q => q.PhuongXa));

            if (hopDongKhamSucKhoe == null)
            {
                return NotFound();
            }

            var hopDongKhamSucKhoeResultModel =
                hopDongKhamSucKhoe.ToModel<KhamDoanHopDongKhamSucKhoeNhanVienViewModel>();

            hopDongKhamSucKhoeResultModel.NgheNghiep = hopDongKhamSucKhoe.NgheNghiep?.Ten;
            hopDongKhamSucKhoeResultModel.QuocTich = hopDongKhamSucKhoe.QuocTich?.Ten;
            hopDongKhamSucKhoeResultModel.DanToc = hopDongKhamSucKhoe.DanToc?.Ten;
            hopDongKhamSucKhoeResultModel.TinhThanh = hopDongKhamSucKhoe.TinhThanh?.Ten;
            hopDongKhamSucKhoeResultModel.QuanHuyen = hopDongKhamSucKhoe.QuanHuyen?.Ten;
            hopDongKhamSucKhoeResultModel.PhuongXa = hopDongKhamSucKhoe.PhuongXa?.Ten;


            return Ok(hopDongKhamSucKhoeResultModel);
        }

        [HttpPost("UpdateChiSoSinhTonAsync")]
        [ClaimRequirement(Enums.SecurityOperation.Update, Enums.DocumentType.KhamDoanChiSoSinhTon, Enums.DocumentType.KhamDoanKhamBenhTatCaPhong)]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<ActionResult<KhamDoanHopDongKhamSucKhoeNhanVienViewModel>> UpdateChiSoSinhTonAsync([FromBody]KhamDoanSendChiSoSinhTon dataSourceSinhHieu)
        {
            var hopDongKhamSucKhoe =
                await _khamDoanService.GetByIdAsync(dataSourceSinhHieu.Id, w => w.Include(q => q.KetQuaSinhHieus));
            var nhanVien = await _userService.GetCurrentUser();

            if (dataSourceSinhHieu.data.Any(e => e.IsDelete))
            {
                foreach (var dataSinhHieuDeleteUi in dataSourceSinhHieu.data.Where(e => e.IsDelete))
                {
                    if (hopDongKhamSucKhoe.KetQuaSinhHieus.Any(q => q.Id == dataSinhHieuDeleteUi.Id))
                    {
                        hopDongKhamSucKhoe.KetQuaSinhHieus.First(q => q.Id == dataSinhHieuDeleteUi.Id).WillDelete =
                            true;
                    }
                }
            }

            // kiểm tra kết quả sinh hiệu
            var lstKetQuaSinhHieu = dataSourceSinhHieu.data;
            if (lstKetQuaSinhHieu.Any())
            {
                lstKetQuaSinhHieu = lstKetQuaSinhHieu.Where(x => x.Id == 0).ToList();
                foreach (var item in lstKetQuaSinhHieu)
                {
                    if (item.BMI == null && item.CanNang == null && item.ChieuCao == null &&
                        item.HuyetApTamThu == null && item.HuyetApTamTruong == null && item.NhipTho == null && item.NhipTim == null &&
                        item.ThanNhiet == null && item.Glassgow == null && item.SpO2 == null)
                    {
                        throw new ApiException(_localizationService.GetResource("KhamBenh.Empty.IsRequired"), (int)HttpStatusCode.BadRequest);
                    }

                    if (item.HuyetApTamThu != null && item.HuyetApTamTruong == null ||
                        item.HuyetApTamThu == null && item.HuyetApTamTruong != null)
                    {
                        throw new ApiException(_localizationService.GetResource("KhamBenh.TamThuTamTruong.Empty.IsRequired"), (int)HttpStatusCode.BadRequest);
                    }
                }

            }

            if (dataSourceSinhHieu.data.Any(e => e.Id != 0 && e.IsUpdate && !e.IsDelete))
            {
                foreach (var dataSinhHieuUpdateUi in dataSourceSinhHieu.data.Where(e => e.IsUpdate && !e.IsDelete))
                {
                    if (hopDongKhamSucKhoe.KetQuaSinhHieus.Any(q => q.Id == dataSinhHieuUpdateUi.Id))
                    {
                        hopDongKhamSucKhoe.KetQuaSinhHieus.First(q => q.Id == dataSinhHieuUpdateUi.Id).ThoiDiemThucHien
                            = DateTime.Now;
                        hopDongKhamSucKhoe.KetQuaSinhHieus.First(q => q.Id == dataSinhHieuUpdateUi.Id).CanNang =
                            dataSinhHieuUpdateUi.CanNang;
                        hopDongKhamSucKhoe.KetQuaSinhHieus.First(q => q.Id == dataSinhHieuUpdateUi.Id).ChieuCao =
                            dataSinhHieuUpdateUi.ChieuCao;
                        hopDongKhamSucKhoe.KetQuaSinhHieus.First(q => q.Id == dataSinhHieuUpdateUi.Id).Glassgow =
                            dataSinhHieuUpdateUi.Glassgow;
                        hopDongKhamSucKhoe.KetQuaSinhHieus.First(q => q.Id == dataSinhHieuUpdateUi.Id).HuyetApTamThu =
                            dataSinhHieuUpdateUi.HuyetApTamThu;
                        hopDongKhamSucKhoe.KetQuaSinhHieus.First(q => q.Id == dataSinhHieuUpdateUi.Id).HuyetApTamTruong =
                            dataSinhHieuUpdateUi.HuyetApTamTruong;
                        hopDongKhamSucKhoe.KetQuaSinhHieus.First(q => q.Id == dataSinhHieuUpdateUi.Id).NhanVienThucHienId =
                            nhanVien.Id;
                        hopDongKhamSucKhoe.KetQuaSinhHieus.First(q => q.Id == dataSinhHieuUpdateUi.Id).NhipTho =
                            dataSinhHieuUpdateUi.NhipTho;
                        hopDongKhamSucKhoe.KetQuaSinhHieus.First(q => q.Id == dataSinhHieuUpdateUi.Id).NhipTim =
                            dataSinhHieuUpdateUi.NhipTim;
                        hopDongKhamSucKhoe.KetQuaSinhHieus.First(q => q.Id == dataSinhHieuUpdateUi.Id).SpO2 =
                            dataSinhHieuUpdateUi.SpO2;
                        hopDongKhamSucKhoe.KetQuaSinhHieus.First(q => q.Id == dataSinhHieuUpdateUi.Id).Bmi =
                            dataSinhHieuUpdateUi.BMI;
                        hopDongKhamSucKhoe.KetQuaSinhHieus.First(q => q.Id == dataSinhHieuUpdateUi.Id).ThanNhiet =
                            dataSinhHieuUpdateUi.ThanNhiet;
                        hopDongKhamSucKhoe.KetQuaSinhHieus.First(q => q.Id == dataSinhHieuUpdateUi.Id).YeuCauTiepNhanId =
                            dataSourceSinhHieu.Id;
                        hopDongKhamSucKhoe.KetQuaSinhHieus.First(q => q.Id == dataSinhHieuUpdateUi.Id).NoiThucHienId =
                            _userAgentHelper.GetCurrentNoiLLamViecId();

                        hopDongKhamSucKhoe.KetQuaSinhHieus.First(q => q.Id == dataSinhHieuUpdateUi.Id).KSKPhanLoaiTheLuc =
                             dataSinhHieuUpdateUi.KSKPhanLoaiTheLuc;
                    }
                }
            }

            if (dataSourceSinhHieu.data.Any(e => e.Id == 0))
            {
                foreach (var dataSinhHieuCreateUi in dataSourceSinhHieu.data.Where(e => e.Id == 0))
                {
                    var ketQuaSinhHieuAddNew = new KetQuaSinhHieu
                    {
                        ThoiDiemThucHien = DateTime.Now,
                        CanNang = dataSinhHieuCreateUi.CanNang,
                        ChieuCao = dataSinhHieuCreateUi.ChieuCao,
                        Glassgow = dataSinhHieuCreateUi.Glassgow,
                        HuyetApTamThu = dataSinhHieuCreateUi.HuyetApTamThu,
                        HuyetApTamTruong = dataSinhHieuCreateUi.HuyetApTamTruong,
                        NhanVienThucHienId = nhanVien.Id,
                        NhipTho = dataSinhHieuCreateUi.NhipTho,
                        NhipTim = dataSinhHieuCreateUi.NhipTim,
                        SpO2 = dataSinhHieuCreateUi.SpO2,
                        Bmi = dataSinhHieuCreateUi.BMI,
                        ThanNhiet = dataSinhHieuCreateUi.ThanNhiet,
                        YeuCauTiepNhanId = dataSourceSinhHieu.Id,
                        NoiThucHienId = _userAgentHelper.GetCurrentNoiLLamViecId(),
                        KSKPhanLoaiTheLuc = dataSinhHieuCreateUi.KSKPhanLoaiTheLuc
                    };
                    hopDongKhamSucKhoe.KetQuaSinhHieus.Add(ketQuaSinhHieuAddNew);
                }


            }

            if (dataSourceSinhHieu.data.Any() &&
                dataSourceSinhHieu.data.LastOrDefault().KSKPhanLoaiTheLuc != null)
            {
                hopDongKhamSucKhoe.KSKPhanLoaiTheLuc = (Enums.PhanLoaiSucKhoe)dataSourceSinhHieu.data.LastOrDefault().KSKPhanLoaiTheLuc;
            }
            else
            {
                hopDongKhamSucKhoe.KSKPhanLoaiTheLuc = null;
            }

            await _khamDoanService.UpdateAsync(hopDongKhamSucKhoe);

            return Ok();
        }
    }
}
