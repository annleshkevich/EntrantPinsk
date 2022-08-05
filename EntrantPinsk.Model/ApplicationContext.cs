using EntrantPinsk.Model.DatabaseModels;
using Microsoft.EntityFrameworkCore;

namespace EntrantPinsk.Model
{
    public class ApplicationContext : DbContext
    {
        public ApplicationContext(DbContextOptions<ApplicationContext> options)
            : base(options)
        {
            Database.EnsureCreated();
        }
        public DbSet<Education> Educations { get; set; } = null!;
        public DbSet<EducationalInstitution> EducationalInstitutions { get; set; } = null!;
        public DbSet<Specialty> Specialties { get; set; } = null!;
        public DbSet<Direction> Directions { get; set; } = null!;

    }
}
