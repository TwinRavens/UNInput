//System Includes
using System;

//Unity Includes
using UnityEngine;

//NUNet Includes
using NUNet;

namespace UniversalNetworkInput.Network
{

    /// <summary>
    /// MonoBehaviour hook for processing data
    /// on Late Update Loop. This component is
    /// added to the UNInput Loop Object.
    /// </summary>
    public class ClientLoop : MonoBehaviour
    {
        private void LateUpdate()
        {
            //If network already started
            if (NUClient.connected)
            {
                //Process message poll for the server
                UNClient.SendInputChanges();
            }
        }
    }

    /// <summary>
    /// Static class that handles the virtual network control,
    /// it's connection, and sent update packages with latest
    /// Input values and buttons pressed.
    /// </summary>
    public static class UNClient
    {
        #region Private Parameters
        private static NetworkInput m_control;                      //The Control instance of this client
        private static string m_control_name = "Network Control";   //Name of Control from which data will be taken
        private static AssetBundle m_bundle;                        //Value that holds the bundle reference (only loaded once)
        #endregion

        #region Public Variables, Gets and Sets

        /// <summary>
        /// The name of the control from whose data will be synced with server
        /// </summary>
        public static string control_name
        {
            get { return m_control_name; }
            set
            {
                if (value.Length > 0)
                    m_control_name = value;
            }
        }

        /// <summary>
        /// Flag that determines wheather or not client is connected
        /// </summary>
        public static bool connected
        {
            get { return m_control.connected; }
        }

        /// <summary>
        /// Reference to the Virtual Control of this Network Client
        /// </summary>
        public static VirtualInput control
        {
            get { return m_control; }
        }

        /// <summary>
        /// The name of the AssetBundle prefab to be instantiated
        /// </summary>
        public static string prefab_name { get; private set; }

        /// <summary>
        /// Last server whose IP we tried to connect to
        /// </summary>
        public static string server_ip { get; private set; }

        /// <summary>
        /// The Instance of the instantiated AssetBundle prefab
        /// </summary>
        public static GameObject prefab_instance { get; private set; }

        /// <summary>
        /// Callback called on a Connection occurs, being it either a new
        /// connection or a reconnection from droped session
        /// </summary>
        public static Action onConnect;

        /// <summary>
        /// Callback called on a disconnection,
        /// either because of a Drop or because
        /// of a desired disconnection
        /// </summary>
        public static Action onDisconnect;

        /// <summary>
        /// Callback called whenever the connection droped
        /// </summary>
        public static Action onConnectionDroped;

        /// <summary>
        /// Callback called whenever we could not
        /// connect to some server
        /// </summary>
        public static Action onConnectionFailure;

        /// <summary>
        /// Callback called whenever a Bundle fragment is received.
        /// Parameter is the fraction of the download (range 0->1)!
        /// </summary>
        public static Action<float> onFragmentReceived;

        /// <summary>
        /// Callback called whenever the Bundle has been entirelly received
        /// </summary>
        public static Action onBundleDownloaded;
        #endregion

        /// <summary>
        /// Static constructor initializes Unity's Low Level API
        /// </summary>
        static UNClient()
        {
            //Instantiate control and register it
            m_control = new NetworkInput(control_name);
            UNInput.RegisterVirtualInput(m_control);

            //Not connected!
            m_control.connected = false;

            //Add loop component to UNInput Loop Object
            UNInput.instance.AddComponent<ClientLoop>();
        }

        /// <summary>
        /// Connects to Server at given IP on given port, ip_address
        /// is given only if you wanna specify the network adapter to
        /// be used for connecting to such server.
        /// </summary>
        /// <param name="server_ip"></param>
        /// <param name="port"></param>
        /// <param name="ip_address"></param>
        /// <returns></returns>
        public static int Connect(string server_ip, ushort port = 25565, string ip_address = "")
        {
            //Return if already initialized
            if (NUClient.connected)
            {
                Debug.LogWarning("Trying to connect already connected client!");
                return 0;
            }

            //Hold parameter values
            string m_ip_address;
            if (ip_address.Length > 0)
                m_ip_address = ip_address;
            else
                m_ip_address = NUUtilities.ListIPv4Addresses()[0].ToString();

            //Setup Callback
            NUClient.onConnected += Connected;
            NUClient.onDisconnected += Disconnected;
            NUClient.onPacketReceived += ReceivedPacket;

            //Connect to server
            try
            {
                NUClient.Connect(server_ip, port);
            }
            catch (Exception ex)
            {
                Debug.LogError(ex.ToString()); return -1;
            }

            //Hold server IP
            UNClient.server_ip = server_ip;

            return 0;

        }

        private static void Connected()
        {
            m_control.connected = true;

            //Check if control exists
            VirtualInput network_control;
            if (!UNInput.GetInputReference(control_name, out network_control))
            {
                Debug.LogError("Control is not registered!");
                return;
            }

            //Reset flag
            m_control.timed_out = true;

            //Call action
            if (onConnect != null)
                onConnect();
        }

        private static void ReceivedPacket(Packet packet)
        {
            byte[] rec_buffer = packet.GetCleanData();

            //Extract message flag
            UNNMessageFlag flag = (UNNMessageFlag)BitConverter.ToInt32(rec_buffer, rec_buffer.Length - 4);

            //Clear buffer from flag
            Array.Resize(ref rec_buffer, rec_buffer.Length - 4);

            //For each type of message, process it according
            switch (flag)
            {
                case UNNMessageFlag.Update:
                    {
                        Debug.Log("Received Updates message!");
                        m_control.PushChangesBuffer(rec_buffer);
                    }

                    break;

                case UNNMessageFlag.Registration:
                    {
                        Debug.Log("Received Registration message!");
                        m_control.PushChangesBuffer(rec_buffer);
                        m_control.PullReliableChangesBuffer();
                        m_control.PullUnreliableChangesBuffer();
                    }
                    break;

                case UNNMessageFlag.AssetBundle:
                    {
                        Debug.Log("Received AssetBundle!");

                        //Get Prefab name from buffer
                        int charCount = BitConverter.ToInt32(rec_buffer, rec_buffer.Length - 4);
                        byte[] prefab_name_data = new byte[charCount];
                        Array.Copy(rec_buffer, rec_buffer.Length - 4 - charCount, prefab_name_data, 0, charCount);
                        prefab_name = System.Text.Encoding.ASCII.GetString(prefab_name_data);

                        //Resize buffer to get bundle data
                        Array.Resize(ref rec_buffer, rec_buffer.Length - 4 - charCount);

                        //Instantiate AssetBundle and hold instance reference
                        m_bundle = AssetBundle.LoadFromMemory(rec_buffer);
                        UnityEngine.Object[] bundleObjects = m_bundle.LoadAssetWithSubAssets(prefab_name);
                        prefab_instance = (GameObject)UnityEngine.Object.Instantiate(bundleObjects[0]);

                        //Do proper callbacks
                        if (onBundleDownloaded != null)
                            onBundleDownloaded();
                    }
                    break;
            }
        }

        private static void Disconnected()
        {
            Debug.Log("Disconnected!");

            //Set connection flag
            m_control.connected = false;

            //Unload bundle
            if (m_bundle != null)
                m_bundle.Unload(false);

            //Handle Callbacks
            if (onDisconnect != null)
                onDisconnect();
        }

        /// <summary>
        /// Sends Changes Buffer from Default Controler
        /// (as defined by "control_name")...
        /// </summary>
        public static void SendInputChanges()
        {
            //If connected
            if (NUClient.connected)
            {
                //Check Input reference
                if (m_control == null)
                {
                    Debug.LogError("Input with name " + control_name + " is not registered! Cannot send changes!");
                    return;
                }

                //Get buffers and sent'em
                byte[] reliable_buffer = m_control.PullReliableChangesBuffer(); //Buttons events must go through a reliable sequenced channel
                byte[] unrealiable_buffer = m_control.PullUnreliableChangesBuffer(); //Realtime axis data data don't need to be sequenced nor reliable

                //Send and debug
                if (reliable_buffer.Length > 0)
                {
                    if (reliable_buffer.Length > 962)
                    {
                        Debug.LogWarning("Reliable Buffer too large! Ignoring this one...");
                        return;
                    }
                    NUClient.SendReliable(new Packet(reliable_buffer, null, Packet.TypeFlag.DATA));
                }

                //Send and debug
                if (unrealiable_buffer.Length > 0)
                {
                    if (unrealiable_buffer.Length > 962)
                    {
                        Debug.LogWarning("Unreliable Buffer too large! Ignoring this one...");
                        return;
                    }
                    NUClient.SendUnreliable(new Packet(unrealiable_buffer, null, Packet.TypeFlag.DATA));
                }
            }

        }

        /// <summary>
        /// Closes current connection with the server
        /// </summary>
        public static void Disconnect()
        {
            NUClient.Disconnect();
        }
    }
}