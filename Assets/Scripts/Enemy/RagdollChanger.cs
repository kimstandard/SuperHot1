using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RagdollChanger : MonoBehaviour
{
    public GameObject charObj;
    public GameObject ragdollObj;

    public GameObject fragmentPrefab;

    public Rigidbody spine;

    public bool isDead;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.N))
        {
            ChangeRagdoll();
        }
    }

    public void ChangeRagdoll()
    {
        ragdollObj.transform.position = charObj.transform.position;
        ragdollObj.transform.rotation = charObj.transform.rotation;

        CopyTransform(charObj.transform, ragdollObj.transform);

        charObj.SetActive(false);
        ragdollObj.SetActive(true);

        spine.AddForce(new Vector3(0f, 0f, -200f), ForceMode.Impulse);
    }

    void CopyTransform(Transform origin, Transform ragdoll)
    {
        ragdoll.localPosition = origin.localPosition;
        ragdoll.localRotation = origin.localRotation;

        foreach (Transform originChild in origin)
        {
            Transform ragdollChild = ragdoll.Find(originChild.name);
            if (ragdollChild != null)
            {
                CopyTransform(originChild, ragdollChild);
            }
        }
    }

    public void TakeDamage(Vector3 pos)
    {
        GameObject fragment = Instantiate(fragmentPrefab, pos, Quaternion.identity);
        Destroy(fragment, 3f);
        if (isDead != true)
        {
            isDead = true;
            ChangeRagdoll();
            GameManager.Instance.EnemyDied();
        }
    }
}
