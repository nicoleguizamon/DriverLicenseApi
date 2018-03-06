using System;
using System.Collections.Generic;

namespace DriverLicense.Models.Models
{
    public partial class Answers
    {
        public int AnswerId { get; set; }
        public int QuestionId { get; set; }
        public string Description { get; set; }
        public bool IsCorrect { get; set; }

        public Questions Question { get; set; }
    }
}
