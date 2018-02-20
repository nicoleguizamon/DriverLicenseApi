using System.Collections.Generic;

namespace DriverLicense.Models.DTOs
{
    public class LocationDTO
    {
        public int LocationId { get; set; }
        public string Name { get; set; }
        public string ImageUrl { get; set; }
        public int ApproveScore { get; set; }

        public IEnumerable<QuestionDTO> Questions { get; set; }
    }
}
