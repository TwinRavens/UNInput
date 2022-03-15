//System Includes
using System.Collections.Generic;

//Unity Includes
using UnityEngine;

namespace UniversalNetworkInput
{
	#region Axis' and Button's Code Enums
	public enum AxisCode
	{
#if UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN //Windows
		None  = 0,
		/// <summary>
		/// Left Stick Horizontal
		/// </summary>
		LSH = 1, //"X axis"
		/// <summary>
		/// Left Stick Horizontal
		/// </summary>
		LeftStickHorizontal = LSH, //"X axis"
		/// <summary>
		/// Left Stick Vertical
		/// </summary>
		LSV = 2, //"Y axis"
		/// <summary>
		/// Left Stick Vertical
		/// </summary>
		LeftStickVertical = LSV, //"Y axis"
		/// <summary>
		/// Triggers
		/// </summary>
		//T = 3, //"3rd axis"
		/// <summary>
		/// Triggers
		/// </summary>
		//Triggers = 3, //"3rd axis"
		/// <summary>
		/// Right Stick Horizontal
		/// </summary>
		RSH = 4, //"4th axis"
		/// <summary>
		/// Right Stick Horizontal
		/// </summary>
		RightStickHorizontal = RSH, //"4th axis"
		/// <summary>
		/// Right Stick Vertical
		/// </summary>
		RSV = 5, //"5th axis"
		/// <summary>
		/// Right Stick Vertical
		/// </summary>
		RightStickVertical = RSV, //"5th axis"
		/*/// <summary>
		/// DPad Horizontal
		/// </summary>
		DPH = 6, //"6th axis"
		/// <summary>
		/// DPad Horizontal
		/// </summary>
		DPadHorizontal = DPH, //"6th axis"
		/// <summary>
		/// DPad Vertical
		/// </summary>
		DPV = 7, //"7th axis"
		/// <summary>
		/// DPad Vertical
		/// </summary>
		DPadVertical = 7, //"7th axis"*/
		/// <summary>
		/// Gamepad Xbox: Left Trigger | Gamepad Playstation: L2
		/// </summary>
		LT = 9, //"9th axis"
		/// <summary>
		/// Gamepad Xbox: Left Trigger | Gamepad Playstation: L2
		/// </summary>
		LeftTrigger = LT, //"9th axis"
		/// <summary>
		/// Gamepad Xbox: Right Trigger | Gamepad Playstation: R2
		/// </summary>
		RT = 10, //"10th axis"
		/// <summary>
		/// Gamepad Xbox: Right Trigger | Gamepad Playstation: R2
		/// </summary>
		RightTrigger = RT, //"10th axis"
#elif UNITY_STANDALONE_OSX || UNITY_EDITOR_OSX || UNITY_IOS //MacOS or iOS
		/// <summary>
		/// Left Stick Horizontal
		/// </summary>
		LSH = 1, //"X axis"
		/// <summary>
		/// Left Stick Horizontal
		/// </summary>
		LefStickHorizontal = LSH, //"X axis"
		/// <summary>
		/// Left Stick Vertical
		/// </summary>
		LSV = 2, //"Y axis"
		/// <summary>
		/// Left Stick Vertical
		/// </summary>
		LeftStickVertical = LSV, //"Y axis"
		/// <summary>
		/// Right Stick Horizontal
		/// </summary>
		RSH = 3, //"4th axis"
		/// <summary>
		/// Right Stick Horizontal
		/// </summary>
		RightStickHorizontal = RSH, //"4th axis"
		/// <summary>
		/// Right Stick Vertical
		/// </summary>
		RSV = 4, //"5th axis"
		/// <summary>
		/// Right Stick Vertical
		/// </summary>
		RightStickVertical = RSV, //"5th axis"
		/// <summary>
		/// Gamepad Xbox: Left Trigger | Gamepad Playstation: L2
		/// </summary>
		LT = 5, //"9th axis"
		/// <summary>
		/// Gamepad Xbox: Left Trigger | Gamepad Playstation: L2
		/// </summary>
		LeftTrigger = LT, //"9th axis"
		/// <summary>
		/// Gamepad Xbox: Right Trigger | Gamepad Playstation: R2
		/// </summary>
		RT = 6, //"10th axis"
		/// <summary>
		/// Gamepad Xbox: Right Trigger | Gamepad Playstation: R2
		/// </summary>
		RightTrigger = RT, //"10th axis"
#elif UNITY_EDITOR_LINUX || UNITY_STANDALONE_LINUX || UNITY_ANDROID //Linux or Android
		/// <summary>
		/// No Axis Code Selected
		/// </summary>
		None = 0,
		/// <summary>
		/// Left Stick Horizontal
		/// </summary>
		LSH = 1, //"X axis"
				 /// <summary>
				 /// Left Stick Horizontal
				 /// </summary>
		LeftStickHorizontal = LSH, //"X axis"
								   /// <summary>
								   /// Left Stick Vertical
								   /// </summary>
		LSV = 2, //"Y axis"
				 /// <summary>
				 /// Left Stick Vertical
				 /// </summary>
		LeftStickVertical = LSV, //"Y axis"
								 /// <summary>
								 /// Right Stick Horizontal
								 /// </summary>
		RSH = 3, //"4th axis"
				 /// <summary>
				 /// Right Stick Horizontal
				 /// </summary>
		RightStickHorizontal = RSH, //"4th axis"
									/// <summary>
									/// Right Stick Vertical
									/// </summary>
		RSV = 4, //"5th axis"
				 /// <summary>
				 /// Right Stick Vertical
				 /// </summary>
		RightStickVertical = RSV, //"5th axis"
								  /// <summary>
								  /// Gamepad Xbox: Left Trigger | Gamepad Playstation: L2
								  /// </summary>
		LT = 12, //"9th axis"
				 /// <summary>
				 /// Gamepad Xbox: Left Trigger | Gamepad Playstation: L2
				 /// </summary>
		LeftTrigger = LT, //"9th axis"
						  /// <summary>
						  /// Gamepad Xbox: Right Trigger | Gamepad Playstation: R2
						  /// </summary>
		RT = 13, //"10th axis"
				 /// <summary>
				 /// Gamepad Xbox: Right Trigger | Gamepad Playstation: R2
				 /// </summary>
		RightTrigger = RT, //"10th axis"
#endif
	}
	public enum ButtonCode
	{
#if UNINPUT_WIN //Windows
		/// <summary>
		/// No Button Code Selected
		/// </summary>
		None = 0,
        /// <summary>
        /// Gamepad Xbox: Button A | Gamepad Playstation: Button X
        /// </summary>
        A = 1,
		/// <summary>
		/// Gamepad Xbox: Button B | Gamepad Playstation: Button Circle
		/// </summary>
		B = 2,
		/// <summary>
		/// Gamepad Xbox: Button X | Gamepad Playstation: Button Square
		/// </summary>
		X = 3,
        /// <summary>
        /// Gamepad Xbox: Button Y | Gamepad Playstation: Button Triangle
        /// </summary>
        Y = 4,
        /// <summary>
        /// Gamepad Xbox: Button Left Bumper | Gamepad Playstation: Button L1
        /// </summary>
        LB = 5,
        /// <summary>
        /// Gamepad Xbox: Button Left Bumper | Gamepad Playstation: Button L1
        /// </summary>
        LeftBumper = LB,
        /// <summary>
        /// Gamepad Xbox: Button Right Bumper | Gamepad Playstation: Button R1
        /// </summary>
        RB = 6,
		/// <summary>
		/// Gamepad Xbox: Button Right Bumper | Gamepad Playstation: Button R1
		/// </summary>
		RightBumper = RB,
        /// <summary>
        /// Gamepad Xbox: Button Back | Gamepad Playstation: Button Select
        /// </summary>
        Back = 7,
		/// <summary>
		/// Button Start
		/// </summary>
		Start = 8,
        /// <summary>
        /// Gamepad Xbox: Button Left Stick | Gamepad Playstation: Button L3
        /// </summary>
        LS = 9,
        /// <summary>
        /// Gamepad Xbox: Button Left Stick | Gamepad Playstation: Button L3
        /// </summary>
        LeftStick = LS,
        /// <summary>
        /// Gamepad Xbox: Button Right Stick | Gamepad Playstation: Button R3
        /// </summary>
        RS = 10,
        /// <summary>
        /// Gamepad Xbox: Button Right Stick | Gamepad Playstation: Button R3
        /// </summary>
        RightStick = RS,
        /// <summary>
        /// Button DPad Up
        /// </summary>
        DPU = 21,
        /// <summary>
        /// Button DPad Up
        /// </summary>
        DPadUp = DPU,
        /// <summary>
        /// Button DPad Down
        /// </summary>
        DPD = 22,
        /// <summary>
        /// Button DPad Down
        /// </summary>
        DPadDown = DPD,
        /// <summary>
        /// Button DPad Left
        /// </summary>
        DPL = 23,
        /// <summary>
        /// Button DPad Left
        /// </summary>
        DPadLeft = DPL,
        /// <summary>
        /// Button DPad Right
        /// </summary>
        DPR = 24,
        /// <summary>
        /// Button DPad Right
        /// </summary>
        DPadRight = DPR
#elif UNINPUT_OSX //MAC
		/// <summary>
		/// Gamepad Xbox: Button A | Gamepad Playstation: Button X
		/// </summary>
		A = 17,
		/// <summary>
		/// Gamepad Xbox: Button B | Gamepad Playstation: Button Circle
		/// </summary>
		B = 18,
		/// <summary>
		/// Gamepad Xbox: Button X | Gamepad Playstation: Button Square
		/// </summary>
		X = 19,
		/// <summary>
		/// Gamepad Xbox: Button Y | Gamepad Playstation: Button Triangle
		/// </summary>
		Y = 20,
		/// <summary>
		/// Gamepad Xbox: Button Left Bumper | Gamepad Playstation: Button L1
		/// </summary>
		LB = 14,
		/// <summary>
		/// Gamepad Xbox: Button Left Bumper | Gamepad Playstation: Button L1
		/// </summary>
		LeftBumper = LB,
		/// <summary>
		/// Gamepad Xbox: Button Right Bumper | Gamepad Playstation: Button R1
		/// </summary>
		RB = 15,
		/// <summary>
		/// Gamepad Xbox: Button Right Bumper | Gamepad Playstation: Button R1
		/// </summary>
		RightBumper = RB,
		/// <summary>
		/// Gamepad Xbox: Button Back | Gamepad Playstation: Button Select
		/// </summary>
		Back = 11,
		/// <summary>
		/// Button Start
		/// </summary>
		Start = 10,
		/// <summary>
		/// Gamepad Xbox: Button Left Stick | Gamepad Playstation: Button L3
		/// </summary>
		LS = 12,
		/// <summary>
		/// Gamepad Xbox: Button Left Stick | Gamepad Playstation: Button L3
		/// </summary>
		LeftStick = LS,
		/// <summary>
		/// Gamepad Xbox: Button Right Stick | Gamepad Playstation: Button R3
		/// </summary>
		RS = 13
		/// <summary>
		/// Gamepad Xbox: Button Right Stick | Gamepad Playstation: Button R3
		/// </summary>
		RightStick = RS,
		/// <summary>
		/// Button DPad Up
		/// </summary>
		DPU = 6,
		/// <summary>
		/// Button DPad Up
		/// </summary>
		DPadUp = DPU,
		/// <summary>
		/// Button DPad Down
		/// </summary>
		DPD = 7,
		/// <summary>
		/// Button DPad Down
		/// </summary>
		DPadDown = DPD,
		/// <summary>
		/// Button DPad Left
		/// </summary>
		DPL = 8,
		/// <summary>
		/// Button DPad Left
		/// </summary>
		DPadLeft = DPL,
		/// <summary>
		/// Button DPad Right
		/// </summary>
		DPR = 9,
		/// <summary>
		/// Button DPad Right
		/// </summary>
		DPadRight = DPR
#elif UNINPUT_ANDROID //Linux
		/// <summary>
		/// No Button Code Selected
		/// </summary>
		None = 0,
		/// <summary>
		/// Gamepad Xbox: Button A | Gamepad Playstation: Button X
		/// </summary>
		A = 0,
		/// <summary>
		/// Gamepad Xbox: Button B | Gamepad Playstation: Button Circle
		/// </summary>
		B = 1,
		/// <summary>
		/// Gamepad Xbox: Button X | Gamepad Playstation: Button Square
		/// </summary>
		X = 2,
		/// <summary>
		/// Gamepad Xbox: Button Y | Gamepad Playstation: Button Triangle
		/// </summary>
		Y = 3,
		/// <summary>
		/// Gamepad Xbox: Button Left Bumper | Gamepad Playstation: Button L1
		/// </summary>
		LB = 4,
		/// <summary>
		/// Gamepad Xbox: Button Left Bumper | Gamepad Playstation: Button L1
		/// </summary>
		LeftBumper = LB,
		/// <summary>
		/// Gamepad Xbox: Button Right Bumper | Gamepad Playstation: Button R1
		/// </summary>
		RB = 5,
		/// <summary>
		/// Gamepad Xbox: Button Right Bumper | Gamepad Playstation: Button R1
		/// </summary>
		RightBumper = RB,
		/// <summary>
		/// Gamepad Xbox: Button Back | Gamepad Playstation: Button Select
		/// </summary>
		Back = 7,
		/// <summary>
		/// Button Start
		/// </summary>
		Start = 10,
		/// <summary>
		/// Gamepad Xbox: Button Left Stick | Gamepad Playstation: Button L3
		/// </summary>
		LS = 8,
		/// <summary>
		/// Gamepad Xbox: Button Left Stick | Gamepad Playstation: Button L3
		/// </summary>
		LeftStick = LS,
		/// <summary>
		/// Gamepad Xbox: Button Right Stick | Gamepad Playstation: Button R3
		/// </summary>
		RS = 9,
		/// <summary>
		/// Gamepad Xbox: Button Right Stick | Gamepad Playstation: Button R3
		/// </summary>
		RightStick = RS,
		/// <summary>
		/// Button DPad Up
		/// </summary>
		DPU = 14,
		/// <summary>
		/// Button DPad Up
		/// </summary>
		DPadUp = DPU,
		/// <summary>
		/// Button DPad Down
		/// </summary>
		DPD = 14,
		/// <summary>
		/// Button DPad Down
		/// </summary>
		DPadDown = DPD,
		/// <summary>
		/// Button DPad Left
		/// </summary>
		DPL = 14,
		/// <summary>
		/// Button DPad Left
		/// </summary>
		DPadLeft = DPL,
		/// <summary>
		/// Button DPad Right
		/// </summary>
		DPR = 14,
		/// <summary>
		/// Button DPad Right
		/// </summary>
		DPadRight = DPR
#endif
	}
	#endregion

	/// <summary>
	/// Abstract class that represents every kind of Input
	/// registered control on UNInput's list
	/// </summary>
	public abstract class VirtualInput
	{
		public Vector3 virtualMousePosition { get; private set; }

		private bool m_connected;

		public bool connected
		{
			get { return m_connected; }
			set { m_connected = value; }
		}

		private string m_name;

		public string name
		{
			get { return m_name; }
			set { if (value.Length != 0) m_name = value; }
		}

		protected Dictionary<string, UNInput.VirtualAxis> m_VirtualAxes =
			new Dictionary<string, UNInput.VirtualAxis>();
		// Dictionary to store the name relating to the virtual axes
		protected Dictionary<string, UNInput.VirtualButton> m_VirtualButtons =
			new Dictionary<string, UNInput.VirtualButton>();
		// list of the axis and button names that have been flagged to always use a virtual axis or button

		public VirtualInput(string name, bool connected = false)
		{
			this.name = name;
			this.connected = connected;
		}

		public bool AxisExists(string name)
		{
			return m_VirtualAxes.ContainsKey(name);
		}

		public bool ButtonExists(string name)
		{
			return m_VirtualButtons.ContainsKey(name);
		}

		public void RegisterVirtualAxis(UNInput.VirtualAxis axis)
		{
			// check if we already have an axis with that name and log and error if we do
			if (m_VirtualAxes.ContainsKey(axis.name))
			{
				Debug.LogWarning("There is already a virtual axis named " + axis.name + " registered.");
			}
			else
			{
				// add any new axes
				m_VirtualAxes.Add(axis.name, axis);
			}
		}


		public void RegisterVirtualButton(UNInput.VirtualButton button)
		{
			// check if already have a buttin with that name and log an error if we do
			if (m_VirtualButtons.ContainsKey(button.name))
			{
				Debug.LogWarning("There is already a virtual button named " + button.name + " registered.");
			}
			else
			{
				// add any new buttons
				m_VirtualButtons.Add(button.name, button);
			}
		}


		public void UnRegisterVirtualAxis(string name)
		{
			// if we have an axis with that name then remove it from our dictionary of registered axes
			if (m_VirtualAxes.ContainsKey(name))
			{
				m_VirtualAxes.Remove(name);
			}
		}


		public void UnRegisterVirtualButton(string name)
		{
			// if we have a button with this name then remove it from our dictionary of registered buttons
			if (m_VirtualButtons.ContainsKey(name))
			{
				m_VirtualButtons.Remove(name);
			}
		}


		// returns a reference to a named virtual axis if it exists otherwise null
		public UNInput.VirtualAxis VirtualAxisReference(string name)
		{
			return m_VirtualAxes.ContainsKey(name) ? m_VirtualAxes[name] : null;
		}


		public void SetVirtualMousePositionX(float f)
		{
			virtualMousePosition = new Vector3(f, virtualMousePosition.y, virtualMousePosition.z);
		}


		public void SetVirtualMousePositionY(float f)
		{
			virtualMousePosition = new Vector3(virtualMousePosition.x, f, virtualMousePosition.z);
		}


		public void SetVirtualMousePositionZ(float f)
		{
			virtualMousePosition = new Vector3(virtualMousePosition.x, virtualMousePosition.y, f);
		}

		public abstract float GetAxis(string name, bool raw);
		public abstract float GetAxis(AxisCode name, bool raw);
		public abstract bool GetButton(string name);
		public abstract bool GetButton(ButtonCode name);
		public abstract bool GetButtonDown(string name);
		public abstract bool GetButtonDown(ButtonCode name);
		public abstract bool GetButtonUp(string name);
		public abstract bool GetButtonUp(ButtonCode name);
		public abstract void SetButtonDown(string name);
		public abstract void SetButtonUp(string name);
		public abstract void SetAxisPositive(string name);
		public abstract void SetAxisNegative(string name);
		public abstract void SetAxisZero(string name);
		public abstract void SetAxis(string name, float value);
		public abstract Vector3 MousePosition();
	}
}
