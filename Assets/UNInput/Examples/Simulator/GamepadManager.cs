//Using the Unity 5 features
using UnityEngine;

//Using the Unity 5 GUI
using UnityEngine.UI;

//Using the UNINPUT features
using UniversalNetworkInput;

//Using the UNINPUT Hardware
using UniversalNetworkInput.Hardware;

//Using the UNINPUT network
using UniversalNetworkInput.Network;

//Using the UNINPUT internal network
using UniversalNetworkInput.Network.Internal;

//Gamepads prefab management script.
class GamepadManager : MonoBehaviour
{
    //Gamepad types
    public enum GamepadType
    {
        AndroidGamepad,
        XBoxGamepad,
        PlayStationGamepad
    }

    [System.NonSerialized] public GamepadType gamepadType = GamepadType.AndroidGamepad;
    private GameObject[] gamepad = new GameObject[3];
    private Dropdown drop;
    private Text ipServer;
    private int i = 0;

    [System.Serializable]
    class Mobile
    {
        public Color start, leftStick, a, b; //Mobile Colors
        public void Init(Transform transform, int gamepad)
        {

        }
    }

        [System.Serializable]
    class Gamepad
    {
        public Color select, start, ex, circle, triangle, quad, l1, r1, l2, r2, l3, r3, dpadUp, dpadDown, dpadLeft, dpadRight; //Gamepad Colors
        [System.NonSerialized] public Image _select, _start, _ex, _triangle, _circle, _quad, _dpadLeft, _dpadRight, _dpadUp, _dpadDown, _l1, _r1, _l2, _r2, _l3, _r3;
        [System.NonSerialized] public GameObject l3GO, r3GO, l2GO, r2GO;
        [System.NonSerialized] public RectTransform l3Stick, r3Stick;
        [System.NonSerialized] public Text l3XText, l3YText, r3XText, r3YText, l2Text, r2Text;
        [System.NonSerialized] public float l3X, l3Y, r3X, r3Y, l2_, r2_;
        public void Init(Transform transform, int gamepad)
        {
            //GameObjects
            l2GO = transform.GetChild(gamepad).GetChild(0).gameObject;
            r2GO = transform.GetChild(gamepad).GetChild(1).gameObject;
            l3GO = transform.GetChild(gamepad).GetChild(5).gameObject;
            r3GO = transform.GetChild(gamepad).GetChild(6).gameObject;

            //RectTransform

            l3Stick = l3GO.transform.GetChild(0).GetComponent<RectTransform>();
            r3Stick = r3GO.transform.GetChild(0).GetComponent<RectTransform>();

            //Images
            _l1 = transform.GetChild(gamepad).GetChild(3).GetComponent<Image>(); // L1
            _r1 = transform.GetChild(gamepad).GetChild(4).GetComponent<Image>(); // R1
            _l2 = l2GO.GetComponent<Image>(); // L2
           _r2 = r2GO.GetComponent<Image>(); // R2
            _l3 = l3Stick.GetComponent<Image>(); // L3
            _r3 = r3Stick.GetComponent<Image>(); // R3
            _select = transform.GetChild(gamepad).GetChild(7).GetComponent<Image>(); // Select
            _start = transform.GetChild(gamepad).GetChild(8).GetComponent<Image>(); // Start
            _ex = transform.GetChild(gamepad).GetChild(9).GetComponent<Image>(); // EX
            _circle = transform.GetChild(gamepad).GetChild(10).GetComponent<Image>(); // Circle
            _quad = transform.GetChild(gamepad).GetChild(11).GetComponent<Image>(); // Quad
            _triangle = transform.GetChild(gamepad).GetChild(12).GetComponent<Image>(); //Triangle
            _dpadLeft = transform.GetChild(gamepad).GetChild(13).GetChild(0).GetComponent<Image>(); //Dpad Left
            _dpadRight = transform.GetChild(gamepad).GetChild(13).GetChild(1).GetComponent<Image>(); //Dpad Right
            _dpadUp = transform.GetChild(gamepad).GetChild(13).GetChild(2).GetComponent<Image>(); //Dpad Up
            _dpadDown = transform.GetChild(gamepad).GetChild(13).GetChild(3).GetComponent<Image>(); //Dpad Down
                                      
            //Text
            l2Text = l2GO.transform.GetChild(0).GetComponent<Text>(); // L2
            r2Text = r2GO.transform.GetChild(0).GetComponent<Text>(); // R2
            l3XText = l3GO.transform.GetChild(1).GetComponent<Text>(); // L3 X
            l3YText = l3GO.transform.GetChild(2).GetComponent<Text>(); // L3 Y
            r3XText = r3GO.transform.GetChild(1).GetComponent<Text>(); // R3 X
            r3YText = r3GO.transform.GetChild(2).GetComponent<Text>(); // R3 Y
        }
    }

    [Header("Buttons action colors")]
    [SerializeField]
    private Mobile android;
    [SerializeField]
    private Gamepad xbox;
    [SerializeField]
    private Gamepad playstation;

    // Use this for initialization
    private void Start()
    {
        //Initialize Gamepads
        xbox.Init(transform, 1);
        playstation.Init(transform, 2);
        //Set Gamepads
        for (i = 0; i < gamepad.Length; i++)
            gamepad[i] = transform.GetChild(i).gameObject;
        //Set Dropdown
        drop = transform.GetChild(3).gameObject.GetComponent<Dropdown>();
        drop.gameObject.SetActive(true);
        SetGamepad();

        //Initialize Server
        //UNServer.Start(4, 25565, UNNetwork.GetLocalIPAddress());

        //Set ip Text
        //ipServer = transform.GetChild(4).GetComponent<Text>();
        //ipServer.text = "IP: " + UNServer.ip_address;
    }

    // Update is called once per frame
    private void Update()
    {
        for (i = 0; i < 8; i++)
        {
            VirtualInput vi;
            int id = UNInput.GetInputIndex("Hardware Joystick " + i.ToString());
            if (UNInput.GetInputReference(id, out vi))
            {
                if (vi.connected)
                {
                    if (gamepadType == GamepadType.AndroidGamepad)
                    {
                        /*if (((NetworkInput)vi)) //Checar se é Mobile
                        {
                            Axes(id);
                            Buttons(id);
                        }*/
                    }
                    else if (gamepadType == GamepadType.XBoxGamepad)
                    {
                        if (((HardwareInput)vi).type == HardwareInput.HardwareType.Xbox)
                        {
                            Axes(id, xbox);
                            Buttons(id, xbox);
                        }
                    }
                    else if (gamepadType == GamepadType.PlayStationGamepad)
                    {
                        if (((HardwareInput)vi).type == HardwareInput.HardwareType.Playstation)
                        {
                            Axes(id, playstation);
                            Buttons(id, playstation);
                        }
                    }
                }
            }
        }
    }

    void Buttons(int id, Gamepad game)
    {
        //Back
        if (UNInput.GetButton(id, ButtonCode.Back))
            game._select.color = game.select;
        else
            game._select.color = Color.white;
        //Start
        if (UNInput.GetButton(id, ButtonCode.Start))
            game._start.color = game.start;
        else
            game._start.color = Color.white;
        //A
        if (UNInput.GetButton(id, ButtonCode.A))
            game._ex.color = game.ex;
        else
            game._ex.color = Color.white;
        //B
        if (UNInput.GetButton(id, ButtonCode.B))
            game._circle.color = game.circle;
        else
            game._circle.color = Color.white;
        //X
        if (UNInput.GetButton(id, ButtonCode.X))
            game._quad.color = game.quad;
        else
            game._quad.color = Color.white;
        //Y
        if (UNInput.GetButton(id, ButtonCode.Y))
            game._triangle.color = game.triangle;
        else
            game._triangle.color = Color.white;
        //Left Stick Click
        if (UNInput.GetButton(id, ButtonCode.LS))
            game._l3.color = game.l3;
        else
            game._l3.color = Color.white;
        //Right Stick Click
        if (UNInput.GetButton(id, ButtonCode.RS))
            game._r3.color = game.r3;
        else
            game._r3.color = Color.white;
        //Left Bumper
        if (UNInput.GetButton(id, ButtonCode.LB))
            game._l1.color = game.l1;
        else
            game._l1.color = Color.white;
        //Right Bumper
        if (UNInput.GetButton(id, ButtonCode.RB))
            game._r1.color = game.r1;
        else
            game._r1.color = Color.white;
        //DPad Right
        if (UNInput.GetButton(id, ButtonCode.DPadRight))
            game._dpadRight.color = game.dpadRight;
        else
            game._dpadRight.color = Color.white;
        //DPad Left
        if (UNInput.GetButton(id, ButtonCode.DPadLeft))
            game._dpadLeft.color = game.dpadLeft;
        else
            game._dpadLeft.color = Color.white;
        //DPad Up
        if (UNInput.GetButton(id, ButtonCode.DPadUp))
            game._dpadUp.color = game.dpadUp;
        else
            game._dpadUp.color = Color.white;
        //DPad Down
        if (UNInput.GetButton(id, ButtonCode.DPadDown))
            game._dpadDown.color = game.dpadDown;
        else
            game._dpadDown.color = Color.white;
    }

    void Axes(int id, Gamepad game)
    {
        //Values
        game.l3X = UNInput.GetAxis(id, AxisCode.LSH);
        game.l3Y = UNInput.GetAxis(id, AxisCode.LSV);
        game.r3X = UNInput.GetAxis(id, AxisCode.RSH);
        game.r3Y = UNInput.GetAxis(id, AxisCode.RSV);
        game.l2_ = UNInput.GetAxis(id, AxisCode.LT);
        game.r2_ = UNInput.GetAxis(id, AxisCode.RT);
        //Left Stick
        if (game.l3X != 0 || game.l3Y != 0)
            game.l3Stick.transform.localPosition = new Vector3(game.l3X * 25f, game.l3Y * 25f, 0f);
        else
            game.l3Stick.transform.localPosition = Vector3.zero;
        //Right Stick
        if (game.r3X != 0 || game.r3Y != 0)
            game.r3Stick.GetComponent<RectTransform>().localPosition = new Vector3(game.r3X * 25f, game.r3Y * 25f, 0f);
        else
            game.r3Stick.GetComponent<RectTransform>().localPosition = Vector3.zero;
        //Left Trigger
        if (game.l2_ != 0)
        {
            game.l2GO.transform.localPosition = new Vector3(game.l2GO.transform.localPosition.x, 220f + (-game.l2_ * 45f), 0f);
            game.l2GO.GetComponent<Image>().color = game.l2;
        }
        else
        {
            game.l2GO.transform.localPosition = new Vector3(game.l2GO.transform.localPosition.x, 220f, 0f);
            game.l2GO.GetComponent<Image>().color = Color.white;
        }
        //Right Trigger
        if (game.r2_ != 0)
        {
            game.r2GO.GetComponent<RectTransform>().localPosition = new Vector3(game.r2GO.transform.localPosition.x, 220f + (game.r2_ * -45f), 0f);
            game.r2GO.GetComponent<Image>().color = game.r2;
        }
        else
        {
            game.r2GO.GetComponent<RectTransform>().localPosition = new Vector3(game.r2GO.transform.localPosition.x, 220f, 0f);
            game.r2GO.GetComponent<Image>().color = Color.white;
        }
        //Texts
        game.l3XText.text = "H: " + game.l3X.ToString("F1");
        game.l3YText.text = "V: " + game.l3Y.ToString("F1");
        game.r3XText.text = "H: " + game.r3X.ToString("F1");
        game.r3YText.text = "V: " + game.r3Y.ToString("F1");
        game.l2Text.text = game.l2_.ToString("F1");
        game.r2Text.text = game.r2_.ToString("F1");
    }

    public void SetGamepad() //Using in Dropdown
    {
        if (drop.value == 0)
            gamepadType = GamepadType.AndroidGamepad;
        else if (drop.value == 1)
            gamepadType = GamepadType.XBoxGamepad;
        else if (drop.value == 2)
            gamepadType = GamepadType.PlayStationGamepad;
        for (i = 0; i < gamepad.Length; i++)
            if (drop.value == i)
                gamepad[i].SetActive(true);
            else
                gamepad[i].SetActive(false);
    }

}
