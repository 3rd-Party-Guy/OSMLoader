using System.Collections;
using System.Collections.Generic;
using UnityEngine;

internal sealed class ImportMapWrapper {
    private readonly DataImportWindow importWindow;
    private readonly string dataFile;

    private readonly Material[] roadMats;
    private readonly Material[] buildingMats;
    private readonly Material[] roofMats;

    private readonly bool importColors;
    private readonly bool generateColliders;

    public ImportMapWrapper(DataImportWindow window, string osmDataFile, Material[] roadMaterials,
                            Material[] buildingMaterials, Material[] roofMaterials, bool importColors, bool generateColliders)
    {
        this.importWindow = window;
        this.dataFile = osmDataFile;

        this.roadMats = roadMaterials;
        this.buildingMats = buildingMaterials;
        this.roofMats = roofMaterials;

        this.importColors = importColors;
        this.generateColliders = generateColliders;
    }

    public void Import() {
        MapReader mapReader = new();
        mapReader.Read(dataFile);

        GameObject parentObject = new("City");

        Road roadConstructor = new(parentObject, mapReader, roadMats, generateColliders);
        Building buildingConstructor = new(parentObject, mapReader, buildingMats, roofMats, importColors, generateColliders);

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