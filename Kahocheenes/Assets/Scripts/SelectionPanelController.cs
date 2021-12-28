using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectionPanelController : MonoBehaviour
{
    [SerializeField] private RawImage playerVehicleImage;
    [SerializeField] private Image voteImage;
    [SerializeField] private Sprite voteForSprite;
    [SerializeField] private Sprite voteAgainstSprite;
    [SerializeField] private List<GameObject> vehicleList;
    [SerializeField] private List<MeshRenderer> vehicleRenderers;
    [SerializeField] private Image playerBorder;
    
    private int _currentVehicleIndex;
    private PlayerColorSO _currentColor;

    private void Start()
    {
        Deactivate();
    }

    public void Deactivate()
    {
        playerVehicleImage.enabled = false;
        voteImage.enabled = false;
        playerBorder.enabled = false;
        foreach (var vehicle in vehicleList)
            vehicle.SetActive(true);
        _currentVehicleIndex = 0;
    }

    public void Activate()
    {
        Debug.Log(gameObject);
        playerVehicleImage.enabled = true;
        voteImage.enabled = true;
        playerBorder.enabled = true;
    }
    
    public void ChangeVehicle(int carIndex)
    {
        vehicleList[_currentVehicleIndex].SetActive(false);
        _currentVehicleIndex = carIndex;
        _currentVehicleIndex %= vehicleList.Count;
        vehicleList[_currentVehicleIndex].SetActive(true);
        ChangeMaterial(_currentColor);
    }

    public void ChangeMaterial(PlayerColorSO playerColor)
    {
        Debug.Log(gameObject);
        playerBorder.material = playerColor.UIMaterial;
        _currentColor = playerColor;
        var materials = vehicleRenderers[_currentVehicleIndex].materials;
        foreach (var index in playerColor.IndexesToSet[_currentVehicleIndex].Indexes)
            materials[index] = _currentColor.CarMaterials[_currentVehicleIndex];
        vehicleRenderers[_currentVehicleIndex].materials = materials;
    }

    public void ChangeVote(bool isFor) => voteImage.sprite = isFor ? voteForSprite : voteAgainstSprite;
}