#define DEBUG_CHECK_LAYER
using UnityEngine;

public class Interactive : MonoBehaviour
{
    public delegate void Interacted();
    public event Interacted OnInteracted;

    // Allows a used interactive object to be used again.
    // NOTE: It would be more elegant to pass in a coroutine and have this
    // behavior occur after it completes, but it would also be harder to work with.
    public void Reactivate()
    {
        collider.enabled = true;
    }

    [SerializeField] private Billboard buttonPrompt;

    new private Transform transform;
    private Collider collider;

    // Used to check if the player is on-foot.
    private FootController foot;

    private void Awake()
    {
        transform = GetComponent<Transform>();
        collider = GetComponent<Collider>();
#if DEBUG_CHECK_LAYER
        if (gameObject.layer != LayerMask.NameToLayer("Interactive")) {
            Debug.LogError("Interactive object not in the \"Interactive\" layer.");
            Debug.Break();
        }
#endif
    }

    private void OnTriggerEnter(Collider other)
    {
        // Can only collide with the player, but we need to find the root
        foot = other.transform.root.GetComponentInChildren<FootController>();
        enabled = true;
    }

    private void OnTriggerExit(Collider other)
    {
        enabled = false;
        buttonPrompt.Hide();
    }

    private void Update()
    {
        // Check if we're capable of being interacted with
        // Note: Use the trigger volume to determine minimum distance
        if (!foot.enabled) {
            buttonPrompt.Hide();
            return;
        }

        // Show button prompt and react to input
        buttonPrompt.Show();
        if (Input.GetButton("Activate")) {
            if (OnInteracted != null) { OnInteracted(); }
            enabled = false;

            // Disable the collider, putting this component into a coma.
            collider.enabled = false;

            buttonPrompt.Hide();
        }
    }
}