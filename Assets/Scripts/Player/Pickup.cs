using UnityEngine;
using System.Collections;

public class Pickup : MonoBehaviour
{
    [Header("조")]
    public Camera playerCamera;
    public PlayerController player;

    [Header("설정")]
    public float pickupRange = 5f;
    public float flySpeed = 5f;         // 무기 날아오는 속도
    public Transform weaponHoldPoint;   // 무기가 도착할 위치 (예: 플레이어 손)

    [Header("무기")]
    public GameObject[] weapons; // 여러 무기 오브젝트

    private void Update()
    {
        if (Input.GetButtonDown("Fire2"))
        {
            TryPickupWeapon();
        }
    }

    /// <summary>
    /// 카메라 앞으로 레이 쏴서 무기 오브젝트 태그를 기준으로 활성화
    /// </summary>
    public void TryPickupWeapon()
    {
        if (playerCamera == null || player == null)
        {
            Debug.LogWarning("[Pickup] 필수 참조가 없습니다!");
            return;
        }

        Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, pickupRange))
        {
            string hitTag = hit.collider.tag;
            Debug.Log($"[Pickup] 맞은 오브젝트 태그: {hitTag}");

            GameObject matchedWeapon = null;

            // 모든 무기 비활성화
            foreach (GameObject weaponObj in weapons)
            {
                if (weaponObj == null) continue;
                weaponObj.SetActive(false);
            }

            // 태그가 일치하는 무기 찾기
            foreach (GameObject weaponObj in weapons)
            {
                if (weaponObj == null) continue;

                if (weaponObj.tag == hitTag)
                {
                    matchedWeapon = weaponObj;
                    break;
                }
            }

            if (matchedWeapon != null)
            {
                // 무기를 플레이어에게 Lerp로 날아오게 함
                StartCoroutine(MoveWeaponToPlayer(matchedWeapon, hit.collider.gameObject));
            }
            else
            {
                Debug.Log($"[Pickup] weapon 배열에 태그가 '{hitTag}'인 무기가 없습니다.");
            }
        }
        else
        {
            Debug.Log("[Pickup] Ray가 아무것도 맞추지 않았습니다.");
        }
    }

    /// <summary>
    /// 무기를 천천히 플레이어 쪽으로 이동시키는 코루틴
    /// </summary>
    private IEnumerator MoveWeaponToPlayer(GameObject weaponObj, GameObject sourceObj)
    {
        sourceObj.GetComponent<Collider>().enabled = false; // 충돌 방지
        Rigidbody rb = sourceObj.GetComponent<Rigidbody>();
        if (rb != null) rb.isKinematic = true;

        Vector3 startPos = sourceObj.transform.position;
        float elapsed = 0f;

        while (elapsed < 1f)
        {
            elapsed += Time.deltaTime * flySpeed;
            sourceObj.transform.position = Vector3.Lerp(startPos, weaponHoldPoint.position, elapsed);
            yield return null;
        }

        sourceObj.transform.position = weaponHoldPoint.position;

        // 장착할 무기 활성화
        weaponObj.SetActive(true);

        switch (weaponObj.tag)
        {
            case "Pistol":
                player.equippedWeapon = weaponObj.GetComponentInParent<Pistol>();
                player.gunRecoil = weaponObj.GetComponentInParent<GunRecoil>();
                Debug.Log("[Pickup] Pistol 장착 완료");
                break;

            case "Shotgun":
                player.equippedWeapon = weaponObj.GetComponentInParent<Shotgun>();
                player.gunRecoil = weaponObj.GetComponentInParent<GunRecoil>();
                Debug.Log("[Pickup] Shotgun 장착 완료");
                break;

            case "Bat":
                player.equippedWeapon = weaponObj.GetComponentInParent<Bat>();
                player.gunRecoil = null;
                Debug.Log("[Pickup] Bat 장착 완료");
                break;

            default:
                Debug.LogWarning($"[Pickup] 알 수 없는 무기 태그 처리: {weaponObj.tag}");
                break;
        }

        // 원래 오브젝트 제거
        Destroy(sourceObj);
    }
}
