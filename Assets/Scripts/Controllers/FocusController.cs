using UnityEngine;
using System;

public class FocusController : MonoBehaviour
{
    [Header("Références")]
    public PlanetInfoPanel infoPanel;

    [Header("Focus")]
    [Tooltip("Multiplicateur de scale appliqué à la planète sélectionnée")]
    public float focusScaleMultiplier = 3f;

    TimeModel timeModel;

    PlanetSelectable currentFocus;
    Vector3 originalScale;

    public void Init(TimeModel model)
    {
        timeModel = model;
    }

    void OnEnable()
    {
        PlanetSelectable.OnPlanetSelected += HandlePlanetSelected;
    }

    void OnDisable()
    {
        PlanetSelectable.OnPlanetSelected -= HandlePlanetSelected;
    }

    void HandlePlanetSelected(PlanetSelectable selected)
    {
        if (currentFocus == selected)
        {
            Unfocus();
            return;
        }

        // Restaure l'ancienne planète si besoin
        if (currentFocus != null)
            Unfocus();

        // Focus sur la nouvelle
        currentFocus = selected;
        originalScale = selected.transform.localScale;
        selected.transform.localScale = originalScale * focusScaleMultiplier;

        if (infoPanel != null)
        {
            // Positionne le panneau au-dessus de la planète
            infoPanel.transform.position = selected.transform.position + selected.transform.parent.up * (selected.transform.localScale.y * 1.5f);

            DateTime time = timeModel != null ? timeModel.CurrentTime : DateTime.Now;
            infoPanel.Show(selected.PlanetView.planet, time);
        }

        Debug.Log($"[FOCUS] Focused on {selected.gameObject.name}");
    }

    void Unfocus()
    {
        if (currentFocus == null) return;

        currentFocus.transform.localScale = originalScale;
        currentFocus = null;

        if (infoPanel != null)
            infoPanel.Hide();

        Debug.Log("[FOCUS] Unfocused");
    }
}
