using UnityEngine;
using System;

// S'exécute avant tous les autres MonoBehaviours (Start en premier)
[DefaultExecutionOrder(-100)]
public class AppBootstrapper : MonoBehaviour
{
    public SolarSystemConfig config;

    public Transform solarSystemRoot;
    public SunView sun;
    public PlanetView[] planets;
    public OrbitRenderer[] orbits;
    public FocusController focusController;
    public ControlPanelUI controlPanel;
    public DebugOverlay debugOverlay;
    TimeModel timeModel;
    TimeController timeController;
    PlanetSystemController controller;

    void Start()
    {
        Debug.Log("[BOOT] Initializing application");

        float planetSizeScale = config != null ? config.planetSizeScale : 1f;

        if (sun != null)
            sun.Init(planetSizeScale);
        else
            Debug.LogWarning("[BOOT] Aucun SunView assigné.");

        timeModel = new TimeModel();

        if (focusController != null)
            focusController.Init(timeModel);

        timeController = gameObject.AddComponent<TimeController>();

        var ephemeris = new PlanetEphemerisService();
        var orbitStartDate = DateTime.Now;

        float distanceScale = config != null ? config.distanceScale : 1f;

        controller = new PlanetSystemController(
            timeModel,
            ephemeris,
            planets,
            distanceScale,
            planetSizeScale
        );

        timeController.Init(timeModel);

        if (controlPanel != null)
        {
            Debug.Log("[BOOT] controlPanel trouvé : " + controlPanel.gameObject.name + " — actif : " + controlPanel.gameObject.activeInHierarchy);
            controlPanel.timeController = timeController;
            if (controlPanel.orbits == null || controlPanel.orbits.Length == 0)
                controlPanel.orbits = orbits;
            controlPanel.Init(timeModel);
        }
        else
            Debug.LogWarning("[BOOT] controlPanel non assigné dans AppBootstrapper — les boutons seront inactifs");

        if (debugOverlay != null)
            debugOverlay.Init(timeModel, timeController);

        bool showOrbits = config != null ? config.showOrbits : true;

        foreach (var orbit in orbits)
        {
            if (orbit == null)
            {
                Debug.LogWarning("[BOOT] Un OrbitRenderer est null dans le tableau orbits.");
                continue;
            }
            orbit.DrawOrbit(orbit.planet, ephemeris, orbitStartDate, distanceScale: distanceScale, show: showOrbits);
        }
    }
}
