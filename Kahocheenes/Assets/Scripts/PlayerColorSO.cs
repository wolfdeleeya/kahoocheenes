using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Objects/Customization Options/Player Color", fileName = "New Color")]
public class PlayerColorSO : ScriptableObject
{
    [Serializable]
    public class IntList
    {
        public List<int> Indexes;
    }
    public List<Material> CarMaterials;
    public List<IntList> IndexesToSet;
    public Material UIMaterial;
}
