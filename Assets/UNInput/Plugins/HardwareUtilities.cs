//Unity Includes
using UnityEngine;

namespace UniversalNetworkInput.Hardware
{
    /// <summary>
    /// Mono Behaviour hook to Check connected
    /// Joysticks on Unity's Update Loop
    /// </summary>
    public class JoystickChecker : MonoBehaviour
    {
        public int[] controlls { get; private set; }

        JoystickChecker()
        {
            controlls = new int[8];
            for (int i = 0; i < 8; i++)
            {
                controlls[i] = -1;
            }
        }

        private void LateUpdate()
        {
            //Get devices reference
            string[] devices = Input.GetJoystickNames();
            //Go through every device registered
            for (int i = 0; i < devices.Length; i++)
            {
				//Get Connected flag
				bool connected = (devices[i] != string.Empty);

                HardwareInput h_input;
                if (UNInput.GetInputReference("Hardware Joystick " + (i + 1).ToString(), out h_input)) //If it exists, update connected flag
                {
                    h_input.connected = connected;
                }
                else //Or, register the device on UNInput's list if connected
                {
                    if (connected)
                    {
						if (devices[i].ToLower().Contains("xbox") || devices[i].ToLower().Contains("xinput"))
                            h_input = new HardwareInput("Hardware Joystick " + (i + 1).ToString(), (i + 1), true, HardwareInput.HardwareType.Xbox);
                        else if (devices[i].ToLower().Contains("wire"))
                            h_input = new HardwareInput("Hardware Joystick " + (i + 1).ToString(), (i + 1), true, HardwareInput.HardwareType.Playstation);
                        else
                            h_input = new HardwareInput("Hardware Joystick " + (i + 1).ToString(), (i + 1), true);

                        controlls[i] = UNInput.RegisterVirtualInput(h_input);
                    }
                }
                #if UNINPUT_WIN || UNINPUT_ANDROID
				if(h_input != null)
				{
					h_input.UpdateDirectionalPads();
				}
                #endif
            }
        }
    }
}

