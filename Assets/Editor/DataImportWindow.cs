using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

public class DataImportWindow : EditorWindow {
    private Material roadMaterial;
    private Material buildingMaterial;

    private string osmFilePath = "OSM Data File Path";
    private string importProgressText;

    private float importProgress;

    private bool isSceneNotSaved = false;
    private bool isFileValid = false;
    private bool isImporting = false;

    [MenuItem("Window/OSMLoader/Data Importer")]
    private static void ShowWindow() {
        EditorWindow window = GetWindow<DataImportWindow>();
        window.titleContent = new GUIContent("Data Importer");
        window.Show();
    }

    private void OnGUI() {
        EditorGUILayout.BeginHorizontal();
            EditorGUI.BeginDisabledGroup(disabled: true);
                EditorGUILayout.TextField(osmFilePath);
            EditorGUI.EndDisabledGroup();
            
            if (GUILayout.Button("...")) {
                string dataPath = EditorUtility.OpenFilePanel("Select OSM Data File", Application.dataPath, "osm");

                if (dataPath.Length > 0)
                    osmFilePath = dataPath;
                    
                isFileValid = (osmFilePath.Length > 0);
            } 
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.Space();

        roadMaterial = (Material)EditorGUILayout.ObjectField("Road Material", roadMaterial, typeof(Material), allowSceneObjects: false);
        buildingMaterial = (Material)EditorGUILayout.ObjectField("Building Material", buildingMaterial, typeof(Material), allowSceneObjects: false);
        EditorGUILayout.Space();
        GUILayout.FlexibleSpace();

        EditorGUI.BeginDisabledGroup(!isFileValid || isSceneNotSaved || isImporting);
            if (GUILayout.Button("Import OSM Data")) {
                OnImport();
            }
        EditorGUI.EndDisabledGroup();

        if (isImporting) {
            Rect rect = EditorGUILayout.BeginHorizontal();
                EditorGUI.ProgressBar(rect, importProgress, importProgressText);
            EditorGUILayout.EndHorizontal();
        }

        if (isSceneNotSaved)
            EditorGUILayout.HelpBox("Scene is not saved!", MessageType.Warning, true);
    }

    public void UpdateImportProgress(float progress, string progressText) {
        importProgress = progress;
        importProgressText = progressText;

        Repaint();
    }

    public void ResetProgress() {
        importProgress = 0f;
        importProgressText = "";
    }

    private void Update() {
        isSceneNotSaved = EditorSceneManager.GetActiveScene().isDirty;
    }

    private void OnImport() {
        Debug.Log("This gets called");
        isImporting = true;

        ImportMapWrapper mapWrapper = new ImportMapWrapper(this, osmFilePath, roadMaterial, buildingMaterial);
        mapWrapper.Import();

        isImporting = false;
    }
}