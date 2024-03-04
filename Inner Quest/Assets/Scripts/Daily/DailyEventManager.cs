using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;

namespace InnerQuest
{
    public class DailyEventManager : MonoBehaviour
    {
        public TMP_Text eventTitleText;

        public Image image;

        private int dailyEventIndex;

        private Action<int, GameObject> editEventCallback;

        public void SetDailyEvent(string eventTitle, int index, Action<int, GameObject> callback)
        {
            dailyEventIndex = index;
            eventTitleText.SetText(eventTitle);
            editEventCallback = callback;
        } // SetDailyEvent

        // When an event is clicked on
        public void OnClick()
        {
            editEventCallback(dailyEventIndex, gameObject);
        } // OnClick
    } // DailyEventManager
} // namespace
