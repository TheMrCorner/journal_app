using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace InnerQuest
{
    [CreateAssetMenu(menuName = "InnerQuest/New_Login_Data", order = 1)]
    public class LoginData : ScriptableObject
    {
        public string username;
        public string password;
    } // LoginData
} // InnerQuest
