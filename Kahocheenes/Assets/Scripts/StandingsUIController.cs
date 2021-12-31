using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StandingsUIController : MonoBehaviour
{
    [SerializeField] private List<Vector2> standingsPositions;
    [SerializeField] private List<StandingDisplayProperties> displayProperties;
    [SerializeField] private float animationDuration;
    [SerializeField] private float pulledXAmount;
    [SerializeField] private AnimationCurve xCurve;
    [SerializeField] private AnimationCurve yCurve;

    private int _maxScore;
    private List<int> _currentStandings;
    private List<Coroutine> _coroutines;

    private void Start()
    {
        _maxScore = GameplayManager.Instance.MaxScore;
        var clientManager = ClientManager.Instance;
        var playerSelection = PlayerSelectionManager.Instance;

        int numOfPlayers = clientManager.NumOfPlayers;
        _currentStandings = new List<int>();
        _coroutines = new List<Coroutine>(new Coroutine[numOfPlayers]);

        for (int i = 0; i < numOfPlayers; ++i)
        {
            int playerID = i + 1;
            displayProperties[i].DisplayImage.material = playerSelection.GetPlayerColor(playerID).UIMaterial;
            displayProperties[i].NameText.text = "PLAYER " + playerID;
            displayProperties[i].ScoreText.text = "0 / " + _maxScore;
            _currentStandings.Add(i);
        }

        for (int i = numOfPlayers; i < 4; ++i) //Disable unused panels
            displayProperties[i].Transform.gameObject.SetActive(false);
    }

    public void ScoresChanged(List<int> scores)
    {
        var standings = CalculateStandings(scores);

        int numOfPlayers = standings.Count;

        var oldPositions = new List<Vector2>(new Vector2[numOfPlayers]);
        var newPositions = new List<Vector2>(new Vector2[numOfPlayers]);

        for (int i = 0; i < numOfPlayers; ++i)
        {
            int currentStandingsIndex = _currentStandings[i];
            int nextStandingsIndex = standings[i];
            oldPositions[currentStandingsIndex] = standingsPositions[i];
            newPositions[nextStandingsIndex] = standingsPositions[i];
            displayProperties[i].ScoreText.text = scores[i] + " / " + _maxScore;
        }

        for (int i = 0; i < numOfPlayers; ++i)
        {
            if ((oldPositions[i] - newPositions[i]).magnitude > 1e-4)
            {
                if (_coroutines[i] != null)
                    StopCoroutine(_coroutines[i]);
                _coroutines[i] =
                    StartCoroutine(MoveStandingTabCRT(displayProperties[i], oldPositions[i], newPositions[i]));
            }
        }

        _currentStandings = standings;
    }

    private IEnumerator MoveStandingTabCRT(StandingDisplayProperties tab, Vector2 oldPos, Vector2 newPos)
    {
        float startY = oldPos.y;
        float endY = newPos.y;

        float minX = oldPos.x;
        float maxX = minX + pulledXAmount;

        float t = 0;

        while (t < animationDuration)
        {
            yield return null;
            t += Time.deltaTime;
            float val = t / animationDuration;
            float x = Mathf.Lerp(minX, maxX, xCurve.Evaluate(val));
            float y = Mathf.Lerp(startY, endY, yCurve.Evaluate(val));

            tab.Transform.anchoredPosition = new Vector3(x, y);
        }

        tab.Transform.anchoredPosition = new Vector3(minX, endY);
    }

    private static List<int> CalculateStandings(List<int> scores)
    {
        var sorted = scores.Select((x, i) => new KeyValuePair<int, int>(i, x)).OrderBy(x => -x.Value).ToList();

        return sorted.Select(x => x.Key).ToList();
    }

    [Serializable]
    public class StandingDisplayProperties
    {
        public RectTransform Transform;
        public Image DisplayImage;
        public TextMeshProUGUI NameText;
        public TextMeshProUGUI ScoreText;
    }
}