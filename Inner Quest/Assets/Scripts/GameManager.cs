using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace InnerQuest
{
    public class GameManager : MonoBehaviour
    {
        [Header("Login and authentication configuration")]
        public LoginData login;
        public GameObject requestManager;
        private GameObject requestManagerInstance;

        [Header("Resolution configuration")]
        public Canvas canvas;
        public new Camera camera;

        [Header("Error showing")]
        public GameObject errorPanel;

        private static GameManager _instance;
        private Scaler scaler;
        private Vector2 referenceResolution;

        public static GameManager GetInstance()
        {
            return _instance;
        } // GetInstance

        private void Awake()
        {
            if(_instance == null)
            {
                _instance = this;

                DontDestroyOnLoad(gameObject);

                Debug.Log("Initializing authentication...");
                requestManagerInstance = Instantiate(requestManager);
                requestManagerInstance.SetActive(true);
                requestManagerInstance.transform.SetParent(this.transform);

                requestManagerInstance.GetComponent<RequestManager>().StartRequestManager(new Login(login.username, login.password));

                referenceResolution = canvas.GetComponent<CanvasScaler>().referenceResolution;

                scaler = new Scaler(new Vector2(Screen.width, Screen.height), referenceResolution, (int)camera.orthographicSize);
                errorPanel = canvas.transform.GetChild(1).gameObject;
                SetUpErrorPanel(ref errorPanel);
            } // if
            else
            {
                _instance.canvas = canvas;
                _instance.camera = camera;
                _instance.errorPanel = canvas.transform.GetChild(1).gameObject;
                SetUpErrorPanel(ref _instance.errorPanel);
                Destroy(this);
            } // else
        } // Awake

        private void SetUpErrorPanel(ref GameObject errorPanel)
        {
            try
            {
                Button okButton = errorPanel.transform.GetChild(2).GetComponent<Button>();
                okButton.onClick.AddListener(_instance.HideError);
            } // try
            catch (Exception ex)
            {
                Debug.Log("Error while retrieving component: " + ex.Message);
            } // catch
        } // SetUpErrorPanel

        public Scaler GetScaler()
        {
            return _instance.scaler;
        } // Scaler

        public RequestManager GetRequestManager()
        {
            return _instance.requestManagerInstance.GetComponent<RequestManager>();
        } // getRequestManager

        public void ShowError(string message)
        {
            _instance.errorPanel.SetActive(true);
            GameObject child = _instance.errorPanel.transform.GetChild(0).gameObject;
            TMP_Text component = child.GetComponent<TMP_Text>();
            _instance.errorPanel.transform.GetChild(0).gameObject.GetComponent<TMP_Text>().SetText(message);
        } // ShowError

        public void HideError()
        {
            _instance.errorPanel.SetActive(false);
        } // HideError
    }
}
