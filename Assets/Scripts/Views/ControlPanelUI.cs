using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.XR.Interaction.Toolkit.UI;
using TMPro;
using System;
public class ControlPanelUI : MonoBehaviour
{
    [Header("Références système")]
    public TimeController timeController;
    public ScaleController scaleController;
    public OrbitRenderer[] orbits;
    public Transform xrOrigin;

    void Awake()
    {
        var canvas = GetComponent<Canvas>();
        if (canvas != null && GetComponent<TrackedDeviceGraphicRaycaster>() == null)
        {
            var existing = GetComponent<GraphicRaycaster>();
            if (existing != null) Destroy(existing);
            gameObject.AddComponent<TrackedDeviceGraphicRaycaster>();
            Debug.Log("[XR] TrackedDeviceGraphicRaycaster ajouté sur ControlPanel");
        }
    }

    [Header("Boutons")]
    public Button playPauseButton;
    public Button slowerButton;
    public Button fasterButton;
    public Button toggleOrbitsButton;
    public Button resetScaleButton;
    public Button resetViewButton;

    [Header("Labels")]
    public TextMeshProUGUI dateLabel;
    public TextMeshProUGUI speedLabel;
    public TextMeshProUGUI playPauseLabel;

    // Vitesses disponibles en jours simulés par seconde réelle
    static readonly float[] Speeds = { 0.01f, 0.1f, 1f, 10f, 100f, 365f, 3650f };
    int speedIndex = 2; // démarre à 1 j/s

    TimeModel timeModel;
    bool orbitsVisible = true;
    Vector3 originResetPosition;
    Quaternion originResetRotation;
    Vector3 scaleResetValue;

    void Start()
    {
        Debug.Log("[INPUT] ControlPanelUI.Start() — GameObject actif, Init appelé : " + (timeModel != null));
        if (timeModel == null)
            Debug.LogWarning("[INPUT] ControlPanelUI : Init() n'a pas encore été appelé par AppBootstrapper. Vérifier que le champ 'Control Panel' est assigné.");
    }

    public void Init(TimeModel model)
    {
        Debug.Log("[INPUT] ControlPanelUI.Init() appelé");
        timeModel = model;
        timeModel.OnTimeChanged += OnTimeChanged;

        if (xrOrigin != null)
        {
            originResetPosition = xrOrigin.position;
            originResetRotation = xrOrigin.rotation;
        }

        if (scaleController != null)
            scaleResetValue = scaleController.transform.localScale;
        else
            Debug.LogWarning("[INPUT] ControlPanelUI : scaleController non assigné");

        if (timeController == null)
            Debug.LogWarning("[INPUT] ControlPanelUI : timeController non assigné");

        ApplySpeed();
        RefreshPlayPauseLabel();

        if (playPauseButton    == null) Debug.LogWarning("[INPUT] playPauseButton non assigné");
        if (slowerButton       == null) Debug.LogWarning("[INPUT] slowerButton non assigné");
        if (fasterButton       == null) Debug.LogWarning("[INPUT] fasterButton non assigné");

        playPauseButton    ?.onClick.AddListener(OnPlayPause);
        slowerButton       ?.onClick.AddListener(OnSlower);
        fasterButton       ?.onClick.AddListener(OnFaster);
        toggleOrbitsButton ?.onClick.AddListener(OnToggleOrbits);
        resetScaleButton   ?.onClick.AddListener(OnResetScale);
        resetViewButton    ?.onClick.AddListener(OnResetView);

        Debug.Log("[INPUT] ControlPanelUI : listeners enregistrés sur les boutons");
    }

    void OnDestroy()
    {
        if (timeModel != null)
            timeModel.OnTimeChanged -= OnTimeChanged;
    }

    void OnPlayPause()
    {
        if (timeModel.IsPlaying)
        {
            timeModel.Pause();
            Debug.Log("[INPUT] Play/Pause → Pause");
        }
        else
        {
            timeModel.Play();
            Debug.Log("[INPUT] Play/Pause → Play");
        }
        RefreshPlayPauseLabel();
    }

    void OnSlower()
    {
        speedIndex = Mathf.Max(0, speedIndex - 1);
        ApplySpeed();
        Debug.Log($"[INPUT] Vitesse réduite → {Speeds[speedIndex]} j/s");
    }

    void OnFaster()
    {
        speedIndex = Mathf.Min(Speeds.Length - 1, speedIndex + 1);
        ApplySpeed();
        Debug.Log($"[INPUT] Vitesse augmentée → {Speeds[speedIndex]} j/s");
    }

    void OnToggleOrbits()
    {
        orbitsVisible = !orbitsVisible;
        foreach (var orbit in orbits)
        {
            if (orbit == null) continue;
            var lr = orbit.GetComponent<UnityEngine.LineRenderer>();
            if (lr != null) lr.enabled = orbitsVisible;
        }
        Debug.Log($"[INPUT] Orbites {(orbitsVisible ? "affichées" : "masquées")}");
    }

    void OnResetScale()
    {
        if (scaleController != null)
        {
            scaleController.transform.localScale = scaleResetValue;
            Debug.Log($"[INPUT] Scale réinitialisée → {scaleResetValue.x:F3}");
        }
    }

    void OnResetView()
    {
        if (xrOrigin != null)
        {
            xrOrigin.SetPositionAndRotation(originResetPosition, originResetRotation);
            Debug.Log("[INPUT] Viewpoint réinitialisé");
        }
    }

    void ApplySpeed()
    {
        if (timeController != null)
            timeController.secondsPerDay = Speeds[speedIndex];

        string label = Speeds[speedIndex] >= 365f
            ? $"×{Speeds[speedIndex] / 365f:F0} an/s"
            : $"×{Speeds[speedIndex]:F2} j/s";

        if (speedLabel != null)
            speedLabel.text = $"Vitesse : {label}";
    }

    void RefreshPlayPauseLabel()
    {
        if (playPauseLabel != null)
            playPauseLabel.text = (timeModel != null && timeModel.IsPlaying) ? "⏸" : "▶";
    }

    void OnTimeChanged(System.DateTime time)
    {
        if (dateLabel != null)
            dateLabel.text = time.ToString("dd MMM yyyy");
    }
}
