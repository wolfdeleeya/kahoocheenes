using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Objects/Car Prefabs Data", fileName = "New Car")]
public class CarPrefabData : ScriptableObject
{
    public GameObject MainMenuPrefab;
    public GameObject GameplayPrefab;
}
