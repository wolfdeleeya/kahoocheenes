using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct PlayerScoreData
{
    public int PlayerID;
    public int PlayerScore;
}

public class GameplayManager : MonoBehaviour
{
    [SerializeField] private int maxScore;
    private List<PlayerScoreData> _scores;
    
    public int MaxScore => maxScore;
}
