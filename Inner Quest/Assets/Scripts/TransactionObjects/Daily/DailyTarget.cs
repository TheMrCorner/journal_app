using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace InnerQuest
{

    [System.Serializable]
    public class DailyTarget 
    {
        public int idTarget = 0;
        public int idDay = 0;
        public string firstTarget = "";
        [JsonConverter(typeof(StringEnumConverter))]
        public TargetDuration ftDuration = TargetDuration.HALF_HOUR;
        public string secondTarget = "";
        [JsonConverter(typeof(StringEnumConverter))]
        public TargetDuration stDuration = TargetDuration.HALF_HOUR;
        public string thirdTarget = "";
        [JsonConverter(typeof(StringEnumConverter))]
        public TargetDuration ttDuration = TargetDuration.HALF_HOUR;
    } // DailyTarget
} // namespace
