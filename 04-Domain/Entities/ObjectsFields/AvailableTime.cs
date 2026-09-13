namespace _04_Domain.Entities.ObjectsFields
{
    public class AvailableTime : BaseEntity
    {
        // Disponibilidade de Tempo: Período integral, meio período, fins de semana, sob demanda.
        public string AvailableTimeName { get; set; }
    }
}