using UnityEngine;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance { get; private set; }

    [System.Serializable]
    public class DialogueSet
    {
        [TextArea(3, 10)]
        public string[] sentences;
    }

    [Header("대사 내용 그룹")]
    public List<DialogueSet> dialogueSets;

    [Header("수퍼핫")]
    [TextArea(3, 10)]
    public string[] superHot;

    [Header("타이핑 효과 설정")]
    public float typingSpeed = 0.05f;

    [Header("UI 요소")]
    public TextMeshProUGUI dialogueText;
    public GameObject dialoguePanel;

    private int index = 0;
    private bool isTyping = false;
    private DialogueSet currentSet; // 현재 선택된 대사 세트

    public static bool IsDialogueActive { get; private set; } = false;

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

    void Update()
    {
        if (dialoguePanel.activeSelf && Input.GetMouseButtonDown(0))
        {            
            if (isTyping)
            {
                Debug.Log("멈춤");
                StopAllCoroutines();
                dialogueText.text = currentSet.sentences[index];
                isTyping = false;
            }
            else
            {
                NextSentence();
            }
        }

    }

    public void StartSuperHot()
    {
        Debug.Log("수퍼핫호출");
        Time.timeScale = 0.1f;
        dialoguePanel.SetActive(true);
        IsDialogueActive = true;
        StopAllCoroutines();
        StartCoroutine(LoopSuperHotDialogue());
    }

    IEnumerator LoopSuperHotDialogue()
    {
        int i = 0;
        while (true)
        {
            dialogueText.text = superHot[i];
            i = (i + 1) % superHot.Length;
            yield return new WaitForSeconds(0.1f);
        }
    }

    // 대사 시작: 특정 인덱스를 받아서 해당 대사 그룹을 재생
    public void StartDialogue(int setIndex)
    {
        if (setIndex < 0 || setIndex >= dialogueSets.Count)
        {
            Debug.LogWarning("잘못된 대사 인덱스");
            return;
        }

        Time.timeScale = 0f;
        IsDialogueActive = true;

        currentSet = dialogueSets[setIndex]; // 현재 세트 설정
        index = 0;

        dialoguePanel.SetActive(true);
        StartCoroutine(TypeSentence());
    }

    void NextSentence()
    {
        StopAllCoroutines();
        if (index < currentSet.sentences.Length - 1)
        {
            index++;
            StartCoroutine(TypeSentence());
        }
        else
        {
            EndDialogue();
        }
    }

    IEnumerator TypeSentence()
    {
        isTyping = true;
        dialogueText.text = "";

        foreach (char letter in currentSet.sentences[index].ToCharArray())
        {
            dialogueText.text += letter;
            yield return new WaitForSecondsRealtime(typingSpeed);
        }

        isTyping = false;
    }

    void EndDialogue()
    {
        dialogueText.text = "";
        dialoguePanel.SetActive(false);

        Time.timeScale = 1f;
        IsDialogueActive = false;
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
        var textObj = GameObject.Find("dialogueText");
        if (textObj != null)
        dialogueText = textObj.GetComponent<TextMeshProUGUI>();        
        dialoguePanel.SetActive(false);

        switch (scene.name)
        {
            case "Stage1":
                StartDialogue(0);
                break;
            case "End":
                StartDialogue(1);
                break;            
            // 필요한 만큼 추가하세요
            default:
                break;
        }
    }
    //--------------------------------------
}
