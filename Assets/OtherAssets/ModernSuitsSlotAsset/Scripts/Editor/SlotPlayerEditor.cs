using UnityEngine;
using UnityEditor;

namespace Mkey
{
    [CustomEditor(typeof(SlotPlayer))]
    public class SlotPlayerEditor : Editor
    {
        private bool test = true;
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            EditorGUILayout.Space();
            EditorGUILayout.Space();
            #region test
            if (EditorApplication.isPlaying)
            {
                if (test = EditorGUILayout.Foldout(test, "Test"))
                {

                    EditorGUILayout.BeginHorizontal("box");
                    if (GUILayout.Button("Add 500 coins"))
                    {
                        SlotPlayer sP = (SlotPlayer)target;
                        if (sP)
                            sP.AddCoins(500);
                    }
                    if (GUILayout.Button("Clear coins"))
                    {
                        SlotPlayer sP = (SlotPlayer)target;
                        if (sP)
                            sP.SetCoinsCount(0);
                    }

                    EditorGUILayout.EndHorizontal();

                    #region coins
                    EditorGUILayout.BeginHorizontal("box");
                    if (GUILayout.Button("Set 500 coins"))
                    {
                        SlotPlayer sP = (SlotPlayer)target;
                        if (sP)
                            sP.SetCoinsCount(500);
                    }
                    if (GUILayout.Button("Add coins -500"))
                    {
                        SlotPlayer sP = (SlotPlayer)target;
                        if (sP)
                            sP.AddCoins(-500);
                    }

                    if (GUILayout.Button("Set coins -500"))
                    {
                        SlotPlayer sP = (SlotPlayer)target;
                        if (sP)
                            sP.SetCoinsCount(-500);
                    }
                    EditorGUILayout.EndHorizontal();
                    #endregion coins

                    #region freespins
                    EditorGUILayout.BeginHorizontal("box");
                    if (GUILayout.Button("Set 5 freespins"))
                    {
                        SlotPlayer sP = (SlotPlayer)target;
                        if (sP)
                            sP.SetFreeSpinsCount(5);
                    }
                    if (GUILayout.Button("Add 1 freespin"))
                    {
                        SlotPlayer sP = (SlotPlayer)target;
                        if (sP)
                            sP.AddFreeSpins(1);
                    }

                    if (GUILayout.Button("Add -1 freespin"))
                    {
                        SlotPlayer sP = (SlotPlayer)target;
                        if (sP)
                            sP.AddFreeSpins(-1);
                    }
                    EditorGUILayout.EndHorizontal();
                    #endregion freespins

                    #region autospins
                    EditorGUILayout.BeginHorizontal("box");
                    if (GUILayout.Button("Set 5 autospins"))
                    {
                        SlotPlayer sP = (SlotPlayer)target;
                        if (sP)
                            sP.SetAutoSpinsCount(5);
                    }
                    if (GUILayout.Button("Add 1 autospin"))
                    {
                        SlotPlayer sP = (SlotPlayer)target;
                        if (sP)
                            sP.AddAutoSpins(1);
                    }

                    if (GUILayout.Button("Add -1 autospin"))
                    {
                        SlotPlayer sP = (SlotPlayer)target;
                        if (sP)
                            sP.AddAutoSpins(-1);
                    }
                    EditorGUILayout.EndHorizontal();
                    #endregion autospins

                    #region level
                    EditorGUILayout.BeginHorizontal("box");
                    if (GUILayout.Button("Set 5 level"))
                    {
                        SlotPlayer sP = (SlotPlayer)target;
                        if (sP)
                            sP.SetLevel(5);
                    }
                    if (GUILayout.Button("Add 1 level"))
                    {
                        SlotPlayer sP = (SlotPlayer)target;
                        if (sP)
                            sP.AddLevel(1);
                    }

                    if (GUILayout.Button("Add -1 level"))
                    {
                        SlotPlayer sP = (SlotPlayer)target;
                        if (sP)
                            sP.AddLevel(-1);
                    }
                    EditorGUILayout.EndHorizontal();
                    #endregion level

                    #region levelprogress
                    EditorGUILayout.BeginHorizontal("box");
                    if (GUILayout.Button("Set 50 levprogr"))
                    {
                        SlotPlayer sP = (SlotPlayer)target;
                        if (sP)
                            sP.SetLevelProgress(50);
                    }
                    if (GUILayout.Button("Add 20 levprogr"))
                    {
                        SlotPlayer sP = (SlotPlayer)target;
                        if (sP)
                            sP.AddLevelProgress(20);
                    }

                    if (GUILayout.Button("Add -20 levprogr"))
                    {
                        SlotPlayer sP = (SlotPlayer)target;
                        if (sP)
                            sP.AddLevelProgress(-20);
                    }
                    EditorGUILayout.EndHorizontal();
                    #endregion levelprogress

                    #region scenes
                    EditorGUILayout.BeginHorizontal("box");
                    if (GUILayout.Button("Scene 0"))
                    {
                        SceneLoader.Instance.LoadScene(0);
                    }
                    if (GUILayout.Button("Scene 1"))
                    {
                        SceneLoader.Instance.LoadScene(1);
                    }

                    if (GUILayout.Button("Scene 2"))
                    {
                        SceneLoader.Instance.LoadScene(2);
                    }
                    if (GUILayout.Button("Scene 3"))
                    {
                        SceneLoader.Instance.LoadScene(3);
                    }
                    EditorGUILayout.EndHorizontal();
                    #endregion scenes

                    if (GUILayout.Button("Reset to default"))
                    {
                        SlotPlayer.Instance.SetDefaultData();
                    }
                }
            }
            else
            {
                EditorGUILayout.LabelField("Goto play mode for test");
            }
            #endregion test
        }
    }
}