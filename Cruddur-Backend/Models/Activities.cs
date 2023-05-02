namespace Cruddur_Backend.Models
{
    public class Activities
    {
        public string message { get; set; }
        public Guid uuid { get; set; }
        public string handle { get; set; }
        public DateTime created_at { get; set; }
        public DateTime expires_at { get; set; }
        public int likes_count { get; set; }
        public int replies_count { get; set; }
        public int reposts_count { get; set; }
        public List<Replies> replies { get; set; }
    }
}
