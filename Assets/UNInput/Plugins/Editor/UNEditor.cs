//System includes
using System.IO;

//Unity includes
using UnityEditor;
using UnityEngine;
using UnityEditorInternal;

//Custom includes
using UniversalNetworkInput.Network.Internal;

namespace UniversalNetworkInput.Internal
{
    /// <summary>
    /// Editor Windows for UNInput configuration
    /// </summary>
    public class UNInputWindow : EditorWindow
    {
        static UNInputWindow window;
        static GUIStyle base_style;
        static GUIStyle border_style;
        static BuildTarget targetPlataform;
        [SerializeField]
        static GameObject prefabReference;

        public UNServerPreferences prefs;
        ReorderableList reordableList;
        SerializedObject serializedObject;
        static Texture axis2d;
        static bool hasPro;

        [MenuItem("Window/UNInput")]
        static void Init()
        {
            //Get existing open window or if none, make a new one:
            window = GetWindow<UNInputWindow>();
            window.name = "UNInput";
            window.titleContent = new GUIContent("UNInput");
            window.maxSize = new Vector2(420, 2160);
            window.minSize = new Vector2(420, 380);
            window.Show();

            //Instantiate GUIStyles
            base_style = new GUIStyle();
            base_style.fontStyle = FontStyle.Italic;
            border_style = new GUIStyle(EditorStyles.helpBox);
            hasPro = UnityEditorInternal.InternalEditorUtility.HasPro();
            if (hasPro)
                axis2d = EditorGUIUtility.Load("Assets/UNInput/Plugins/Editor/axisPRO.png") as Texture;
            else
                axis2d = EditorGUIUtility.Load("Assets/UNInput/Plugins/Editor/axis.png") as Texture;
        }

        void OnGUI()
        {
            serializedObject.Update();

            if (border_style == null)
                border_style = new GUIStyle(EditorStyles.helpBox);

            GUILayout.Label("Hardware Inputs (will be registered to Unity Inputs)", EditorStyles.boldLabel);

            EditorGUILayout.BeginVertical(border_style);

            if (!SetupInputManager.ActiveUNInput())
            {
                EditorGUILayout.BeginHorizontal();
                if (base_style == null)
                {
                    base_style = new GUIStyle();
                    base_style.fontStyle = FontStyle.Italic;
                }
                EditorGUILayout.TextArea("  Hardware Inputs NOT Registered!.", base_style);
                EditorGUILayout.EndHorizontal();

                if (GUILayout.Button("Register Hardware Inputs"))
                    SetupInputManager.SetupUNInput();
            }
            else
            {
                EditorGUILayout.BeginHorizontal();
                if (base_style == null)
                {
                    base_style = new GUIStyle();
                    base_style.fontStyle = FontStyle.Italic;
                }
                EditorGUILayout.TextArea("  Hardware Inputs Registered", base_style);
                EditorGUILayout.EndHorizontal();

                if (GUILayout.Button("Unregister Hardware Inputs"))
                    SetupInputManager.DelUNInput();
            }

            EditorGUILayout.EndVertical();

            GUILayout.Label("Network Inputs (will be registered on client side)", EditorStyles.boldLabel);

            EditorGUILayout.BeginVertical(EditorStyles.inspectorFullWidthMargins);

            reordableList.DoLayoutList();

            serializedObject.ApplyModifiedProperties();

            EditorGUILayout.EndVertical();

            GUILayout.Label("Network Interface Asset (will be instantiated on client side)", EditorStyles.boldLabel);

            EditorGUILayout.BeginVertical(border_style);

            EditorGUILayout.BeginHorizontal();

            //EditorGUILayout.PropertyField(serializedObject.FindProperty("InterfaceAsset"));
            prefabReference = (GameObject)EditorGUILayout.ObjectField("Interface Asset", prefabReference, typeof(GameObject), true);
            if (prefabReference != null)
            {
                //Object reference assign
                serializedObject.FindProperty("InterfaceAsset").objectReferenceValue = prefabReference;

                //Define AssetBundle Label
                string path = AssetDatabase.GetAssetPath(prefabReference);
                serializedObject.FindProperty("AssetName").stringValue = prefabReference.name + ".prefab";

                //If a valid path, continue
                if (path != null || path.Length != 0)
                {
                    EditorGUILayout.EndHorizontal();

                    EditorGUILayout.BeginHorizontal();

                    targetPlataform = (BuildTarget)EditorGUILayout.EnumPopup("Target Plataform", targetPlataform);

                    EditorGUILayout.EndHorizontal();

                    EditorGUILayout.BeginHorizontal();

                    //Check if Game and Target plataforms are defined
                    if (targetPlataform == BuildTarget.NoTarget)
                    {
                        EditorGUILayout.TextArea("You must select a game and a target build plataform!");
                    }
                    else if (GUILayout.Button("Create UNInput Bundle"))
                    {
                        //If export asset bundle defined
                        AssetImporter importer = AssetImporter.GetAtPath(path);
                        importer.assetBundleName = "UNInputBundle";

                        //Explicitly save Asset and Reimport
                        //Otherwise only happens when project is saved!
                        importer.SaveAndReimport();

                        //Override Asset Bundle Name for all prefab dependencies
                        Object[] dependencies = EditorUtility.CollectDependencies(new Object[] { prefabReference });
                        for (int i = 0; i < dependencies.Length; i++)
                        {
                            path = AssetDatabase.GetAssetPath(dependencies[i]);

                            //If not a valid asset path continue to next dependencie
                            if (path == null || path.Length == 0)
                                continue;

                            //Only get assets dependencies (otherwise it will take Unity's DLLs)
                            if (!path.StartsWith("Assets/"))
                                continue;

                            //Avoid scripts!
                            if (path.EndsWith(".cs") || path.EndsWith(".js"))
                                continue;

                            //Get asset reference
                            importer = AssetImporter.GetAtPath(path);

                            //If not a valid asset continue
                            if (importer == null)
                                continue;

                            //Override assetBundleName
                            importer.SetAssetBundleNameAndVariant("UNInputBundle", "");

                            //Save and reimport
                            importer.SaveAndReimport();
                        }

                        if (!Directory.Exists("Assets/Resources/Bundles"))
                            Directory.CreateDirectory("Assets/Resources/Bundles");

                        //Clear old UNInputBundle
                        File.Delete(Application.dataPath + "/Resources/Bundles/uninputbundle.bytes");
                        File.Delete(Application.dataPath + "/Resources/Bundles/uninputbundle.bytes.meta");

                        //Save AssetBundles to given Build Target with Chunk Based compression for sending it through the network
                        Debug.Log("UNAsset Bundle being built to " + targetPlataform.ToString());

                        //Build Asset Bundle
                        BuildPipeline.BuildAssetBundles("Assets/Resources/Bundles", BuildAssetBundleOptions.None, targetPlataform);

                        //And then refresh it
                        AssetDatabase.Refresh();

                        //Clear folder of manifest file
                        File.Delete(Application.dataPath + "/Resources/Bundles/uninputbundle.manifest");
                        File.Delete(Application.dataPath + "/Resources/Bundles/uninputbundle.manifest.meta");

                        //Save UNInputBundle with .bytes extension, for late loading as binnary
                        AssetDatabase.RenameAsset("Assets/Resources/Bundles/uninputbundle", "uninputbundle.bytes");

                        //Let us apply the serializer properties
                        serializedObject.ApplyModifiedProperties();

                        //Save our database
                        AssetDatabase.SaveAssets();

                        //And then refresh it
                        AssetDatabase.Refresh();
                    }
                }
            }

            EditorGUILayout.EndHorizontal();

            EditorGUILayout.EndVertical();

        }

        private void OnEnable()
        {
            //Load preferences asset
            prefs = AssetDatabase.LoadAssetAtPath<UNServerPreferences>("Assets/Resources/UNServerPrefs.asset");
            if (prefs == null)
            {
                prefs = CreateInstance<UNServerPreferences>();
                if (!Directory.Exists("Assets/Resources"))
                    Directory.CreateDirectory("Assets/Resources");
                AssetDatabase.CreateAsset(prefs, "Assets/Resources/UNServerPrefs.asset");
            }

            //Setup target variable
            targetPlataform = BuildTarget.Android;

            //Create serialized object from Asset Prefab
            serializedObject = new SerializedObject(prefs);

            //Set prefab reference from file
            prefabReference = (GameObject)serializedObject.FindProperty("InterfaceAsset").objectReferenceValue;

            //Create a list of the serializable asset object
            reordableList = new ReorderableList(serializedObject, serializedObject.FindProperty("Inputs"),
                    true, true, true, true);

            //reordableList.elementHeight = EditorGUIUtility.singleLineHeight * 3f;
            reordableList.elementHeight = EditorGUIUtility.singleLineHeight * 3f;

            //Draw callback overrides
            reordableList.drawHeaderCallback = (Rect rect) =>
            {
                EditorGUI.LabelField(rect, "Network Inputs");
            };

            reordableList.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
            {
                var element = reordableList.serializedProperty.GetArrayElementAtIndex(index);
                rect.y += 2;
                EditorGUI.PropertyField(
                    new Rect(rect.x, rect.y, 60, EditorGUIUtility.singleLineHeight),
                    element.FindPropertyRelative("type"), GUIContent.none);
                EditorGUI.PropertyField(
                    new Rect(rect.x + 65, rect.y, 255, EditorGUIUtility.singleLineHeight),
                    element.FindPropertyRelative("name"), GUIContent.none);
                if ((element.FindPropertyRelative("type").enumValueIndex) == 0)
                {
                    EditorGUI.PropertyField(
                        new Rect(rect.x + 325, rect.y, 60, EditorGUIUtility.singleLineHeight),
                        element.FindPropertyRelative("axis_code"), GUIContent.none);
                    EditorGUI.PropertyField(
                        new Rect(rect.x, rect.y + EditorGUIUtility.singleLineHeight * 1.35f, 60, EditorGUIUtility.singleLineHeight),
                        element.FindPropertyRelative("key_down"), GUIContent.none);
                    EditorGUI.PropertyField(
                        new Rect(rect.size.x - 37, rect.y + EditorGUIUtility.singleLineHeight * 1.35f, 60, EditorGUIUtility.singleLineHeight),
                        element.FindPropertyRelative("key_up"), GUIContent.none);
                    EditorGUI.DrawTextureTransparent(
                     new Rect((rect.size.x / 2f) - 107, rect.y + EditorGUIUtility.singleLineHeight * 1.2f, 260, 16),
                     axis2d, ScaleMode.StretchToFill);
                    EditorGUI.LabelField(
                        new Rect(rect.size.x/2 - 10, rect.y + EditorGUIUtility.singleLineHeight * 2f, 60, EditorGUIUtility.singleLineHeight),
                        new GUIContent("Keyboard", "Set the keyboard buttons that will react to this input."));                    
                }
                else
                {
                    EditorGUI.PropertyField(
                        new Rect(rect.x + 325, rect.y, 60, EditorGUIUtility.singleLineHeight),
                        element.FindPropertyRelative("button_code"), GUIContent.none);
                    EditorGUI.PropertyField(
                       new Rect(rect.size.x /2, rect.y + EditorGUIUtility.singleLineHeight * 1.5f, 60, EditorGUIUtility.singleLineHeight),
                       element.FindPropertyRelative("key_down"), GUIContent.none);                    
                    EditorGUI.LabelField(
                        new Rect(rect.size.x/2 - 60, rect.y + EditorGUIUtility.singleLineHeight * 1.45f, 60, EditorGUIUtility.singleLineHeight),
                        new GUIContent("Keyboard", "Set the keyboard buttons that will react to this input."));
                }

            };

            //Filtering callbacks
            reordableList.onSelectCallback = (ReorderableList list) =>
            {
                //Debug.Log("Select!");
                for (int i = 0; i < list.count; i++)
                {
                    string name = list.serializedProperty.GetArrayElementAtIndex(i).FindPropertyRelative("name").stringValue;
                    int type = list.serializedProperty.GetArrayElementAtIndex(i).FindPropertyRelative("type").enumValueIndex;
                    if (name.Length == 0)
                    {
                        list.serializedProperty.GetArrayElementAtIndex(i).FindPropertyRelative("name").stringValue = "Input " + i.ToString();
                    }
                }
            };
        }

        private void OnDisable()
        {
            //Save Data
            serializedObject.ApplyModifiedProperties();
            AssetDatabase.SaveAssets();



            //Refresh Database
            AssetDatabase.Refresh();
        }
    }

}