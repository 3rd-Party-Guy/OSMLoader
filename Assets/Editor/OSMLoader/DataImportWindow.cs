using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEditorInternal;
using System.Linq;

public class DataImportWindow : EditorWindow
{
    SerializedObject serializedObject;

    public Material[] roadMaterials = { };
    public Material[] buildingMaterials = { };
    public Material[] roofMaterials = { };

    private string osmFilePath = "OSM Data File Path";
    private string importProgressText;

    private float importProgress;

    private bool isSceneNotSaved = false;
    private bool isFileValid = false;
    private bool isImporting = false;

    private bool importColors = false;
    private bool generateColliders = false;

    [MenuItem("Window/OSMLoader/Data Importer")]
    private static void ShowWindow()
    {
        EditorWindow window = GetWindow<DataImportWindow>();
        window.titleContent = new GUIContent("Data Importer");
        window.Show();
    }

    private void OnEnable()
    {
        var target = this;
        serializedObject = new SerializedObject(target);
    }

    private void OnGUI()
    {
        EditorGUILayout.BeginHorizontal();
            EditorGUI.BeginDisabledGroup(disabled: true);
                EditorGUILayout.TextField(osmFilePath);
            EditorGUI.EndDisabledGroup();

            if (GUILayout.Button("..."))
            {
                string dataPath = EditorUtility.OpenFilePanel("Select OSM Data File", Application.dataPath, "osm,txt,xml");

                if (dataPath.Length > 0)
                    osmFilePath = dataPath;

                isFileValid = (osmFilePath.Length > 0);
            }
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.Space();

        GUILayout.Label("Materials", EditorStyles.boldLabel);

        serializedObject.Update();
        EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(roadMaterials)), true);
        EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(buildingMaterials)), true);
        EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(roofMaterials)), true);
        serializedObject.ApplyModifiedProperties();

        EditorGUILayout.Space();

        GUILayout.Label("Import Settings", EditorStyles.boldLabel);
        importColors = EditorGUILayout.Toggle("Import Colors", importColors);
        generateColliders = EditorGUILayout.Toggle("Generate Colliders", generateColliders);
        EditorGUILayout.Space();
        GUILayout.FlexibleSpace();

        EditorGUI.BeginDisabledGroup(!isFileValid || isSceneNotSaved || isImporting);
            if (GUILayout.Button("Import OSM Data"))
            {
                OnImport();
            }
        EditorGUI.EndDisabledGroup();

        if (isImporting)
        {
            Rect rect = EditorGUILayout.BeginHorizontal();
                EditorGUI.ProgressBar(rect, importProgress, importProgressText);
            EditorGUILayout.EndHorizontal();
        }

        if (isSceneNotSaved)
            EditorGUILayout.HelpBox("Scene is not saved!", MessageType.Warning, true);
    }

    public void UpdateImportProgress(float progress, string progressText)
    {
        importProgress = progress;
        importProgressText = progressText;

        Repaint();
    }

    public void ResetProgress()
    {
        importProgress = 0f;
        importProgressText = "";
    }

    private void Update()
    {
        isSceneNotSaved = EditorSceneManager.GetActiveScene().isDirty;
    }

    private void OnImport()
    {
        isImporting = true;

        ImportMapWrapper mapWrapper = new(this, osmFilePath, roadMaterials,
                                        buildingMaterials, roofMaterials,
                                        importColors, generateColliders);
        mapWrapper.Import();

        isImporting = false;
    }
}