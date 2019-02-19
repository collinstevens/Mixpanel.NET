using CuttingEdge.Conditions;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Mixpanel
{
    public class MixpanelEvent
    {
        private static readonly string[] ReservedProperties = new[] { "distinct_id", "token", "time", "ip", "$insert_id" };
        
        internal MixpanelEvent(string token)
        {
            Condition.Requires(token, nameof(token)).IsNotNull().IsNotEmpty().IsNotNullOrWhiteSpace();

            SerializedProperties.Add("token", token);
        }

        [JsonProperty("event")]
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

        [JsonProperty("properties")]
        internal Dictionary<string, object> SerializedProperties { get; } = new Dictionary<string, object>();

        public object this[string propertyName]
        {
            get => SerializedProperties[propertyName];
            set
            {
                Condition.Requires(propertyName, nameof(propertyName))
                    .IsNotNull()
                    .IsNotEmpty()
                    .IsNotNullOrWhiteSpace()
                    .DoesNotStartWith("mp_", StringComparison.CurrentCultureIgnoreCase, "Properties for events may not start with \"mp_\" because they are reserved by Mixpanel.")
                    .Evaluate(!ReservedProperties.Any(i => i.Equals(propertyName, StringComparison.CurrentCultureIgnoreCase)), $"{propertyName} is a reserved property by Mixpanel");

                SetProperty(propertyName, value);
            }
        }

        private void SetProperty(string propertyName, object propertyValue)
        {
            OriginalProperties[propertyName] = propertyValue;

            if (propertyValue is DateTime dateTime)
            {
                dateTime = dateTime.ToUniversalTime();

                var datePart = dateTime.ToString("yyyy-MM-dd");

                var timePart = dateTime.ToString("HH:mm:ss");

                SerializedProperties[propertyName] = $"{datePart}T{timePart}";

                return;
            }

            SerializedProperties[propertyName] = propertyValue;
        }
    }
}
