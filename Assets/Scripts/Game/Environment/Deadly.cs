using UnityEngine;

public class Deadly : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        Death death = other.GetComponent<Death>();
        if (death != null) {
            death.OnFall();
        }
    }
}