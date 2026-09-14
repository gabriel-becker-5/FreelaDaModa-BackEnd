namespace _04_Domain.Entities.ObjectsFields
{
    public class Specialty : BaseEntity
    {
        // Especialidades: Costura reta, Overloque, Modelagem, Peça piloto, Amostras, Acabamento, Bordado, Estamparia.
        public string SpecialtyName { get; set; }
        public ICollection<FreelancerSpecialties>? FreelancerSpecialties { get; set; }
    }
}