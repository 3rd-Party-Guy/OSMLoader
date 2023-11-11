using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

internal sealed class Building : BaseInfrastructure
{
    private Material buildingMat;
    private bool importColors;
    private bool generateColliders;

    public override int NodeCount {
        get {
            return map.ways.FindAll((w) => {
                return w.IsBuilding && w.NodeIDs.Count > 1;
            }).Count;
        }
    }

    public Building(GameObject parentObj, MapReader mapReader, Material buildingMaterial, bool importColors,
                bool generateColliders) : base(mapReader, parentObj)
    {
        buildingMat = buildingMaterial;
        this.importColors = importColors;
        this.generateColliders = generateColliders;
    }

    public override IEnumerable<int> Process() {
        int count = 0;

        foreach (var way in map.ways.FindAll((w) => { return w.IsBuilding && w.NodeIDs.Count > 1; }))
        {
            CreateObject(way, buildingMat, "Building", importColors, generateColliders);

            count++;
            yield return count;
        }
    }

    protected override void OnObjectCreated(OSMWay way, Vector3 origin, List<Vector3> vectors,
                                        List<Vector3> normals, List<Vector2> uvs,
                                        List<int> indices, GameObject goBuilding = null) {
        Vector3 oTop = new Vector3(0, way.Height, 0);

        vectors.Add(oTop);
        normals.Add(Vector3.up);
        uvs.Add(new Vector2(0.5f, 0.5f));

        for (int i = 1; i < way.NodeIDs.Count; i++) {
            OSMNode p1 = map.nodes[way.NodeIDs[i - 1]];
            OSMNode p2 = map.nodes[way.NodeIDs[i]];

            Vector3 v1 = p1 - origin;
            Vector3 v2 = p2 - origin;
            Vector3 v3 = v1 + new Vector3(0, way.Height, 0);
            Vector3 v4 = v2 + new Vector3(0, way.Height, 0);

            vectors.Add(v1);
            vectors.Add(v2);
            vectors.Add(v3);
            vectors.Add(v4);

            uvs.Add(new Vector2(0, 0));
            uvs.Add(new Vector2(1, 0));
            uvs.Add(new Vector2(0, 1));
            uvs.Add(new Vector2(1, 1));

            normals.Add(-Vector3.forward);
            normals.Add(-Vector3.forward);
            normals.Add(-Vector3.forward);
            normals.Add(-Vector3.forward);

            int idx1, idx2, idx3, idx4;
            idx4 = vectors.Count - 1;
            idx3 = vectors.Count - 2;
            idx2 = vectors.Count - 3;
            idx1 = vectors.Count - 4;

            // triangle 1, 3, 2
            indices.Add(idx1);
            indices.Add(idx3);
            indices.Add(idx2);

            // triangle 3, 4, 2
            indices.Add(idx3);
            indices.Add(idx4);
            indices.Add(idx2);

            // triangle 2, 3, 1
            indices.Add(idx2);
            indices.Add(idx3);
            indices.Add(idx1);

            // triangle 2, 4, 3
            indices.Add(idx2);
            indices.Add(idx4);
            indices.Add(idx3);

            List<int> roofIndices = new()
            {
                // roof triangle
                0,
                idx3,
                idx4,

                // updside down
                idx4,
                idx3,
                0
            };

            CreateRooftopTile(goBuilding, way.Height, roofIndices, vectors, normals, uvs);
            CombineRooftop(goBuilding);
        }
    }

    private void CreateRooftopTile(GameObject parentObj, float height,
                            List<int> indices, List<Vector3> vectors,
                            List<Vector3> normals, List<Vector2> uvs)
    {
        GameObject goRoof = new GameObject("RooftopTile");
        goRoof.transform.position = parentObj.transform.position;
        goRoof.transform.parent = parentObj.transform;

        MeshFilter mf = goRoof.AddComponent<MeshFilter>();

        mf.sharedMesh = new Mesh
        {
            vertices = vectors.ToArray(),
            normals = normals.ToArray(),
            triangles = indices.ToArray(),
            uv = uvs.ToArray()
        };
    }

    private void CombineRooftop(GameObject parentObj)
    {
        GameObject goRoof = new("Rooftop");
        List<MeshFilter> roofTiles = new();

        foreach (Transform roofTile in parentObj.transform)
            roofTiles.Add(roofTile.GetComponent<MeshFilter>());

        var combine = new CombineInstance[roofTiles.Count];
        for (int i = 0; i < roofTiles.Count; i++)
        {
            combine[i].mesh = roofTiles[i].sharedMesh;
            combine[i].transform = roofTiles[i].transform.localToWorldMatrix;
        }

        foreach (Transform roofTile in parentObj.transform)
            GameObject.DestroyImmediate(roofTile.gameObject);

        var finalRoofMesh = new Mesh();
        finalRoofMesh.CombineMeshes(combine);

        var mf = goRoof.AddComponent<MeshFilter>();
        var mr = goRoof.AddComponent<MeshRenderer>();
        
        mr.material = buildingMat;

        mf.sharedMesh = finalRoofMesh;

        goRoof.transform.position = parentObj.transform.position;
        goRoof.transform.parent = parentObj.transform;
    }
}