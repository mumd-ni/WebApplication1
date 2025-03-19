namespace WebApplication1.Services.EmailServicses
{
    public interface IEmailService
    {
        Task SendEmailAsync(string email, string subject, string message);
    }
}
