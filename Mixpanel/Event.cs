using CuttingEdge.Conditions;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace Mixpanel
{
    public class Event
    {
        internal Event(string token)
        {
            Condition.Requires(token, nameof(token)).IsNotNull().IsNotEmpty().IsNotNullOrWhiteSpace();

            Properties.Add("token", token);
        }

        [JsonProperty("Event")]
        public string Name { get; set; }

        [JsonProperty]
        internal Dictionary<string, object> Properties { get; } = new Dictionary<string, object>();

        public object this[string index]
        {
            get => Properties[index];
            set 
            {
                Condition.Requires(index, nameof(index)).IsNotNull().IsNotEmpty().IsNotNullOrWhiteSpace().DoesNotStartWith("mp_", "Properties for events may not start with \"mp_\" because they are reserved by Mixpanel.");

                Properties[index] = value;
            }
        }
    }
}
