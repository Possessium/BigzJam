using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class BJ_GameManager : MonoBehaviour
{
    public static BJ_GameManager I { get; private set; }

    [SerializeField] TMP_Text scoreText;
    [SerializeField] TMP_Text timeText;
    [SerializeField] BJ_Player player;
    [SerializeField] GameObject transition;
    [SerializeField] GameObject pauseUI;

    [SerializeField] AudioClip introLoop;
    [SerializeField] AudioClip gameMusic;
    [SerializeField] AudioClip endMusic;
    [SerializeField] AudioSource audioSource;

    #region EndUI
    [SerializeField] GameObject deathUI;
    [SerializeField] TMP_Text highScore;
    [SerializeField] GameObject newHighscore;
    #endregion

    int score = 0;
    float time = 0;
    bool playTimer = false;
    bool gameStarted = false;
    bool roomReady = false;

    private void Awake()
    {
        I = this;
    }

    private void Update()
    {
        if(playTimer)
        {
            time += Time.deltaTime;
            timeText.text = time.ToString("000");
        }
        if (!audioSource.isPlaying)
        {
            audioSource.clip = introLoop;
            audioSource.Play();
            audioSource.loop = true;
        }
    }


    public void AddScore(int _amount)
    {
        score += _amount;

        if (score > 999999)
            score = 999999;

        scoreText.text = score.ToString("000000");
    }

    public void RoomReady()
    {
        roomReady = true;

        if(gameStarted)
            StartFloor();
    }

    public void StartFloor()
    {
        if (!roomReady)
            return;

        if(!gameStarted)
        {
            audioSource.clip = gameMusic;
            audioSource.Play();
            audioSource.loop = true;
        }

        gameStarted = true;
        BJ_Enemy[] _enemies = FindObjectsOfType<BJ_Enemy>();
        foreach (BJ_Enemy _en in _enemies)
        {
            _en.SetMove(true);
        }
        pauseUI.SetActive(false);
        if(transition)
            transition.SetActive(false);
        time = 0;
        playTimer = true;
        player.SetMove(true);
    }

    public void EndGame()
    {
        playTimer = false;
        player.SetMove(false);

        if(PlayerPrefs.HasKey("Highscore"))
        {
            int _hs = PlayerPrefs.GetInt("Highscore");
            if (_hs < score)
            {
                PlayerPrefs.SetInt("Highscore", score);
                _hs = score;
                newHighscore.SetActive(true);
            }

            highScore.text = _hs.ToString("000000");
        }

        else
        {
            PlayerPrefs.SetInt("Highscore", score);
            newHighscore.SetActive(true);
            highScore.text = score.ToString("000000");
        }
        audioSource.clip = endMusic;
        audioSource.Play();
        deathUI.SetActive(true);
    }

    public void NextFloor()
    {
        if(transition)
            transition.SetActive(true);
        FindObjectOfType<BJ_Player>().transform.position = new Vector3(0, 75, 0);
        playTimer = false;
        player.SetMove(false);
        DungeonGenerator.I.CreateDungeon();

        int _timeScore = 10000 - (int)time;
        if (_timeScore < 500)
            _timeScore = 500;

        AddScore(_timeScore);

    }

    public void Pause(UnityEngine.InputSystem.InputAction.CallbackContext _ctx)
    {
        if(gameStarted)
            Pause(playTimer);
    }

    public void Pause(bool _s)
    {
        pauseUI.SetActive(_s);

        playTimer = !_s;
        player.SetMove(!_s);
    }


    public void Quit() => Application.Quit();

    public void Restart()
    {
        audioSource.clip = gameMusic;
        audioSource.Play();
        audioSource.loop = true;
        deathUI.SetActive(false);
        score = 0;
        time = 0;
        player.Respawn();
        AddScore(-10000);
        NextFloor();
    }

    private void OnApplicationQuit()
    {
        if (PlayerPrefs.HasKey("Highscore"))
        {
            int _hs = PlayerPrefs.GetInt("Highscore");
            if (_hs < score)
                PlayerPrefs.SetInt("Highscore", score);
        }
    }

}
