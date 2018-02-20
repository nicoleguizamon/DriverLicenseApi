using System;
using System.Collections.Generic;

namespace DriverLicense.Models.Models
{
    public partial class Locations
    {
        public Locations()
        {
            Questions = new HashSet<Questions>();
        }

        public int LocationId { get; set; }
        public string Name { get; set; }
        public string ImageUrl { get; set; }
        public int ApproveScore { get; set; }

        public ICollection<Questions> Questions { get; set; }
    }
}
