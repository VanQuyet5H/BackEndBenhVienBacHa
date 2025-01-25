namespace Camino.Services.Messages
{
    public interface IEmailService
    {
        bool SendEmailTaoMatKhau(string toEmailAddress, string passCode);
        bool SendEmailTaoMatKhauWithHoTen(string toEmailAddress, string passCode, string hoTen, string domain);
    }
}