using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public TileBoard board;
    public CanvasGroup gameOver;
    
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI bestScoreText;
    private int score;
    private void Start()
    {
        
        NewGame();
    }

    public void NewGame()
    {
        SetScore(0);
        bestScoreText.text = LoadBestScore().ToString();
        gameOver.alpha = 0;
        gameOver.interactable = false;
        board.ClearBoard();
        board.CreateTile();
        board.CreateTile();
        board.enabled = true;
    }

    public void GameOver()
    {
        board.enabled = false;
        gameOver.interactable = true;
        StartCoroutine(Fade(gameOver, 1f, 0.2f));
    }

    private IEnumerator Fade(CanvasGroup canvasGroup, float to, float delay)
    {
        yield return new WaitForSeconds(delay);
        float elapsed = 0f;
        float duration = 0.5f;
        float from = canvasGroup.alpha;
        while (elapsed < duration)
        {
            canvasGroup.alpha = Mathf.Lerp(from, to, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        canvasGroup.alpha = to;
        
    }

    public void IncreaseScore(int number)
    {
        SetScore(score + number);
    }
    
    private void SetScore(int score)
    {
        this.score = score;
        scoreText.text = score.ToString();
        SaveBestScore();
    }

    private void SaveBestScore()
    {
        int currentBestScore = LoadBestScore();
        if (score > currentBestScore)
        {
            PlayerPrefs.SetInt("BestScore", score);
        }
    }

    private int LoadBestScore()
    {
        return PlayerPrefs.GetInt("BestScore", 0);
    }
}
