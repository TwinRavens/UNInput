using UnityEngine;
using UnityEngine.UI;
using UniversalNetworkInput;
using UniversalNetworkInput.Network;
using UniversalNetworkInput.Network.Internal;

public class UNApp : MonoBehaviour
{
    public InputField m_field;
    public Slider downloadBar;
    public Text downloadLabel;
    public GameObject panel, disconnectionPanel, options;
    VirtualInput m_input;
    UNInput.VirtualAxis[] m_axis;
    UNInput.VirtualButton m_button;

    // Use this for initialization
    void Awake()
    {
        UNClient.control_name = "Network Control";
        UNClient.onConnect += OnConnected;
        UNClient.onDisconnect += OnDisconnect;
        UNClient.onFragmentReceived += OnFragmentReceived;
        UNClient.onConnectionDroped += OnConnectionDropped;
        UNClient.onBundleDownloaded += OnBundleDownloaded;

        if (PlayerPrefs.HasKey("lastIP"))
        {
            m_field.text = PlayerPrefs.GetString("lastIP");
        }
    }

    public void OnConnected()
    {
        panel.SetActive(false);
        downloadBar.gameObject.SetActive(true);
    }

    public void OnDisconnect()
    {
        if (UNClient.prefab_instance != null)
            Destroy(UNClient.prefab_instance);
        panel.SetActive(true);
        disconnectionPanel.SetActive(true);
        downloadBar.gameObject.SetActive(false);
        options.SetActive(false);
    }

    public void OnConnectionDropped()
    {
        if (UNClient.prefab_instance != null)
            Destroy(UNClient.prefab_instance);
        panel.SetActive(true);
        disconnectionPanel.SetActive(true);
        downloadBar.gameObject.SetActive(false);
    }

    public void OnFragmentReceived(float percentage)
    {
        downloadLabel.text = "Download Interface (" + (percentage * 100.0f).ToString("n2") + "%)";
        downloadBar.value = percentage;
    }

    public void OnBundleDownloaded()
    {
        downloadBar.gameObject.SetActive(false);
        options.SetActive(true);
    }

    public void OnConnectButton()
    {
        if (!UNClient.connected)
        {
            PlayerPrefs.SetString("lastIP", m_field.text);
            UNClient.Connect(m_field.text);
        }
    }

    public void OnDisconnectionContinueButton()
    {
        disconnectionPanel.SetActive(false);
    }

    public void OnDisconnectButton()
    {
        UNClient.Disconnect();
    }
}
