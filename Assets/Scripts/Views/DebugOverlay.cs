using UnityEngine;
using TMPro;
using System.Collections.Generic;

/// <summary>
/// Overlay de debug world-space ou screen-space.
/// Affiche : FPS, date simulée + vitesse, dernière action, warnings.
/// </summary>
public class DebugOverlay : MonoBehaviour
{
    [Header("UI")]
    public TextMeshProUGUI text;

    [Header("Paramètres")]
    [Tooltip("Nombre maximum de logs conservés")]
    public int maxLogLines = 10;

    [Tooltip("Intervalle de rafraîchissement en secondes")]
    public float refreshInterval = 0.25f;

    TimeModel timeModel;
    TimeController timeController;

    readonly Queue<string> logLines = new Queue<string>();
    float refreshTimer;

    // FPS lissé
    float fps;
    float fpsSmooth = 0.9f;

    public void Init(TimeModel model, TimeController controller)
    {
        timeModel = model;
        timeController = controller;
    }

    void OnEnable()
    {
        Application.logMessageReceived += HandleLog;
    }

    void OnDisable()
    {
        Application.logMessageReceived -= HandleLog;
    }

    void Update()
    {
        float instantFps = 1f / Mathf.Max(Time.unscaledDeltaTime, 0.0001f);
        fps = fps * fpsSmooth + instantFps * (1f - fpsSmooth);

        refreshTimer += Time.unscaledDeltaTime;
        if (refreshTimer >= refreshInterval)
        {
            refreshTimer = 0f;
            Refresh();
        }
    }

    void Refresh()
    {
        if (text == null) return;

        string date  = timeModel != null ? timeModel.CurrentTime.ToString("dd MMM yyyy") : "—";
        float  speed = timeController != null ? timeController.secondsPerDay : 0f;
        string playState = (timeModel != null && timeModel.IsPlaying) ? "▶" : "⏸";

        string header =
            $"<b>[DEBUG]</b>\n" +
            $"FPS : <color=#AAFFAA>{fps:F0}</color>  |  ΔT : {Time.unscaledDeltaTime * 1000f:F1} ms\n" +
            $"Date : {date}  {playState}  ({speed:F2} j/s)\n" +
            "<size=80%>─────────────────────</size>\n";

        string logs = string.Join("\n", logLines);

        text.text = header + logs;
    }

    void HandleLog(string logString, string stackTrace, LogType type)
    {
        string prefix = type switch
        {
            LogType.Warning => "<color=#FFD700>[WARN]</color> ",
            LogType.Error   => "<color=#FF6666>[ERR]</color>  ",
            LogType.Exception => "<color=#FF4444>[EXC]</color>  ",
            _               => "<color=#CCCCCC></color>",
        };

        bool isStructured = logString.StartsWith("[") ;
        bool isImportant  = type == LogType.Warning || type == LogType.Error || type == LogType.Exception;

        if (!isStructured && !isImportant) return;
        string msg = logString.Length > 80 ? logString[..80] + "…" : logString;

        logLines.Enqueue($"<size=75%>{prefix}{msg}</size>");
        while (logLines.Count > maxLogLines)
            logLines.Dequeue();

        if (isImportant)
            Refresh();
    }
}
