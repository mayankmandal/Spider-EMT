using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace Spider_EMT.Models
{
    public class ProfileSite
    {
        [DisplayName("Profile Id")]
        public int ProfileId { get; set; }
        public string ProfileName { get; set; }
        public DateTime CreateDate { get; set; }
        public int CreateUserId { get; set; }
        public DateTime UpdateDate { get; set; }
        public int UpdateUserId { get; set; }
    }
}
