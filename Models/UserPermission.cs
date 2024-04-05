namespace Spider_EMT.Models
{
    public class UserPermission
    {
        public int ProfileId { get; set; }
        public Profile Profile { get; set; }
        public int PageId {  get; set; }
        public Page Page {  get; set; } 
    }
}
