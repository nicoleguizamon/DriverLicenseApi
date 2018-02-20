using System;
using System.Collections.Generic;

namespace DriverLicense.Models.Models
{
    public partial class Questions
    {
        public Questions()
        {
            Answers = new HashSet<Answers>();
        }

        public int QuestionId { get; set; }
        public string Description { get; set; }
        public string Image { get; set; }
        public int ValidAnswer { get; set; }
        public int LocationId { get; set; }

        public Locations Location { get; set; }
        public ICollection<Answers> Answers { get; set; }
    }
}
