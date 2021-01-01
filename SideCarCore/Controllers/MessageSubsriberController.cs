using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SideCarCore.DomainEntity;
using SideCarCore.KafkaRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SideCarCore.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class MessageSubscriberController : ControllerBase
    {
        [HttpGet]
        [Obsolete]
        public PaymentContext Get()
        {
            PaymentContext py=Subscriber.PushMessage();
            return py;
        }
    }
}
