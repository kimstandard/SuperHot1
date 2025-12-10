using TMPro;
using UnityEngine;
using static DialogueManager;
using UnityEngine.SceneManagement;

public class TimescaleController : MonoBehaviour
{
    public static TimescaleController Instance { get; private set; }

    [Header("Time Scale Settings")]
    public float normalTimeScale = 1f;
    public float slowTimeScale = 0.3f;
    public float inputCheckDelay = 0.1f;

    [Header("Attack Button Name")]
    public string attackButton = "Fire1"; // 기본 마우스 좌클릭 / InputManager에서 변경 가능

    public GameObject dialoguePanel;

    private float lastInputTime;
    public GameManager gameManager;

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

    private void Start()
    {       
    }


    void Update()
    {
        if (dialoguePanel != null && dialoguePanel.activeSelf && gameManager.enemyAliveCount <= 0)
        {
            Time.timeScale = slowTimeScale;
            return;
        }

        // 기본 이동 입력 감지
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");

        // 공격 입력 감지
        bool attackPressed = Input.GetButton(attackButton);

        // 입력 여부
        bool hasInput = h != 0 || v != 0 || attackPressed;

        if (hasInput)
        {
            lastInputTime = Time.unscaledTime;
            Time.timeScale = normalTimeScale;
        }
        else
        {
            if (Time.unscaledTime - lastInputTime > inputCheckDelay)
            {
                Time.timeScale = slowTimeScale;
            }
        }

        // Time.fixedDeltaTime을 timeScale에 맞춰 조정
        Time.fixedDeltaTime = 0.02f * Time.timeScale;
    }

    //씬바뀔때---------------------------------------------
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
        // 씬이 로드될 때 videoPanel 다시 찾고 꺼주기
        dialoguePanel = GameObject.Find("dialoguePanel");        
    }
    //--------------------------------------
}
