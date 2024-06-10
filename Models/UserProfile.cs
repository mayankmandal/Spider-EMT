namespace Spider_EMT.Models
{
    public class UserProfile
    {
        public int ProfileId { get; set; }
        public ProfileSite Profile { get; set; }
        public int UserId { get; set; }
        public DateTime CreateDate { get; set; }
        public int CreateUserId { get; set; }
        public DateTime UpdateDate { get; set; }
        public int UpdateUserId { get; set; }
    }
}
