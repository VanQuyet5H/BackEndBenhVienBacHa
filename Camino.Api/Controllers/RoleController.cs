using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Camino.Api.Auth;
using Camino.Api.Extensions;
using Camino.Api.Models.Error;
using Camino.Api.Models.Users;
using Camino.Core.Domain;
using Camino.Core.Domain.Entities.Users;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Domain.ValueObject.Users;
using Camino.Core.Helpers;
using Camino.Services.Users;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Camino.Api.Controllers
{
    public class RoleController : CaminoBaseController
    {
        private readonly IRoleService _roleService;

        public RoleController(IRoleService roleService)
        {
            _roleService = roleService;
        }

        [HttpPost("GetLookup")]
        public async Task<ActionResult<ICollection<LookupItemVo>>> GetLookup()
        {
            var lookup = await _roleService.GetLookupAsync();
            return Ok(lookup);
        }

        #region CRUD

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetDataForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.Role)]
        public async Task<ActionResult<GridDataSource>> GetDataForGridAsync([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _roleService.GetDataForGridAsync(queryInfo);
            return Ok(gridData);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetTotalPageForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.Role)]
        public async Task<ActionResult<GridDataSource>> GetTotalPageForGridAsync([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _roleService.GetTotalPageForGridAsync(queryInfo);
            return Ok(gridData);
        }


        /// <summary>
        ///     Get role
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<RoleViewModel>> Get(long id)
        {
            var result = await _roleService.GetByIdAsync(id, s => s.Include(u => u.RoleFunctions));
            if (result == null)
            {
                return NotFound();
            }
            var r = result.ToModel<RoleViewModel>();

            //Chỉ hiển thị document type trong nội bộ
            AddDefaultPermission(r);
            foreach (var roleFunction in r.RoleFunctions.Where(o=>(int)o.DocumentType<=1000))
            {
                if (r.RoleFunctionGrids.Any(p => p.DocumentType == roleFunction.DocumentType))
                {
                    var roleFunctionGrid = r.RoleFunctionGrids.Find(p => p.DocumentType == roleFunction.DocumentType);
                    roleFunctionGrid.DocumentType = roleFunctionGrid.DocumentType;
                    roleFunctionGrid.DocumentName = roleFunctionGrid.DocumentType.GetDescription();
                    AddPermissionForRoleFunctionGirds(roleFunctionGrid, roleFunction.SecurityOperation);
                }
                else
                {
                    var roleFunctionToAdd = new RoleFunctionGrids
                    {
                        RoleId = roleFunction.RoleId,
                        DocumentType = roleFunction.DocumentType,
                        DocumentName = roleFunction.DocumentType.GetDescription(),
                    };
                    AddPermissionForRoleFunctionGirds(roleFunctionToAdd, roleFunction.SecurityOperation);
                    r.RoleFunctionGrids.Add(roleFunctionToAdd);
                }
            }
            //check all ?
            r.IsCheckAll = (r.RoleFunctions.Any() && r.RoleFunctions.Count(p => p.SecurityOperation == Enums.SecurityOperation.Add ||
                                            p.SecurityOperation == Enums.SecurityOperation.Delete
                                            || p.SecurityOperation == Enums.SecurityOperation.Update ||
                                            p.SecurityOperation == Enums.SecurityOperation.View) >=
                            (r.RoleFunctionGrids.Count * 4));
            return Ok(r);
        }
        
        [HttpPost]
        [ClaimRequirement(Enums.SecurityOperation.Add, Enums.DocumentType.Role)]
        public async Task<ActionResult<RoleViewModel>> Post([FromBody] RoleViewModel roleViewModel)
        {
            var lstRoleFunction = GetListRoleFuntion(roleViewModel);
            if (lstRoleFunction.All(p => p.SecurityOperation != Enums.SecurityOperation.View))
            {
                throw new ApiException("Phải chọn ít nhất một quyền xem", (int)HttpStatusCode.BadRequest);
            }
            var role = roleViewModel.ToEntity<Role>();
            _roleService.Add(role);
            //add permission for role
            await _roleService.AddPermissionForRole(lstRoleFunction, role.Id);
            var actionName = nameof(Get);
            return CreatedAtAction(actionName, new { id = role.Id }, role.ToModel<RoleViewModel>());
        }

        [HttpPut]
        [ClaimRequirement(Enums.SecurityOperation.Update, Enums.DocumentType.Role)]
        public async Task<ActionResult> Put([FromBody] RoleViewModel roleViewModel)
        {
            var result = await _roleService.GetByIdAsync(roleViewModel.Id);
            if (result == null || result.IsDefault)
            {
                return NotFound();
            }


            var lstRoleFunction = GetListRoleFuntion(roleViewModel);
            if (lstRoleFunction.All(p => p.SecurityOperation != Enums.SecurityOperation.View))
            {
                throw new ApiException("Phải chọn ít nhất một quyền xem", (int)HttpStatusCode.BadRequest);
            }
            roleViewModel.ToEntity(result);

            await _roleService.UpdateRoleFunctionForRole(lstRoleFunction, result.Id).ConfigureAwait(false);

            await _roleService.UpdateAsync(result);
            return NoContent();
        }
        
        [HttpGet("GetRoleFunctionForAdd")]
        public ActionResult<RoleViewModel> GetRoleFunctionForAdd()
        {
            var roleViewModel = new RoleViewModel();
            AddDefaultPermission(roleViewModel);
            return Ok(roleViewModel.RoleFunctionGrids);
        }


        /// <summary>
        ///     Xoa role
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        [ClaimRequirement(Enums.SecurityOperation.Delete, Enums.DocumentType.Role)]
        public async Task<ActionResult> Delete(long id)
        {
            var role = await _roleService.GetByIdAsync(id, s => s.Include(u => u.NhanVienRoles).Include(u => u.RoleFunctions));
            if (role == null || role.IsDefault)
            {
                return NotFound();
            }
            await _roleService.DeleteAsync(role);
            return NoContent();
        }
        #endregion CRUD

        #region Private Class

        private void AddPermissionForRoleFunctionGirds(RoleFunctionGrids result, Enums.SecurityOperation securityOperation)
        {
            switch (securityOperation)
            {
                case Enums.SecurityOperation.None:
                    break;
                case Enums.SecurityOperation.View:
                    result.IsView = true;
                    break;
                case Enums.SecurityOperation.Process:
                    break;
                case Enums.SecurityOperation.Add:
                    result.IsInsert = true;
                    break;
                case Enums.SecurityOperation.Delete:
                    result.IsDelete = true;
                    break;
                case Enums.SecurityOperation.Update:
                    result.IsUpdate = true;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(securityOperation), securityOperation, null);
            }
        }
        /// <summary>
        /// Chỉ hiển thị document type trong nội bộ
        /// </summary>
        /// <param name="r"></param>
        private void AddDefaultPermission(RoleViewModel r)
        {
            var lstDocumentEnums = Enum.GetValues(typeof(Enums.DocumentType)).Cast<Enum>();
            var result = lstDocumentEnums.Where(o=> Convert.ToInt32(o) <= 1000).Select(item => new LookupItemVo
            {
                DisplayName = item.GetDescription(),
                KeyId = Convert.ToInt32(item),
            })
                .ToList();
            foreach (var documentType in result)
            {
                if ((Enums.DocumentType)documentType.KeyId == Enums.DocumentType.None) continue;
                r.RoleFunctionGrids.Add(new RoleFunctionGrids
                {
                    DocumentType = (Enums.DocumentType)documentType.KeyId,
                    DocumentName = documentType.DisplayName,
                    RoleId = r.Id
                });
            }
        }

        private List<RoleFunction> GetListRoleFuntion(RoleViewModel roleViewModel)
        {
            var listRoleFunction = new List<RoleFunction>();
            foreach (var roleFunctionGrid in roleViewModel.RoleFunctionGrids)
            {
                if (roleFunctionGrid.IsView)
                {
                    listRoleFunction.Add(new RoleFunction
                    {
                        DocumentType = roleFunctionGrid.DocumentType,
                        SecurityOperation = Enums.SecurityOperation.View,
                        RoleId = roleFunctionGrid.RoleId
                    });
                }
                if (roleFunctionGrid.IsInsert)
                {
                    listRoleFunction.Add(new RoleFunction
                    {
                        DocumentType = roleFunctionGrid.DocumentType,
                        SecurityOperation = Enums.SecurityOperation.Add,
                        RoleId = roleFunctionGrid.RoleId
                    });
                }
                if (roleFunctionGrid.IsUpdate)
                {
                    listRoleFunction.Add(new RoleFunction
                    {
                        DocumentType = roleFunctionGrid.DocumentType,
                        SecurityOperation = Enums.SecurityOperation.Update,
                        RoleId = roleFunctionGrid.RoleId
                    });
                }
                if (roleFunctionGrid.IsDelete)
                {
                    listRoleFunction.Add(new RoleFunction
                    {
                        DocumentType = roleFunctionGrid.DocumentType,
                        SecurityOperation = Enums.SecurityOperation.Delete,
                        RoleId = roleFunctionGrid.RoleId
                    });
                }
            }

            return listRoleFunction;
        }
        #endregion Private Class

        [HttpPost("GetRoleTypeNhanVienNoiBoAsync")]
        public async Task<ActionResult<ICollection<LookupItemVo>>> GetRoleTypeNhanVienNoiBoAsync()
        {
            var lookup = await _roleService.GetRoleTypeNhanVienNoiBoAsync();
            return Ok(lookup);
        }

        [HttpPost("GetRoleTypeKhachVanLaiBoAsync")]
        public ActionResult GetRoleTypeKhachVanLaiBoAsync()
        {
            var lookup = _roleService.GetRoleTypeKhachVanLaiBoAsync();
            return Ok(lookup);
        }

        [HttpPost("GetRoleTypeAsync")]
        public ActionResult<ICollection<LookupItemVo>> GetRoleTypeAsync()
        {
            var values = ((Enums.UserType[])Enum.GetValues(typeof(Enums.UserType))).ToList();

            var data = values.Select(s => new LookupItemVo()
            {
                KeyId = (values.IndexOf(s) + 1),
                DisplayName = s.GetDescription()
            }).ToList();
            data.Insert(0, new LookupItemVo
            {
                KeyId =0,
                DisplayName = "Tất cả"
            });
            return Ok(data);
        }

        [HttpPost("GetRoleBenNgoai")]
        public ActionResult<ICollection<LookupItemVo>> GetRoleBenNgoai()
        {
            var listHinhThucThanhToan = EnumHelper.GetListEnum<Enums.UserType>();

            var result = listHinhThucThanhToan.Select(item => new LookupItemVo
            {
                DisplayName = item.GetDescription(),
                KeyId = Convert.ToInt32(item),
            }).Where(o => o.KeyId != (int)Enums.UserType.NhanVien).ToList();

            result.Insert(0, new LookupItemVo
            {
                KeyId = 0,
                DisplayName = "Tất cả"
            });
            return Ok(result);
        }
        
        
        [HttpPost("GetRoleQuyenHanNhanVienAsync")]
        public async Task<ActionResult<ICollection<LookupItemTextVo>>> GetRoleQuyenHanNhanVienAsync()
        {
            var lookup = await _roleService.GetRoleQuyenHanNhanVienAsync();
            return Ok(lookup);
        }


    }
}
