
namespace EntrantPinsk.Model.DatabaseModels
{
    public class Direction
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<Specialty> Specialties { get; set; } = new();
    }
}
