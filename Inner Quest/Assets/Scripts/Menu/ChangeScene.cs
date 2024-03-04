using System.Collections;
using System.Collections.Generic;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace InnerQuest
{
    public class ChangeScene : MonoBehaviour
    {
        [Header("Scene to change")]
        public string scene;

        // Start is called before the first frame update
        void Start()
        {
            gameObject.GetComponent<Button>().onClick.AddListener(ChangeSceneListener);
        } // Start

        void ChangeSceneListener()
        {
            SceneManager.LoadScene(scene);
        } // ChangeSceneListener
    } // ChangeScene
} // namespace