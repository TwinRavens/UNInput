//System Includes
using System;
using System.Collections.Generic;

//Unity Includes
using UnityEngine;

//NUNet Includes
using NUNet;

//Custom Includes
using UniversalNetworkInput.Network.Internal;

namespace UniversalNetworkInput.Network
{

    /// <summary>
    /// Enum flag to define the type of message that
    /// has been trasmitted trough the Network
    /// </summary>
    public enum UNNMessageFlag
    {
        /// <summary>
        /// State update received
        /// </summary>
        Update,
        /// <summary>
        /// Registration string
        /// </summary>
        Registration,
        /// <summary>
        /// Asset Bundle received
        /// </summary>
        AssetBundle,
        /// <summary>
        /// Last flag defines protocol size
        /// </summary>
        UNNProtocolSize
    }

    /// <summary>
    /// Structure that hold Network Control information,
    /// as registered in UNInput's Device List...
    /// </summary>
    public struct NetworkControlInfo
    {
        /// <summary>
        /// Control name registered
        /// </summary>
        public string name;

        /// <summary>
        /// Index in UNInput's list
        /// </summary>
        public int id;
    }

    /// <summary>
    /// Static class that handles virtual network controls'
    /// connections and updates their information besed on
    /// the update packets.
    /// </summary>
    public static class UNServer
    {
        #region Private Parameters
        private static int m_max_connections; //Maximum number of avalible control slots
        private static Dictionary<Guid, NetworkControlInfo> m_clients; //List of Connected Clients
        private static string m_control_prefix = "Network Control "; //Prefix name of Network Control's IDs
        private static UNServerPreferences m_prefs; //Server preferences loaded from external asset
        private static Packet[] bundlePackets;
        #endregion

        #region Public Variables, Gets and Sets
        /// <summary>
        /// Current Server's Host Port
        /// </summary>
        public static ushort host_port
        {
            get { return NUServer.port; }
        }

        /// <summary>
        /// Current Server's Host IP Address
        /// </summary>
        public static string ip_address
        {
            get { return NUServer.ipAddress.ToString(); }
        }

        /// <summary>
        /// Current Number of Max Concurrent Connections
        /// </summary>
        public static int max_connections
        {
            get { return m_max_connections; }
        }

        /// <summary>
        /// Prefix that goes on front of every connected control
        /// </summary>
        public static string control_prefix
        {
            get { return m_control_prefix; }
            set
            {
                if (value.Length > 0)
                {
                    if (value[value.Length - 1] != ' ')
                    {
                        Debug.LogError("Control prefix must end with a space!");
                        return;
                    }
                    m_control_prefix = value;
                    if (m_clients == null) return;
                    foreach (KeyValuePair<Guid, NetworkControlInfo> kv in m_clients)
                    {
                        VirtualInput input;
                        UNInput.GetInputReference(kv.Value.id, out input);
                        string[] parts = input.name.Split(new char[] { ' ' });
                        input.name = value + parts[parts.Length - 1];
                    }
                }
            }
        }

        /// <summary>
        /// Server Preferences loaded from external assets
        /// </summary>
        public static UNServerPreferences preferences
        {
            get { return m_prefs; }
        }

        /// <summary>
        /// Flag that tells weather server has been
        /// intialized and is running...
        /// </summary>
        public static bool initialized
        {
            get { return NUServer.started; }
        }
        #endregion

        /// <summary>
        /// Static constructor initializes Unity's Low Level API
        /// </summary>
        static UNServer()
        {
            //Load Server preferences
            m_prefs = Resources.Load<UNServerPreferences>("UNServerPrefs");

            //Log errors
            if (m_prefs == null || m_prefs.Inputs.Count == 0)
            {
                Debug.LogError("No Input registered in UNInput preferences! Go to Window->UNInput and register something!");
            }
        }

        /// <summary>
        /// Start Server with max number of connections on given port,
        /// ip_address is given only if you wanna specify the network
        /// adapter to be used for hosting this server.
        /// </summary>
        /// <param name="max_connections"></param>
        /// <param name="port"></param>
        /// <param name="ip_address"></param>
        /// <returns></returns>
        public static int Start(int max_connections = 4, ushort port = 25565, string ip_address = "")
        {
            //Return if already initialized
            if (NUServer.started)
            {
                Debug.LogWarning("Trying to initialize already initialized server!");
                return 0;
            }

            //Hold Variables
            m_max_connections = max_connections;
            string m_ip_address;
            if (ip_address.Length > 0)
                m_ip_address = ip_address;
            else
                m_ip_address = NUUtilities.ListIPv4Addresses()[0].ToString();

            //Initialize clients
            m_clients = new Dictionary<Guid, NetworkControlInfo>(max_connections);

            //Setup callbacks
            NUServer.onClientConnected += SomeoneConnected;
            NUServer.onClientReconnected += SomeoneConnected;
            NUServer.onClientDisconnected += SomeoneDisconnected;
            NUServer.onClientTimedOut += SomeoneTimedOut;
            NUServer.onClientPacketReceived += ReceivedPacket;

            //Start Server
            NUServer.Start(ip_address, port);

            //Loading asset bundle and creating bundle network packet
            TextAsset bundleAsText = Resources.Load<TextAsset>("Bundles/uninputbundle");

            //Handling no Bundle error
            if (bundleAsText == null)
            {
                Debug.LogError("Bundle Could not be loaded! Please rebuild it!");
                return -1;
            }


            int bundleSize = bundleAsText.bytes.Length;
            string assetName = m_prefs.AssetName;
            byte[] assetNameData = System.Text.Encoding.ASCII.GetBytes(assetName);
            int assetNameSize = assetNameData.Length;
            byte[] bundleDataWithFlag = new byte[bundleSize + assetNameSize + 4 + 4];
            bundleAsText.bytes.CopyTo(bundleDataWithFlag, 0);                                               //Append Asset Bundle
            assetNameData.CopyTo(bundleDataWithFlag, bundleSize); bundleSize += assetNameSize;              //Append Asset Name
            BitConverter.GetBytes(assetNameSize).CopyTo(bundleDataWithFlag, bundleSize); bundleSize += 4;   //Append Name Size
            BitConverter.GetBytes((int)UNNMessageFlag.AssetBundle).CopyTo(bundleDataWithFlag, bundleSize);  //Append Message Flag

            //Get Boundle Packets for late resending by just changing destination GUIDs
            Packet boundlePacket = new Packet(bundleDataWithFlag, null, Packet.TypeFlag.DATA);
            bundlePackets = NUUtilities.SplitPacket(boundlePacket);

            //Return safe code
            return 0;
        }

        private static void SomeoneConnected(Guid guid)
        {
            if (m_clients.Count >= m_max_connections)
            {
                NUServer.SendReliable(new Packet("Room is Full!", new Guid[] { guid }, Packet.TypeFlag.DCONNECT));
                return;
            }

            //Log Connection Event
            Debug.Log("Control Connected! ID: " + guid);

            //A control has been connected! Create it's Virtual Input instance
            //and setup it's buttons and axis according to the default Layout (WIP)
            string input_name = control_prefix + guid.ToString();
            NetworkInput network_control;

            //Device Input ID
            int input_id = -1;

            //If it already exists, control reconnected!
            if (UNInput.GetInputReference(input_name, out network_control))
            {
                input_id = UNInput.GetInputIndex(input_name);
                Debug.Log("Control " + input_name + " connected!");
                network_control.connected = true;
            }
            else
            {
                //Instantiate a new Network control!
                network_control = new NetworkInput(input_name, true);

                //Then create every Input
                for (int i = 0; i < preferences.Inputs.Count; i++)
                {
                    UNServerPreferences.Input input = preferences.Inputs[i];
                    switch (input.type)
                    {
                        case UNServerPreferences.Input.InputType.Axis:
                            UNInput.VirtualAxis axis_input = new UNInput.VirtualAxis(input.name);
                            network_control.RegisterVirtualAxis(axis_input);
                            if (input.axis_code > 0)
                                network_control.RegisterAxisCode(input.name, input.axis_code);
                            break;
                        case UNServerPreferences.Input.InputType.Button:
                            UNInput.VirtualButton button_input = new UNInput.VirtualButton(input.name);
                            network_control.RegisterVirtualButton(button_input);
                            if (input.button_code > 0)
                                network_control.RegisterButtonCode(input.name, input.button_code);
                            break;
                        default:
                            Debug.LogError("Unknown Input Type! Type: " + input.type.ToString());
                            break;
                    }
                }

                //Register the control and hold it's index in the list...
                input_id = UNInput.RegisterVirtualInput(network_control);

                //If got an error registering, skip this message
                if (input_id < 0)
                    return;
            }

            //Then add it to the list of connected devices...
            m_clients.Add(guid, new NetworkControlInfo { id = input_id, name = input_name });

            //Send registration package string to client
            Packet registration_package;
            int packet_size;
            if ((packet_size = network_control.GetRegistrationPackage(guid, out registration_package)) > 0)
            {
                try { NUServer.SendReliable(registration_package); }
                catch (Exception ex) { Debug.LogError(ex.ToString()); }
            }

            //Send each of the bundle packets
            for (int i = 0; i < bundlePackets.Length; i++)
            {
                //Change Destination ID
                bundlePackets[i].OverrideDestination(new Guid[] { guid });
                try { NUServer.SendReliable(bundlePackets[i]); }
                catch (Exception ex) { Debug.LogError(ex.ToString()); }
            }
        }

        private static void ReceivedPacket(Guid guid, Packet packet)
        {
            //It appears we have some data!
            //Let us take this data and update our control information!
            NetworkInput n_input = null;

            //Check weather or not the control exists
            if(!m_clients.ContainsKey(guid))
            {
                Debug.LogError("Received packet from Unregistered client: " + 
                    guid + packet.ToString());
                return;
            }

            if (!UNInput.GetInputReference(m_clients[guid].name, out n_input))
            {
                Debug.LogError("No Controler registered with ID " + guid.ToString() + "!");
                return;
            }

            //Clear received buffer of trash
            byte[] rec_buffer = packet.GetCleanData();

            //Get message flag
            UNNMessageFlag flag = (UNNMessageFlag)BitConverter.ToInt32(rec_buffer, rec_buffer.Length - 4);

            //Resize buffer to remove flag
            Array.Resize(ref rec_buffer, rec_buffer.Length - 4);

            //For each message type
            switch (flag)
            {
                case UNNMessageFlag.Update:
                    //Push changes
                    n_input.PushChangesBuffer(rec_buffer);
                    break;
                default:
                    Debug.LogError("Message type cannot be processed! Type of: " + flag.ToString());
                    break;
            }
        }

        private static void SomeoneDisconnected(Guid guid)
        {
            if (!m_clients.ContainsKey(guid))
                return;

            Debug.Log("Disconnected device: " + m_clients[guid].name);
            VirtualInput vi;
            if (UNInput.GetInputReference(m_clients[guid].id, out vi))
            {
                vi.connected = false;
            }
            m_clients.Remove(guid);
        }

        private static void SomeoneTimedOut(Guid guid)
        {
            //Get network device and check if was connected, otherwise set timeout flag
            VirtualInput vi;
            if (UNInput.GetInputReference(m_clients[guid].id, out vi))
            {
                if (vi.connected)
                {
                    ((NetworkInput)vi).timed_out = true;
                    Debug.Log("Control " + vi.name + " Timed Out!");
                }
            }
            m_clients.Remove(guid);
        }

        /// <summary>
        /// Shutdown Network Transport Layer and close connection with all clients
        /// </summary>
        public static void Shutdown()
        {
            NUServer.Shutdown();
            m_clients.Clear();
        }
    }
}
