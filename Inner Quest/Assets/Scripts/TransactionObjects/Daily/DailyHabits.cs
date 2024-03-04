using Newtonsoft.Json.Converters;
using Newtonsoft.Json;

namespace InnerQuest
{

    [System.Serializable]
    public class DailyHabits
    {
        public int idDailyHabit = 0;
        public int idDay = 0;
        public int idHabit = 0;
        [JsonConverter(typeof(StringEnumConverter))]
        public HabitStatus status = HabitStatus.NOT_DONE;
        public Habit habit = new Habit();
    } // DailyHabits
} // namespace
