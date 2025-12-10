using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;
using static DialogueManager;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Video Settings")]
    public GameObject videoPanel;
    public VideoPlayer videoPlayer;

    [Header("Scene Settings")]
    public string nextSceneName;

    //private bool isPlayingVideo = false;
    public int enemyAliveCount = 0;
    public bool clear;    

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }


    void Start()
    {        
        EnemyCount();

        videoPanel.SetActive(false);
        videoPlayer.loopPointReached += OnVideoFinished;
    }

    private void Update()
    {
        if (clear == true && Input.GetMouseButtonDown(0)) { PlayDeathVideo(); }
        
    }

    public void EnemyCount()
    {
        enemyAliveCount = GameObject.FindGameObjectsWithTag("Enemy").Length;
    }

    public void OnPlayerDied()
    {
        PlayDeathVideo();
    }

    void PlayDeathVideo()
    {
        //isPlayingVideo = true;
        videoPanel.SetActive(true);
        videoPlayer.Play();
    }

    void OnVideoFinished(VideoPlayer vp)
    {
        if (!clear)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
        else
        {
            clear = false;
            videoPanel.SetActive(false);
            SceneManager.LoadScene(nextSceneName);
        }
    }

    // ÀûÀÌ Á×À» ¶§ È£Ãâ
    public void EnemyDied()
    {
        enemyAliveCount--;
        if (enemyAliveCount <= 0)
        {            
            DialogueManager.Instance.StartSuperHot();
            Debug.Log("Àû»ç¸Á");
            clear = true;
        }
    }

    //¾À¹Ù²ð¶§---------------------------------------------
    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // ¾ÀÀÌ ·ÎµåµÉ ¶§ videoPanel ´Ù½Ã Ã£°í ²¨ÁÖ±â
        videoPanel = GameObject.Find("videoPanel");                    
        videoPlayer = videoPanel.GetComponent<VideoPlayer>();
        videoPanel.SetActive(false);
    }
    //--------------------------------------
}
