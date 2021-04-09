using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Gifter.Models
{
    public class Subscription
    {
        public int Id { get; set; }
        public int SubscriberId { get; set; }
        public int ProviderId { get; set; }
        public UserProfile UserProfile { get; set; }
    }
}
