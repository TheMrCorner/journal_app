using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Profiling.Memory.Experimental;
using UnityEngine;

namespace InnerQuest
{
    [Serializable]
    public class URLHolderEntry
    {
        public string key;
        public string value;
    }

    [CreateAssetMenu(menuName = "InnerQuest/New_URL_Holder", order = 1)]
    public class URLHolder : ScriptableObject
    {
        public List<URLHolderEntry> urls;

        public URLHolderEntry findEntryByKey(string key)
        {
            return urls.FirstOrDefault(url => url.key == key);
        } // URLHolderEntry
    } // URLHolder
} // namespace
