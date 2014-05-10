using UnityEngine;

public class GameController : MonoBehaviour
{
	[SerializeField] private Hover hoverboard;
    [SerializeField] private bool beginWithGlide;
    [SerializeField] private bool beginWithGrind;
    [SerializeField] private bool beginWithWater;

    [SerializeField] private SoundMachine soundMachine;

    [SerializeField] private Respawn respawn;
    [SerializeField] private Transform firstNightRespawn;
    [SerializeField] private Transform secondNightRespawn;
    [SerializeField] private Transform thirdNightRespawn;

    [SerializeField] private TimeOfDay timeOfDay;

    [SerializeField] private Upgrade[] majorUpgrades;
    [SerializeField] private Upgrade[] minorUpgrades;
    private int numMinorUpgrades;

	[SerializeField] private UpgradeCinematic upgradeCinematic;
    [SerializeField] private SoundMachineCinematic soundMachineCinematic;

    private int day = 1;
    private int minorUpgradesFound = 0;

    private void OnMajorUpgradeCollected(Upgrade which)
    {
        switch (day) {
        case 1:
            OnDayOneComplete();
            break;
        case 2:
            OnDayTwoComplete();
            break;
        case 3:
            OnDayThreeComplete();
            break;
        }

        upgradeCinematic.Play();
        respawn.OnNightfall();
        soundMachine.Reactivate();
    }

    private void OnMinorUpgradeCollected(Upgrade which)
    {
        minorUpgradesFound++;
        hoverboard.OnGetMinorUpgrade(minorUpgradesFound / numMinorUpgrades);
    }

    private void OnSoundMachineUsed()
    {
        switch (day) {
        case 1:
            OnNightOneComplete();
            break;
        case 2:
            OnNightTwoComplete();
            break;
        case 3:
            OnNightThreeComplete();
            break;
        }

		soundMachineCinematic.Play();
        respawn.OnSunrise();
        
        day++;
    }

    // Day-specific logic:

    private void OnDayOneComplete()
    {
        respawn.SpawnLocation = firstNightRespawn.position;
    }

    private void OnNightOneComplete()
    {
        hoverboard.OnGetGlideUpgrade();
    }
    
    private void OnDayTwoComplete()
    {
        respawn.SpawnLocation = secondNightRespawn.position;
    }

    private void OnNightTwoComplete()
    {
        hoverboard.OnGetGrindUpgrade();
    }
    
    private void OnDayThreeComplete()
    {
        respawn.SpawnLocation = thirdNightRespawn.position;
    }

    private void OnNightThreeComplete()
    {
        hoverboard.OnGetWaterUpgrade();
    }
    
    private void Start()
    {
        // Subscribe to upgrade events
        for (int i = 0; i < majorUpgrades.Length; i++) {
            majorUpgrades[i].OnCollected += OnMajorUpgradeCollected;
        }
        numMinorUpgrades = minorUpgrades.Length;
        for (int j = 0; j < numMinorUpgrades; j++) {
            minorUpgrades[j].OnCollected += OnMinorUpgradeCollected;
        }

        // Subscribe to sound machine event
        soundMachine.OnUsed += OnSoundMachineUsed;

        // Auto-upgrade the board if we're cheating
        if (beginWithGlide) {
            hoverboard.OnGetGlideUpgrade();
        }
        if (beginWithGrind) {
            hoverboard.OnGetGrindUpgrade();
        }
        if (beginWithWater) {
            hoverboard.OnGetWaterUpgrade();
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

        // Unsubscribe to sound machine event
        soundMachine.OnUsed -= OnSoundMachineUsed;
    }
}