using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class EndScreenController : MonoBehaviour
{
    [SerializeField] private List<PlacementColumnController> columns;
    [SerializeField] private TextMeshProUGUI congratsText;
    [SerializeField] private string congratsString1;
    [SerializeField] private string congratsString2;
    
    private void Start()
    {
        var scores = GameplayManager.Instance.CurrentScores;
        var sorted = scores.Select((x, i) => new KeyValuePair<int, int>(i, x)).OrderBy(x => -x.Value).ToList();
        var standings = sorted.Select((x) => x.Key).ToList();

        int numOfPlayers = scores.Count;

        int lastIndex = Mathf.Clamp(numOfPlayers-1, 0, 2);

        for (int i = 0; i <= lastIndex; ++i)
            columns[i].Spawn(standings[i]+1);
        congratsText.text = congratsString1 + (standings[0] + 1) + congratsString2;
        columns[lastIndex].Rise();
    }

    public void PlayAgain()
    {
        SceneManager.Instance.ChangeScene(SceneManager.Scene.Gameplay);
    }

    public void GoToMenu()
    {
        NetworkControllerManager.Instance.DisconnectFromServer();
        SceneManager.Instance.ChangeScene(SceneManager.Scene.MainMenu);
    }
}