using Newtonsoft.Json.Converters;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace InnerQuest
{

    [System.Serializable]
    public class DailyReview
    {
        public int idDailyReview = 0;
        public int idDay = 0;
        [JsonConverter(typeof(StringEnumConverter))]
        public Mood mood = Mood.EXCELLENT;
        public int planToReality = 0;
        public int winDay = 0;
        public string notes = "";
        public bool ftComplete = false;
        public bool stComplete = false;
        public bool ttComplete = false;
        public List<DailyHabits> dailyHabits = new List<DailyHabits>();
    } // DailyReview
} // namespace
