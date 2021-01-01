
namespace SideCarCore.DomainEntity
{
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;
    using System;
 
    public partial class PaymentContext
    {
        public string CardHolderName { get; set; }

        public string CardType { get; set; }
        //[JsonConverter(typeof(DateFormatConverter))]
        //public DateTime TransactionDate { get; set; }
        public string TransactionDate { get; set; }
    }
    public class DateFormatConverter : IsoDateTimeConverter
    {
        public DateFormatConverter(string format)
        {
            base.DateTimeFormat = "yyyy-MM-ddTHH:mm:ss.fffZ";
        }
    }
}
