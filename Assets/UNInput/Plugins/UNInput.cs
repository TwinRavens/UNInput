//System Includes
using System.Collections.Generic;

//Unity Includes
using UnityEngine;

namespace UniversalNetworkInput
{
    /// <summary>
    /// Main class of Universal Network Input
    /// </summary>
    public static class UNInput
    {
        private static List<VirtualInput> registeredDevices
            = new List<VirtualInput>();

        private static GameObject m_instance;

        public static GameObject instance
        {
            get { return m_instance; }
        }

        static UNInput()
        {
            //Instantiate loop object to run our coroutine loop
            m_instance = GameObject.Instantiate(new GameObject("UNInput Loop Object"));

            //Make object untouchable
            m_instance.transform.hideFlags = HideFlags.HideInHierarchy | HideFlags.HideInInspector | HideFlags.NotEditable | HideFlags.DontUnloadUnusedAsset;
            GameObject.DontDestroyOnLoad(m_instance);

            //Add loop component
            m_instance.AddComponent<Hardware.JoystickChecker>();
        }

        public static int GetInputIndex(string name)
        {
            for (int i = 0; i < registeredDevices.Count; i++)
            {
                if (registeredDevices[i].name == name)
                    return i;
            }
            return -1;
        }

        public static bool GetInputReference(int id, out VirtualInput vi)
        {
            if (id >= 0 && id < registeredDevices.Count)
            {
                vi = registeredDevices[id];
                return true;
            }
            vi = null;
            return false;
        }

        public static bool GetInputReference<T>(string name, out T vi) where T : VirtualInput
        {
            for (int i = 0; i < registeredDevices.Count; i++)
            {
                if (registeredDevices[i].name == name)
                {
                    vi = (T)registeredDevices[i];
                    return true;
                }
            }
            vi = null;
            return false;
        }

        public static bool InputExists(string name)
        {
            bool exists = false;
            for (int i = 0; i < registeredDevices.Count; i++)
            {
                exists |= (registeredDevices[i].name == name);
            }
            return exists;
        }

        public static bool AxisExists(string name)
        {
            bool exists = false;
            for (int i = 0; i < registeredDevices.Count; i++)
            {
                exists |= registeredDevices[i].AxisExists(name);
            }
            return exists;
        }

        public static bool AxisExists(int id, string name)
        {
            VirtualInput vi;
            if (GetInputReference(id, out vi))
                return vi.AxisExists(name);

            return false;
        }

        public static bool ButtonExists(string name)
        {
            bool exists = false;
            for (int i = 0; i < registeredDevices.Count; i++)
            {
                exists |= registeredDevices[i].ButtonExists(name);
            }
            return exists;
        }

        public static bool ButtonExists(int id, string name)
        {
            VirtualInput vi;
            if (GetInputReference(id, out vi))
                return vi.ButtonExists(name);

            return false;
        }

        public static float GetAxis(string name)
        {
            return GetAxis(name, false);
        }

        public static float GetAxis(AxisCode axis)
        {
            return GetAxis(axis, false);
        }

        public static float GetAxis(int id, string name)
        {
            return GetAxis(id, name, false);
        }

        public static float GetAxis(int id, AxisCode axis)
        {
            return GetAxis(id, axis, false);
        }

        public static float GetAxisRaw(string name)
        {
            return GetAxis(name, true);
        }

        public static float GetAxisRaw(AxisCode axis)
        {
            return GetAxis(axis, true);
        }

        public static float GetAxisRaw(int id, string name)
        {
            return GetAxis(id, name, true);
        }

        public static float GetAxisRaw(int id, AxisCode axis)
        {
            return GetAxis(id, axis, true);
        }

        private static float GetAxis(string name, bool raw)
        {
            float axis = 0.0f;
            for (int i = 0; i < registeredDevices.Count; i++)
            {
                axis += registeredDevices[i].GetAxis(name, raw);
            }
            return Mathf.Clamp(axis, -1.0f, 1.0f);
        }

        private static float GetAxis(AxisCode axis, bool raw)
        {
            float value = 0.0f;
            for (int i = 0; i < registeredDevices.Count; i++)
            {
                value += registeredDevices[i].GetAxis(axis, raw);
            }
            return Mathf.Clamp(value, -1.0f, 1.0f);
        }

        private static float GetAxis(int id, string name, bool raw)
        {
            VirtualInput vi;
            if (GetInputReference(id, out vi))
                return vi.GetAxis(name, raw);

            return 0;
        }

        private static float GetAxis(int id, AxisCode axis, bool raw)
        {
            VirtualInput vi;
            if (GetInputReference(id, out vi))
                return vi.GetAxis(axis, raw);

            return 0;
        }

        public static bool GetButton(string name)
        {
            bool button = false;
            for (int i = 0; i < registeredDevices.Count; i++)
            {
                button |= registeredDevices[i].GetButton(name);
            }
            return button;
        }

        public static bool GetButton(ButtonCode code)
        {
            bool button = false;
            for (int i = 0; i < registeredDevices.Count; i++)
            {
                button |= registeredDevices[i].GetButton(code);
            }
            return button;
        }

        public static bool GetButton(int id, string name)
        {
            VirtualInput vi;
            if (GetInputReference(id, out vi))
                return vi.GetButton(name);

            return false;
        }

        public static bool GetButton(int id, ButtonCode code)
        {
            VirtualInput vi;
            if (GetInputReference(id, out vi))
                return vi.GetButton(code);

            return false;
        }

        public static bool GetButtonDown(string name)
        {
            bool button = false;
            for (int i = 0; i < registeredDevices.Count; i++)
            {
                button |= registeredDevices[i].GetButtonDown(name);
            }
            return button;
        }

        public static bool GetButtonDown(ButtonCode code)
        {
            bool button = false;
            for (int i = 0; i < registeredDevices.Count; i++)
            {
                button |= registeredDevices[i].GetButtonDown(code);
            }
            return button;
        }

        public static bool GetButtonDown(int id, string name)
        {
            VirtualInput vi;
            if (GetInputReference(id, out vi))
                return vi.GetButtonDown(name);

            return false;
        }

        public static bool GetButtonDown(int id, ButtonCode code)
        {
            VirtualInput vi;
            if (GetInputReference(id, out vi))
                return vi.GetButtonDown(code);

            return false;
        }

        public static bool GetButtonUp(string name)
        {
            bool button = false;
            for (int i = 0; i < registeredDevices.Count; i++)
            {
                button |= registeredDevices[i].GetButtonUp(name);
            }
            return button;
        }

        public static bool GetButtonUp(ButtonCode code)
        {
            bool button = false;
            for (int i = 0; i < registeredDevices.Count; i++)
            {
                button |= registeredDevices[i].GetButtonUp(code);
            }
            return button;
        }

        public static bool GetButtonUp(int id, string name)
        {
            VirtualInput vi;
            if (GetInputReference(id, out vi))
                return vi.GetButtonUp(name);

            return false;
        }

        public static bool GetButtonUp(int id, ButtonCode code)
        {
            VirtualInput vi;
            if (GetInputReference(id, out vi))
                return vi.GetButtonUp(code);

            return false;
        }

        public static void SetButtonDown(string name)
        {
            for (int i = 0; i < registeredDevices.Count; i++)
            {
                registeredDevices[i].SetButtonDown(name);
            }
        }

        public static void SetButtonDown(int id, string name)
        {
            VirtualInput vi;
            if (GetInputReference(id, out vi))
                vi.SetButtonDown(name);
        }

        public static void SetButtonUp(string name)
        {
            for (int i = 0; i < registeredDevices.Count; i++)
            {
                registeredDevices[i].SetButtonUp(name);
            }
        }

        public static void SetButtonUp(int id, string name)
        {
            VirtualInput vi;
            if (GetInputReference(id, out vi))
                vi.SetButtonUp(name);
        }

        public static void SetAxisPositive(string name)
        {
            for (int i = 0; i < registeredDevices.Count; i++)
            {
                registeredDevices[i].SetAxisPositive(name);
            }
        }

        public static void SetAxisPositive(int id, string name)
        {
            VirtualInput vi;
            if (GetInputReference(id, out vi))
                vi.SetAxisPositive(name);
        }

        public static void SetAxisNegative(string name)
        {
            for (int i = 0; i < registeredDevices.Count; i++)
            {
                registeredDevices[i].SetAxisNegative(name);
            }
        }

        public static void SetAxisNegative(int id, string name)
        {
            VirtualInput vi;
            if (GetInputReference(id, out vi))
                vi.SetAxisNegative(name);
        }

        public static void SetAxisZero(string name)
        {
            for (int i = 0; i < registeredDevices.Count; i++)
            {
                registeredDevices[i].SetAxisZero(name);
            }
        }

        public static void SetAxisZero(int id, string name)
        {
            VirtualInput vi;
            if (GetInputReference(id, out vi))
                vi.SetAxisZero(name);
        }

        public static void SetAxis(string name, float value)
        {
            for (int i = 0; i < registeredDevices.Count; i++)
            {
                registeredDevices[i].SetAxis(name, value);
            }
        }

        public static void SetAxis(int id, string name, float value)
        {
            VirtualInput vi;
            if (GetInputReference(id, out vi))
                vi.SetAxis(name, value);
        }

		public static int RegisterVirtualInput(VirtualInput input)
		{
			for (int i = 0; i < registeredDevices.Count; i++)
			{
				if (registeredDevices[i].name == input.name)
				{
					Debug.LogError("Input with name " + input.name + " already exists!");
					return -1;
				}

				if (!registeredDevices[i].connected)
				{
					registeredDevices[i] = input;
					return i;
				}
			}

			registeredDevices.Add(input);
			return registeredDevices.Count - 1;
		}

		public static Vector3 mousePosition
        {
            get { return Input.mousePosition; }
        }

        // virtual axis and button classes - applies to mobile input
        // Can be mapped to touch joysticks, tilt, gyro, etc, depending on desired implementation.
        // Could also be implemented by other input devices - kinect, electronic sensors, etc
        public class VirtualAxis
        {
            public string name { get; private set; }
            private float m_Value;
            public bool matchWithInputManager { get; private set; }

            public VirtualAxis(string name) : this(name, true){  }

            public VirtualAxis(string name, bool matchToInputSettings)
            {
                this.name = name;
                matchWithInputManager = matchToInputSettings;
            }

            // a controller gameobject (eg. a virtual thumbstick) should update this class
            public void Update(float value)
            {
                m_Value = value;
            }

            public float GetValue
            {
                get { return m_Value; }
            }

            public float GetValueRaw
            {
                get { return m_Value; }
            }
        }

        // a controller gameobject (eg. a virtual GUI button) should call the
        // 'pressed' function of this class. Other objects can then read the
        // Get/Down/Up state of this button.
        public class VirtualButton
        {
            public string name { get; private set; }
            public bool matchWithInputManager { get; private set; }

            private int m_LastPressedFrame = -5;
            private int m_ReleasedFrame = -5;
            private bool m_Pressed;

            public VirtualButton(string name)
                : this(name, true)
            {
            }

            public VirtualButton(string name, bool matchToInputSettings)
            {
                this.name = name;
                matchWithInputManager = matchToInputSettings;
            }

            // A controller gameobject should call this function when the button is pressed down
            public void Pressed()
            {
                if (m_Pressed)
                {
                    return;
                }
                m_Pressed = true;
                m_LastPressedFrame = Time.frameCount;
            }

            // A controller gameobject should call this function when the button is released
            public void Released()
            {
                m_Pressed = false;
                m_ReleasedFrame = Time.frameCount;
            }

            // these are the states of the button which can be read via the cross platform input system
            public bool GetButton
            {
                get { return m_Pressed; }
            }

            public bool GetButtonDown
            {
                get
                {
                    return m_LastPressedFrame - Time.frameCount == -1;
                }
            }

            public bool GetButtonUp
            {
                get
                {
                    return (m_ReleasedFrame == Time.frameCount - 1);
                }
            }
        }
    }
}
