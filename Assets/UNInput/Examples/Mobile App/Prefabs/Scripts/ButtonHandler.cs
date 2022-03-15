using UnityEngine;

namespace UniversalNetworkInput
{
    public class ButtonHandler : MonoBehaviour
    {

        public string Name;
        int input_id;

        void Start()
        {
            input_id = UNInput.GetInputIndex("Network Control");                
        }

        public void SetDownState()
        {
            UNInput.SetButtonDown(input_id, Name);
        }


        public void SetUpState()
        {
            UNInput.SetButtonUp(input_id, Name);
        }


        public void SetAxisPositiveState()
        {
            UNInput.SetAxisPositive(input_id, Name);
        }


        public void SetAxisNeutralState()
        {
            UNInput.SetAxisZero(input_id, Name);
        }


        public void SetAxisNegativeState()
        {
            UNInput.SetAxisNegative(input_id, Name);
        }

        public void Update()
        {

        }
    }
}
