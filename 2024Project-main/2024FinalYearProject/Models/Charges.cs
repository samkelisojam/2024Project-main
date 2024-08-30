namespace _2024FinalYearProject.Models
{
    public class Charges
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Amount { get; set; }
        public DateTime ChargeDate { get; set; }
        public string Description { get; set; }

        public string UserEmail { get; set; }
    }

}
