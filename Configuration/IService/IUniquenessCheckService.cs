namespace Spider_EMT.Configuration.IService
{
    public interface IUniquenessCheckService
    {
        Task<bool> IsUniqueAsync(string field, string value);
    }
}
