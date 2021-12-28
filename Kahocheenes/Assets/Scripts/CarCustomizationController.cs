using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarCustomizationController : MonoBehaviour
{
    [SerializeField] private MeshRenderer carMeshRenderer;

    public void SetMaterial(Material material, List<int> indexes)
    {
        var materials = carMeshRenderer.materials;
        foreach (var index in indexes)
            materials[index] = material;
        carMeshRenderer.materials = materials;
    }
}
