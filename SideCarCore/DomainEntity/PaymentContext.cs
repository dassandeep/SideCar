using SideCarCore.MongoLogRepository;

namespace SideCarCore.DomainEntity
{
    [BsonCollection("Payment")]
    public class PaymentContext : MongoLogRepository.Document
    {
        public string CardHolderName { get; set; }
        public string CardType { get; set; }
        public string Topic { get; set; }
    }
}
