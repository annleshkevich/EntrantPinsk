using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EntrantPinsk.Model.DatabaseModels
{
    public class EducationalInstitution
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
        public string Email { get; set; }
        public string Website { get; set; }
        [ForeignKey("EducationId")]
        public int EducationId { get; set; }
        public Education Education { get; set; }
        public List<Specialty> Specialties { get; set; } = new();
    }
}
