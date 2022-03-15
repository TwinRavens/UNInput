namespace UniversalNetworkInput.Internal
{
    /// <summary>
    /// Internal Representation of Input Manager's axis Type!
    /// </summary>
    public enum AxisType
    {
        KeyOrMouseButton = 0,
        MouseMovement = 1,
        JoystickAxis = 2
    };

    /// <summary>
    /// Internal Representation of Input Manager's InputAxis
    /// </summary>
    public class InputAxis
    {
        public string name;
        public string descriptiveName;
        public string descriptiveNegativeName;
        public string negativeButton;
        public string positiveButton;
        public string altNegativeButton;
        public string altPositiveButton;

        public float gravity;
        public float dead;
        public float sensitivity;

        public bool snap = false;
        public bool invert = false;

        public AxisType type;

        public int axis;
        public int joyNum;

        public InputAxis(string name, string descriptiveName, string descriptiveNegativeName, string negativeButton, string positiveButton, string altNegativeButton, string altPositiveButton,
            float gravity, float dead, float sensitivity,
            bool snap, bool invert,
            AxisType type,
            int axis,
            int joyNum)
        {
            this.name = name;
            this.descriptiveName = descriptiveName;
            this.descriptiveNegativeName = descriptiveNegativeName;
            this.negativeButton = negativeButton;
            this.positiveButton = positiveButton;
            this.altNegativeButton = altNegativeButton;
            this.altPositiveButton = altPositiveButton;
            this.gravity = gravity;
            this.dead = dead;
            this.sensitivity = sensitivity;
            this.snap = snap;
            this.invert = invert;
            this.type = type;
            this.axis = axis;
            this.joyNum = joyNum;
        }
    }
}