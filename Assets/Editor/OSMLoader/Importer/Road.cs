using System.Collections;
using System.Collections.Generic;
using UnityEngine;

internal sealed class Road : BaseInfrastructure
{
    public Material roadMat;
    private bool generateColliders;

    public override int NodeCount {
        get { return map.ways.FindAll((w) => { return w.IsRoad; }).Count; }
    }

    public Road(GameObject parentObj, MapReader mapReader, Material roadMaterial,
            bool generateColliders) : base(mapReader, parentObj)
    {
        roadMat = roadMaterial;
        this.generateColliders = generateColliders;
    }

    public override IEnumerable<int> Process() {
        int count = 0;

        foreach (var way in map.ways.FindAll((w) => { return w.IsRoad; }))
        {
            CreateObject(way, roadMat, way.Name, false, generateColliders);

            count++;
            yield return count;
        }
    }

    protected override void OnObjectCreated(OSMWay way, Vector3 origin, List<Vector3> vectors,
                                        List<Vector3> normals, List<Vector2> uvs,
                                        List<int> indices, GameObject goBuilding = null) {
        for (int i = 1; i < way.NodeIDs.Count; i++) {
            OSMNode p1 = map.nodes[way.NodeIDs[i - 1]];
            OSMNode p2 = map.nodes[way.NodeIDs[i]];

            Vector3 s1 = p1 - origin;
            Vector3 s2 = p2 - origin;

            Vector3 diff = (s2 - s1).normalized;

            var cross = Vector3.Cross(diff, Vector3.up) * 3.7f * way.Lanes;

            Vector3 v1 = s1 + cross;
            Vector3 v2 = s1 - cross;
            Vector3 v3 = s2 + cross;
            Vector3 v4 = s2 - cross;

            vectors.Add(v1);
            vectors.Add(v2);
            vectors.Add(v3);
            vectors.Add(v4);

            uvs.Add(new Vector2(0, 0));
            uvs.Add(new Vector2(1, 0));
            uvs.Add(new Vector2(0, 1));
            uvs.Add(new Vector2(1, 1));

            normals.Add(Vector3.up);
            normals.Add(Vector3.up);
            normals.Add(Vector3.up);
            normals.Add(Vector3.up);

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
        }
    }
    
}