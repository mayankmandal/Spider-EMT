namespace Spider_EMT.Configuration.IService
{
    public interface IEmailService
    {
        Task SendAsync(string from, string to, string subject, string body);
    }
}