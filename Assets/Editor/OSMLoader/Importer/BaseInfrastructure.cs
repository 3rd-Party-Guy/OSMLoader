using System.Collections;
using System.Collections.Generic;
using UnityEngine;

internal abstract class BaseInfrastructure
{
    public abstract int NodeCount { get; }

    public abstract IEnumerable<int> Process();

    protected MapReader map;

    public BaseInfrastructure(MapReader mapReader) {
        map = mapReader;
    }


    protected Vector3 GetCentre(OSMWay way) {
        Vector3 total = Vector3.zero;

        foreach(ulong id in way.NodeIDs)
            total += map.nodes[id];

        return total / way.NodeIDs.Count;
    }

    protected abstract void OnObjectCreated(OSMWay way, Vector3 origin, List<Vector3> vectors, List<Vector3> normals, List<Vector2> uvs, List<int> indices);

    protected void CreateObject(OSMWay way, Material mat, string objectName) {
        objectName = string.IsNullOrEmpty(objectName) ? "OSMWay" : objectName;

        GameObject go = new GameObject(objectName);
        Vector3 localOrigin = GetCentre(way);
        go.transform.position = localOrigin - map.bounds.Centre;

        MeshFilter mf = go.AddComponent<MeshFilter>();
        MeshRenderer mr = go.AddComponent<MeshRenderer>();

        mr.material = mat;

        List<Vector3> vectors = new List<Vector3>();
        List<Vector3> normals = new List<Vector3>();
        List<Vector2> uvs = new List<Vector2>();
        List<int> indices = new List<int>();

        OnObjectCreated(way, localOrigin, vectors, normals, uvs, indices);

        mf.sharedMesh = new Mesh();
        mf.sharedMesh.vertices = vectors.ToArray();
        mf.sharedMesh.normals = normals.ToArray();
        mf.sharedMesh.triangles = indices.ToArray();
        mf.sharedMesh.uv = uvs.ToArray();
        // mf.sharedMesh.RecalculateNormals();
    }
}