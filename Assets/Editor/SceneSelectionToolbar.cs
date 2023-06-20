using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityToolbarExtender;

/// <summary>
/// #unitytips: Scene Selection Toolbar - http://diegogiacomelli.com.br/unitytips-scene-selection-toolbar
/// </summary>
[InitializeOnLoad]
public static class SceneSelectionToolbar
{
    static List<SceneInfo> _scenes = new();
    static SceneInfo _sceneOpened;
    static int _selectedIndex;
    static string[] _displayedOptions;

    static SceneSelectionToolbar()
    {
        PopulateScenes();
        ToolbarExtender.LeftToolbarGUI.Add(OnToolbarGUI);
        EditorSceneManager.sceneOpened += HandleSceneOpened;

        string lastScene = GetPref("SceneSelectionToolbar.LatestOpenedScene");
        if (!string.IsNullOrEmpty(lastScene))
            SetOpenedScene(_scenes.Find(s => s.Path == lastScene));
    }

    static void OnToolbarGUI()
    {
        GUILayout.FlexibleSpace();

        _selectedIndex = EditorGUILayout.Popup(_selectedIndex, _displayedOptions); ;
        GUI.enabled = true;
        if (GUI.changed && _scenes.Count > _selectedIndex)
            EditorSceneManager.OpenScene(_scenes[_selectedIndex].Path);
    }

    static void RefreshDisplayedOptions()
    {
        _displayedOptions = new string[_scenes.Count];

        for (int i = 0; i < _scenes.Count; i++)
            _displayedOptions[i] = _scenes[i].Name;
    }

    static void HandleSceneOpened(Scene scene, OpenSceneMode mode) => SetOpenedScene(scene);

    static void SetOpenedScene(SceneInfo scene)
    {
        if (scene == null || string.IsNullOrEmpty(scene.Path))
            return;

        for (int i = 0; i < _scenes.Count; i++)
        {
            if (_scenes[i].Path == scene.Path)
            {
                _sceneOpened = _scenes[i];
                _selectedIndex = i;
                SaveToPlayerPrefs(true);
                return;
            }
        }

        _sceneOpened = scene;
        _selectedIndex = 0;
        SaveToPlayerPrefs(true);
    }

    static void SetOpenedScene(Scene scene) => SetOpenedScene(new SceneInfo(scene));

    static void AddScene(SceneInfo scene)
    {
        if (scene == null)
            return;

        _scenes.Add(scene);
    }

    static void SaveToPlayerPrefs(bool onlyLatestOpenedScene = false)
    {
        if (_sceneOpened != null)
            SetPref("SceneSelectionToolbar.LatestOpenedScene", _sceneOpened.Path);
    }

    static void PopulateScenes()
    {
        var path = "Assets/Scenes/";
        string[] files = Directory.GetFiles(path, "*.unity", SearchOption.TopDirectoryOnly);
        foreach (var file in files)
            AddScene(new SceneInfo(file));

        RefreshDisplayedOptions();
    }

    static void SetPref(string name, string value) => EditorPrefs.SetString($"{Application.productName}_{name}", value);
    static string GetPref(string name) => EditorPrefs.GetString($"{Application.productName}_{name}");

    [Serializable]
    class SceneInfo
    {
        public SceneInfo() { }
        public SceneInfo(Scene scene)
        {
            Name = scene.name;
            Path = scene.path;
        }

        public SceneInfo(string path)
        {
            Name = System.IO.Path.GetFileNameWithoutExtension(path);
            Path = path;
        }

        public string Name;
        public string Path;
    }
}
