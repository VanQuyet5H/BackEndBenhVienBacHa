using System;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Camino.Api.Auth;
using Camino.Api.Infrastructure.Auth;
using Camino.Api.Models.Auth;
using Camino.Api.Models.Error;
using Camino.Core.Configuration;
using Camino.Core.Helpers;
using Camino.Core.Domain;
using Camino.Services;
using Camino.Services.CauHinh;
using Camino.Services.Localization;
using Camino.Services.Messages;
using Camino.Services.NhanVien;
using Camino.Services.Users;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Camino.Api.Models.KhoaPhong;

namespace Camino.Api.Controllers
{
    public class AuthController : CaminoBaseController
    {
        readonly IJwtFactory _iJwtFactory;
        readonly IUserService _userService;
        readonly INhanVienService _nhanVienService;
        readonly IRoleService _roleService;
        readonly ICauHinhService _cauHinhService;
        private readonly ILocalizationService _localizationService;
        private readonly IEmailService _emailService;
        private readonly ISmsService _smsService;
        private readonly SmsConfig _smsConfig;
        private readonly IUserMessagingTokenService _userMessagingTokenService;
        public AuthController(IUserService userService, IRoleService roleService, IJwtFactory iJwtFactory,
            ILocalizationService localizationService, ITemplateService templateService, INhanVienService nhanVienService, IUserMessagingTokenService userMessagingTokenService,
        ISmsService smsService, IEmailService emailService, ICauHinhService cauHinhService, SmsConfig smsConfig)
        {
            _userService = userService;
            _roleService = roleService;
            _iJwtFactory = iJwtFactory;
            _localizationService = localizationService;
            _cauHinhService = cauHinhService;
            _smsService = smsService;
            _emailService = emailService;
            _smsConfig = smsConfig;
            _nhanVienService = nhanVienService;
            _userMessagingTokenService = userMessagingTokenService;
        }

        private int ThoiGianHetHanMaXacNhan => _cauHinhService.GetSettingByKey("CauHinhHeThong.ThoiGianHetHanMaXacNhan", 180);
        [HttpPost("VerifyUsername")]
        public async Task<IActionResult> VerifyUsername(LoginViewModel loginViewModel)
        {
            var user = await _userService.GetUserByPhoneNumberOrEmail(loginViewModel.UserName, Enums.Region.Internal);
            if (user == null)
            {
                throw new ApiException(_localizationService.GetResource("DangNhap.WrongUsername"));
            }
            else
            {
                if (user.IsActive != true)
                {
                    throw new ApiException(_localizationService.GetResource("DangNhap.InActive"),
                        (int)HttpStatusCode.Unauthorized);
                }
                else
                {
                    if (string.IsNullOrEmpty(user.Password))
                    {
                        if (user.ExpiredCodeDate > DateTime.Now)
                        {
                            return Ok(user.ExpiredCodeDate);
                        }
                        else
                        {
                            if (CommonHelper.IsPhoneValid(loginViewModel.UserName))
                            {
                                if (!string.IsNullOrEmpty(_smsConfig.PassCodeDault))
                                {
                                    user.PassCode = _smsConfig.PassCodeDault;
                                    user.ExpiredCodeDate = DateTime.Now.AddSeconds(ThoiGianHetHanMaXacNhan);
                                    _userService.Update(user);
                                    return Ok(user.ExpiredCodeDate);
                                }
                                else
                                {
                                    var passCode = RandomHelper.RandomPassCode(6);
                                    if (_smsService.SendSmsTaoMatKhau(loginViewModel.UserNameRemoveFormat, passCode))
                                    {
                                        user.PassCode = passCode;
                                        user.ExpiredCodeDate = DateTime.Now.AddSeconds(ThoiGianHetHanMaXacNhan);
                                        _userService.Update(user);
                                        return Ok(user.ExpiredCodeDate);
                                    }
                                    else
                                    {
                                        throw new ApiException(
                                            _localizationService.GetResource("DangNhap.SendPassCodeFail"));
                                    }
                                }
                            }
                            else
                            {
                                if (CommonHelper.IsMailValid(loginViewModel.UserName))
                                {
                                    var passCode = RandomHelper.RandomPassCode(6);
                                    if (_emailService.SendEmailTaoMatKhau(loginViewModel.UserName, passCode))
                                    {
                                        user.PassCode = passCode;
                                        user.ExpiredCodeDate = DateTime.Now.AddSeconds(ThoiGianHetHanMaXacNhan);
                                        _userService.Update(user);
                                        return Ok(user.ExpiredCodeDate);
                                    }
                                    else
                                    {
                                        throw new ApiException(
                                            _localizationService.GetResource("DangNhap.SendPassCodeFail"));
                                    }
                                }
                                else
                                {
                                    throw new ApiException(
                                        _localizationService.GetResource("DangNhap.WrongUsernameFormat"),
                                        (int)HttpStatusCode.BadRequest);
                                }
                            }
                        }
                    }
                    else
                    {
                        return Ok(null);
                    }
                }
            }
        }

        [HttpPost("SendPassCode")]
        public async Task<IActionResult> SendPassCode(LoginViewModel loginViewModel)
        {
            var user = await _userService.GetUserByPhoneNumberOrEmail(loginViewModel.UserName, Enums.Region.Internal);
            if (user == null)
            {
                throw new ApiException(_localizationService.GetResource("DangNhap.WrongUsername"));
            }
            else
            {
                if (user.IsActive != true)
                {
                    throw new ApiException(_localizationService.GetResource("DangNhap.InActive"),
                        (int)HttpStatusCode.Unauthorized);
                }
                else
                {
                    if (user.ExpiredCodeDate != null && user.ExpiredCodeDate > DateTime.Now)
                    {
                        return Ok(user.ExpiredCodeDate);
                    }
                    else
                    {
                        if (CommonHelper.IsPhoneValid(loginViewModel.UserName))
                        {
                            if (!string.IsNullOrEmpty(_smsConfig.PassCodeDault))
                            {
                                user.PassCode = _smsConfig.PassCodeDault;
                                user.ExpiredCodeDate = DateTime.Now.AddSeconds(ThoiGianHetHanMaXacNhan);
                                _userService.Update(user);
                                return Ok(user.ExpiredCodeDate);
                            }
                            else
                            {
                                var passCode = RandomHelper.RandomPassCode(6);
                                if (_smsService.SendSmsTaoMatKhau(loginViewModel.UserNameRemoveFormat, passCode))
                                {
                                    user.PassCode = passCode;
                                    user.ExpiredCodeDate = DateTime.Now.AddSeconds(ThoiGianHetHanMaXacNhan);
                                    _userService.Update(user);
                                    return Ok(user.ExpiredCodeDate);
                                }
                                else
                                {
                                    throw new ApiException(_localizationService.GetResource("DangNhap.SendPassCodeFail"));
                                }
                            }
                        }
                        else
                        {
                            if (CommonHelper.IsMailValid(loginViewModel.UserName))
                            {
                                var passCode = RandomHelper.RandomPassCode(6);
                                if (_emailService.SendEmailTaoMatKhau(loginViewModel.UserName, passCode))
                                {
                                    user.PassCode = passCode;
                                    user.ExpiredCodeDate = DateTime.Now.AddSeconds(ThoiGianHetHanMaXacNhan);
                                    _userService.Update(user);
                                    return Ok(user.ExpiredCodeDate);
                                }
                                else
                                {
                                    throw new ApiException(_localizationService.GetResource("DangNhap.SendPassCodeFail"));
                                }
                            }
                            else
                            {
                                throw new ApiException(_localizationService.GetResource("DangNhap.WrongUsernameFormat"), (int)HttpStatusCode.BadRequest);
                            }
                        }
                    }
                }
            }
        }

        [HttpPost("ForgetPassword")]
        public async Task<IActionResult> ForgetPassword(ForgotPasswordViewModel forgotPasswordViewModel)
        {
            if (forgotPasswordViewModel.ForgotPasswordStage != EnumForgotPasswordStage.IsForget)
            {
                throw new ApiException(_localizationService.GetResource("Auth.ForgotPassword.ForgotPasswordStage.Inappropriate"), (int)HttpStatusCode.BadRequest);
            }

            var user = await _userService.GetUserByPhoneNumberOrEmail(forgotPasswordViewModel.UserName, Enums.Region.Internal);
            if (user == null)
            {
                throw new ApiException(_localizationService.GetResource("DangNhap.AccountNotAvailable"));
            }
            else
            {
                if (user.IsActive != true)
                {
                    throw new ApiException(_localizationService.GetResource("DangNhap.InActive"), (int)HttpStatusCode.Unauthorized);
                }
                else
                {
                    if (forgotPasswordViewModel.UserNameType == EnumUserNameType.IsPhone)
                    {
                        if (user.ExpiredCodeDate != null && user.ExpiredCodeDate > DateTime.Now)
                        {
                            return Ok(user.ExpiredCodeDate);
                        }
                        else
                        {
                            if (!string.IsNullOrEmpty(_smsConfig.PassCodeDault))
                            {
                                user.PassCode = _smsConfig.PassCodeDault;
                                user.ExpiredCodeDate = DateTime.Now.AddSeconds(ThoiGianHetHanMaXacNhan);
                                _userService.Update(user);
                                return Ok(user.ExpiredCodeDate);
                            }
                            else
                            {
                                var passCode = RandomHelper.RandomPassCode(6);
                                if (_smsService.SendSmsTaoMatKhau(forgotPasswordViewModel.UserNameRemoveFormat, passCode))
                                {
                                    user.PassCode = passCode;
                                    user.ExpiredCodeDate = DateTime.Now.AddSeconds(ThoiGianHetHanMaXacNhan);
                                    _userService.Update(user);
                                    return Ok(user.ExpiredCodeDate);
                                }
                                else
                                {
                                    throw new ApiException(_localizationService.GetResource("DangNhap.SendPassCodeFail"));
                                }
                            }
                        }
                    }
                    else
                    {
                        if (string.IsNullOrEmpty(forgotPasswordViewModel.Domain))
                        {
                            throw new ApiException(_localizationService.GetResource("Auth.ForgetPassword.SendEmail.InvalidDomain"));
                        }

                        var passCode = RandomHelper.RandomPassCode(6);
                        //if (_emailService.SendEmailTaoMatKhau(loginViewModel.UserName, passCode))
                        if (_emailService.SendEmailTaoMatKhauWithHoTen(forgotPasswordViewModel.UserName, passCode, user.HoTen, forgotPasswordViewModel.Domain))
                        {
                            user.PassCode = passCode;
                            user.ExpiredCodeDate = DateTime.Now.AddSeconds(ThoiGianHetHanMaXacNhan);
                            _userService.Update(user);
                            return Ok(user.ExpiredCodeDate);
                        }
                        else
                        {
                            throw new ApiException(_localizationService.GetResource("DangNhap.SendPassCodeFail"));
                        }
                    }
                }
            }
        }

        [HttpPost("VerifyPassCode")]
        public async Task<IActionResult> VerifyPassCode(LoginViewModel loginViewModel)
        {
            var user = await _userService.GetUserByPassCode(loginViewModel.UserName, loginViewModel.PassCode, Enums.Region.Internal);
            if (user == null)
            {
                throw new ApiException(_localizationService.GetResource("DangNhap.WrongPassCode"));
            }
            if (user.IsActive != true)
            {
                throw new ApiException(_localizationService.GetResource("DangNhap.InActive"),
                    (int)HttpStatusCode.Unauthorized);
            }
            if (user.ExpiredCodeDate < DateTime.Now)
            {
                throw new ApiException(_localizationService.GetResource("DangNhap.ExpiredPassCode"));
            }
            return Ok(true);
        }

        [HttpPost("VerifyPassCodeForForgottenPassword")]
        public async Task<IActionResult> VerifyPassCodeForForgottenPassword(ForgotPasswordViewModel forgotPasswordViewModel)
        {
            if(forgotPasswordViewModel.ForgotPasswordStage != EnumForgotPasswordStage.IsVerify)
            {
                throw new ApiException(_localizationService.GetResource("Auth.ForgotPassword.ForgotPasswordStage.Inappropriate"), (int)HttpStatusCode.BadRequest);
            }

            var user = await _userService.GetUserByPhoneNumberOrEmail(forgotPasswordViewModel.DecodedEmail, Enums.Region.Internal);
            if (user == null)
            {
                throw new ApiException(_localizationService.GetResource("DangNhap.WrongPassCode"));
            }
            if (user.IsActive != true)
            {
                throw new ApiException(_localizationService.GetResource("DangNhap.InActive"), (int)HttpStatusCode.Unauthorized);
            }

            var base64Data = new
            {
                UserName = user.Email,
                PassCode = user.PassCode
            };

            if (!EmailHelper.VerifyHashedUrl(forgotPasswordViewModel.DecodedBase64Data, JsonConvert.SerializeObject(base64Data)))
            {
                throw new ApiException(_localizationService.GetResource("DangNhap.WrongPassCode"));
            }

            if (user.ExpiredCodeDate == null || user.ExpiredCodeDate < DateTime.Now)
            {
                //throw new ApiException(_localizationService.GetResource("DangNhap.ExpiredPassCode"));
                return Ok(false);
            }

            return Ok(true);
        }

        [HttpPost("ResetPassword")]
        public async Task<IActionResult> ResetPassword(ForgotPasswordViewModel forgotPasswordViewModel)
        {
            if (forgotPasswordViewModel.ForgotPasswordStage != EnumForgotPasswordStage.IsReset)
            {
                throw new ApiException(_localizationService.GetResource("Auth.ForgotPassword.ForgotPasswordStage.Inappropriate"), (int)HttpStatusCode.BadRequest);
            }

            var user = await _userService.GetUserByPhoneNumberOrEmail(forgotPasswordViewModel.DecodedEmail, Enums.Region.Internal);
            if (user == null)
            {
                throw new ApiException(_localizationService.GetResource("DangNhap.WrongPassCode"));
            }
            if (user.IsActive != true)
            {
                throw new ApiException(_localizationService.GetResource("DangNhap.InActive"), (int)HttpStatusCode.Unauthorized);
            }

            var base64Data = new
            {
                UserName = user.Email,
                PassCode = user.PassCode
            };

            if (!EmailHelper.VerifyHashedUrl(forgotPasswordViewModel.DecodedBase64Data, JsonConvert.SerializeObject(base64Data)))
            {
                throw new ApiException(_localizationService.GetResource("DangNhap.WrongPassCode"));
            }

            if (user.ExpiredCodeDate == null || user.ExpiredCodeDate < DateTime.Now)
            {
                //throw new ApiException(_localizationService.GetResource("DangNhap.ExpiredPassCode"));
                return Ok(false);
            }

            user.Password = PasswordHasher.HashPassword(forgotPasswordViewModel.Password);
            user.ExpiredCodeDate = DateTime.Now;

            await _userService.UpdateAsync(user);

            return Ok(true);
        }

        [HttpPost("SetPassword")]
        public async Task<IActionResult> SetPassword(LoginViewModel loginViewModel)
        {
            var user = await _userService.GetUserByPassCode(loginViewModel.UserName, loginViewModel.PassCode, Enums.Region.Internal);
            if (user == null)
            {
                throw new ApiException(_localizationService.GetResource("DangNhap.WrongPassCode"));
            }

            if (user.IsActive != true)
            {
                throw new ApiException(_localizationService.GetResource("DangNhap.InActive"),
                    (int)HttpStatusCode.Unauthorized);
            }
            user.Password = PasswordHasher.HashPassword(loginViewModel.Password);
            _userService.Update(user);
            if (!string.IsNullOrEmpty(loginViewModel.FcmToken))
            {
                await _userMessagingTokenService.SetupUserMessagingTokenAsync(user.Id, loginViewModel.FcmToken, Enums.DeviceType.Web);
            }
            long[] userRoles = user.Region == Enums.Region.Internal
                ? await _nhanVienService.GetNhanVienRoles(user.Id)
                : new long[] { };
            var PhongBenhVienId = await _userService.PhongBenhVienByUserId(user.Id);
            var accessUser = new AccessUser()
            {
                AccessToken = _iJwtFactory.GenerateInternalToken(user.Id, userRoles),
                UserName = user.SoDienThoai,
                Id = user.Id,
                PhongBenhVienId = PhongBenhVienId,
                MenuInfo = _roleService.GetMenuInfo(userRoles),
                Permissions = _roleService.GetPermissions(userRoles)
            };
            return Ok(accessUser);
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login(LoginViewModel loginViewModel)
        {
            var user = await _userService.GetUserByPhoneNumberOrEmailAndPassword(loginViewModel.UserName, loginViewModel.Password, Enums.Region.Internal);
            if (user == null)
            {
                throw new ApiException(_localizationService.GetResource("DangNhap.WrongUserOrPassword"));//DangNhap.WrongPassword
            }
            if (user != null && string.IsNullOrEmpty(user.Password))
            {
                throw new ApiException(_localizationService.GetResource("DangNhap.AccountNotAvailable"));
            }
            if (user.IsActive != true)
            {
                throw new ApiException(_localizationService.GetResource("DangNhap.InActive"));
            }
            if (!string.IsNullOrEmpty(loginViewModel.FcmToken))
            {
                await _userMessagingTokenService.SetupUserMessagingTokenAsync(user.Id, loginViewModel.FcmToken, Enums.DeviceType.Web);
            }
            long[] userRoles = user.Region == Enums.Region.Internal
                ? await _nhanVienService.GetNhanVienRoles(user.Id)
                : new long[] { };

            var PhongBenhVienId = await _userService.PhongBenhVienByUserId(user.Id);

            var accessUser = new AccessUser()
            {
                AccessToken = _iJwtFactory.GenerateInternalToken(user.Id, userRoles),
                UserName = user.SoDienThoai,
                Id = user.Id,
                HinhThucKhamBenh = loginViewModel.HinhThucKhamBenh,
                PhongBenhVienId = PhongBenhVienId,
                MenuInfo = _roleService.GetMenuInfo(userRoles),
                Permissions = _roleService.GetPermissions(userRoles)
            };
            return Ok(accessUser);
        }

        [HttpPost("LoginWithPassCode")]
        public async Task<IActionResult> LoginWithPassCode(LoginPassCodeViewModel loginPassCodeViewModel)
        {
            var user = await _userService.GetUserByPhoneAndPassCode(loginPassCodeViewModel.Phone, loginPassCodeViewModel.PassCode);
            if (user == null)
            {
                throw new ApiException("Invalid PassCode.", (int)HttpStatusCode.Unauthorized);
            }
            if (!string.IsNullOrEmpty(loginPassCodeViewModel.FcmToken))
            {
                await _userMessagingTokenService.SetupUserMessagingTokenAsync(user.Id, loginPassCodeViewModel.FcmToken, Enums.DeviceType.Web);
            }
            long[] userRoles = user.Region == Enums.Region.Internal
                ? await _nhanVienService.GetNhanVienRoles(user.Id)
                : new long[] { };

            var kiemTraNhanVienTrongPhongBan = await _userService.PhongBenhVienByUserId(user.Id);

            var accessUser = new AccessUser()
            {
                AccessToken = _iJwtFactory.GenerateInternalToken(user.Id, userRoles),
                UserName = user.SoDienThoai,
                Id = user.Id,
                PhongBenhVienId = kiemTraNhanVienTrongPhongBan,
                MenuInfo = _roleService.GetMenuInfo(userRoles),
                Permissions = _roleService.GetPermissions(userRoles)
            };
            return Ok(accessUser);
        }

        [HttpPost("AutoLogin")]
        [ClaimRequirement(Enums.SecurityOperation.None, Enums.DocumentType.None)]
        public async Task<IActionResult> AutoLogin()
        {
            var user = await _userService.GetCurrentUser();
            if (user == null)
            {
                throw new ApiException("Auto Login Error", (int)HttpStatusCode.Unauthorized);
            }
            long[] userRoles = user.Region == Enums.Region.Internal
                ? await _nhanVienService.GetNhanVienRoles(user.Id)
                : new long[] { };

            var PhongBenhVienId = await _userService.PhongBenhVienByUserId(user.Id);
            var accessUser = new AccessUser()
            {
                AccessToken = _iJwtFactory.GenerateInternalToken(user.Id, userRoles),
                UserName = user.SoDienThoai,
                Id = user.Id,
                PhongBenhVienId = PhongBenhVienId,
                MenuInfo = _roleService.GetMenuInfo(userRoles),
                Permissions = _roleService.GetPermissions(userRoles)
            };
            return Ok(accessUser);
        }

        [HttpPost("Logout")]
        public async Task<IActionResult> Logout(PhongNhanVienViewModel model)
        {
            //Xóa đi hoạt động phòng hiện tại của nó trong table HoatDongNhanVien 
            _userService.XoaHoatDongPhongKhiLogout(model.PhongKhamBenhId);
            return Ok();
        }

    }
}