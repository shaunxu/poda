using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Nanook.Entities
{
    public class Tag
    {
        public Guid ID { get; set; }
        public string Value { get; set; }
        public long ReferneceCount { get; set; }
    }
}