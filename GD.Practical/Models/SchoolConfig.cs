using System.ComponentModel.DataAnnotations;

namespace GD.Practical.Models
{
    public class SchoolConfig
    {
        [Key]
        public int Id { get; set; }

        [Range(1, 7)]
        public int WorkDays { get; set; }

        public int TotalSubject { get; set; }

        [Range(1, 7)]
        public int SubjectPerDay { get; set; }
    }
}
