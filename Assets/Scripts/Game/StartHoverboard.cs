#define DEBUG_CHECK_LAYER
using UnityEngine;

public class StartHoverboard : MonoBehaviour
{
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
		enabled = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        // Can only collide with the player, but we need to find the root
        foot = other.transform.root.GetComponentInChildren<FootController>();   
		buttonPrompt.Show();
    }

    private void OnTriggerExit(Collider other)
    {
        buttonPrompt.Hide();
    }

    private void Update()
    {
        // Show button prompt and react to input
        
        if (Input.GetButton("Fire3")) {
            this.gameObject.SetActive(false);
			buttonPrompt.Hide();
        }
    }
}