//System Includes
using NUNet;
using System;
using System.Collections.Generic;
using System.Globalization;

//Unity Includes
using UnityEngine;

//Custom Includes
using UniversalNetworkInput.Network.Internal;

namespace UniversalNetworkInput.Network
{
    /// <summary>
    /// Network Implementation of Virtual Input
    /// interface. Handles abstraction devices
    /// connected through the Network Layer
    /// </summary>
    public class NetworkInput : VirtualInput
    {
        /// <summary>
        /// Strings that hold every changes since last Pull
        /// </summary>
        private string reliable_changes = "";
        private string unreliable_changes = "";
        public bool timed_out = false;

        private List<KeyValuePair<AxisCode, string>> m_axis = new List<KeyValuePair<AxisCode, string>>
        {
            //new KeyValuePair<AxisCode, string>( AxisCode.DPH, "DPad Horizontal" ),
            //new KeyValuePair<AxisCode, string>( AxisCode.DPV, "DPad Vertical" ),
            //new KeyValuePair<AxisCode, string>( AxisCode.LSH, "Left Stick Horizontal" ),
            //new KeyValuePair<AxisCode, string>( AxisCode.LSH, "Horizontal" ),
            //new KeyValuePair<AxisCode, string>( AxisCode.LSV, "Left Stick Vertical" ),
            //new KeyValuePair<AxisCode, string>( AxisCode.LSV, "Vertical" ),
            //new KeyValuePair<AxisCode, string>( AxisCode.LT , "Left Trigger" ),
            //new KeyValuePair<AxisCode, string>( AxisCode.RSH, "Right Stick Horizontal" ),
            //new KeyValuePair<AxisCode, string>( AxisCode.RSV, "Right Stick Vertical" ),
            //new KeyValuePair<AxisCode, string>( AxisCode.RT , "Right Trigger" ),
            //new KeyValuePair<AxisCode, string>( AxisCode.T  , "Triggers" )
        };

        private List<KeyValuePair<ButtonCode, string>> m_buttons = new List<KeyValuePair<ButtonCode, string>>
        {
            //new KeyValuePair<ButtonCode, string>( ButtonCode.A    , "A" ),
            //new KeyValuePair<ButtonCode, string>( ButtonCode.A    , "Jump" ),
            //new KeyValuePair<ButtonCode, string>( ButtonCode.A    , "Submit" ),
            //new KeyValuePair<ButtonCode, string>( ButtonCode.A    , "Action" ),
            //new KeyValuePair<ButtonCode, string>( ButtonCode.B    , "B" ),
            //new KeyValuePair<ButtonCode, string>( ButtonCode.B    , "Back" ),
            //new KeyValuePair<ButtonCode, string>( ButtonCode.Back , "Select" ),
            //new KeyValuePair<ButtonCode, string>( ButtonCode.LB   , "Left Bumper" ),
            //new KeyValuePair<ButtonCode, string>( ButtonCode.LS   , "Left Stick" ),
            //new KeyValuePair<ButtonCode, string>( ButtonCode.RB   , "Right Bumper" ),
            //new KeyValuePair<ButtonCode, string>( ButtonCode.RS   , "Right Stick" ),
            //new KeyValuePair<ButtonCode, string>( ButtonCode.Start, "Start" ),
            //new KeyValuePair<ButtonCode, string>( ButtonCode.X    , "X" ),
            //new KeyValuePair<ButtonCode, string>( ButtonCode.Y    , "Y" )
        };

        public NetworkInput(string name) : base(name)
        {
            timed_out = false;
        }

        public NetworkInput(string name, bool connected) : base(name, connected)
        {
            timed_out = false;
        }

        private void AddButton(string name)
        {
            //We have not registered this button yet so add it, happens in the constructor
            RegisterVirtualButton(new UNInput.VirtualButton(name));
        }

        private void AddAxes(string name)
        {
            //We have not registered this button yet so add it, happens in the constructor
            RegisterVirtualAxis(new UNInput.VirtualAxis(name));
        }

        public bool RegisterAxisCode(string axis_name, AxisCode code)
        {
            for (int i = 0; i < m_axis.Count; i++)
            {
                //Check if an entry like that already exists
                if(m_axis[i].Key == code && m_axis[i].Value == axis_name)
                    return false;
            }
            m_axis.Add(new KeyValuePair<AxisCode, string>(code, axis_name));
            return true;
        }

        public bool RegisterButtonCode(string button_name, ButtonCode code)
        {
            for (int i = 0; i < m_buttons.Count; i++)
            {
                if (m_buttons[i].Key == code && m_buttons[i].Value == button_name)
                    return false;
            }
            m_buttons.Add(new KeyValuePair<ButtonCode, string>(code, button_name));
            return true;
        }

        public void UnregisterAxisCode(AxisCode code)
        {
            int i = 0;
            while(i < m_axis.Count)
            {
                if (m_axis[i].Key == code)
                {
                    m_axis.RemoveAt(i);
                    continue;
                }
                i++;
            }
        }

        public void UnregisterButtonCode(ButtonCode code)
        {
            int i = 0;
            while (i < m_buttons.Count)
            {
                if (m_buttons[i].Key == code)
                {
                    m_buttons.RemoveAt(i);
                    continue;
                }
                i++;
            }
        }

        public override float GetAxis(string name, bool raw)
        {
            if (!m_VirtualAxes.ContainsKey(name))
            {
                AddAxes(name);
            }
            return m_VirtualAxes[name].GetValue;
        }

        public override float GetAxis(AxisCode axis, bool raw)
        {
            float axis_value = 0;

            for (int i = 0; i < m_axis.Count; i++)
            {
                if(m_axis[i].Key == axis)
                {
                    axis_value = Mathf.Clamp(axis_value+GetAxis(m_axis[i].Value, raw), -1.0f, 1.0f);
                }
            }

            return axis_value;
        }

        public override bool GetButton(string name)
        {
            if (m_VirtualButtons.ContainsKey(name))
            {
                return m_VirtualButtons[name].GetButton;
            }

            AddButton(name);
            return m_VirtualButtons[name].GetButton;
        }

        public override bool GetButton(ButtonCode button)
        {
            bool pressed = false;

            for (int i = 0; i < m_buttons.Count; i++)
            {
                if (m_buttons[i].Key == button)
                {
                    pressed |= GetButton(m_buttons[i].Value);
                }
            }

            return pressed;
        }

        public override bool GetButtonDown(string name)
        {
            if (m_VirtualButtons.ContainsKey(name))
            {
                return m_VirtualButtons[name].GetButtonDown;
            }

            AddButton(name);
            return m_VirtualButtons[name].GetButtonDown;
        }

        public override bool GetButtonDown(ButtonCode button)
        {
            bool pressedDown = false;

            for (int i = 0; i < m_buttons.Count; i++)
            {
                if (m_buttons[i].Key == button)
                {
                    pressedDown |= GetButtonDown(m_buttons[i].Value);
                }
            }

            return pressedDown;
        }

        public override bool GetButtonUp(string name)
        {
            if (m_VirtualButtons.ContainsKey(name))
            {
                return m_VirtualButtons[name].GetButtonUp;
            }

            AddButton(name);
            return m_VirtualButtons[name].GetButtonUp;
        }

        public override bool GetButtonUp(ButtonCode button)
        {
            bool pressedUp = false;

            for (int i = 0; i < m_buttons.Count; i++)
            {
                if (m_buttons[i].Key == button)
                {
                    pressedUp |= GetButtonDown(m_buttons[i].Value);
                }
            }

            return pressedUp;
        }

        public override void SetButtonDown(string name)
        {
            if (!m_VirtualButtons.ContainsKey(name))
            {
                AddButton(name);
            }
            m_VirtualButtons[name].Pressed();
            reliable_changes += "btn" + "|" + name + "|" + "down;";
        }

        public override void SetButtonUp(string name)
        {
            if (!m_VirtualButtons.ContainsKey(name))
            {
                AddButton(name);
            }
            m_VirtualButtons[name].Released();
            reliable_changes += "btn" + "|" + name + "|" + "up;";
        }

        public void SetAxisReliably(string name, float value)
        {
            if (!m_VirtualAxes.ContainsKey(name))
            {
                AddAxes(name);
            }

            //Check if value has changed a significant amount
            if (m_VirtualAxes[name].GetValue.ToString("0.0000") != value.ToString("0.0000"))
            {
                //Register changes string
                reliable_changes += "axis" + "|" + name + "|" + value.ToString() + ";";
                m_VirtualAxes[name].Update(value);
            }
        }

        public override void SetAxisPositive(string name)
        {
            SetAxisReliably(name, 1.0f);
        }

        public override void SetAxisNegative(string name)
        {
            SetAxisReliably(name, -1.0f);
        }

        public override void SetAxisZero(string name)
        {
            SetAxisReliably(name, 0.0f);
        }

        public override void SetAxis(string name, float value)
        {
            if (!m_VirtualAxes.ContainsKey(name))
            {
                AddAxes(name);
            }

            //Check if value has changed a significant amount
            if (m_VirtualAxes[name].GetValue.ToString("0.0000") != value.ToString("0.0000"))
            {
                //Register changes string
                unreliable_changes += "axis" + "|" + name + "|" + value.ToString(CultureInfo.InvariantCulture.NumberFormat) + ";";
                m_VirtualAxes[name].Update(value);
            }
        }

        public override Vector3 MousePosition()
        {
            return virtualMousePosition;
        }

        /// <summary>
        /// Get Bytes from Changes Buffer String
        /// </summary>
        /// <returns></returns>
        public byte[] PullReliableChangesBuffer()
        {
            byte[] buffer = System.Text.Encoding.UTF8.GetBytes(reliable_changes);
            reliable_changes = "";
            int size = buffer.Length;
            if (size > 0)
            {
                Array.Resize(ref buffer, size + 4);
                BitConverter.GetBytes((int)UNNMessageFlag.Update).CopyTo(buffer, size);
            }
            return buffer;
        }

        /// <summary>
        /// Get Bytes from Changes Buffer String
        /// </summary>
        /// <returns></returns>
        public byte[] PullUnreliableChangesBuffer()
        {
            byte[] buffer = System.Text.Encoding.UTF8.GetBytes(unreliable_changes);
            unreliable_changes = "";
            int size = buffer.Length;
            if (size > 0)
            {
                Array.Resize(ref buffer, size + 4);
                BitConverter.GetBytes((int)UNNMessageFlag.Update).CopyTo(buffer, size);
            }
            return buffer;
        }

        /// <summary>
        /// Apply changes from parsed Changes Buffer String
        /// </summary>
        /// <param name="buffer"></param>
        public void PushChangesBuffer(byte[] buffer)
        {
            string buffer_str = System.Text.Encoding.UTF8.GetString(buffer);
            //Debug.Log(buffer_str);
            string[] changes = buffer_str.Split(new char[] { ';' });
            for (int i = 0; i < changes.Length - 1; i++)
            {
                string[] parts = changes[i].Split(new char[] { '|' });
                if (parts.Length < 3)
                {
                    Debug.LogError("Change " + i.ToString() + " is invalid (" + parts.Length.ToString() + " parts): " + changes[i]);
                    continue;
                }
                string type = parts[0];
                string name = parts[1];
                string change = parts[2];
                if (type == "btn")
                {
                    if (change == "down")
                    {
                        SetButtonDown(name);
                    }
                    else if (change == "up")
                    {
                        SetButtonUp(name);
                    }

                    if (parts.Length > 3)
                    {
                        for (int j = 4; j < parts.Length; j++)
                        {
                            object axis_obj = Enum.Parse(typeof(ButtonCode), parts[j], true);
                            if (axis_obj != null)
                                m_buttons.Add(new KeyValuePair<ButtonCode, string>((ButtonCode)axis_obj, name));
                        }
                    }
                }
                else if (type == "axis")
                {
                    SetAxis(name, float.Parse(change, CultureInfo.InvariantCulture.NumberFormat));

                    if (parts.Length > 3)
                    {
                        for (int j = 4; j < parts.Length; j++)
                        {
                            object axis_obj = Enum.Parse(typeof(AxisCode), parts[j], true);
                            if (axis_obj != null)
                                m_axis.Add(new KeyValuePair<AxisCode, string>((AxisCode)axis_obj, name));
                        }
                    }
                }
                
            }
        }

        /// <summary>
        /// Returns the size of the packet generated and outputs the
        /// array of bytes from the registration packet, if it has an
        /// key code assigned also append it.
        /// </summary>
        /// <returns></returns>
        public int GetRegistrationPackage(Guid guid, out Packet packet)
        {
            string registration_string = "";
            foreach (UNInput.VirtualAxis v in m_VirtualAxes.Values)
            {
                registration_string += "axis|" + v.name + "|0";
                for (int i = 0; i < m_axis.Count; i++)
                {
                    if (m_axis[i].Value == v.name)
                        registration_string += "|" + m_axis[i].Key.ToString();
                }
                registration_string += ";";
            }
            foreach (UNInput.VirtualButton b in m_VirtualButtons.Values)
            {
                registration_string += "btn|" + b.name + "|up";
                for (int i = 0; i < m_buttons.Count; i++)
                {
                    if (m_buttons[i].Value == b.name)
                        registration_string += "|" + m_buttons[i].Key.ToString();
                }
                registration_string += ";";
            }
            byte[] str_bytes = System.Text.Encoding.UTF8.GetBytes(registration_string);

            //Debug.Log(registration_string);

            //Resize to fit flag
            int size = str_bytes.Length;
            Array.Resize(ref str_bytes, size + 4);

            //Set message flag header
            BitConverter.GetBytes((int)UNNMessageFlag.Registration).CopyTo(str_bytes, size);

            //Retur message length
            packet = new Packet(str_bytes, new Guid[] { guid }, Packet.TypeFlag.DATA);
            return str_bytes.Length;
        }
    }

}
