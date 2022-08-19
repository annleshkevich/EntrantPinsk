using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EntrantPinsk.Model.DatabaseModels
{
    public class Education
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public List<EducationalInstitution> Institutions { get; set; } = new();
    }
}
