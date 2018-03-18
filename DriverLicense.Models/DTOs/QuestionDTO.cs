using System.Collections.Generic;

namespace DriverLicense.Models.DTOs
{
    public class QuestionDTO
    {
        public int QuestionId { get; set; }
        public string Description { get; set; }
        public string Image { get; set; }
        public int LocationId { get; set; }

        
        public IEnumerable<AnswerDTO> Answers { get; set; }
    }
}
