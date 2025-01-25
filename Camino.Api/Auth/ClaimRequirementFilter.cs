using System.Linq;
using Camino.Core.Domain;
using Camino.Core.Helpers;
using Camino.Services.Users;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Camino.Api.Auth
{
    public class ClaimRequirementFilter : IAuthorizationFilter
    {
        readonly Enums.DocumentType[] _documentTypes;
        readonly Enums.SecurityOperation _securityOperation;
        readonly IRoleService _roleService;

        public ClaimRequirementFilter(IRoleService roleService, Enums.DocumentType[] documentTypes, Enums.SecurityOperation securityOperation)
        {
            _documentTypes = documentTypes;
            _securityOperation = securityOperation;
            _roleService = roleService;
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var rolClaim = context.HttpContext.User.Claims.FirstOrDefault(o => o.Type == Constants.JwtClaimTypes.Role);
            
            if (rolClaim == null)
            {
                context.Result = new UnauthorizedResult();
                return;
            }
            var roleIds = rolClaim.Value.Split(Constants.JwtRoleSeparator).Select(long.Parse).ToArray();
            if (!_roleService.VerifyAccess(roleIds, _documentTypes, _securityOperation))
            {
                context.Result = new ForbidResult();
            }
        }
    }
}
