namespace Spider_EMT.Data.Account
{
    public interface IAudittable
    {
        DateTime? CreateDate { get; set; }
        int? CreateUserId { get; set; }
        DateTime? UpdateDate { get; set; }
        int? UpdateUserId { get; set; }
    }
}
