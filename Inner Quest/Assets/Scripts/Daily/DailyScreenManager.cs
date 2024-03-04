using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Net;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

namespace InnerQuest
{
    public class DailyScreenManager : MonoBehaviour
    {
        [Header("Preview")]
        public TMP_InputField gratefulData;
        public TMP_InputField goal;
        public TMP_InputField todayGreatData;
        public TMP_InputField firstTarget;
        public TMP_InputField secondTarget;
        public TMP_InputField thirdTarget;
        public Slider ftDuration;
        public Slider stDuration;
        public Slider ttDuration;
        public RectTransform schedule;
        [Tooltip("Precision of events duration, by default 15 minutes")]
        public int scheduleSlotLength = 15;
        [Tooltip("Day schedule configuration")]
        public string scheduleIniHour = "06:00:00";
        public string scheduleEndHour = "23:00:00";
        public GameObject eventPrefab;
        public float eventOffsetPercentage = 0.5f;
        public Button newDailyEventButton;
        public NewEventWindow newEventWindow;

        [Header("Review")]
        public Slider planToReality;
        public Slider winTheDay;
        public Slider mood;
        public Button habitStatusModifier;
        public DailyHabitsWindow dailyHabitsWindow;
        public Slider habitCompletion;
        public TMP_InputField notes;
        public Toggle firstTargetCompleted;
        public Toggle secondTargetCompleted;
        public Toggle thirdTargetCompleted;

        [Header("Configuration")]
        public URLHolder urlHolder;
        public ImagesHolder imagesHolder;

        [Header("Upper panel")]
        public Button previous;
        public Button next;
        public Button save;
        public TMP_Text currentDateText;

        // Dailydata
        private DateTime currentDay;
        private DailyPreview currentDailyPreview;
        private DailyReview currentDailyReview;
        private List<Habit> currentActiveHabits;

        // Schedule configuration
        private DateTime minHourSchedule;
        private DateTime maxHourSchedule;
        private TimeSpan scheduleTimeSpan;
        private float scheduleSlots;
        private float scheduleSlotHeight;
        private float scheduleWidth;
        private float eventOffsetSchedule;

        // Temporal variables
        private GameObject tempEventObject;

        // Constants
        private const string DATE_FORMAT = "yyyy-MM-dd";
        private const string TIME_FORMAT = "HH:mm:ss";
        private const string DATE_TEXT_FORMAT = "dd MMMM yyyy";

        #region StartUp
        // Start is called before the first frame update
        void Start()
        {
            currentDay = DateTime.Now;
            minHourSchedule = DateTime.ParseExact(scheduleIniHour, TIME_FORMAT, null);
            maxHourSchedule = DateTime.ParseExact(scheduleEndHour, TIME_FORMAT, null);
            scheduleTimeSpan = maxHourSchedule - minHourSchedule;
            scheduleSlots = (float)scheduleTimeSpan.TotalMinutes / scheduleSlotLength;       // Number of slots
            scheduleSlotHeight = schedule.rect.height / scheduleSlots;                // height of a slot = total height / slots amount
            scheduleWidth = schedule.rect.width;
            eventOffsetSchedule = scheduleWidth * eventOffsetPercentage;

            // Callbacks Preview
            newDailyEventButton.onClick.AddListener(CreateEvent);
            gratefulData.onValueChanged.AddListener(GratefulChange);
            goal.onValueChanged.AddListener(GoalChange);
            todayGreatData.onValueChanged.AddListener(TodayGreatChange);
            firstTarget.onValueChanged.AddListener(TargetDescriptionChange);
            secondTarget.onValueChanged.AddListener(TargetDescriptionChange);
            thirdTarget.onValueChanged.AddListener(TargetDescriptionChange);
            ftDuration.onValueChanged.AddListener(TargetSliderPreviewChange);
            stDuration.onValueChanged.AddListener(TargetSliderPreviewChange);
            ttDuration.onValueChanged.AddListener(TargetSliderPreviewChange);

            // Callbacks Review
            planToReality.onValueChanged.AddListener(PlanToRealityChange);
            winTheDay.onValueChanged.AddListener(WinTheDayChange);
            mood.onValueChanged.AddListener(MoodChange);
            habitStatusModifier.onClick.AddListener(SetupHabitWindow);
            notes.onValueChanged.AddListener(NotesChanged);
            firstTargetCompleted.onValueChanged.AddListener(TargetCompletedChanged);
            secondTargetCompleted.onValueChanged.AddListener(TargetCompletedChanged);
            thirdTargetCompleted.onValueChanged.AddListener(TargetCompletedChanged);


            // Upperpanel
            previous.onClick.AddListener(PreviousDay);
            next.onClick.AddListener(NextDay);
            save.onClick.AddListener(SaveData);

            ObtainDailyData();
        } // Start

        private void ObtainDailyData()
        {
            GameManager.GetInstance().GetRequestManager().SendGetRequest(String.Format(urlHolder.findEntryByKey("findDailyPreview").value, currentDay.ToString(DATE_FORMAT, null)), ParseDailyPreviewResponse);
        } // InitializeDay
        #endregion

        #region API Calls
        #region Retrieve data
        public void ParseDailyPreviewResponse(RequestResponse response)
        {
            if (response.status == (long)HttpStatusCode.NotFound)
            {
                currentDailyPreview = new DailyPreview();
                currentDailyPreview.currentDay = currentDay.ToString(DATE_FORMAT);
            } // if
            else if (response.status == (long)HttpStatusCode.OK)
            {
                currentDailyPreview = JsonConvert.DeserializeObject<DailyPreview>(response.result);
                currentDay = DateTime.ParseExact(currentDailyPreview.currentDay, DATE_FORMAT, null);
            } // else if
            else
            {
                GameManager.GetInstance().ShowError(response.error);
                return;
            } // else

            GameManager.GetInstance().GetRequestManager().SendGetRequest(String.Format(urlHolder.findEntryByKey("findReviewDay").value, currentDailyPreview.idDay), ParseDailyReviewResponse);

            // Text fields
            currentDateText.text = currentDay.Date.ToString(DATE_TEXT_FORMAT, new CultureInfo("en-US"));
            gratefulData.SetTextWithoutNotify(currentDailyPreview.gratefulData);
            goal.SetTextWithoutNotify(currentDailyPreview.goal);
            firstTarget.SetTextWithoutNotify(currentDailyPreview.dailyTargets.firstTarget);
            secondTarget.SetTextWithoutNotify(currentDailyPreview.dailyTargets.secondTarget);
            thirdTarget.SetTextWithoutNotify(currentDailyPreview.dailyTargets.thirdTarget);
            ftDuration.SetValueWithoutNotify((float)currentDailyPreview.dailyTargets.ftDuration);
            stDuration.SetValueWithoutNotify((float)currentDailyPreview.dailyTargets.stDuration);
            ttDuration.SetValueWithoutNotify((float)currentDailyPreview.dailyTargets.ttDuration);
            todayGreatData.SetTextWithoutNotify(currentDailyPreview.greatData);

            BuildSchedule(currentDailyPreview.dailyEvents);
        } // ParseDailyPreviewResponse

        public void ParseDailyReviewResponse(RequestResponse response)
        {
            if (response.status == (long)HttpStatusCode.NotFound)
            {
                currentDailyReview = new DailyReview();
                currentDailyReview.idDay = currentDailyPreview.idDay;
            } // if
            else if (response.status == (long)HttpStatusCode.OK)
            {
                currentDailyReview = JsonConvert.DeserializeObject<DailyReview>(response.result);
            } // else if
            else
            {
                GameManager.GetInstance().ShowError(response.error);
                return;
            } // else

            // TextFields
            notes.SetTextWithoutNotify(currentDailyReview.notes);

            // Sliders
            winTheDay.SetValueWithoutNotify(currentDailyReview.winDay);
            planToReality.SetValueWithoutNotify(currentDailyReview.planToReality);
            mood.SetValueWithoutNotify((int)currentDailyReview.mood);

            // Habits
            GameManager.GetInstance().GetRequestManager().SendGetRequest(String.Format(urlHolder.findEntryByKey("findHabitsByDay").value, currentDailyPreview.currentDay), ParseActiveHabitsByDay);
        } // ParseDailyReviewResponse

        public void ParseActiveHabitsByDay(RequestResponse response)
        {
            if (response.status == (long)HttpStatusCode.OK)
            {
                currentActiveHabits = JsonConvert.DeserializeObject<List<Habit>>(response.result);
            } // else if
            else
            {
                GameManager.GetInstance().ShowError(response.error);
                return;
            } // else

            foreach (Habit habit in currentActiveHabits)
            {
                int index = currentDailyReview.dailyHabits.FindIndex(dHabit => dHabit.idHabit == habit.idHabit);
                if (index < 0)
                {
                    DailyHabits dailyHabit = new DailyHabits();
                    dailyHabit.habit = habit;
                    dailyHabit.idHabit = habit.idHabit;
                    currentDailyReview.dailyHabits.Add(dailyHabit);
                } // if
            } // foreach
        } // ParseHabitsByDay
        #endregion

        #region Save data

        public void ParseSaveDailyPreviewResponse(RequestResponse response)
        {
            if (response.status == (long)HttpStatusCode.OK)
            {
                currentDailyPreview = JsonConvert.DeserializeObject<DailyPreview>(response.result);
            } // else if
            else
            {
                GameManager.GetInstance().ShowError(response.error);
                return;
            } // else

            currentDailyReview.idDay = currentDailyPreview.idDay;
            for(int i = 0; i < currentDailyReview.dailyHabits.Count; i++)
            {
                currentDailyReview.dailyHabits[i].idDay = currentDailyPreview.idDay;
            } // for
            GameManager.GetInstance().GetRequestManager().SendPostRequest(urlHolder.findEntryByKey("saveDailyReview").value, JsonConvert.SerializeObject(currentDailyReview), ParseSaveDailyReviewResponse);
        } // ParseSaveDailyPreviewResponse

        public void ParseSaveDailyReviewResponse(RequestResponse response)
        {
            if (response.status == (long)HttpStatusCode.OK)
            {
                currentDailyReview = JsonConvert.DeserializeObject<DailyReview>(response.result);
            } // else if
            else
            {
                GameManager.GetInstance().ShowError(response.error);
                return;
            } // else
        } // ParseSaveDailyReviewResponse

        #endregion
        #endregion

        #region Auxiliary methods
        private void BuildSchedule(List<DailyEvent> events)
        {
            ClearSchedule();
            for (int i = 0; i < events.Count; i++)
            {
                DailyEvent eventElement = events[i];
                SetupNewEvent(eventElement, i);
            } // for
        } // BuildSchedule

        private void SetupNewEvent(DailyEvent newEvent, int index)
        {
            // Extract times
            DateTime iniTime = DateTime.ParseExact(newEvent.iniHour, TIME_FORMAT, null);
            DateTime endTime = DateTime.ParseExact(newEvent.endHour, TIME_FORMAT, null);
            TimeSpan interval = endTime - iniTime;

            // Calculate slots (15 minutes)
            float minutes = (float)interval.TotalMinutes;
            float slots = minutes / scheduleSlotLength;
            float height = slots * scheduleSlotHeight;

            // Calculate event dimensions
            TimeSpan start = iniTime - DateTime.ParseExact(scheduleIniHour, TIME_FORMAT, null);
            float startPoint = (float)(start.TotalMinutes / scheduleSlotLength) * scheduleSlotHeight;
            float endPoint = startPoint + height;

            // Instantiate event and set dimensions
            GameObject instantiation = Instantiate(eventPrefab, schedule);
            instantiation.GetComponent<RectTransform>().offsetMin = new Vector2(eventOffsetSchedule, -endPoint);
            instantiation.GetComponent<RectTransform>().offsetMax = new Vector2(0, -startPoint);


            if (!currentDailyPreview.dailyEvents.Contains(newEvent))
            {
                currentDailyPreview.dailyEvents.Add(newEvent);
                index = currentDailyPreview.dailyEvents.IndexOf(newEvent);
            } // if

            instantiation.GetComponent<DailyEventManager>().SetDailyEvent(newEvent.title, index, EditEvent);
        } // SetupNewEvent

        private void ClearSchedule()
        {
            foreach(Transform child in schedule.transform)
            {
                Destroy(child.gameObject);
            } // foreach
        } // ClearSchedule
        #endregion

        #region Callbacks
        #region EventCallbacks
        private void EditEvent(int eventIndex, GameObject eventObject)
        {
            Debug.Log("Editing event: " + eventIndex);
            tempEventObject = eventObject;
            newEventWindow.BuildNewEventWindow(currentDailyPreview.dailyEvents[eventIndex], eventIndex, currentDailyPreview.currentDay, scheduleSlotLength, maxHourSchedule.Hour - 1, minHourSchedule.Hour, SaveDailyEvent);
            newEventWindow.gameObject.SetActive(true);
        } // EditEvent

        private void CreateEvent()
        {
            Debug.Log("Creating new event");
            DailyEvent dailyEvent = new DailyEvent();
            dailyEvent.iniHour = scheduleIniHour;
            dailyEvent.endHour = String.Format("{0}:00:00", maxHourSchedule.Hour - 1);
            dailyEvent.idDay = currentDailyPreview.idDay;
            newEventWindow.BuildNewEventWindow(dailyEvent, -1, currentDailyPreview.currentDay, scheduleSlotLength, maxHourSchedule.Hour - 1, minHourSchedule.Hour, SaveDailyEvent);
            newEventWindow.gameObject.SetActive(true);
        } // CreateEvent

        private void SaveDailyEvent(DailyEvent dEvent, int index)
        {
            SetupNewEvent(dEvent, index);
            if (tempEventObject != null)
            {
                Destroy(tempEventObject);
            } // if
        } // SaveDailyEvent
        #endregion

        #region Preview
        private void TargetSliderPreviewChange(float value)
        {
            // Update values
            Debug.Log("Slider value changed");
            currentDailyPreview.dailyTargets.ftDuration = (TargetDuration) ftDuration.value;
            currentDailyPreview.dailyTargets.stDuration = (TargetDuration) stDuration.value;
            currentDailyPreview.dailyTargets.ttDuration = (TargetDuration) ttDuration.value;
        } // SliderPreviewChange

        private void TargetDescriptionChange(string value)
        {
            Debug.Log("Updating targets");
            currentDailyPreview.dailyTargets.firstTarget = firstTarget.text;
            currentDailyPreview.dailyTargets.secondTarget = secondTarget.text;
            currentDailyPreview.dailyTargets.thirdTarget = thirdTarget.text;
        } // TargetDescriptionChange

        private void GratefulChange(string value)
        {
            currentDailyPreview.gratefulData = value;
        } // GratefulChange

        private void GoalChange(string value)
        {
            currentDailyPreview.goal = value;
        } // GoalChange

        private void TodayGreatChange(string value)
        {
            currentDailyPreview.greatData = value;
        } // TodayGreatChange
        #endregion

        #region Review

        private void PlanToRealityChange(float value)
        {
            currentDailyReview.planToReality = (int)value;
        } // PlanToRealityChanged

        private void WinTheDayChange(float value)
        {
            currentDailyReview.winDay = (int) value;
        } // WinTheDayChange

        private void MoodChange(float value)
        {
            currentDailyReview.mood = (Mood) value;
        } // MoodChange

        private void SetupHabitWindow()
        {
            dailyHabitsWindow.gameObject.SetActive(true);
            dailyHabitsWindow.SetupDailyHabitsWindow(currentDailyReview.dailyHabits, HabitsChanged);
        } // SetupHabitWindow

        private void HabitsChanged(List<DailyHabits> value)
        {
            currentDailyReview.dailyHabits = value;
        } // HabitsChanged

        private void NotesChanged(string value)
        {
            currentDailyReview.notes = value;
        } // NotesChanged

        private void TargetCompletedChanged(bool value)
        {
            currentDailyReview.ftComplete = firstTargetCompleted.isOn;
            currentDailyReview.stComplete = secondTargetCompleted.isOn;
            currentDailyReview.ttComplete = thirdTargetCompleted.isOn;
        } // ToggleChanged
        #endregion

        #region UpperPanel
        private void NextDay()
        {
            currentDay = currentDay.AddDays(1);
            Debug.Log(currentDay.ToString(DATE_TEXT_FORMAT));
            ObtainDailyData();
        } // NextDay

        private void PreviousDay()
        {
            currentDay = currentDay.AddDays(-1);
            Debug.Log(currentDay.ToString(DATE_TEXT_FORMAT));
            ObtainDailyData();
        } // PreviousDay

        private void SaveData()
        {
            GameManager.GetInstance().GetRequestManager().SendPostRequest(urlHolder.findEntryByKey("saveDailyPreview").value, JsonConvert.SerializeObject(currentDailyPreview), ParseSaveDailyPreviewResponse);
        } // SaveData
        #endregion
        #endregion
    } // DailyScreenManager
} // namespace
