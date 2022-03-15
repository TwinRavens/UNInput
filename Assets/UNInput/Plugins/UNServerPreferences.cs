//System Includes
using System;
using System.Collections.Generic;

//Unity Includes
using UnityEngine;

namespace UniversalNetworkInput.Network.Internal
{
    /// <summary>
    /// Structure that defines serialized
    /// server preferences asset object on disk
    /// </summary>
    [Serializable]
    public class UNServerPreferences : ScriptableObject
    {
        [Serializable]
        public struct Input
        {
            public enum InputType
            {
                Axis,
                Button
            }

            public InputType type;

            public AxisCode axis_code;

            public KeyCode key_down, key_up;

            public ButtonCode button_code;

            public string name;
        }

        public GameObject InterfaceAsset;

        public string AssetName;

        public List<Input> Inputs = new List<Input>();
    }
}
