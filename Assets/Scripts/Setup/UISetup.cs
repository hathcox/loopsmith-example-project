#if UNITY_EDITOR
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;
using CubeCollector;

public class UISetup : IGameSetup
{
    private const int SetupOrder = 400;

    public int ExecutionOrder => SetupOrder;

    public void Execute()
    {
        CleanupExisting();

        var canvas = CreateCanvas();
        var scoreText = CreateScoreText(canvas.transform);
        var winText = CreateWinMessage(canvas.transform);

        var gameManager = Object.FindAnyObjectByType<GameManager>();
        if (gameManager == null)
        {
            Debug.LogError("[UISetup] GameManager not found in scene. Cannot wire UIManager.");
            return;
        }

        var uiManager = canvas.AddComponent<UIManager>();
        uiManager.Initialize(scoreText, winText, gameManager);

        EnsureEventSystem();

        EditorSceneManager.SaveOpenScenes();
        Debug.Log("[UISetup] UI setup complete.");
    }

    private void CleanupExisting()
    {
        var existingCanvas = Object.FindAnyObjectByType<Canvas>();
        if (existingCanvas != null)
        {
            Object.DestroyImmediate(existingCanvas.gameObject);
            Debug.Log("[UISetup] Removed existing Canvas.");
        }
    }

    private GameObject CreateCanvas()
    {
        var canvasObj = new GameObject("UICanvas");

        var canvas = canvasObj.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;

        var scaler = canvasObj.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);

        canvasObj.AddComponent<GraphicRaycaster>();

        Debug.Log("[UISetup] Created UI Canvas.");
        return canvasObj;
    }

    private TextMeshProUGUI CreateScoreText(Transform parent)
    {
        var scoreObj = new GameObject("ScoreText");
        scoreObj.transform.SetParent(parent, false);

        var rectTransform = scoreObj.AddComponent<RectTransform>();
        rectTransform.anchorMin = new Vector2(0.5f, 1f);
        rectTransform.anchorMax = new Vector2(0.5f, 1f);
        rectTransform.pivot = new Vector2(0.5f, 1f);
        rectTransform.anchoredPosition = new Vector2(0f, -20f);
        rectTransform.sizeDelta = new Vector2(200f, 60f);

        var tmp = scoreObj.AddComponent<TextMeshProUGUI>();
        tmp.text = "";
        tmp.fontSize = 36;
        tmp.color = Color.white;
        tmp.alignment = TextAlignmentOptions.Center;

        Debug.Log("[UISetup] Created score text element.");
        return tmp;
    }

    private TextMeshProUGUI CreateWinMessage(Transform parent)
    {
        var winObj = new GameObject("WinMessage");
        winObj.transform.SetParent(parent, false);

        var rectTransform = winObj.AddComponent<RectTransform>();
        rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
        rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
        rectTransform.pivot = new Vector2(0.5f, 0.5f);
        rectTransform.anchoredPosition = Vector2.zero;
        rectTransform.sizeDelta = new Vector2(600f, 100f);

        var tmp = winObj.AddComponent<TextMeshProUGUI>();
        tmp.text = "You Win!";
        tmp.fontSize = 72;
        tmp.color = new Color(1f, 0.85f, 0.2f, 1f); // Gold color
        tmp.alignment = TextAlignmentOptions.Center;

        winObj.SetActive(false);

        Debug.Log("[UISetup] Created win message element (hidden by default).");
        return tmp;
    }

    private void EnsureEventSystem()
    {
        if (Object.FindAnyObjectByType<EventSystem>() != null)
        {
            Debug.Log("[UISetup] EventSystem already exists — skipping creation.");
            return;
        }

        var eventSystemObj = new GameObject("EventSystem");
        eventSystemObj.AddComponent<EventSystem>();
        eventSystemObj.AddComponent<StandaloneInputModule>();

        Debug.Log("[UISetup] Created EventSystem.");
    }
}
#endif
