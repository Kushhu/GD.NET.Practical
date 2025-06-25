using GD.Practical.Models;
using Microsoft.EntityFrameworkCore;

namespace GD.Practical.Database
{
    public class TimetableDbContext : DbContext
    {
        public DbSet<SchoolConfig> SchoolConfig { get; set; }

        public DbSet<Subject> Subjects { get; set; }    

        public TimetableDbContext(DbContextOptions<TimetableDbContext> options)
            : base(options)
        {

        }
    }
}
