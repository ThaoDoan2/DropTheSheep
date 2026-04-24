using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    private TMP_Text    _timerText;
    private TMP_Text    _msgText;
    private GameObject  _msgPanel;

    void Awake()
    {
        Instance = this;
        BuildUI();
    }

    // ---------------------------------------------------------------- Build

    private void BuildUI()
    {
        // EventSystem with new Input System
        if (FindObjectOfType<EventSystem>() == null)
        {
            var es = new GameObject("EventSystem");
            es.AddComponent<EventSystem>();
            es.AddComponent<InputSystemUIInputModule>();
        }

        // Canvas — root object so ScreenSpaceOverlay renders correctly
        var cvGO = new GameObject("GameCanvas");
        // Do NOT parent to this transform; keep as scene root for ScreenSpaceOverlay
        var cv = cvGO.AddComponent<Canvas>();
        cv.renderMode   = RenderMode.ScreenSpaceOverlay;
        cv.sortingOrder = 100;
        var scaler = cvGO.AddComponent<CanvasScaler>();
        scaler.uiScaleMode         = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(430, 730);
        scaler.matchWidthOrHeight  = 0.5f;
        cvGO.AddComponent<GraphicRaycaster>();

        // ---- Timer bar (top)
        var timerBg = MakePanel(cvGO.transform, "TimerBg",
            new Vector2(0f, 0.915f), new Vector2(1f, 1f),
            new Color(0f, 0f, 0f, 0.70f));

        _timerText = MakeText(timerBg.transform, "TimerTxt",
            Vector2.zero, Vector2.one, 28, Color.white, TextAlignmentOptions.Center);
        _timerText.text = "90s";

        // ---- Message panel (hidden by default)
        _msgPanel = MakePanel(cvGO.transform, "MsgPanel",
            new Vector2(0.08f, 0.38f), new Vector2(0.92f, 0.62f),
            new Color(0f, 0f, 0f, 0.82f));
        _msgPanel.SetActive(false);

        _msgText = MakeText(_msgPanel.transform, "MsgTxt",
            new Vector2(0f, 0.52f), new Vector2(1f, 1f), 48, Color.white, TextAlignmentOptions.Center);

        // Restart button
        var btnGO = MakePanel(_msgPanel.transform, "RestartBtn",
            new Vector2(0.20f, 0.06f), new Vector2(0.80f, 0.46f),
            new Color(0.18f, 0.55f, 0.95f));
        var btn = btnGO.AddComponent<Button>();
        btn.onClick.AddListener(OnRestart);
        MakeText(btnGO.transform, "BtnLbl",
            Vector2.zero, Vector2.one, 28, Color.white, TextAlignmentOptions.Center).text = "RESTART";
    }

    // ---------------------------------------------------------------- Helpers

    private GameObject MakePanel(Transform parent, string name, Vector2 anchorMin, Vector2 anchorMax, Color color)
    {
        var go  = new GameObject(name);
        go.transform.SetParent(parent, false);
        var img = go.AddComponent<Image>();
        img.color = color;
        var rt  = go.GetComponent<RectTransform>();
        rt.anchorMin = anchorMin;
        rt.anchorMax = anchorMax;
        rt.offsetMin = Vector2.zero;
        rt.offsetMax = Vector2.zero;
        return go;
    }

    private TMP_Text MakeText(Transform parent, string name, Vector2 anchorMin, Vector2 anchorMax,
                              float fontSize, Color color, TextAlignmentOptions align)
    {
        var go  = new GameObject(name);
        go.transform.SetParent(parent, false);
        var t   = go.AddComponent<TextMeshProUGUI>();
        t.fontSize  = fontSize;
        t.color     = color;
        t.alignment = align;
        var rt  = t.rectTransform;
        rt.anchorMin = anchorMin;
        rt.anchorMax = anchorMax;
        rt.offsetMin = Vector2.zero;
        rt.offsetMax = Vector2.zero;
        return t;
    }

    // ---------------------------------------------------------------- Public API

    public void UpdateTimer(float timeLeft)
    {
        if (_timerText == null) return;
        int s = Mathf.CeilToInt(timeLeft);
        _timerText.text  = $"Time: {s}s";
        _timerText.color = timeLeft < 15f ? new Color(1f, 0.3f, 0.3f) : Color.white;
    }

    public void ShowMessage(string msg, bool win)
    {
        if (_msgPanel == null) return;
        _msgPanel.SetActive(true);
        _msgText.text = msg;
        var bg = _msgPanel.GetComponent<Image>();
        if (bg) bg.color = win ? new Color(0f, 0.30f, 0.06f, 0.88f) : new Color(0.40f, 0f, 0f, 0.88f);
    }

    public void HideMessage()
    {
        if (_msgPanel != null) _msgPanel.SetActive(false);
    }

    private void OnRestart() => GameManager.Instance?.RestartGame();
}
