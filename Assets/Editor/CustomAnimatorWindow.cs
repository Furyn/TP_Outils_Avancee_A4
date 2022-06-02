using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class CustomAnimatorWindow : EditorWindow
{

    Animator[] allAnimator = new Animator[0];
    bool[] allDisplay;

    Dictionary<Animator, AnimationClip> clipPlayByAnimator = new Dictionary<Animator, AnimationClip>();
    Dictionary<Animator, bool> boolAutoAnimator = new Dictionary<Animator, bool>();
    Dictionary<AnimationClip, float> clipCurrentTime = new Dictionary<AnimationClip, float>();

    static PlayModeStateChange stateMode = PlayModeStateChange.EnteredEditMode;

    double tmp_time = 0;

    // Add menu named "My Window" to the Window menu
    [MenuItem("Window/AnimatorEditor")]
    static void Init()
    {
        // Get existing open window or if none, make a new one:
        CustomAnimatorWindow window = (CustomAnimatorWindow)EditorWindow.GetWindow(typeof(CustomAnimatorWindow));
        window.Show();
    }

    void OnEnable() { EditorApplication.update += UpdateAnimation; EditorApplication.playModeStateChanged += playModeState; }
    void OnDisable() { EditorApplication.update -= UpdateAnimation; EditorApplication.playModeStateChanged -= playModeState; }

    void OnGUI()
    {
        GUILayout.Space(18);

        if (GUILayout.Button("Load Animator List"))
        {
            allAnimator = FindObjectsOfType<Animator>();
            allDisplay = new bool[allAnimator.Length];
            clipPlayByAnimator.Clear();
            clipCurrentTime.Clear();
            boolAutoAnimator.Clear();
            foreach (Animator item in allAnimator)
            {
                boolAutoAnimator[item] = true;
            }
            AnimationMode.StopAnimationMode();
        }

        if (stateMode == PlayModeStateChange.EnteredEditMode)
        {
            AnimationMode.StartAnimationMode();
        }

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
                        if (GUILayout.Button(clip.name) && stateMode == PlayModeStateChange.EnteredEditMode)
                        {
                            clipPlayByAnimator[allAnimator[i]] = clip;
                            clipCurrentTime[clip] = 0;
                        }
                    }
                    GUILayout.EndHorizontal();

                    GUILayout.BeginHorizontal();
                    if (clipPlayByAnimator.ContainsKey(allAnimator[i]))
                    {
                        if (boolAutoAnimator.Count == 0)
                        {
                            foreach (Animator item in allAnimator)
                            {
                                boolAutoAnimator[item] = true;
                            }
                        }
                        if (boolAutoAnimator[allAnimator[i]])
                        {
                            EditorGUILayout.Slider(clipCurrentTime[clipPlayByAnimator[allAnimator[i]]], 0, clipPlayByAnimator[allAnimator[i]].length);
                        }
                        else
                        {
                            clipCurrentTime[clipPlayByAnimator[allAnimator[i]]] = EditorGUILayout.Slider(clipCurrentTime[clipPlayByAnimator[allAnimator[i]]], 0, clipPlayByAnimator[allAnimator[i]].length);
                        }
                        string textAuto = "Auto Mode";
                        if (!boolAutoAnimator[allAnimator[i]])
                        {
                            textAuto = "Slider Mode";
                        }
                        if (GUILayout.Button(textAuto))
                        {
                            boolAutoAnimator[allAnimator[i]] = !boolAutoAnimator[allAnimator[i]];
                        }
                    }
                    GUILayout.EndHorizontal();

                }
                else
                {
                    GUILayout.Label("None", GUI.skin.button);
                }
            }
        }
    }

    public void UpdateAnimation()
    {
        double timeToSave = EditorApplication.timeSinceStartup - tmp_time;
        PlayAnimation(timeToSave);
        tmp_time = EditorApplication.timeSinceStartup;
    }

    private void PlayAnimation(double time)
    {
        if (clipPlayByAnimator.Count == 0 || !AnimationMode.InAnimationMode())
        {
            return;
        }
        bool repaint = false;
        foreach (KeyValuePair<Animator, AnimationClip> item in clipPlayByAnimator)
        {
            if (boolAutoAnimator[item.Key])
            {
                clipCurrentTime[item.Value] += (float)time;
                if (clipCurrentTime[item.Value] >= item.Value.length)
                {
                    clipCurrentTime[item.Value] = 0;
                }
            }
            AnimationMode.SampleAnimationClip(item.Key.gameObject, item.Value, clipCurrentTime[item.Value]);
            if (boolAutoAnimator[item.Key])
            {
                repaint = true;
            }
        }

        if (repaint)
        {
            Repaint();
        }

    }

    private static void playModeState(PlayModeStateChange state)
    {
        if (state == PlayModeStateChange.EnteredPlayMode)
        {
            AnimationMode.StopAnimationMode();
            stateMode = state;
        }

        if (state == PlayModeStateChange.EnteredEditMode)
        {
            AnimationMode.StopAnimationMode();
            stateMode = state;
        }
    }
}