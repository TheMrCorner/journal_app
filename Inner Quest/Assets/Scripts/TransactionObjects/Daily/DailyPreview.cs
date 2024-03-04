using System.Collections;
using System.Collections.Generic;

namespace InnerQuest
{

    [System.Serializable]
    public class DailyPreview
    {
        public int idDay = 0;
        public string goal = "";
        public string gratefulData = "";
        public string greatData = "";
        public string currentDay = "";
        public List<DailyEvent> dailyEvents = new List<DailyEvent>();
        public DailyTarget dailyTargets = new DailyTarget();
        public DailyMealPrep mealPrep = new DailyMealPrep();
    } // DailyPreview
} // namespace
