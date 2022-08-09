
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EntrantPinsk.Model.DatabaseModels
{
    public class Direction
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public List<Specialty> Specialties { get; set; } = new();

        [ForeignKey("EducationalInstitutionId")]
        public int EducationalInstitutionId { get; set; }
        public EducationalInstitution EducationalInstitution { get; set; }
    }
}
