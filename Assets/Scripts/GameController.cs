using UnityEngine;

public class GameController : MonoBehaviour
{
    [SerializeField] private Respawn respawn;
    [SerializeField] private Transform firstNightRespawn;
    [SerializeField] private Transform secondNightRespawn;
    [SerializeField] private Transform thirdNightRespawn;

    [SerializeField] private Upgrade[] majorUpgrades;
    [SerializeField] private Upgrade[] minorUpgrades;

    private int day = 1;

    private void OnMajorUpgradeCollected(Upgrade which)
    {
        // Set spawn location depending upon which day it is
        switch (day) {
        case 1:
            respawn.SpawnLocation = firstNightRespawn.position;
            break;
        case 2:
            respawn.SpawnLocation = secondNightRespawn.position;
            break;
        case 3:
            respawn.SpawnLocation = thirdNightRespawn.position;
            break;
        }
        // Transition to night, and notify all systems whose behavior depends upon the time of day
        // timeOfDay.GotoNight();
        respawn.OnNightfall();
    }

    private void OnMinorUpgradeCollected(Upgrade which)
    {
        // increase hoverboard speed?
    }

    private void OnSoundMachineUsed()
    {
        respawn.OnSunrise();
    }

    private void Start()
    {
        // Subscribe to upgrade events
        for (int i = 0; i < majorUpgrades.Length; i++) {
            majorUpgrades[i].OnCollected += OnMajorUpgradeCollected;
        }
        for (int j = 0; j < minorUpgrades.Length; j++) {
            minorUpgrades[j].OnCollected += OnMinorUpgradeCollected;
        }
    }

    private void OnApplicationQuit()
    {
        // Unsubscribe to upgrade events
        for (int i = 0; i < majorUpgrades.Length; i++) {
            majorUpgrades[i].OnCollected -= OnMajorUpgradeCollected;
        }
        for (int j = 0; j < minorUpgrades.Length; j++) {
            minorUpgrades[j].OnCollected -= OnMinorUpgradeCollected;
        }
    }
}