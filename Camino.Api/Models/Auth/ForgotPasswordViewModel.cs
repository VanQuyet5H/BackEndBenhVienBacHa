using Camino.Api.Models.Error;
using Camino.Core.Helpers;
using System;

namespace Camino.Api.Models.Auth
{
    public class ForgotPasswordViewModel
    {
        public string UserName { get; set; }
        public string UserNameRemoveFormat => !string.IsNullOrEmpty(UserName) ? UserName.RemoveFormatPhone() : "";
        public string Email { get; set; }
        public string DecodedEmail
        {
            get
            {
                try
                {
                    return ForgotPasswordStage == EnumForgotPasswordStage.IsReset || ForgotPasswordStage == EnumForgotPasswordStage.IsVerify ? Email.DecodeHexString() : ""; 
                }
                catch (Exception)
                {
                    throw new ApiException("Đường dẫn không hợp lệ");
                }
            }
        }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
        public string Base64Data { get; set; }
        public string DecodedBase64Data2 => ForgotPasswordStage == EnumForgotPasswordStage.IsReset || ForgotPasswordStage == EnumForgotPasswordStage.IsVerify ? Base64Data.DecodeHexString() : "";
        public string DecodedBase64Data
        {
            get
            {
                try
                {
                    return ForgotPasswordStage == EnumForgotPasswordStage.IsReset || ForgotPasswordStage == EnumForgotPasswordStage.IsVerify ? Base64Data.DecodeHexString() : "";
                }
                catch (Exception)
                {
                    throw new ApiException("Đường dẫn không hợp lệ");
                }
            }
        }
        public string Domain { get; set; }
        public EnumUserNameType UserNameType => string.IsNullOrEmpty(UserName) ? EnumUserNameType.None : (CommonHelper.IsPhoneValid(UserName) ? EnumUserNameType.IsPhone : (CommonHelper.IsMailValid(UserName) ? EnumUserNameType.IsEmail : EnumUserNameType.None));
        public EnumForgotPasswordStage ForgotPasswordStage { get; set; }
    }

    public enum EnumUserNameType
    {
        IsPhone = 1,
        IsEmail = 2,
        None = 3
    }

    public enum EnumForgotPasswordStage
    {
        IsForget = 1,
        IsVerify = 2,
        IsReset = 3
    }
}
