using CuttingEdge.Conditions;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Mixpanel
{
    public class Event
    {
        private static readonly string[] ReservedProperties = new[] { "distinct_id", "token", "time", "ip", "$insert_id" };

        internal Event(string token)
        {
            Condition.Requires(token, nameof(token)).IsNotNull().IsNotEmpty().IsNotNullOrWhiteSpace();

            SerializedProperties.Add("token", token);
        }

        [JsonProperty("Event")]
        public string Name { get; set; }

        [JsonIgnore]
        public string UserId
        {
            get
            {
                if (SerializedProperties.TryGetValue("distinct_id", out var userId))
                    return userId as string;

                return null;
            }
            set => SerializedProperties["distinct_id"] = value;
        }

        [JsonIgnore]
        public DateTime? Epoch
        {
            get
            {
                if (OriginalProperties.TryGetValue("time", out var value) && value is DateTime epoch)
                {
                    return epoch;
                }

                return null;
            }
            set
            {
                if (value is null)
                    SetProperty("time", null);

                OriginalProperties["time"] = value.Value;

                var epoch = (long)value.Value.ToUniversalTime().Subtract(new DateTime(1970, 1, 1)).TotalSeconds;

                SerializedProperties["time"] = epoch;
            }
        }

        private Dictionary<string, object> OriginalProperties { get; } = new Dictionary<string, object>();

        [JsonProperty("Properties")]
        internal Dictionary<string, object> SerializedProperties { get; } = new Dictionary<string, object>();

        public object this[string index]
        {
            get => SerializedProperties[index];
            set
            {
                Condition.Requires(index, nameof(index))
                    .IsNotNull()
                    .IsNotEmpty()
                    .IsNotNullOrWhiteSpace()
                    .DoesNotStartWith("mp_", StringComparison.CurrentCultureIgnoreCase, "Properties for events may not start with \"mp_\" because they are reserved by Mixpanel.")
                    .Evaluate(!ReservedProperties.Any(i => i.Equals(index, StringComparison.CurrentCultureIgnoreCase)), $"{index} is a reserved property by Mixpanel");

                SetProperty(index, value);
            }
        }

        private void SetProperty(string index, object value)
        {
            OriginalProperties[index] = value;

            if (value is DateTime dateTime)
            {
                dateTime = dateTime.ToUniversalTime();

                var datePart = dateTime.ToString("yyyy-MM-DD");

                var timePart = dateTime.ToString("HH:mm:ss");

                SerializedProperties[index] = $"{datePart}T{timePart}";

                return;
            }

            SerializedProperties[index] = value;
        }
    }
}
