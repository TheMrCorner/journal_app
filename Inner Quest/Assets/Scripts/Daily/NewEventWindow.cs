using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace InnerQuest
{
    public class NewEventWindow : MonoBehaviour
    {
        public TMP_Text dateText;
        public TMP_Text startHourText;
        public TMP_Text startMintText;
        public TMP_Text endHourText;
        public TMP_Text endMintText;
        public TMP_InputField title;
        public TMP_InputField description;
        public Toggle important;

        private DailyEvent dailyEvent;
        private Action<DailyEvent, int> callback;
        private const string TIME_FORMAT = "HH:mm:ss";

        private int iniHour;
        private int iniMinute;
        private int endHour;
        private int endMinute;
        private int slotMinute;
        private int index;
        private int maxHour;
        private int minHour;

        public void BuildNewEventWindow(DailyEvent dEvent, int index, string date, int sMinute, int maxHour, int minHour, Action<DailyEvent, int> callbackFunction)
        {
            dailyEvent = dEvent;
            slotMinute = sMinute;
            dateText.text = date;
            callback = callbackFunction;
            this.index = index;
            this.maxHour = maxHour;
            this.minHour = minHour;
            title.text = dailyEvent.title;
            description.text = dailyEvent.description;
            important.SetIsOnWithoutNotify(dailyEvent.important);
            DateTime initHour = DateTime.ParseExact(dailyEvent.iniHour, TIME_FORMAT, null);
            iniHour = initHour.Hour;
            iniMinute = initHour.Minute;
            startHourText.text = iniHour.ToString("D2");
            startMintText.text = iniMinute.ToString("D2");
            DateTime endingHour = DateTime.ParseExact(dailyEvent.endHour, TIME_FORMAT, null);
            endHour = endingHour.Hour;
            endMinute = endingHour.Minute;
            endHourText.text = endHour.ToString("D2");
            endMintText.text = endMinute.ToString("D2");
            title.onValueChanged.AddListener(TitleChangeCallback);
            description.onValueChanged.AddListener(DescriptionChangeCallback);
            important.onValueChanged.AddListener(ImportanceChangeCallback);
        } // BuildNewEventWindow

        public void SaveEvent()
        {
            // Validate data
            if(iniHour > endHour || title.text == "")
            {
                GameManager.GetInstance().ShowError("Error while saving event, please check that title has a value and the times are correctly entered");
            } // if
            else
            {
                // Save the event
                callback(dailyEvent, index);
                gameObject.SetActive(false);
            } // else
        } // SaveEvent

        public void CancelEditing()
        {
            gameObject.SetActive(false);
        }
        private void TitleChangeCallback(string newTitle)
        {
            dailyEvent.title = newTitle;
        } // TitleChangeCallback

        private void DescriptionChangeCallback(string newDescription)
        {
            dailyEvent.description = newDescription;
        } // DescriptionChangeCallback

        private void ImportanceChangeCallback(bool important)
        {
            dailyEvent.important = important;
        } // ImportanceChangeCallback

        private void UpdateStartTime()
        {
            dailyEvent.iniHour = String.Format("{0}:{1}:00", iniHour.ToString("D2"), iniMinute.ToString("D2"));
        } // UpdateStartTime

        private void UpdateEndTime()
        {
            dailyEvent.endHour = String.Format("{0}:{1}:00", endHour.ToString("D2"), endMinute.ToString("D2"));
        } // UpdateEndTime

        // Time callbacks
        public void CallbackChangeStartHour(int value)
        {
            iniHour += value;
            LoopNumber(ref iniHour, minHour, maxHour);
            startHourText.text = iniHour.ToString("D2");
            UpdateStartTime();
        } // CallbackAddHour

        public void CallbackChangeStartMinute(int value)
        {
            iniMinute += (slotMinute * value);
            LoopNumber(ref iniMinute, 0, 59);
            startMintText.text = iniMinute.ToString("D2");
            UpdateStartTime();
        } // CallbackChangeMinute

        public void CallbackChangeEndHour(int value)
        {
            endHour += value;
            LoopNumber(ref endHour, minHour, maxHour);
            endHourText.text = endHour.ToString("D2");
            UpdateEndTime();
        } // CallbackChangeEndHour

        public void CallbackChangeEndMinute(int value)
        {
            endMinute += (slotMinute * value);
            LoopNumber(ref endMinute, 0, 59);
            endMintText.text = endMinute.ToString("D2");
            UpdateEndTime();
        } // CallbackChangeEndMinute

        public void LoopNumber( ref int value, int min, int max)
        {
            if(value > max)
            {
                value = min;
            } // if
            else if(value < min)
            {
                value = max;
            } // else if
        } // LoopHour
    } // NewEventWindow
} // namespace