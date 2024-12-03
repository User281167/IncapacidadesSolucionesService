namespace IncapacidadesSoluciones.Dto.Inability
{
    public class InabilityPaymentRes
    {
        public Guid CollaboratorId { get; set; }
        public Guid InabilityId { get; set; }
        public string Name { get; set; }
        public string LastName { get; set; }
        public string Cedula { get; set; }
        public ulong CompanyPayment { get; set; }
        public ulong HealthEntityPayment { get; set; }
        public ulong Pay { get; set; }
        public string BankAccount { get; set; }
        public string BankingEntity { get; set; }
    }
}