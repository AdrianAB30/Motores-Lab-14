using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [SerializeField] private TMP_Text scoreText;
    [SerializeField] private RectTransform panelScore;
    [SerializeField] private RectTransform panelWin;
    private int totalCoins = 0;
    [SerializeField] private float timeEase;
    [SerializeField] private Ease myEase;

    private void Start()
    {
        panelScore.DOAnchorPos(new Vector2(0f, 16), timeEase).SetEase(myEase);
    }
    private void OnEnable()
    {
        CoinControl.OnCoinCollected += AddCoin; 
    }

    private void OnDisable()
    {
        CoinControl.OnCoinCollected -= AddCoin; 
    }
    private void AddCoin(int coinValue)
    {
        totalCoins += coinValue; 
        UpdateScoreText();
        PanelWin();
    }
    private void UpdateScoreText()
    {
        scoreText.text = "Coins: " + totalCoins.ToString();
    }
    private void PanelWin()
    {
        if (totalCoins >= 5)
        {
            panelWin.DOAnchorPos(new Vector2(0f, 0f), timeEase).SetEase(myEase);
            StartCoroutine(WinStopped());
        }
    }
    public void RestartGame()
    {
        SceneManager.LoadScene("Game");
        Time.timeScale = 1f;
    }
    IEnumerator WinStopped()
    {
        yield return new WaitForSeconds(1);
        Time.timeScale = 0f;
    }
}
