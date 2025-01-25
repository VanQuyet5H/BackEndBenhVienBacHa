using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Camino.Api.Auth;
using Camino.Api.Models.BHYT;
using Camino.Api.Models.Error;
using Camino.Core.Domain;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Helpers;
using Camino.Services.Localization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using Camino.Services.BHYT;
using Camino.Core.Domain.ValueObject.BHYT;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using System.Text;
using Camino.Api.Models.HamGuiHoSoWatchings;
using Camino.Services.HamGuiHoSoWatchings;
using Camino.Core.Domain.Entities.HamGuiHoSoWatchings;
using System.Xml.Linq;
using Camino.Services.GoiBaoHiemYTe;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Domain.ValueObject.GoiBaoHiemYTe;
using System.Globalization;
using Camino.Services.ExportImport;
using Camino.Services.Helpers;
using Camino.Core.Domain.ValueObject.ExcelChungTu;
using Camino.Core.Domain.ValueObject.ExportExcelItemVo;

namespace Camino.Api.Controllers
{
    public partial class BHYTController : CaminoBaseController
    {
        [HttpPost("ExportChungTu")]
        [ClaimRequirement(Enums.SecurityOperation.Process, Enums.DocumentType.DanhSachXuatChungTuExcel)]
        public ActionResult ExportChungTu(ExcelChungTuQueryInfo excelChungTuQueryInfo)
        {
            switch (excelChungTuQueryInfo.LoaiChungTu)
            {
                case LoaiChungTuXuatExcel.GiayRaVien: 
                    var bytes = _goiBaoHiemYTeService.ExportGiayRaVien(excelChungTuQueryInfo);
                    HttpContext.Response.Headers.Add("content-disposition", "attachment; filename=GiayRaVien" + DateTime.Now.Year + ".xls");
                    Response.ContentType = "application/vnd.ms-excel";
                    return new FileContentResult(bytes, "application/vnd.ms-excel");

                case LoaiChungTuXuatExcel.GiayNghiHuongBHXH:                   
                    var byteChungSinhs = _goiBaoHiemYTeService.ExportGiayNghiHuongBHXH(excelChungTuQueryInfo);
                    HttpContext.Response.Headers.Add("content-disposition", "attachment; filename=GiayNghiHuongBHXH" + DateTime.Now.Year + ".xls");
                    Response.ContentType = "application/vnd.ms-excel";
                    return new FileContentResult(byteChungSinhs, "application/vnd.ms-excel");

                case LoaiChungTuXuatExcel.GiayNghiDuongThai:
                    var byteDuongThais = _goiBaoHiemYTeService.ExportGiayNghiDuongThai(excelChungTuQueryInfo);
                    HttpContext.Response.Headers.Add("content-disposition", "attachment; filename=GiayNghiDuongThai" + DateTime.Now.Year + ".xls");
                    Response.ContentType = "application/vnd.ms-excel";
                    return new FileContentResult(byteDuongThais, "application/vnd.ms-excel");     
                    
                case LoaiChungTuXuatExcel.TomTatBenhAn:
                    var byteTomTatBenhAns = _goiBaoHiemYTeService.ExportGiayTomTatBenhAn(excelChungTuQueryInfo);
                    HttpContext.Response.Headers.Add("content-disposition", "attachment; filename=TomTatBenhAn" + DateTime.Now.Year + ".xls");
                    Response.ContentType = "application/vnd.ms-excel";
                    return new FileContentResult(byteTomTatBenhAns, "application/vnd.ms-excel");

                case LoaiChungTuXuatExcel.GiayChungSinh:
                    var byteGiayChungSinhs = _goiBaoHiemYTeService.ExportGiayChungSinh(excelChungTuQueryInfo);
                    HttpContext.Response.Headers.Add("content-disposition", "attachment; filename=GiayChungSinh" + DateTime.Now.Year + ".xls");
                    Response.ContentType = "application/vnd.ms-excel";
                    return new FileContentResult(byteGiayChungSinhs, "application/vnd.ms-excel");       
                    
                default:
                    //do a different thing
                    break;
            }
            return null;
        }
    }
}
