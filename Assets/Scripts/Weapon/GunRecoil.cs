using UnityEngine;

public class GunRecoil : MonoBehaviour
{
    [Header("Recoil Settings")]
    public float recoilMoveAmount = 0.05f;     // 위로 살짝 올라가는 거리
    public float recoilRotateAngle = 5f;       // 위로 살짝 드는 각도
    public float recoilReturnSpeed = 5f;       // 원래로 돌아오는 속도

    private Vector3 originalPosition;
    private Quaternion originalRotation;

    private Vector3 currentOffset;
    private float currentAngle;

    Pistol pistol;

    void Start()
    {
        pistol = GetComponent<Pistol>();
        originalPosition = transform.localPosition;
        originalRotation = transform.localRotation;
    }

    void Update()
    {
        // 부드럽게 복원
        currentOffset = Vector3.Lerp(currentOffset, Vector3.zero, recoilReturnSpeed * Time.deltaTime);
        currentAngle = Mathf.Lerp(currentAngle, 0f, recoilReturnSpeed * Time.deltaTime);

        // 적용
        transform.localPosition = originalPosition + new Vector3(0, currentOffset.y, 0);
        //transform.localRotation = originalRotation * Quaternion.Euler(0, 0, currentAngle);
        transform.localRotation = originalRotation * Quaternion.Euler(-currentAngle, 0, 0);
    }

    public void ApplyRecoil()
    {
        if(pistol.isRecoiling == true) return;
        currentOffset.y += recoilMoveAmount;       // 위로 살짝 이동
        currentAngle += recoilRotateAngle;         // 위로 살짝 회전        
    }
}
