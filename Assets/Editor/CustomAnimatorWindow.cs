using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class CustomAnimatorWindow : EditorWindow
{

    Animator[] allAnimator = new Animator[0];
    bool[] allDisplay;

    Dictionary<Animator, AnimationClip> clipPlayByAnimator = new Dictionary<Animator, AnimationClip>();
    Dictionary<AnimationClip, float> clipCurrentTime = new Dictionary<AnimationClip, float>();

    double tmp_time = 0;

    // Add menu named "My Window" to the Window menu
    [MenuItem("Window/AnimatorEditor")]
    static void Init()
    {
        // Get existing open window or if none, make a new one:
        CustomAnimatorWindow window = (CustomAnimatorWindow)EditorWindow.GetWindow(typeof(CustomAnimatorWindow));
        window.Show();
        //EditorApplication.update = EditorUpdate;
    }

    void OnGUI()
    {
        GUILayout.Space(18);

        if (GUILayout.Button("Load Animator List"))
        {
            allAnimator = FindObjectsOfType<Animator>();
            allDisplay = new bool[allAnimator.Length];
            clipPlayByAnimator.Clear();
            clipCurrentTime.Clear();
            AnimationMode.StopAnimationMode();
        }
        AnimationMode.StartAnimationMode();

        foreach (Animator item in allAnimator)
        {
            if (!item)
            {
                return;
            }
        }

        for (int i = 0; i < allAnimator.Length; i++)
        {
            GUI.backgroundColor = Color.cyan;
            GUILayout.Label(allAnimator[i].name, GUI.skin.button);

            GUI.backgroundColor = Color.green;
            GUILayout.BeginHorizontal();

            GUIContent btnTxt = new GUIContent("Focus");
            var rt = GUILayoutUtility.GetRect(btnTxt, GUI.skin.button, GUILayout.MaxWidth(150), GUILayout.MaxHeight(25));
            rt.center = new Vector2(EditorGUIUtility.currentViewWidth / 3.5f, rt.center.y);

            if (GUI.Button(rt, btnTxt, GUI.skin.button))
            {
                Selection.activeGameObject = allAnimator[i].gameObject;
            }

            string menu_txt = "Open Menu";
            if (allDisplay[i])
            {
                menu_txt = "Close Menu";
            }

            GUIContent btnTxt2 = new GUIContent(menu_txt);
            var rt2 = GUILayoutUtility.GetRect(btnTxt2, GUI.skin.button, GUILayout.MaxWidth(150), GUILayout.MaxHeight(25));
            rt2.center = new Vector2(EditorGUIUtility.currentViewWidth / 1.5f, rt2.center.y);

            if (GUI.Button(rt2, btnTxt2, GUI.skin.button))
            {
                allDisplay[i] = !allDisplay[i];
            }

            GUILayout.EndHorizontal();

            GUI.backgroundColor = Color.white;
            if (allDisplay[i] == true)
            {
                GUILayout.BeginHorizontal();
                if (allAnimator[i].runtimeAnimatorController != null)
                {
                    AnimationClip[] allClip = allAnimator[i].runtimeAnimatorController.animationClips;
                    
                    foreach (AnimationClip clip in allClip)
                    {
                        if (GUILayout.Button(clip.name))
                        {
                            Debug.Log("Play anim '" + clip.name + "' on the " + allAnimator[i]);
                            clipPlayByAnimator[allAnimator[i]] = clip;
                            clipCurrentTime[clip] = 0;
                        }
                    }
                }
                else
                {
                    GUILayout.Label("None", GUI.skin.button);
                }
                GUILayout.EndHorizontal();
            }
        }
    }

    /*public static void EditorUpdate()
    {
        double timeToSave = EditorApplication.timeSinceStartup - window.tmp_time;
        window.PlayAnimation(timeToSave);
        window.tmp_time = EditorApplication.timeSinceStartup;
    }*/

    private void PlayAnimation(double time)
    {
        if (clipPlayByAnimator.Count == 0)
        {
            return;
        }
        foreach (KeyValuePair<Animator, AnimationClip> item in clipPlayByAnimator)
        {
            Debug.Log(time);
            clipCurrentTime[item.Value] += (float)time;
            AnimationMode.SampleAnimationClip(item.Key.gameObject, item.Value, clipCurrentTime[item.Value]);
        }
    }
}