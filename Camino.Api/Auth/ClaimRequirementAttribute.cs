using Camino.Core.Domain;
using Microsoft.AspNetCore.Mvc;

namespace Camino.Api.Auth
{
    public class ClaimRequirementAttribute : TypeFilterAttribute
    {
        public ClaimRequirementAttribute(Enums.SecurityOperation securityOperation, params Enums.DocumentType[] documentTypes) : base(typeof(ClaimRequirementFilter))
        {
            Arguments = new object[] { documentTypes, securityOperation };
        }
    }
}
