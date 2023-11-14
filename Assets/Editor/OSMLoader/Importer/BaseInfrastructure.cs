using System.Collections;
using System.Collections.Generic;
using UnityEngine;

internal abstract class BaseInfrastructure
{
    private readonly GameObject parentObj;
    protected MapReader map;
    public abstract int NodeCount { get; }

    public BaseInfrastructure(MapReader mapReader, GameObject parentObj) {
        map = mapReader;
        this.parentObj = parentObj;
    }
  
    public abstract IEnumerable<int> Process();

    protected Vector3 GetCentre(OSMWay way) {
        Vector3 total = Vector3.zero;

        foreach(ulong id in way.NodeIDs)
            total += (Vector3)map.nodes[id];

        return total / way.NodeIDs.Count;
    }

    protected void CreateObject(OSMWay way, Material mat, string objectName,
                            bool importColor, bool generateColliders) {
        objectName = string.IsNullOrEmpty(objectName) ? "OSMWay" : objectName;

        GameObject go = new(objectName);
        go.transform.parent = parentObj.transform;

        Vector3 localOrigin = GetCentre(way);
        go.transform.position = localOrigin - map.bounds.Centre;

        MeshFilter mf = go.AddComponent<MeshFilter>();
        MeshRenderer mr = go.AddComponent<MeshRenderer>();

        mr.material = mat;
        if (!string.IsNullOrEmpty(way.Color) && importColor)
        {
            Color newCol;
            if (!ColorUtility.TryParseHtmlString(way.Color, out newCol))
                Debug.LogWarning("Failed to parse building color into RGB");
            else
                mr.material.color = newCol;
        }

        List<Vector3> vectors = new();
        List<Vector3> normals = new();
        List<Vector2> uvs = new();
        List<int> indices = new();

        OnObjectCreated(way, localOrigin, vectors, normals, uvs, indices, go);

        var mesh = new Mesh
        {
            vertices = vectors.ToArray(),
            normals = normals.ToArray(),
            triangles = indices.ToArray(),
            uv = uvs.ToArray()
        };
        mesh.Optimize();
        mf.sharedMesh = mesh;

        if (generateColliders)
        {
            var col = go.AddComponent<MeshCollider>();
            col.convex = true;
        }
    }

    protected abstract void OnObjectCreated(OSMWay way, Vector3 origin,
                                            List<Vector3> vectors, List<Vector3> normals,
                                            List<Vector2> uvs, List<int> indices,
                                            GameObject goBuilding = null);
}