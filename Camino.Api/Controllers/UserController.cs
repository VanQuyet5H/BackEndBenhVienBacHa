using System.Collections.Generic;
using System.Threading.Tasks;
using Camino.Api.Auth;
using Camino.Api.Extensions;
using Camino.Api.Infrastructure.Auth;
using Camino.Api.Models.Users;
using Camino.Core.Domain;
using Camino.Core.Domain.Entities.Users;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Helpers;
using Camino.Services.HoatDongNhanVien;
using Camino.Services.PhongBenhVien;
using Camino.Services.Users;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Camino.Api.Controllers
{

    public partial class UserController : CaminoBaseController
    {
        readonly IUserService _userService;
        readonly IPhongBenhVienService _phongBenhVienService;
        readonly IHoatDongNhanVienService _hoatDongNhanVienService;

        public UserController(IUserService userService, IJwtFactory iJwtFactory,
            IPhongBenhVienService phongBenhVienService, IHoatDongNhanVienService hoatDongNhanVienService)
        {
            _userService = userService;
            _phongBenhVienService = phongBenhVienService;
            _hoatDongNhanVienService = hoatDongNhanVienService;
        }

        [HttpPost]
        //[ClaimRequirement(Enums.SecurityOperation.Add, Enums.DocumentType.User)]
        public ActionResult<UserViewModel> Post([FromBody] UserViewModel userViewModel)
        {
            var user = userViewModel.ToEntity<User>();
            _userService.Add(user);
            var us = _userService.GetById(user.Id);
            var actionName = nameof(Get);
            return CreatedAtAction(actionName, new { id = user.Id }, us.ToModel<UserViewModel>());
        }

        /// <summary>
        ///     Get user
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.User)]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<ActionResult<UserViewModel>> Get(long id)
        {
            var user = await _userService.GetByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            return Ok(user.ToModel<UserViewModel>());
        }
        [HttpPut]
        [ClaimRequirement(Enums.SecurityOperation.Update, Enums.DocumentType.User)]
        public async Task<ActionResult> Put([FromBody] UserViewModel userViewModel)
        {
            var user = await _userService.GetByIdAsync(userViewModel.Id);

            if (user == null)
            {
                return NotFound();
            }
            userViewModel.ToEntity(user);
            await _userService.UpdateAsync(user);
            return NoContent();
        }
        [HttpDelete("{id}")]
        [ClaimRequirement(Enums.SecurityOperation.Delete, Enums.DocumentType.User)]
        public async Task<ActionResult> Delete(long id)
        {
            var user = await _userService.GetByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            await _userService.DeleteByIdAsync(id);
            return NoContent();
        }

        [HttpPost("GetDataForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.User)]
        public async Task<ActionResult<GridDataSource>> GetDataForGridAsync([FromBody]QueryInfo queryInfo)
        {

            var gridData = await _userService.GetDataForGridAsync(queryInfo);
            return Ok(gridData);
        }


        [HttpPost("GetTotalPageForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.User)]
        public async Task<ActionResult<GridDataSource>> GetTotalPageForGridAsync([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _userService.GetTotalPageForGridAsync(queryInfo);
            return Ok(gridData);
        }


        [HttpPost("KhoaTaiKhoan")]
        [ClaimRequirement(Enums.SecurityOperation.Update, Enums.DocumentType.User)]
        public async Task<ActionResult> KhoaTaiKhoan(long id)
        {
            var user = await _userService.GetByIdAsync(id);
            user.IsActive = !user.IsActive;
            //    nhanvien.KichHoat = false;
            await _userService.UpdateAsync(user);
            return NoContent();
        }
        [HttpGet("CheckPhone")]
        public async Task<ActionResult<bool>> CheckPhone(string phone)
        {
            var isExist = await _userService.CheckIsExistPhone(phone, (int)Enums.Region.Internal);
            return Ok(isExist);
        }
        [HttpGet("GetUserAfter")]
        public async Task<ActionResult<ICollection<User>>> GetUserAfter(long after, int limit, string searchString)
        {
            var gridData = await _userService.GetUserAfter(after, limit, searchString);
            return Ok(gridData);
        }
        [HttpPost("UpdateTaiKhoan")]
        public async Task<ActionResult> UpdateTaiKhoan(UserUpdateExternalViewModel userViewModel)
        {
            var user = await _userService.GetByIdAsync(userViewModel.Id);
            if (!string.IsNullOrEmpty(userViewModel.Phone))
                user.SoDienThoai = userViewModel.Phone;
            if (!string.IsNullOrEmpty(userViewModel.Email))
                user.Email = userViewModel.Email;
            await _userService.UpdateAsync(user);
            return NoContent();
        }


        [HttpPost("HoatDongHienTaiNhanVien")]
        public async Task<ActionResult<long>> HoatDongHienTaiNhanVien(long idNhanVien)
        {
            var hoatDongPhongs = _hoatDongNhanVienService.GetHoatDongNhanVienByNhanVien(idNhanVien);
            if (hoatDongPhongs != null)
            {
                return hoatDongPhongs.PhongBenhVienId;
            }
            return 0;
        }

        [HttpPost("GetUserAndDepartmentInformation")]
        public async Task<ActionResult> GetUserAndDepartmentInformation(long idNhanVien, long idPhongBenhVien)
        {
            var isChucDanhEmpty = false;
            var user = await _userService.GetByIdAsync(idNhanVien,
                s => s.Include(k => k.NhanVien).ThenInclude(k => k.ChucDanh).ThenInclude(h => h.NhomChucDanh));

            if (user.NhanVien.ChucDanh == null)
            {
                isChucDanhEmpty = true;
            }

            if (idPhongBenhVien == 0)
            {
                return Ok(new UserAndDepartmentViewModel
                {
                    User = user.SoDienThoai.ApplyFormatPhone(),
                    Ten = user.HoTen,
                    ChucDanh = user.NhanVien.ChucDanh?.NhomChucDanh?.Ten,
                    TenKhoaPhong = null,
                    MaKhoaPhong = null,
                    IsChucDanhEmpty = isChucDanhEmpty,
                    IsKhoaPhongEmpty = true,
                    IsPhongEmpty = true,
                });
            }

            var department = await _phongBenhVienService.GetByIdAsync(idPhongBenhVien,
            s => s.Include(k => k.KhoaPhong));

            return Ok(new UserAndDepartmentViewModel
            {
                User = user.SoDienThoai.ApplyFormatPhone(),
                Ten = user.HoTen,
                ChucDanh = user.NhanVien.ChucDanh?.NhomChucDanh?.Ten,
                TenKhoaPhong = department.KhoaPhong.Ten,
                MaKhoaPhong = department.KhoaPhong.Ma,
                IsChucDanhEmpty = isChucDanhEmpty,
                IsKhoaPhongEmpty = false,
                KhoaId = department.KhoaPhongId,
                TenPhong = department.Ten,
                MaPhong = department.Ma,
                IsPhongEmpty = false,
                PhongBenhVienId = idPhongBenhVien
            });
        }
    }
}