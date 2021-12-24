using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Objects/Customization Options/Player Color", fileName = "New Color")]
public class PlayerColorSO : ScriptableObject
{
    public List<Material> CarMaterials;
    public Material UIMaterial;
}
