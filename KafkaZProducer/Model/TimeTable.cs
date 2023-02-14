using KafkaZProducer.Util;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace KafkaZProducer.Model
{
    public class TimeTable
    {
        public string key { get; set; }
        public Time value { get; set; }
        public void Populate(int time) { 
            this.key = RandomGenerator.GetContainerKey();
            this.value = new Time()
            {
                time = RandomGenerator.GetTimeKey(time),
                value = RandomGenerator.GetTimeValue()
            };
        }
        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}
