namespace Cruddur_Backend.Models
{
    public class Replies
    {
        public Guid uuid { get; set; }
        public Guid reply_to_activity_uuid { get; set; }
        public string message { get; set; }
        public string handle { get; set; }
        public DateTime created_at { get; set; }
        public int likes_count { get; set; }
        public int replies_count { get; set; }
        public int reposts_count { get; set; }

    }
}
