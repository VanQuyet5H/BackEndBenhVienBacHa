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
using Camino.Services.Users;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Camino.Api.Controllers
{

    public class UserInternalController : CaminoBaseController
    {
        readonly IUserService _userService;
        public UserInternalController(IUserService userService, IJwtFactory iJwtFactory)
        {
            _userService = userService;
        }

        [HttpPost]
        [ClaimRequirement(Enums.SecurityOperation.Add, Enums.DocumentType.User)]
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
          
            var gridData = await _userService.GetDataInternalForGridAsync(queryInfo);
            return Ok(gridData);
        }


        [HttpPost("GetTotalPageForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.User)]
        public async Task<ActionResult<GridDataSource>> GetTotalPageForGridAsync([FromBody]QueryInfo queryInfo)
        {
         
            var gridData = await _userService.GetTotalPageInternalForGridAsync(queryInfo);
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
        public async Task<ActionResult> UpdateTaiKhoan(UserUpdateViewModel userViewModel)
        {
            var user = await _userService.GetByIdAsync(userViewModel.Id, s => s.Include(u => u.NhanVien));
            
            if(!string.IsNullOrEmpty(userViewModel.Phone))
                user.SoDienThoai = userViewModel.Phone;
            if (!string.IsNullOrEmpty(userViewModel.Email))
                user.Email = userViewModel.Email;
            await _userService.UpdateAsync(user);
            return NoContent();
        }
    }
}