//System Includes
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

//Unity Includes
using UnityEngine;

namespace UniversalNetworkInput.Hardware
{
	/// <summary>
	/// Hardware implementation of Virtual Input
	/// interface, translates data to proper auto
	/// registered UNInput entrys on Input Manager
	/// </summary>
	public class HardwareInput : VirtualInput
	{
		public enum HardwareType
		{
			Xbox,
			Playstation,
			Generic
		}

		#region maps

		private static float[] axisXbox = new float[]
		{
            #if UNINPUT_WIN
             1f, //X axis
            -1f, //Y axis
             1f, //3rd axis
             1f, //4th axis
            -1f, //5th axis
             1f, //6th axis
             1f, //7th axis
             1f, //8th axis
             1f, //9th axis
             1f  //10th axis
            #elif UNINPUT_MAC
             1f, //X axis
            -1f, //Y axis
             1f, //3rd axis
             1f, //4th axis
            -1f, //5th axis
             1f, //6th axis
             1f, //7th axis
             1f, //8th axis
             1f, //9th axis
             1f  //10th axis
            #elif UNINPUT_ANDROID
             1f, //X axis
            -1f, //Y axis
             1f, //3rd axis
             1f, //4th axis
            -1f, //5th axis
             1f, //6th axis
             1f, //7th axis
             1f, //8th axis
             1f, //9th axis
             1f  //10th axis
            #endif
        };

		private static float[] axisPS = new float[]
		{
            #if UNINPUT_WIN
             1f, //X axis
            -1f, //Y axis
             1f, //3rd axis
             1f, //4th axis
             1f, //5th axis
            -1f, //6th axis
             1f, //7th axis
             1f, //8th axis
             1f, //9th axis
             1f  //10th axis
            #elif UNINPUT_MAC
             1f, //X axis
             1f, //Y axis
             1f, //3rd axis
             1f, //4th axis
             1f, //5th axis
            -1f, //6th axis
             1f, //7th axis
             1f, //8th axis
             1f, //9th axis
             1f  //10th axis
            #elif UNINPUT_ANDROID
             1f, //X axis
             1f, //Y axis
             1f, //3rd axis
             1f, //4th axis
             1f, //5th axis
            -1f, //6th axis
             1f, //7th axis
             1f, //8th axis
             1f, //9th axis
             1f  //10th axis           
            #endif
        };

		private static string[] axis_names =
		{
			"",
			"X axis",
			"Y axis",
			"3rd axis",
			"4th axis",
			"5th axis",
			"6th axis",
			"7th axis",
			"8th axis",
			"9th axis",
			"10th axis"
		};

		private Dictionary<string, AxisCode> m_axis = new Dictionary<string, AxisCode>
		{
            #if UNINPUT_WIN
            { "Left Stick Horizontal", AxisCode.LSH },
            { "Horizontal", AxisCode.LSH },
            { "Left Stick Vertical", AxisCode.LSV },
            { "Vertical", AxisCode.LSV },
            { "Left Trigger", AxisCode.LT },
            { "Right Stick Horizontal", AxisCode.RSH },
            { "Right Stick Vertical", AxisCode.RSV },
            { "Right Trigger", AxisCode.RT }  
            #elif UNINPUT_MAC
			{ "Left Stick Horizontal", AxisCode.LSH },
			{ "Horizontal", AxisCode.LSH },
			{ "Left Stick Vertical", AxisCode.LSV },
			{ "Vertical", AxisCode.LSV },
			{ "Left Trigger", AxisCode.LT },
			{ "Right Stick Horizontal", AxisCode.RSH },
			{ "Right Stick Vertical", AxisCode.RSV },
			{ "Right Trigger", AxisCode.RT }
            #elif UNINPUT_ANDROID
			{ "Left Stick Horizontal", AxisCode.LSH },
			{ "Horizontal", AxisCode.LSH },
			{ "Left Stick Vertical", AxisCode.LSV },
			{ "Vertical", AxisCode.LSV },
			{ "Left Trigger", AxisCode.LT },
			{ "Right Stick Horizontal", AxisCode.RSH },
			{ "Right Stick Vertical", AxisCode.RSV },
			{ "Right Trigger", AxisCode.RT }
            #endif
        };

		private Dictionary<string, ButtonCode> m_buttons = new Dictionary<string, ButtonCode>
		{
            #if UNINPUT_WIN
            { "A",  ButtonCode.A },
            { "Jump", ButtonCode.A },
            { "Submit", ButtonCode.A },
            { "Action", ButtonCode.A },
            { "B", ButtonCode.B },
            { "Back", ButtonCode.B },
            { "Select", ButtonCode.Back },
            { "Left Bumper", ButtonCode.LB },
            { "Left Stick", ButtonCode.LS },
            { "Right Bumper", ButtonCode.RB },
            { "Right Stick", ButtonCode.RS },
            { "Start", ButtonCode.Start },
            { "X", ButtonCode.X },
            { "Y", ButtonCode.Y }
            #elif UNINPUT_MAC
			{ "A",  ButtonCode.A },
			{ "Jump", ButtonCode.A },
			{ "Submit", ButtonCode.A },
			{ "Action", ButtonCode.A },
			{ "B", ButtonCode.B },
			{ "Back", ButtonCode.B },
			{ "Select", ButtonCode.Back },
			{ "Left Bumper", ButtonCode.LB },
			{ "Left Stick", ButtonCode.LS },
			{ "Right Bumper", ButtonCode.RB },
			{ "Right Stick", ButtonCode.RS },
			{ "Start", ButtonCode.Start },
			{ "X", ButtonCode.X },
			{ "Y", ButtonCode.Y },
			{ "DPad Right", ButtonCode.DPadRight },
			{ "DPad Left", ButtonCode.DPadLeft },
			{ "DPad Up", ButtonCode.DPadUp },
			{ "DPad Down", ButtonCode.DPadDown }
            #elif UNINPUT_ANDROID
			{ "A",  ButtonCode.A },
			{ "Jump", ButtonCode.A },
			{ "Submit", ButtonCode.A },
			{ "Action", ButtonCode.A },
			{ "B", ButtonCode.B },
			{ "Back", ButtonCode.B },
			{ "Select", ButtonCode.Back },
			{ "Left Bumper", ButtonCode.LB },
			{ "Left Stick", ButtonCode.LS },
			{ "Right Bumper", ButtonCode.RB },
			{ "Right Stick", ButtonCode.RS },
			{ "Start", ButtonCode.Start },
			{ "X", ButtonCode.X },
			{ "Y", ButtonCode.Y },
			{ "DPad Right", ButtonCode.DPadRight },
			{ "DPad Left", ButtonCode.DPadLeft },
			{ "DPad Up", ButtonCode.DPadUp },
			{ "DPad Down", ButtonCode.DPadDown }
            #endif
        };

		private List<KeyValuePair<AxisCode, int>> xboxForPS_Axis = new List<KeyValuePair<AxisCode, int>>
		{
            #if UNINPUT_WIN
            new KeyValuePair<AxisCode, int>(AxisCode.LeftStickHorizontal,   0),
            new KeyValuePair<AxisCode, int>(AxisCode.LeftStickVertical,    1),
            new KeyValuePair<AxisCode, int>(AxisCode.LeftTrigger,          3),
            new KeyValuePair<AxisCode, int>(AxisCode.LSH,                  0),
            new KeyValuePair<AxisCode, int>(AxisCode.LSV,                  1),
            new KeyValuePair<AxisCode, int>(AxisCode.LT,                   3),
            new KeyValuePair<AxisCode, int>(AxisCode.RightStickHorizontal, 2),
            new KeyValuePair<AxisCode, int>(AxisCode.RightStickVertical,   5),
            new KeyValuePair<AxisCode, int>(AxisCode.RightTrigger,         4),
            new KeyValuePair<AxisCode, int>(AxisCode.RSH,                  2),
            new KeyValuePair<AxisCode, int>(AxisCode.RSV,                  5),
            new KeyValuePair<AxisCode, int>(AxisCode.RT,                   4)
            #elif UNINPUT_MAC
            new KeyValuePair<AxisCode, int>(AxisCode.LeftStickHorizontal,   0),
            new KeyValuePair<AxisCode, int>(AxisCode.LeftStickVertical,    1),
            new KeyValuePair<AxisCode, int>(AxisCode.LeftTrigger,          3),
            new KeyValuePair<AxisCode, int>(AxisCode.LSH,                  0),
            new KeyValuePair<AxisCode, int>(AxisCode.LSV,                  1),
            new KeyValuePair<AxisCode, int>(AxisCode.LT,                   3),
            new KeyValuePair<AxisCode, int>(AxisCode.RightStickHorizontal, 2),
            new KeyValuePair<AxisCode, int>(AxisCode.RightStickVertical,   5),
            new KeyValuePair<AxisCode, int>(AxisCode.RightTrigger,         4),
            new KeyValuePair<AxisCode, int>(AxisCode.RSH,                  2),
            new KeyValuePair<AxisCode, int>(AxisCode.RSV,                  5),
            new KeyValuePair<AxisCode, int>(AxisCode.RT,                   4)
            #elif UNINPUT_ANDROID
            new KeyValuePair<AxisCode, int>(AxisCode.LeftStickHorizontal,   0),
			new KeyValuePair<AxisCode, int>(AxisCode.LeftStickVertical,    1),
			new KeyValuePair<AxisCode, int>(AxisCode.LeftTrigger,          3),
			new KeyValuePair<AxisCode, int>(AxisCode.LSH,                  0),
			new KeyValuePair<AxisCode, int>(AxisCode.LSV,                  1),
			new KeyValuePair<AxisCode, int>(AxisCode.LT,                   3),
			new KeyValuePair<AxisCode, int>(AxisCode.RightStickHorizontal, 2),
			new KeyValuePair<AxisCode, int>(AxisCode.RightStickVertical,   5),
			new KeyValuePair<AxisCode, int>(AxisCode.RightTrigger,         4),
			new KeyValuePair<AxisCode, int>(AxisCode.RSH,                  2),
			new KeyValuePair<AxisCode, int>(AxisCode.RSV,                  5),
			new KeyValuePair<AxisCode, int>(AxisCode.RT,                   4)
            #endif
        };

		private List<KeyValuePair<ButtonCode, int>> xboxForPS_Button = new List<KeyValuePair<ButtonCode, int>>
		{
            #if UNINPUT_WIN
            new KeyValuePair<ButtonCode, int>(ButtonCode.A, 1),
            new KeyValuePair<ButtonCode, int>(ButtonCode.B, 2),
            new KeyValuePair<ButtonCode, int>(ButtonCode.Back, 8),
            new KeyValuePair<ButtonCode, int>(ButtonCode.LB, 4),
            new KeyValuePair<ButtonCode, int>(ButtonCode.LeftBumper, 4),
            new KeyValuePair<ButtonCode, int>(ButtonCode.LeftStick, 10),
            new KeyValuePair<ButtonCode, int>(ButtonCode.LS, 10),
            new KeyValuePair<ButtonCode, int>(ButtonCode.RB, 5),
            new KeyValuePair<ButtonCode, int>(ButtonCode.RightBumper, 5),
            new KeyValuePair<ButtonCode, int>(ButtonCode.RightStick, 11),
            new KeyValuePair<ButtonCode, int>(ButtonCode.RS, 11),
            new KeyValuePair<ButtonCode, int>(ButtonCode.Start, 9),
            new KeyValuePair<ButtonCode, int>(ButtonCode.X, 0),
            new KeyValuePair<ButtonCode, int>(ButtonCode.Y, 3),
            new KeyValuePair<ButtonCode, int>(ButtonCode.DPadUp, 16),
            new KeyValuePair<ButtonCode, int>(ButtonCode.DPadDown, 17),
            new KeyValuePair<ButtonCode, int>(ButtonCode.DPadLeft, 18),
            new KeyValuePair<ButtonCode, int>(ButtonCode.DPadRight, 19)
#elif UNINPUT_MAC
            new KeyValuePair<ButtonCode, int>(ButtonCode.A, 1),
            new KeyValuePair<ButtonCode, int>(ButtonCode.B, 2),
            new KeyValuePair<ButtonCode, int>(ButtonCode.Back, 8),
            new KeyValuePair<ButtonCode, int>(ButtonCode.LB, 4),
            new KeyValuePair<ButtonCode, int>(ButtonCode.LeftBumper, 4),
            new KeyValuePair<ButtonCode, int>(ButtonCode.LeftStick, 10),
            new KeyValuePair<ButtonCode, int>(ButtonCode.LS, 10),
            new KeyValuePair<ButtonCode, int>(ButtonCode.RB, 5),
            new KeyValuePair<ButtonCode, int>(ButtonCode.RightBumper, 5),
            new KeyValuePair<ButtonCode, int>(ButtonCode.RightStick, 11),
            new KeyValuePair<ButtonCode, int>(ButtonCode.RS, 11),
            new KeyValuePair<ButtonCode, int>(ButtonCode.Start, 9),
            new KeyValuePair<ButtonCode, int>(ButtonCode.X, 0),
            new KeyValuePair<ButtonCode, int>(ButtonCode.Y, 3),
            new KeyValuePair<ButtonCode, int>(ButtonCode.DPadUp, 3),
            new KeyValuePair<ButtonCode, int>(ButtonCode.DPadDown, 3),          
            new KeyValuePair<ButtonCode, int>(ButtonCode.DPadLeft, 3),            
            new KeyValuePair<ButtonCode, int>(ButtonCode.DPadRight, 3)
#elif UNINPUT_ANDROID
            new KeyValuePair<ButtonCode, int>(ButtonCode.A, 1),
			new KeyValuePair<ButtonCode, int>(ButtonCode.B, 2),
			new KeyValuePair<ButtonCode, int>(ButtonCode.Back, 8),
			new KeyValuePair<ButtonCode, int>(ButtonCode.LB, 4),
			new KeyValuePair<ButtonCode, int>(ButtonCode.LeftBumper, 4),
			new KeyValuePair<ButtonCode, int>(ButtonCode.LeftStick, 10),
			new KeyValuePair<ButtonCode, int>(ButtonCode.LS, 10),
			new KeyValuePair<ButtonCode, int>(ButtonCode.RB, 5),
			new KeyValuePair<ButtonCode, int>(ButtonCode.RightBumper, 5),
			new KeyValuePair<ButtonCode, int>(ButtonCode.RightStick, 11),
			new KeyValuePair<ButtonCode, int>(ButtonCode.RS, 11),
			new KeyValuePair<ButtonCode, int>(ButtonCode.Start, 9),
			new KeyValuePair<ButtonCode, int>(ButtonCode.X, 0),
			new KeyValuePair<ButtonCode, int>(ButtonCode.Y, 3),
			new KeyValuePair<ButtonCode, int>(ButtonCode.DPadUp, 3),
			new KeyValuePair<ButtonCode, int>(ButtonCode.DPadDown, 3),
			new KeyValuePair<ButtonCode, int>(ButtonCode.DPadLeft, 3),
			new KeyValuePair<ButtonCode, int>(ButtonCode.DPadRight, 3)
#endif
        };

		#endregion

		private HardwareType m_type;
		public HardwareType type
		{
			get { return m_type; }
		}

		private int m_id;
		public int id
		{
			get { return m_id; }
			set { m_id = value; }
		}

		public HardwareInput(string i_name, int id = 0, bool connected = false, HardwareType type = HardwareType.Xbox) : base(i_name, connected)
		{
			m_type = type;
#if UNINPUT_WIN
            RegisterVirtualButton(new UNInput.VirtualButton("DPR"));
            RegisterVirtualButton(new UNInput.VirtualButton("DPL"));
            RegisterVirtualButton(new UNInput.VirtualButton("DPU"));
            RegisterVirtualButton(new UNInput.VirtualButton("DPD"));
#endif
			this.id = id;
		}

		private int ButtonforIntPS(ButtonCode key)
		{
			for (int i = 0; i < xboxForPS_Button.Count; ++i)
			{
				if (xboxForPS_Button[i].Key == key)
					return xboxForPS_Button[i].Value;
			}
			return 0;
		}

		private int AxisforIntPS(AxisCode key)
		{
			for (int i = 0; i < xboxForPS_Axis.Count; ++i)
			{
				if (xboxForPS_Axis[i].Key == key)
					return xboxForPS_Axis[i].Value;
			}
			return 0;
		}

		public override float GetAxis(string name, bool raw)
		{
			AxisCode axis;

			if (m_axis.TryGetValue(name, out axis))
			{
				return GetAxis(axis, raw);
			}

			return 0;
		}

		public override float GetAxis(AxisCode axis, bool raw)
		{
			if (m_id < 0 || m_id > 8)
				return 0f;
			else
			{
				string axis_name;

				if (m_type == HardwareInput.HardwareType.Xbox)
				{
					axis_name = "UNInput " + m_id + " " + axis_names[(int)(axis)];
					//Debug.Log(axis_name + " - (int)axis:" + (((int)axis) - 1) + " - " + "axisXbox[" + (((int)axis) - 1) + "]:" + axisXbox[(int)axis - 1]);
					return raw ? Input.GetAxisRaw(axis_name) * axisXbox[(int)axis - 1] : Input.GetAxis(axis_name) * axisXbox[(int)axis - 1];
				}
				else if (m_type == HardwareInput.HardwareType.Playstation)
				{
					int index = AxisforIntPS(axis);
					axis_name = "UNInput " + m_id + " " + axis_names[index + 1];
					return raw ? Input.GetAxisRaw(axis_name) * axisPS[index] : Input.GetAxis(axis_name) * axisPS[index];
				}
				else
					return 0f;
			}
		}

		public override bool GetButton(string name)
		{
			ButtonCode button;

			if (m_buttons.TryGetValue(name, out button))
			{
				return GetButton(button);
			}

			return false;
		}

		public override bool GetButton(ButtonCode button)
		{
			if (m_id < 0 || m_id > 8)
				return false;
			else
			{
				string axis_name;
				if (m_type == HardwareInput.HardwareType.Xbox)
				{
#if UNINPUT_WIN
                    UNInput.VirtualButton vi;
                    if (m_VirtualButtons.TryGetValue(Regex.Replace(button.ToString(), "([a-z])", ""), out vi))
                        return vi.GetButton;
#endif
					axis_name = "UNInput " + m_id + " button " + (int)(button - 1);
				}
				else if (m_type == HardwareInput.HardwareType.Playstation)
				{
#if UNINPUT_WIN
                    UNInput.VirtualButton vi;
                    if (m_VirtualButtons.TryGetValue(Regex.Replace(button.ToString(), "([a-z])", ""), out vi))
                        return vi.GetButton;
#endif
					axis_name = "UNInput " + m_id + " button " + ButtonforIntPS(button);
				}
				else
					return false;
				return Input.GetButton(axis_name);
			}
		}


		public override bool GetButtonDown(string name)
		{
			ButtonCode button;

			if (m_buttons.TryGetValue(name, out button))
			{
				return GetButtonDown(button);
			}

			return false;
		}

		public override bool GetButtonDown(ButtonCode button)
		{
			if (m_id < 0 || m_id > 8)
				return false;
			else
			{
				string axis_name;
				if (m_type == HardwareInput.HardwareType.Xbox)
				{
#if UNINPUT_WIN
                    UNInput.VirtualButton vi;
                    if (m_VirtualButtons.TryGetValue(Regex.Replace(button.ToString(), "([a-z])", ""), out vi))
                        return vi.GetButtonDown;
#endif
					axis_name = "UNInput " + m_id + " button " + (int)(button - 1);
				}
				else if (m_type == HardwareInput.HardwareType.Playstation)
				{
#if UNINPUT_WIN
                    UNInput.VirtualButton vi;
                    if (m_VirtualButtons.TryGetValue(Regex.Replace(button.ToString(), "([a-z])", ""), out vi))
                        return vi.GetButtonDown;
#endif
					axis_name = "UNInput " + m_id + " button " + ButtonforIntPS(button);
				}
				else
					return false;
				return Input.GetButtonDown(axis_name);
			}
		}

		public override bool GetButtonUp(string name)
		{
			ButtonCode button;

			if (m_buttons.TryGetValue(name, out button))
			{
				return GetButtonUp(button);
			}

			return false;
		}

		public override bool GetButtonUp(ButtonCode button)
		{
			if (m_id < 0 || m_id > 8)
				return false;
			else
			{
				string axis_name;
				if (m_type == HardwareInput.HardwareType.Xbox)
				{
#if UNINPUT_WIN
                    UNInput.VirtualButton vi;
                    if (m_VirtualButtons.TryGetValue(Regex.Replace(button.ToString(), "([a-z])", ""), out vi))
                        return vi.GetButtonUp;
#endif
					axis_name = "UNInput " + m_id + " button " + (int)(button - 1);
				}
				else if (m_type == HardwareInput.HardwareType.Playstation)
				{
#if UNINPUT_WIN
                    UNInput.VirtualButton vi;
                    if (m_VirtualButtons.TryGetValue(Regex.Replace(button.ToString(), "([a-z])", ""), out vi))
                        return vi.GetButtonUp;
#endif
					axis_name = "UNInput " + m_id + " button " + ButtonforIntPS(button);
				}
				else
					return false;
				return Input.GetButtonUp(axis_name);
			}
		}


		//Update DPad Buttons from DPad Axis on XBOX Controls (WINDOWS)
		public void UpdateDirectionalPads()
		{
			if (m_id < 0 || m_id > 8)
				return;
			else
			{
				if (m_type == HardwareType.Xbox)
				{
#if UNINPUT_WIN
                    SetButtonDPad(6, 7);
#elif UNINPUT_ANDROID
					SetButtonDPad(5, 6);
#endif
				}
				else if (m_type == HardwareType.Playstation)
					SetButtonDPad(7, 8);
			}
		}

		void SetButtonDPad(int Horizontal, int Vertical)
		{
			float Dpad;
			string axis_name;
			//Tratar Axis DPad Horizontal no XBOX
			axis_name = "UNInput " + m_id + " " + axis_names[Horizontal];
			Dpad = Input.GetAxis(axis_name) * axisXbox[Horizontal - 1];

			if (Dpad > 0)
			{
				m_VirtualButtons["DPR"].Pressed();
				m_VirtualButtons["DPL"].Released();
			}
			else if (Dpad < 0)
			{
				m_VirtualButtons["DPL"].Pressed();
				m_VirtualButtons["DPR"].Released();
			}
			else
			{
				m_VirtualButtons["DPR"].Released();
				m_VirtualButtons["DPL"].Released();
			}

			//Tratar Axis DPad Vertical no XBOX
			axis_name = "UNInput " + m_id + " " + axis_names[Vertical];
			Dpad = Input.GetAxis(axis_name) * axisXbox[Vertical - 1];

			if (Dpad > 0)
			{
				m_VirtualButtons["DPU"].Pressed();
				m_VirtualButtons["DPD"].Released();
			}
			else if (Dpad < 0)
			{
				m_VirtualButtons["DPD"].Pressed();
				m_VirtualButtons["DPU"].Released();
			}
			else
			{
				m_VirtualButtons["DPU"].Released();
				m_VirtualButtons["DPD"].Released();
			}
		}


		public override void SetButtonDown(string name)
		{
			throw new Exception(" This is not possible to be called for standalone input. Please check your platform and code where this is called");
		}


		public override void SetButtonUp(string name)
		{
			throw new Exception(" This is not possible to be called for standalone input. Please check your platform and code where this is called");
		}


		public override void SetAxisPositive(string name)
		{
			throw new Exception(" This is not possible to be called for standalone input. Please check your platform and code where this is called");
		}


		public override void SetAxisNegative(string name)
		{
			throw new Exception(" This is not possible to be called for standalone input. Please check your platform and code where this is called");
		}


		public override void SetAxisZero(string name)
		{
			throw new Exception(" This is not possible to be called for standalone input. Please check your platform and code where this is called");
		}


		public override void SetAxis(string name, float value)
		{
			throw new Exception(" This is not possible to be called for standalone input. Please check your platform and code where this is called");
		}


		public override Vector3 MousePosition()
		{
			return new Vector2(GetAxis("Right Stick Horizontal", false), GetAxis("Right Stick Vertical", false));
		}

		public void AxisforButton()
		{

		}
	}
}