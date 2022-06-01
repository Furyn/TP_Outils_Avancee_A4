using UnityEngine;
using UnityEditor;

public class CustomAnimatorWindow : EditorWindow
{

    Animator[] allAnimator;
    bool[] allDisplay;

    // Add menu named "My Window" to the Window menu
    [MenuItem("Window/AnimatorEditor")]
    static void Init()
    {
        // Get existing open window or if none, make a new one:
        CustomAnimatorWindow window = (CustomAnimatorWindow)EditorWindow.GetWindow(typeof(CustomAnimatorWindow));
        window.Show();
    }

    void OnGUI()
    {

        GUILayout.Space(18);

        if (GUILayout.Button("Load Animator List"))
        {
            allAnimator = FindObjectsOfType<Animator>();
            allDisplay = new bool[allAnimator.Length];
        }

        for (int i = 0; i < allAnimator.Length; i++)
        {
            GUI.backgroundColor = Color.green;
            if (GUILayout.Button(allAnimator[i].name , GUILayout.MaxWidth(200), GUILayout.MaxHeight(50)))
            {
                Selection.activeGameObject = allAnimator[i].gameObject;
                allDisplay[i] = !allDisplay[i];
            }

            if (allDisplay[i] == true)
            {
                EditorGUILayout.TextField("Text Field", "TESTE");
            }
            
        }

        /*GUILayout.Label("Base Settings", EditorStyles.boldLabel);
        myString = EditorGUILayout.TextField("Text Field", myString);

        groupEnabled = EditorGUILayout.BeginToggleGroup("Optional Settings", groupEnabled);
        myBool = EditorGUILayout.Toggle("Toggle", myBool);
        myFloat = EditorGUILayout.Slider("Slider", myFloat, -3, 3);
        EditorGUILayout.EndToggleGroup();*/
    }
}