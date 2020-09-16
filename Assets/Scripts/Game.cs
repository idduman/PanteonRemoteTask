using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Scripting;
using UnityEngine.UI;

public class Game : MonoBehaviour
{
    public Transform scoreBoard;
    public Paint wall;
    public Slider percentageBar;
    public GameObject endScreen;

    private Player player;
    private List<Transform> opponents;
    private enum GameState { play, dead, painting, finish};
    private GameState gameState;

    private Text scoreText,percentageText;
    private float paintPercent;
    // Start is called before the first frame update
    void Start()
    {
        gameState = GameState.play;
        player = GameObject.Find("Player").GetComponentInChildren<Player>();
        wall = GameObject.Find("Wall").GetComponent<Paint>();
        var opponentsParent = GameObject.Find("Opponents");
        opponents = new List<Transform>();
        foreach (Transform t in opponentsParent.transform)
        {
            if (t.parent = opponentsParent.transform)
                opponents.Add(t);
        }
        scoreText = scoreBoard.GetComponentInChildren<Text>();
        scoreText.text = "- / -";
        percentageText = percentageBar.GetComponentInChildren<Text>();
    }

    // Update is called once per frame
    void Update()
    {
        switch (gameState)
        {
            case GameState.play:
                if (player.playerState == Player.PlayerState.Dead)
                    gameState = GameState.dead;
                if (player.playerState == Player.PlayerState.Finished)
                {
                    wall.isEnabled = true;
                    percentageBar.gameObject.SetActive(true);
                    gameState = GameState.painting;
                }
                scoreText.text = GetScore() + " / 11";
                break;
            case GameState.dead:
                player.Reset();
                percentageBar.gameObject.SetActive(false);
                SceneManager.LoadScene("Menu");
                break;

            case GameState.painting:
                paintPercent = GetPercent();
                percentageText.text = "Wall painted: " + paintPercent + "%";
                percentageBar.value = paintPercent/100f;
                if (paintPercent >= 100)
                    gameState = GameState.finish;
                break;
            case GameState.finish:
                wall.isEnabled = false;
                percentageBar.gameObject.SetActive(false);
                endScreen.SetActive(true);
                break;
        }
    }

    public void ReplayButton()
    {
        SceneManager.LoadScene("Scene");
    }

    public void MenuButton()
    {
        SceneManager.LoadScene("Menu");
    }
    private int GetScore()
    {
        int counter = 1;
        for (int i = 0; i < opponents.Count; i++)
        {
            if (opponents[i].transform.position.z > player.transform.position.z
                || opponents[i].GetComponent<Opponent>().isFinished)
                counter++;
        }
        return counter;
    }

    private int GetPercent()
    {
        return wall.counter * 100 / wall.total;
    }
}
