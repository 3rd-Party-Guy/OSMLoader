using System.Collections;
using System.Collections.Generic;
using UnityEngine;

internal sealed class ImportMapWrapper {
    private DataImportWindow importWindow;
    private string dataFile;
    private Material roadMat;
    private Material buildingMat;

    public ImportMapWrapper(DataImportWindow window, string osmDataFile, Material roadMaterial, Material buildingMaterial) {
        importWindow = window;
        dataFile = osmDataFile;
        roadMat = roadMaterial;
        buildingMat = buildingMaterial;
    }

    public void Import() {
        MapReader mapReader = new MapReader();
        mapReader.Read(dataFile);

        Road roadConstructor = new Road(mapReader, roadMat);
        Building buildingConstructor = new Building(mapReader, buildingMat);

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