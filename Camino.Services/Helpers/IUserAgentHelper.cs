using System;
using System.Collections.Generic;
using System.Text;
using Camino.Core.Domain;
using Camino.Core.Domain.ValueObject;

namespace Camino.Services.Helpers
{
    public interface IUserAgentHelper
    {
        Enums.LanguageType GetUserLanguage();
        //PortalUserType GetPortalUserType();
        long GetCurrentUserId();

        long GetCurrentNoiLLamViecId();

        List<long> GetListCurrentUserRoleId();
    }
}
