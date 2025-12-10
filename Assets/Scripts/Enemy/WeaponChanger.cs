using UnityEngine;

public class WeaponChanger : MonoBehaviour
{
    public GameObject replacementPrefab; // 교체될 오브젝트 (예: 무기나 죽은 모습 등)
    public GameObject originalObject;    // 원래 있던 오브젝트
    private RagdollChanger ragdollChanger;

    private bool changed = false;

    void Start()
    {
        ragdollChanger = GetComponent<RagdollChanger>();
    }

    void Update()
    {
        if (!changed && ragdollChanger != null && ragdollChanger.isDead)
        {
            ChangeObject();
            changed = true;
        }
    }

    void ChangeObject()
    {
        if (replacementPrefab != null)
        {
            GameObject newObj = Instantiate(
                replacementPrefab,
                originalObject.transform.position,
                originalObject.transform.rotation
            );

            newObj.transform.localScale = originalObject.transform.localScale;

            originalObject.SetActive(false); // 기존 오브젝트 숨김
        }
    }
}
