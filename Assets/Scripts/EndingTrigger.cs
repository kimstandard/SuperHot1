using UnityEngine;
using UnityEngine.Video;
using UnityEngine.SceneManagement;

public class EndingTrigger : MonoBehaviour
{
    public VideoPlayer videoPlayer;      // 영상 재생기
    public GameObject videoScreen;       // 영상 화면 (예: RawImage, Mesh 등)
    public GameObject bgrdImg;

    private bool videoStarted = false;

    private void Start()
    {
        // 시작 시 영상 화면은 꺼져 있음
        if (videoScreen != null)
            videoScreen.SetActive(false);

        // 영상이 끝났을 때 호출될 함수 등록
        if (videoPlayer != null)
            videoPlayer.loopPointReached += OnVideoFinished;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (videoStarted) return;

        // 플레이어가 부딪힌 경우 (태그로 확인)
        if (other.CompareTag("Bullet"))
        {
            videoStarted = true;

            if (videoScreen != null)
                videoScreen.SetActive(true);
                bgrdImg.SetActive(true);

            if (videoPlayer != null)
            {
                videoPlayer.Play();
                
            }
        }
    }

    void OnVideoFinished(VideoPlayer vp)
    {

        // 씬 전환
        SceneManager.LoadScene("Lobby");
    }
}
