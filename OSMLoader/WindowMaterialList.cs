using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindowMaterialList : MonoBehaviour
{
    [HideInInspector]
    public Material[] materials = new Material[] { };

    public Material[] GetMaterials() { return materials; }
}
