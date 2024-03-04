using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace InnerQuest
{
    public class DropdownWithInfo : MonoBehaviour
    {
        private int valueToReturn;

        private Action<int, int> callbackFunction;

        public void StartDropdownWithInfo(int valueToReturn, Action<int, int> callbackFunction, TMP_Dropdown dropdown, List<TMP_Dropdown.OptionData> options, int defaultValue)
        {
            this.valueToReturn = valueToReturn;
            this.callbackFunction = callbackFunction;
            dropdown.options = options;
            dropdown.SetValueWithoutNotify(defaultValue);
            dropdown.onValueChanged.AddListener(callback);
        } // public

        private void callback(int value)
        {
            callbackFunction(valueToReturn, value);
        } // callback
    } // DropdownWithInfo
} // namespace