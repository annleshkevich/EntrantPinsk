using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EntrantPinsk.Model.DatabaseModels
{
    public class Specialty
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        [ForeignKey("EducationalInstitutionId")]
        public int EducationalInstitutionId { get; set; }
        public EducationalInstitution Institution { get; set; }

        [ForeignKey("DirectionId")]
        public int DirectionId { get; set; }
        public Direction Direction { get; set; }

    }
}
