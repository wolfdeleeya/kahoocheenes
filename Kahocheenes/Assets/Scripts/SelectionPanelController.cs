using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectionPanelController : MonoBehaviour
{
    [SerializeField] private Image playerVehicleImage;
    [SerializeField] private Image voteImage;
    [SerializeField] private Sprite voteForSprite;
    [SerializeField] private Sprite voteAgainstSprite;
    [SerializeField] private List<GameObject> vehicleList;
    [SerializeField] private List<MeshRenderer> vehicleRenderers;
    [SerializeField] private Image playerBorder;
    
    private int _currentVehicleIndex;
    private PlayerColorSO _currentColor;

    public void Activate()
    {
        playerVehicleImage.enabled = true;
        voteImage.enabled = true;
        playerBorder.enabled = true;
    }
    
    public void ChangeVehicle(bool isUp)
    {
        vehicleList[_currentVehicleIndex].SetActive(false);
        _currentVehicleIndex += isUp ? 1 : -1;
        _currentVehicleIndex %= vehicleList.Count;
        vehicleList[_currentVehicleIndex].SetActive(true);
        vehicleRenderers[_currentVehicleIndex].materials[0] = _currentColor.CarMaterials[_currentVehicleIndex];
    }

    public void ChangeMaterial(PlayerColorSO playerColor)
    {
        playerBorder.material = playerColor.UIMaterial;
        _currentColor = playerColor;
        vehicleRenderers[_currentVehicleIndex].materials[0] = _currentColor.CarMaterials[_currentVehicleIndex];
    }

    public void ChangeVote(bool isFor) => voteImage.sprite = isFor ? voteForSprite : voteAgainstSprite;
}