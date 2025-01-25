using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Camino.Services.HoatDongNhanVien;
using Camino.Api.Models.KhoaPhong;
using Camino.Core.Domain.Entities.NhanViens;
using System;
using Camino.Api.Extensions;
using Camino.Services.Helpers;
using System.Collections.Generic;
using Camino.Core.Domain.ValueObject;

namespace Camino.Api.Controllers
{
    public class HoatDongNhanVienController : CaminoBaseController
    {
        private readonly IHoatDongNhanVienService _hoatDongNhanVienService;
        private readonly ILichSuHoatDongNhanVienService _lichSuHoatDongNhanVienService;
        private readonly IUserAgentHelper _userAgentHelper;

        public HoatDongNhanVienController
            (IHoatDongNhanVienService hoatDongNhanVienService,
             IUserAgentHelper userAgentHelper,
             ILichSuHoatDongNhanVienService lichSuHoatDongNhanVienService
            )
        {
            _lichSuHoatDongNhanVienService = lichSuHoatDongNhanVienService;
            _hoatDongNhanVienService = hoatDongNhanVienService;
            _userAgentHelper = userAgentHelper;
        }

        [HttpPost("LuuHoatDongNhanVien")]
        public async Task<ActionResult> LuuHoatDongNhanVien([FromBody]PhongNhanVienViewModel model)
        {
            if (model.PhongKhamBenhId != 0)
            {
                var hoatDongNhanVien = await _hoatDongNhanVienService.LuuHoatDongNhanVienAsync(_userAgentHelper.GetCurrentUserId(), model.PhongKhamBenhId);
                return Ok(hoatDongNhanVien);
            }
            return Ok();
        }

        [HttpGet("GetPhongBenhVienByCurrentUser")]
        public async Task<ActionResult<LookupItemVo>> GetPhongBenhVienByCurrentUser()
        {
            var lookup = await _hoatDongNhanVienService.GetPhongBenhVienByCurrentUser();
            return Ok(lookup);
        }

        [HttpGet("GetPhongChinhLamViec")]
        public async Task<ActionResult<LookupItemVo>> GetPhongChinhLamViec()
        {
            var lookup = await _hoatDongNhanVienService.GetPhongChinhLamViec();
            return Ok(lookup);
        }

    }
}

