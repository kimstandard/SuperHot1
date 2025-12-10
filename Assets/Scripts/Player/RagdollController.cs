using UnityEngine;

public class RagdollController : MonoBehaviour
{
    [Header("Animator for character animation")]
    public Animator animator;

    [Header("All rigidbodies to control")]
    public Rigidbody[] ragdollRigidbodies;

    [Header("Enable Ragdoll? (Toggle this in Inspector)")]
    [SerializeField]
    private bool _isRagdollEnabled = false;

    /// <summary>
    /// Public property to get/set ragdoll state.
    /// When set, it calls Enable/Disable automatically.
    /// </summary>
    public bool IsRagdollEnabled
    {
        get => _isRagdollEnabled;
        set
        {
            _isRagdollEnabled = value;
            if (_isRagdollEnabled)
            {
                EnableRagdoll();
            }
            else
            {
                DisableRagdoll();
            }
        }
    }

    private void Awake()
    {
        // Gather all child rigidbodies if not set manually
        if (ragdollRigidbodies == null || ragdollRigidbodies.Length == 0)
        {
            ragdollRigidbodies = GetComponentsInChildren<Rigidbody>();
        }

        DisableRagdoll();
    }

    private void OnValidate()
    {
        // This makes it respond to changes in the inspector
        if (_isRagdollEnabled)
            EnableRagdoll();
        else
            DisableRagdoll();
    }

    private void EnableRagdoll()
    {
        if (animator != null) animator.enabled = false;

        foreach (var rb in ragdollRigidbodies)
        {
            rb.isKinematic = false;
            rb.detectCollisions = true;
        }
    }

    private void DisableRagdoll()
    {
        if (animator != null) animator.enabled = true;

        foreach (var rb in ragdollRigidbodies)
        {
            rb.isKinematic = true;
            rb.detectCollisions = false;
        }
    }
}
