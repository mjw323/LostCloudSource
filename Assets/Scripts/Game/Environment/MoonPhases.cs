//#define CATCH_DAY_ERROR
using UnityEngine;
using System.Collections;

public class MoonPhases : MonoBehaviour
{
    public void OnNightfall()
    {
        switch(day) {
        case 1:
            renderer.sharedMaterial = crescentMoon;
            break;
        case 2:
            renderer.sharedMaterial = waxMoon;
            break;
        case 3:
            renderer.sharedMaterial = fullMoon;
            break;
        default:
#if CATCH_DAY_ERROR
            Debug.LogError("Day value of " + day + ", which is outside the range [1, 3].");
            Debug.Break();
#endif
            break;
        }
        day++;
    }

    [SerializeField] private Material crescentMoon;
    [SerializeField] private Material waxMoon;
    [SerializeField] private Material fullMoon;

    private int day = 1;
}