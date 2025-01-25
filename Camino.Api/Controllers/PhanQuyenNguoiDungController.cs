using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Camino.Api.Auth;
using Camino.Api.Extensions;
using Camino.Api.Models.PhanQuyenNguoiDungs;
using Camino.Core.Caching;
using Camino.Core.Domain;
using Camino.Core.Domain.Entities.Users;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.ExportExcelItemVo;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Domain.ValueObject.PhanQuyenNguoiDungs;
using Camino.Core.Helpers;
using Camino.Services.ExportImport;
using Camino.Services.Helpers;
using Camino.Services.PhanQuyenNguoiDungs;
using Camino.Services.Users;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Camino.Api.Controllers
{
    public class PhanQuyenNguoiDungController : CaminoBaseController
    {
        private readonly IPhanQuyenNguoiDungService _phanQuyenNguoiDungService;
        private readonly IExcelService _excelService;
        private readonly IStaticCacheManager _cacheManager;

        public PhanQuyenNguoiDungController(IPhanQuyenNguoiDungService phanQuyenNguoiDungService, IExcelService excelService, IStaticCacheManager cacheManager)
        {
            _phanQuyenNguoiDungService = phanQuyenNguoiDungService;
            _excelService = excelService;
            _cacheManager = cacheManager;
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetDataForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.Role)]
        public async Task<ActionResult<GridDataSource>> GetDataForGridAsync
            ([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _phanQuyenNguoiDungService.GetDataForGridAsync(queryInfo);
            return Ok(gridData);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetTotalPageForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.Role)]
        public async Task<ActionResult<GridDataSource>> GetTotalPageForGridAsync
            ([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _phanQuyenNguoiDungService.GetTotalPageForGridAsync(queryInfo);
            return Ok(gridData);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpGet("GetListDocumentType")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.Role)]
        public ActionResult GetListDocumentType()
        {
            var listDocumentType = _phanQuyenNguoiDungService.GetListDocumentType();
            return Ok(listDocumentType);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetListUserType")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.Role)]
        public ActionResult GetListUserType([FromBody]LookupQueryInfo model)
        {
            var listUserType = _phanQuyenNguoiDungService.GetListUserType(model);
            return Ok(listUserType);
        }

        [HttpGet("{id}")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.Role)]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<ActionResult<PhanQuyenNguoiDungViewModel>> Get(long id)
        {
            var phanQuyenNguoiDung = await _phanQuyenNguoiDungService.GetByIdAsync(id, x => x.Include(w => w.RoleFunctions));
            if (phanQuyenNguoiDung == null)
            {
                return NotFound();
            }

            var phanQuyenNguoiDungViewModel = phanQuyenNguoiDung.ToModel<PhanQuyenNguoiDungViewModel>();

            var roleEntityFunctionPrev = new RoleFunction();
            DocumentTypeViewModel roleFunctionModel = new DocumentTypeViewModel();

            foreach (var roleEntityFunction in phanQuyenNguoiDung.RoleFunctions.OrderBy(x => x.DocumentType))
            {
                if (roleEntityFunctionPrev.DocumentType == Enums.DocumentType.None)
                {
                    roleFunctionModel.DocumentType = roleEntityFunction.DocumentType;
                    if (roleEntityFunction.SecurityOperation == Enums.SecurityOperation.View)
                    {
                        roleFunctionModel.IsView = true;
                    }
                    else if (roleEntityFunction.SecurityOperation == Enums.SecurityOperation.Process)
                    {
                        roleFunctionModel.IsProcess = true;
                    }
                    else if (roleEntityFunction.SecurityOperation == Enums.SecurityOperation.Delete)
                    {
                        roleFunctionModel.IsDelete = true;
                    }
                    else if (roleEntityFunction.SecurityOperation == Enums.SecurityOperation.Add)
                    {
                        roleFunctionModel.IsInsert = true;
                    }
                    else if (roleEntityFunction.SecurityOperation == Enums.SecurityOperation.Update)
                    {
                        roleFunctionModel.IsUpdate = true;
                    }
                }
                else if (roleEntityFunctionPrev.DocumentType != roleEntityFunction.DocumentType)
                {
                    phanQuyenNguoiDungViewModel.DocumentTypes.Add(roleFunctionModel);
                    roleFunctionModel = new DocumentTypeViewModel
                    {
                        DocumentType = roleEntityFunction.DocumentType,
                        IsView = roleEntityFunction.SecurityOperation == Enums.SecurityOperation.View,
                        IsDelete = roleEntityFunction.SecurityOperation == Enums.SecurityOperation.Delete,
                        IsInsert = roleEntityFunction.SecurityOperation == Enums.SecurityOperation.Add,
                        IsProcess = roleEntityFunction.SecurityOperation == Enums.SecurityOperation.Process,
                        IsUpdate = roleEntityFunction.SecurityOperation == Enums.SecurityOperation.Update
                    };
                }
                else if (roleEntityFunctionPrev.DocumentType == roleEntityFunction.DocumentType)
                {
                    if (roleEntityFunction.SecurityOperation == Enums.SecurityOperation.View)
                    {
                        roleFunctionModel.IsView = true;
                    }
                    else if (roleEntityFunction.SecurityOperation == Enums.SecurityOperation.Process)
                    {
                        roleFunctionModel.IsProcess = true;
                    }
                    else if (roleEntityFunction.SecurityOperation == Enums.SecurityOperation.Delete)
                    {
                        roleFunctionModel.IsDelete = true;
                    }
                    else if (roleEntityFunction.SecurityOperation == Enums.SecurityOperation.Add)
                    {
                        roleFunctionModel.IsInsert = true;
                    }
                    else if (roleEntityFunction.SecurityOperation == Enums.SecurityOperation.Update)
                    {
                        roleFunctionModel.IsUpdate = true;
                    }
                }

                roleEntityFunctionPrev = roleEntityFunction;
            }
            phanQuyenNguoiDungViewModel.DocumentTypes.Add(roleFunctionModel);
            phanQuyenNguoiDungViewModel.UserTypeDisplay = phanQuyenNguoiDungViewModel.UserType.GetDescription();

            return Ok(phanQuyenNguoiDungViewModel);
        }

        [HttpDelete("{id}")]
        [ClaimRequirement(Enums.SecurityOperation.Delete, Enums.DocumentType.Role)]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<ActionResult> Delete(long id)
        {
            var phanQuyenNguoiDung = await _phanQuyenNguoiDungService.GetByIdAsync(id, x => x.Include(w => w.RoleFunctions));
            if (phanQuyenNguoiDung == null)
            {
                return NotFound();
            }

            await _phanQuyenNguoiDungService.DeleteByIdAsync(id);
            _cacheManager.RemoveByPattern(RoleService.ROLES_PATTERN_KEY);
            return NoContent();
        }

        [HttpPost]
        [ClaimRequirement(Enums.SecurityOperation.Add, Enums.DocumentType.Role)]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<ActionResult<PhanQuyenNguoiDungViewModel>> Post
            ([FromBody]PhanQuyenNguoiDungViewModel phanQuyenViewModel)
        {
            var phanQuyenEntity = phanQuyenViewModel.ToEntity<Role>();
            var phanQuyenModelPrev = new DocumentTypeViewModel();

            foreach (var phanQuyenItemModel in phanQuyenViewModel.DocumentTypes.Where(x => x.DocumentType != null).OrderBy(x => x.DocumentType))
            {
                if (phanQuyenModelPrev.DocumentType == null || phanQuyenModelPrev.DocumentType != phanQuyenItemModel.DocumentType)
                {
                    AddNewRoleFunction(phanQuyenItemModel, phanQuyenEntity);
                }
                else if (phanQuyenModelPrev.DocumentType == phanQuyenItemModel.DocumentType)
                {
                    phanQuyenModelPrev = phanQuyenItemModel;
                    continue;
                }

                phanQuyenModelPrev = phanQuyenItemModel;
            }
            await _phanQuyenNguoiDungService.AddRoleAsync(phanQuyenEntity);
            _cacheManager.RemoveByPattern(RoleService.ROLES_PATTERN_KEY);
            return Ok();
        }

        private void AddNewRoleFunction(DocumentTypeViewModel phanQuyenItemModel, Role phanQuyenEntity)
        {
            if (phanQuyenItemModel.IsDelete == true)
            {
                var roleFunctionEntity = new RoleFunction
                {
                    DocumentType = phanQuyenItemModel.DocumentType.GetValueOrDefault(),
                    RoleId = phanQuyenEntity.Id,
                    SecurityOperation = Enums.SecurityOperation.Delete
                };
                phanQuyenEntity.RoleFunctions.Add(roleFunctionEntity);
            }

            if (phanQuyenItemModel.IsInsert == true)
            {
                var roleFunctionEntity = new RoleFunction
                {
                    DocumentType = phanQuyenItemModel.DocumentType.GetValueOrDefault(),
                    RoleId = phanQuyenEntity.Id,
                    SecurityOperation = Enums.SecurityOperation.Add
                };
                phanQuyenEntity.RoleFunctions.Add(roleFunctionEntity);
            }

            if (phanQuyenItemModel.IsProcess == true)
            {
                var roleFunctionEntity = new RoleFunction
                {
                    DocumentType = phanQuyenItemModel.DocumentType.GetValueOrDefault(),
                    RoleId = phanQuyenEntity.Id,
                    SecurityOperation = Enums.SecurityOperation.Process
                };
                phanQuyenEntity.RoleFunctions.Add(roleFunctionEntity);
            }

            if (phanQuyenItemModel.IsUpdate == true)
            {
                var roleFunctionEntity = new RoleFunction
                {
                    DocumentType = phanQuyenItemModel.DocumentType.GetValueOrDefault(),
                    RoleId = phanQuyenEntity.Id,
                    SecurityOperation = Enums.SecurityOperation.Update
                };
                phanQuyenEntity.RoleFunctions.Add(roleFunctionEntity);
            }

            if (phanQuyenItemModel.IsView == true)
            {
                var roleFunctionEntity = new RoleFunction
                {
                    DocumentType = phanQuyenItemModel.DocumentType.GetValueOrDefault(),
                    RoleId = phanQuyenEntity.Id,
                    SecurityOperation = Enums.SecurityOperation.View
                };
                phanQuyenEntity.RoleFunctions.Add(roleFunctionEntity);
            }
        }

        [HttpPut]
        [ClaimRequirement(Enums.SecurityOperation.Update, Enums.DocumentType.Role)]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<ActionResult> CapNhatQuyen([FromBody]PhanQuyenNguoiDungViewModel phanQuyenViewModel)
        {
            var quyenHanEntity = await _phanQuyenNguoiDungService.GetByIdAsync(phanQuyenViewModel.Id, x => x.Include(w => w.RoleFunctions));
            phanQuyenViewModel.ToEntity(quyenHanEntity);

            foreach (var phanQuyenDeleteModel in phanQuyenViewModel.DocumentTypes.Where(x => x.DocumentType != null && x.IsChange == true &&
                 (x.IsDelete == false || x.IsInsert == false || x.IsProcess == false || x.IsUpdate == false || x.IsView == false)).OrderBy(x => x.DocumentType))
            {
                foreach (var quyenHanDeleteEntity in quyenHanEntity.RoleFunctions.Where(x => x.DocumentType == phanQuyenDeleteModel.DocumentType))
                {
                    if (quyenHanDeleteEntity.SecurityOperation == Enums.SecurityOperation.Delete &&
                        phanQuyenDeleteModel.IsDelete == false)
                    {
                        quyenHanDeleteEntity.WillDelete = true;
                    }
                    else if (quyenHanDeleteEntity.SecurityOperation == Enums.SecurityOperation.Add &&
                              phanQuyenDeleteModel.IsInsert == false)
                    {
                        quyenHanDeleteEntity.WillDelete = true;
                    }
                    else if (quyenHanDeleteEntity.SecurityOperation == Enums.SecurityOperation.Update &&
                             phanQuyenDeleteModel.IsUpdate == false)
                    {
                        quyenHanDeleteEntity.WillDelete = true;
                    }
                    else if (quyenHanDeleteEntity.SecurityOperation == Enums.SecurityOperation.Process &&
                            phanQuyenDeleteModel.IsProcess == false)
                    {
                        quyenHanDeleteEntity.WillDelete = true;
                    }
                    else if (quyenHanDeleteEntity.SecurityOperation == Enums.SecurityOperation.View &&
                             phanQuyenDeleteModel.IsView == false)
                    {
                        quyenHanDeleteEntity.WillDelete = true;
                    }
                }
            }

            foreach (var phanQuyenInsertModel in phanQuyenViewModel.DocumentTypes.Where(x => x.DocumentType != null && x.IsChange == true &&
                (x.IsDelete == true || x.IsInsert == true || x.IsProcess == true || x.IsUpdate == true || x.IsView == true)).OrderBy(x => x.DocumentType))
            {
                if (phanQuyenInsertModel.IsUpdate == true)
                {
                    if (!quyenHanEntity.RoleFunctions.Any(x => x.DocumentType == phanQuyenInsertModel.DocumentType && x.SecurityOperation == Enums.SecurityOperation.Update))
                    {
                        var roleFunctionEntity = new RoleFunction
                        {
                            DocumentType = phanQuyenInsertModel.DocumentType.GetValueOrDefault(),
                            RoleId = quyenHanEntity.Id,
                            SecurityOperation = Enums.SecurityOperation.Update
                        };
                        quyenHanEntity.RoleFunctions.Add(roleFunctionEntity);
                    }
                }
                if (phanQuyenInsertModel.IsInsert == true)
                {
                    if (!quyenHanEntity.RoleFunctions.Any(x => x.DocumentType == phanQuyenInsertModel.DocumentType && x.SecurityOperation == Enums.SecurityOperation.Add))
                    {
                        var roleFunctionEntity = new RoleFunction
                        {
                            DocumentType = phanQuyenInsertModel.DocumentType.GetValueOrDefault(),
                            RoleId = quyenHanEntity.Id,
                            SecurityOperation = Enums.SecurityOperation.Add
                        };
                        quyenHanEntity.RoleFunctions.Add(roleFunctionEntity);
                    }
                }
                if (phanQuyenInsertModel.IsDelete == true)
                {
                    if (!quyenHanEntity.RoleFunctions.Any(x => x.DocumentType == phanQuyenInsertModel.DocumentType && x.SecurityOperation == Enums.SecurityOperation.Delete))
                    {
                        var roleFunctionEntity = new RoleFunction
                        {
                            DocumentType = phanQuyenInsertModel.DocumentType.GetValueOrDefault(),
                            RoleId = quyenHanEntity.Id,
                            SecurityOperation = Enums.SecurityOperation.Delete
                        };
                        quyenHanEntity.RoleFunctions.Add(roleFunctionEntity);
                    }
                }
                if (phanQuyenInsertModel.IsProcess == true)
                {
                    if (!quyenHanEntity.RoleFunctions.Any(x => x.DocumentType == phanQuyenInsertModel.DocumentType && x.SecurityOperation == Enums.SecurityOperation.Process))
                    {
                        var roleFunctionEntity = new RoleFunction
                        {
                            DocumentType = phanQuyenInsertModel.DocumentType.GetValueOrDefault(),
                            RoleId = quyenHanEntity.Id,
                            SecurityOperation = Enums.SecurityOperation.Process
                        };
                        quyenHanEntity.RoleFunctions.Add(roleFunctionEntity);
                    }
                }
                if (phanQuyenInsertModel.IsView == true)
                {
                    if (!quyenHanEntity.RoleFunctions.Any(x => x.DocumentType == phanQuyenInsertModel.DocumentType && x.SecurityOperation == Enums.SecurityOperation.View))
                    {
                        var roleFunctionEntity = new RoleFunction
                        {
                            DocumentType = phanQuyenInsertModel.DocumentType.GetValueOrDefault(),
                            RoleId = quyenHanEntity.Id,
                            SecurityOperation = Enums.SecurityOperation.View
                        };
                        quyenHanEntity.RoleFunctions.Add(roleFunctionEntity);
                    }
                }
            }

            await _phanQuyenNguoiDungService.UpdateAsync(quyenHanEntity);
            _cacheManager.RemoveByPattern(RoleService.ROLES_PATTERN_KEY);
            return NoContent();
        }

        [HttpPost("ExportPhanQuyenNguoiDung")]
        [ClaimRequirement(
            Enums.SecurityOperation.Process,
            Enums.DocumentType.Role
        )]
        public async Task<ActionResult> ExportPhanQuyenNguoiDung(QueryInfo queryInfo)
        {
            var gridData = await _phanQuyenNguoiDungService.GetDataForGridAsync(queryInfo, true);
            var phanQuyenNguoiDungData = gridData.Data.Select(p => (PhanQuyenNguoiDungGridVo)p).ToList();
            var dataExcel = phanQuyenNguoiDungData.Map<List<PhanQuyenNguoiDungExportExcel>>();

            var lstValueObject = new List<(string, string)>();
            lstValueObject.Add((nameof(PhanQuyenNguoiDungExportExcel.Ten), "Tên"));
            lstValueObject.Add((nameof(PhanQuyenNguoiDungExportExcel.LoaiNguoiDung), "Loại người dùng"));
            lstValueObject.Add((nameof(PhanQuyenNguoiDungExportExcel.Quyen), "Quyền mặc định"));

            var bytes = _excelService.ExportManagermentView(dataExcel, lstValueObject, "Phân Quyền Người Dùng");

            HttpContext.Response.Headers.Add("content-disposition", "attachment; filename=PhanQuyenNguoiDung" + DateTime.Now.Year + ".xls");
            Response.ContentType = "application/vnd.ms-excel";

            return new FileContentResult(bytes, "application/vnd.ms-excel");
        }
    }
}
