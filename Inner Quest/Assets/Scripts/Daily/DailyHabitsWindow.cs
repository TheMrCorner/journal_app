using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace InnerQuest
{
    public class DailyHabitsWindow : MonoBehaviour
    {
        public Button okButton;
        public Button cancelButton;
        public RectTransform habitColumn;
        public GameObject dailyHabitsPrefab;

        private Action<List<DailyHabits>> callbackFunction;
        private List<DailyHabits> dailyHabitsList;
        private List<TMP_Dropdown.OptionData> options;

        private void Awake()
        {
            okButton.onClick.AddListener(SaveHabits);
            cancelButton.onClick.AddListener(CloseWindow);
            List<string> optionsNames = Enum.GetNames(typeof(HabitStatus)).ToList();
            options = new List<TMP_Dropdown.OptionData>();
            foreach (string name in optionsNames)
            {
                options.Add(new TMP_Dropdown.OptionData(name));
            } // foreach
        } // Awake

        public void SetupDailyHabitsWindow(List<DailyHabits> dailyHabits, Action<List<DailyHabits>> callback)
        {
            callbackFunction = callback;
            dailyHabitsList = dailyHabits;
            foreach(DailyHabits dailyHabit in dailyHabitsList)
            {
                GameObject instantiatedObject = Instantiate(dailyHabitsPrefab, habitColumn);
                instantiatedObject.transform.GetChild(0).GetComponent<TMP_Text>().text = dailyHabit.habit.title;
                GameObject dropdownSelector = instantiatedObject.transform.GetChild(1).gameObject;
                int defaultValue = (int) dailyHabit.status;
                instantiatedObject.GetComponent<DropdownWithInfo>().StartDropdownWithInfo(dailyHabitsList.IndexOf(dailyHabit), DailyHabitChangedCallback, dropdownSelector.GetComponent<TMP_Dropdown>(), options, defaultValue);
            } // foreach
        } // SetupDailyHabitsWindow

        public void DailyHabitChangedCallback(int dailyHabit, int value)
        {
            dailyHabitsList[dailyHabit].status = (HabitStatus) value;
        } // DailyHabitChangedCallback

        private void SaveHabits()
        {
            callbackFunction(dailyHabitsList);
            ClearWindow();
            gameObject.SetActive(false);
        } // SaveHabits

        private void CloseWindow()
        {
            ClearWindow();
            gameObject.SetActive(false);
        } // CloseWindow

        private void ClearWindow()
        {
            callbackFunction = null;
            dailyHabitsList = null;
            foreach (Transform child in habitColumn.transform)
            {
                Destroy(child.gameObject);
            } // foreach
        } // ClearWindow
    } // DailyHabitsWindow
} // namespace
