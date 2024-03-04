namespace InnerQuest
{

    [System.Serializable]
    public class DailyEvent
    {
        public int idEvent = 0;
        public int idDay = 0;
        public bool important = false;
        public string iniHour = "";
        public string endHour = "";
        public string title = "";
        public string description = "";
    } // DailyEvent
} // namespace
