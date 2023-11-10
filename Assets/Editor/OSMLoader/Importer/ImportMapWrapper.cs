using System.Collections;
using System.Collections.Generic;
using UnityEngine;

internal sealed class ImportMapWrapper {
    private DataImportWindow importWindow;
    private string dataFile;
    private Material roadMat;
    private Material buildingMat;

    private bool importColors;
    private bool generateColliders;

    public ImportMapWrapper(DataImportWindow window, string osmDataFile, Material roadMaterial,
                            Material buildingMaterial, bool importColors, bool generateColliders)
    {
        this.importWindow = window;
        this.dataFile = osmDataFile;
        this.roadMat = roadMaterial;
        this.buildingMat = buildingMaterial;
        this.importColors = importColors;
        this.generateColliders = generateColliders;
    }

    public void Import() {
        MapReader mapReader = new MapReader();
        mapReader.Read(dataFile);

        Road roadConstructor = new Road(mapReader, roadMat, generateColliders);
        Building buildingConstructor = new Building(mapReader, buildingMat, importColors, generateColliders);

        Process(buildingConstructor, "Constructing Buildings...");
        Process(roadConstructor, "Constructing Roads...");
    }

    private void Process(BaseInfrastructure constructor, string progressText) {
        float nodeCount = (float)constructor.NodeCount;
        float progress = 0f;

        foreach (int node in constructor.Process()) {
            progress = node / nodeCount;
            importWindow.UpdateImportProgress(progress, progressText);
        }
    }
}