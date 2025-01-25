using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Camino.Core.Domain.ValueObject.ExportExcelItemVo;
using Camino.Core.Helpers;
using Camino.Services.ExportImport;
using Camino.Services.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Camino.Api.Controllers
{
    public class TestController : CaminoBaseController
    {
        private readonly IRoleService _roleService;
        private readonly IExcelService _excelService;

        public TestController(IRoleService roleService, IExcelService excelService)
        {
            _roleService = roleService;
            _excelService = excelService;
        }
        // GET
        [AllowAnonymous]
        [HttpGet("{value}")]
        public IActionResult TryParseAuditHopDongThueVanPhong(string value)
        {
            var result = CommonHelper.DecodeHexString(value);
            return Ok(result);
        }

        // GET
        [AllowAnonymous]
        [HttpGet]
        public IActionResult TryParseUnicode(string value)
        {
            var result = value.ConvertUnicodeString();
            return Ok(result);
        }

        // GET
        [AllowAnonymous]
        [HttpGet("Fulltext")]
        public IActionResult Fulltext()
        {
            var result = _roleService.Test();
            return Ok(result);
        }

        // GET
        [AllowAnonymous]
        [HttpGet("UpperCase")]
        public IActionResult UpperCase()
        {
            var testStr = "day la ban thu nghiem";
            var result = testStr.ToUpperCaseTheFirstCharacter();
            return Ok(result);
        }

        // GET
        [AllowAnonymous]
        [HttpPost("ExportExcel")]
        public IActionResult ExportExcel()
        {
            var testModel = new List<BenhNhanExportExcel>();
            testModel.Add(new BenhNhanExportExcel
            {
                DiaChi = "Địa chỉ",
                HoTen = "Lê Hồng Vũ",
                GioiTinh = "Nam",
                Email = "lehongvutho@gmai.com",
            });

            testModel.Add(new BenhNhanExportExcel
            {
                DiaChi = "Địa chỉ 2",
                HoTen = "Lê Hồng Vũ 2",
                GioiTinh = "Nam 2",
                Email = "lehongvutho2@gmai.com",
            });

            testModel.Add(new BenhNhanExportExcel
            {
                DiaChi = "Địa chỉ 3",
                HoTen = "Lê Hồng Vũ",
                GioiTinh = "Nam 3",
                Email = "lehongvutho3@gmai.com",
            });

            testModel.Add(new BenhNhanExportExcel
            {
                DiaChi = "Địa chỉ 34Địa chỉ 34Địa chỉ 34Địa chỉ 34Địa chỉ 34Địa chỉ 34Địa chỉ 34",
                HoTen = "Lê Hồng Vũ 4",
                GioiTinh = "Nam 34",
                Email = "lehongvutho4@gmai.com",
            });

            testModel.Add(new BenhNhanExportExcel
            {
                DiaChi = "Địa chỉ 123123123",
                HoTen = "Lê Hồng VũLê Hồng VũLê Hồng VũLê Hồng VũLê Hồng VũLê Hồng VũLê Hồng Vũ",
                GioiTinh = "Nam",
                Email = "lehongvutho@gmai.com",
            });

            //testModel[0].BenhNhanExportExcelChild.Add(new BenhNhanExportExcelChild
            //{
            //    HoTen = "Child Child Child Vu Le",
            //});
            //testModel[0].BenhNhanExportExcelChild.Add(new BenhNhanExportExcelChild
            //{
            //    HoTen = "Child Child Child Vu Le",
            //});

            //testModel[0].BenhNhanExportExcelChild.Add(new BenhNhanExportExcelChild
            //{
            //    HoTen = "hahaha",
            //});

            //testModel[1].BenhNhanExportExcelChild.Add(new BenhNhanExportExcelChild
            //{
            //    HoTen = "Child Child Child Vu Le 2",
            //});

            //testModel[2].BenhNhanExportExcelChild.Add(new BenhNhanExportExcelChild
            //{
            //    HoTen = "Child Child Child Vu Le",
            //});
            //, ("BenhNhanExportExcelChild", "Child")
            var lstValueObject = new List<(string, string)> {("HoTen", "Họ Tên"), ("DiaChi", "Địa Chỉ")};

            var result = _excelService.ExportManagermentView(testModel, lstValueObject, "Người bệnh");
            return new FileContentResult(result, "application/vnd.ms-excel");
            //return Ok(result);
        }
    }
}