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
    Dictionary<AnimationClip, float> clipCurrentSpeed = new Dictionary<AnimationClip, float>();

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

    void OnEnable() 
    { 
        EditorApplication.update += UpdateAnimation; 
        EditorApplication.playModeStateChanged += playModeState;
        UnityEditor.SceneManagement.EditorSceneManager.sceneOpened += SceneOpened;
    }
    void OnDisable() 
    { 
        EditorApplication.update -= UpdateAnimation; 
        EditorApplication.playModeStateChanged -= playModeState;
        UnityEditor.SceneManagement.EditorSceneManager.sceneOpened -= SceneOpened;
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
            clipCurrentSpeed.Clear();
            boolAutoAnimator.Clear();
            foreach (Animator item in allAnimator)
            {
                boolAutoAnimator[item] = true;
            }
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

            if (allDisplay[i] == true)
            {
                if (allAnimator[i].runtimeAnimatorController != null)
                {
                    GUILayout.BeginHorizontal();
                    AnimationClip[] allClip = allAnimator[i].runtimeAnimatorController.animationClips;
                    List<AnimationClip> uniq_clip = new List<AnimationClip>();
                    foreach (AnimationClip clip in allClip)
                    {
                        if (uniq_clip.Contains(clip))
                        {
                            continue;
                        }
                        uniq_clip.Add(clip);
                        if (clipPlayByAnimator.ContainsKey(allAnimator[i]) && clipPlayByAnimator[allAnimator[i]] == clip )
                        {
                            GUI.backgroundColor = Color.blue;
                        }
                        else
                        {
                            GUI.backgroundColor = Color.white;
                        }

                        if (uniq_clip.Count % 5 == 0)
                        {
                            GUILayout.EndHorizontal();
                            GUILayout.BeginHorizontal();
                        }
                        if (GUILayout.Button(clip.name, GUILayout.MaxWidth(100)) && stateMode == PlayModeStateChange.EnteredEditMode)
                        {
                            clipPlayByAnimator[allAnimator[i]] = clip;
                            clipCurrentTime[clip] = 0;
                            if (!clipCurrentSpeed.ContainsKey(clip))
                            {
                                clipCurrentSpeed[clip] = allAnimator[i].speed;
                            }
                        }
                    }
                    GUILayout.EndHorizontal();

                    GUI.backgroundColor = Color.white;
                    if (clipPlayByAnimator.ContainsKey(allAnimator[i]))
                    {
                        GUILayout.BeginHorizontal();
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
                        //GUI.skin.button.wordWrap = false;
                        if (GUILayout.Button(textAuto))
                        {
                            boolAutoAnimator[allAnimator[i]] = !boolAutoAnimator[allAnimator[i]];
                        }
                        GUI.skin.button.wordWrap = true;

                        GUILayout.EndHorizontal();
                        GUILayout.BeginHorizontal();

                        GUILayout.Label("Boucle animation : ");
                        bool loop = EditorGUILayout.Toggle(clipPlayByAnimator[allAnimator[i]].wrapMode == WrapMode.Loop);
                        if (loop)
                        {
                            clipPlayByAnimator[allAnimator[i]].wrapMode = WrapMode.Loop;
                        }
                        else
                        {
                            clipPlayByAnimator[allAnimator[i]].wrapMode = WrapMode.Once;
                        }

                        GUILayout.Label("Speed : ");
                        clipCurrentSpeed[clipPlayByAnimator[allAnimator[i]]] = EditorGUILayout.Slider(clipCurrentSpeed[clipPlayByAnimator[allAnimator[i]]], 0, 5);

                        GUILayout.EndHorizontal();
                        GUILayout.BeginHorizontal();

                        GUILayout.Label("Animation Time : " + clipCurrentTime[clipPlayByAnimator[allAnimator[i]]].ToString("0.000") + "/" + clipPlayByAnimator[allAnimator[i]].length.ToString("0.000"));

                        GUILayout.EndHorizontal();
                    }
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
        if (clipPlayByAnimator.Count == 0)
        {
            if (AnimationMode.InAnimationMode())
            {
                AnimationMode.StopAnimationMode();
            }
            return;
        }

        if (!AnimationMode.InAnimationMode())
        {
            AnimationMode.StartAnimationMode();
        }

        bool repaint = false;
        foreach (KeyValuePair<Animator, AnimationClip> item in clipPlayByAnimator)
        {
            if (boolAutoAnimator[item.Key])
            {
                clipCurrentTime[item.Value] += (float)time * clipCurrentSpeed[item.Value];
                if (clipCurrentTime[item.Value] >= item.Value.length)
                {
                    if (item.Value.wrapMode == WrapMode.Loop)
                    {
                        clipCurrentTime[item.Value] = 0;
                    }
                    else
                    {
                        clipCurrentTime[item.Value] = item.Value.length;
                    }
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

    private void playModeState(PlayModeStateChange state)
    {
        if (state == PlayModeStateChange.EnteredPlayMode)
        {
            clipPlayByAnimator.Clear();
            clipCurrentTime.Clear();
            clipCurrentSpeed.Clear();
            stateMode = state;
        }

        if (state == PlayModeStateChange.EnteredEditMode)
        {
            stateMode = state;
        }
    }

    void SceneOpened(UnityEngine.SceneManagement.Scene scene, UnityEditor.SceneManagement.OpenSceneMode mode)
    {
        clipPlayByAnimator.Clear();
        clipCurrentTime.Clear();
        boolAutoAnimator.Clear();
    }
}