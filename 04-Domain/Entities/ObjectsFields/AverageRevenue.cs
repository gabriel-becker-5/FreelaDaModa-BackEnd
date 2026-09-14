namespace _04_Domain.Entities.ObjectsFields
{
    public class AverageRevenue : BaseEntity
    {
        // Faturamento médio atual: até R$ 1000, de R$ 1000 a R$ 3000 mil, de R$ 3000 A R$ 6000, acima de R$ 6000, prefiro não informar.
        public string AverageRevenueName { get; set; }
    }
}