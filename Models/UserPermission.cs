namespace Spider_EMT.Models
{
    public class UserPermission
    {
        public int ProfileId { get; set; }
        public int PageId { get; set; }
        public int PageCatId { get; set; }
        public DateTime CreateDate { get; set; }
        public int CreateUserId { get; set; }
        public DateTime UpdateDate { get; set; }
        public int UpdateUserId { get; set; }
    }
}
