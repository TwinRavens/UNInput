//Unity Includes
using UnityEditor;

namespace UniversalNetworkInput.Internal
{
    public static class SetupInputManager
    {
        static SerializedObject serializedObject;
        static SerializedProperty axesProperty;
        static bool activeUNInput;

        static SetupInputManager()
        {
            serializedObject = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/InputManager.asset")[0]);
            axesProperty = serializedObject.FindProperty("m_Axes");
            SerializedProperty axisProperty;
            for (int i = 0; i < axesProperty.arraySize; i++)
            {
                axisProperty = axesProperty.GetArrayElementAtIndex(i);
                if (GetChildProperty(axisProperty, "m_Name").stringValue.Contains("UNInput"))
                {
                    activeUNInput = true;
                    break;
                }
                else
                    activeUNInput = false;
            }
        }

        private static string[] axisNames =
            {
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

        private static SerializedProperty GetChildProperty(SerializedProperty parent, string name)
        {
            SerializedProperty child = parent.Copy();
            child.Next(true);
            do
            {
                if (child.name == name) return child;
            }
            while (child.Next(false));
            return null;
        }

        private static bool AxisDefined(string axisName)
        {


            axesProperty.Next(true);
            axesProperty.Next(true);
            while (axesProperty.Next(false))
            {
                SerializedProperty axis = axesProperty.Copy();
                axis.Next(true);
                if (axis.stringValue == axisName) return true;
            }
            return false;
        }

        public static void AddAxis(InputAxis axis)
        {
            axesProperty.arraySize++;
            serializedObject.ApplyModifiedProperties();

            SerializedProperty axisProperty = axesProperty.GetArrayElementAtIndex(axesProperty.arraySize - 1);

            GetChildProperty(axisProperty, "m_Name").stringValue = axis.name;
            GetChildProperty(axisProperty, "descriptiveName").stringValue = axis.descriptiveName;
            GetChildProperty(axisProperty, "descriptiveNegativeName").stringValue = axis.descriptiveNegativeName;
            GetChildProperty(axisProperty, "negativeButton").stringValue = axis.negativeButton;
            GetChildProperty(axisProperty, "positiveButton").stringValue = axis.positiveButton;
            GetChildProperty(axisProperty, "altNegativeButton").stringValue = axis.altNegativeButton;
            GetChildProperty(axisProperty, "altPositiveButton").stringValue = axis.altPositiveButton;
            GetChildProperty(axisProperty, "gravity").floatValue = axis.gravity;
            GetChildProperty(axisProperty, "dead").floatValue = axis.dead;
            GetChildProperty(axisProperty, "sensitivity").floatValue = axis.sensitivity;
            GetChildProperty(axisProperty, "snap").boolValue = axis.snap;
            GetChildProperty(axisProperty, "invert").boolValue = axis.invert;
            GetChildProperty(axisProperty, "type").intValue = (int)axis.type;
            GetChildProperty(axisProperty, "axis").intValue = axis.axis - 1;
            GetChildProperty(axisProperty, "joyNum").intValue = axis.joyNum;
        }

        public static void SetupUNInput()
        {
            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 10; j++)
                {
                    if (i == 0)
                        AddAxis(new InputAxis("UNInput " + axisNames[j], "", "", "", "", "", "", 1000f, 0.001f, 1f, false, false, AxisType.JoystickAxis, j + 1, i));
                    else
                        AddAxis(new InputAxis("UNInput " + i + " " + axisNames[j], "", "", "", "", "", "", 1000f, 0.001f, 1f, false, false, AxisType.JoystickAxis, j + 1, i));
                }
                for (int k = 0; k < 20; k++)
                    if (i == 0)
                        AddAxis(new InputAxis("UNInput button " + k, "", "", "", "joystick button " + k, "", "", 1000f, 0.001f, 1000f, false, false, AxisType.KeyOrMouseButton, 1, i));
                    else
                        AddAxis(new InputAxis("UNInput " + i + " button " + k, "", "", "", "joystick " + i + " button " + k, "", "", 1000f, 0.001f, 1000f, false, false, AxisType.KeyOrMouseButton, 1, i));

                serializedObject.ApplyModifiedProperties();
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
                EditorUtility.SetDirty(serializedObject.targetObject);
                activeUNInput = true;
            }
        }

        public static void DelUNInput()
        {
            SerializedProperty axisProperty;
            for (int i = 0; i < axesProperty.arraySize; i++)
            {
                axisProperty = axesProperty.GetArrayElementAtIndex(i);
                if (GetChildProperty(axisProperty, "m_Name").stringValue.Contains("UNInput"))
                {
                    axisProperty.DeleteCommand();
                    i--;
                }
            }
            serializedObject.ApplyModifiedProperties();
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            EditorUtility.SetDirty(serializedObject.targetObject);
            activeUNInput = false;
        }

        public static bool ActiveUNInput()
        {
            return activeUNInput;
        }
    }
}