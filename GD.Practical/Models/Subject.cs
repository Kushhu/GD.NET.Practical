using System.ComponentModel.DataAnnotations;

namespace GD.Practical.Models
{
    public class Subject
    {
        [Key]
        public int Id { get; set; }

        public string? Name { get; set; }

        public int TotalHours { get; set; }
    }
}
