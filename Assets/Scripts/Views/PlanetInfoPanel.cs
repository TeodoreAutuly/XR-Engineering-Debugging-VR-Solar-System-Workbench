using UnityEngine;
using TMPro;
using System;

public class PlanetInfoPanel : MonoBehaviour
{
    [Header("Textes UI")]
    public TextMeshProUGUI planetNameText;
    public TextMeshProUGUI distanceText;
    public TextMeshProUGUI periodText;
    public TextMeshProUGUI simulatedDateText;

    [Header("Style")]
    [Tooltip("Taille de police du titre (nom de la planète)")]
    public float titleFontSize = 10f;

    [Tooltip("Taille de police des lignes de données")]
    public float bodyFontSize = 7f;

    [Tooltip("Couleur du titre")]
    public Color titleColor = Color.white;

    [Tooltip("Couleur des données")]
    public Color bodyColor = new Color(0.85f, 0.85f, 0.85f, 1f);

    [Tooltip("Couleur du fond du panneau (si Image présente)")]
    public Color backgroundColor = new Color(0f, 0f, 0f, 0.75f);

    void Awake()
    {
        // Fond semi-transparent
        var bg = GetComponent<UnityEngine.UI.Image>();
        if (bg != null)
            bg.color = backgroundColor;

        ApplyStyle(planetNameText, titleFontSize, titleColor, FontStyles.Bold, TextAlignmentOptions.Center);
        ApplyStyle(distanceText,      bodyFontSize, bodyColor,  FontStyles.Normal, TextAlignmentOptions.Left);
        ApplyStyle(periodText,        bodyFontSize, bodyColor,  FontStyles.Normal, TextAlignmentOptions.Left);
        ApplyStyle(simulatedDateText, bodyFontSize, bodyColor,  FontStyles.Italic, TextAlignmentOptions.Left);
    }

    static void ApplyStyle(TextMeshProUGUI label, float size, Color color, FontStyles style, TextAlignmentOptions alignment)
    {
        if (label == null) return;
        label.fontSize       = size;
        label.color          = color;
        label.fontStyle      = style;
        label.alignment      = alignment;
        label.enableWordWrapping = true;
        label.overflowMode   = TextOverflowModes.Ellipsis;
        label.margin         = new Vector4(4f, 2f, 4f, 2f);
    }

    public void Show(PlanetData.Planet planet, DateTime simulatedTime)
    {
        float distanceAU  = PlanetData.GetSemiMajorAxisAU(planet);
        float periodDays  = PlanetData.GetOrbitalPeriodDays(planet);
        float periodYears = periodDays / 365.25f;

        if (planetNameText    != null) planetNameText.text    = planet.ToString().ToUpper();
        if (distanceText      != null) distanceText.text      = $"Distance :  {distanceAU:F3} UA";
        if (periodText        != null) periodText.text        = $"Période   :  {periodYears:F2} ans  ({periodDays:F0} j)";
        if (simulatedDateText != null) simulatedDateText.text = $"Date simulée :  {simulatedTime:dd MMM yyyy}";

        gameObject.SetActive(true);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }
}
