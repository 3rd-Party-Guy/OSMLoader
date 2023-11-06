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

        Building buildingConstructor = new Building(mapReader, buildingMat);
        Road roadConstructor = new Road(mapReader, roadMat);

        // Process(roadConstructor);
        // Process(buildingConstructor);
    }

    // private void Process(BaseInfrastructure base) {
    // }
}