using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Text;
using Camino.Core.DependencyInjection.Attributes;
using Camino.Core.Domain;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Helpers;
using Camino.Services.CauHinh;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;

namespace Camino.Services.Helpers
{
    [ScopedDependency(ServiceType = typeof(IUserAgentHelper))]
    public class UserAgentHelper: IUserAgentHelper
    {
        private readonly ClaimsPrincipal _claimsPrincipal;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private const long SystemUserId = 1;
        public UserAgentHelper(IHttpContextAccessor httpContextAccessor, IPrincipal principal)
        {
            _httpContextAccessor = httpContextAccessor;
            _claimsPrincipal = principal as ClaimsPrincipal;
        }
        public Enums.LanguageType GetUserLanguage()
        {
            var languageHeader = _httpContextAccessor.HttpContext?.Request.Headers["Language"];
            if (languageHeader.HasValue && !string.IsNullOrEmpty(languageHeader.ToString()) && int.TryParse(languageHeader.ToString(),out var ngonngu))
            {
                return (Enums.LanguageType)ngonngu;
            }
            return Enums.LanguageType.VietNam;
        }
        //public PortalUserType GetPortalUserType()
        //{
        //    var portalUserType = new PortalUserType
        //    {
        //        LoaiNguoiDung = Enums.UserType.KhachVangLai
        //    };
        //    var userTypeHeader = _httpContextAccessor.HttpContext?.Request.Headers["UserType"];
        //    if (!string.IsNullOrEmpty(userTypeHeader))
        //    {
        //        var userTypes = userTypeHeader.ToString().Split(Constants.HeaderUserTypeSeparator)
        //            .Select(long.Parse).ToArray();
        //        if (userTypes.Length == 2)
        //        {
        //            portalUserType.LoaiNguoiDung = (Enums.UserType)userTypes[0];
        //            switch (portalUserType.LoaiNguoiDung)
        //            {
        //                case Enums.UserType.CuDan:
        //                    portalUserType.CanHoId = Convert.ToInt64(userTypes[1]);
        //                    break;
        //            }

        //        }
        //        else
        //        {
        //            if (userTypes.Length == 3)
        //            {
        //                portalUserType.LoaiNguoiDung = (Enums.UserType)userTypes[0];
        //                switch (portalUserType.LoaiNguoiDung)
        //                {
        //                    case Enums.UserType.VanPhong:
        //                        portalUserType.HopDongVanPhongId = Convert.ToInt64(userTypes[1]);
        //                        portalUserType.PhuLucVanPhongId = Convert.ToInt64(userTypes[2]);
        //                        break;
        //                    case Enums.UserType.KhachHangTrungTamThuoc:
        //                        portalUserType.HopDongQuayId = Convert.ToInt64(userTypes[1]);
        //                        portalUserType.PhuLucQuayId = Convert.ToInt64(userTypes[2]);
        //                        break;
        //                }

        //            }
        //        }
        //    }
        //    return portalUserType;
        //}
        public long GetCurrentUserId()
        {
            var userIdClaim = _claimsPrincipal?.Claims.FirstOrDefault(o => o.Type == Constants.JwtClaimTypes.Id);
            if (userIdClaim != null)
            {
                return long.Parse(userIdClaim.Value);
            }
            return SystemUserId;
        }

        public List<long> GetListCurrentUserRoleId()
        {
            var userIdClaim = _claimsPrincipal?.Claims.FirstOrDefault(o => o.Type == Constants.JwtClaimTypes.Role);

            var result = new List<long>();

            if (userIdClaim != null)
            {
                if (userIdClaim.Value.Contains(","))
                {
                    var lstIdString = userIdClaim.Value.Split(",");
                    foreach (var idString in lstIdString)
                    {
                        result.Add(long.Parse(idString));
                    }
                }
                else
                {
                    result.Add(long.Parse(userIdClaim.Value));
                }
            }

            return result;
        }

        public long GetCurrentNoiLLamViecId()
        {
            //todo: need update 
            int.TryParse(_httpContextAccessor.HttpContext?.Request.Headers["PhongBenhVienId"],
                out int phongBenhVienId);
            return phongBenhVienId;
        }
    }
}
