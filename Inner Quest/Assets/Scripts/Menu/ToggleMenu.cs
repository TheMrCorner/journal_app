using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace InnerQuest
{
    public class ToggleMenu : MonoBehaviour
    {
        [Header("Menu to toggle")]
        public RectTransform menuToToggle;

        private Button button;
        private bool visible;

        // Start is called before the first frame update
        void Start()
        {
            button = gameObject.GetComponent<Button>();

            button.onClick.AddListener(ToggleMenuOnClick);
        } // Start

        void ToggleMenuOnClick()
        {
            menuToToggle.gameObject.SetActive(!menuToToggle.gameObject.activeSelf);
        } // ToggleMenuOnClick
    } // ToggleMenu
} // namespace
