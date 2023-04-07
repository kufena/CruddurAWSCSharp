using Amazon.SimpleSystemsManagement;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CruddurSQL
{
    public class ActivitiesModel
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)] // Identity or Computed?
        public Guid uuid { get; set; }
        public Guid user_uuid { get; set; }
        public string text { get; set; }
        public int replies_count { get; set; }
        public int reposts_count { get; set; }
        public int likes_count { get; set; }
        public int reply_to_activity_uuid;
        public DateTime expires_at { get; set; }
        public DateTime created_at { get; set; }

    }
}
