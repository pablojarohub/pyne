#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Editor tool that automatically sets up the MainScene with placeholder objects.
/// Window -> Food Rush -> Setup Scene
/// </summary>
public class SceneSetupTool : EditorWindow
{
    [MenuItem("Window/Food Rush/Setup Scene")]
    public static void ShowWindow()
    {
        GetWindow<SceneSetupTool>("Food Rush Setup");
    }

    private void OnGUI()
    {
        GUILayout.Label("Food Rush Delivery - Scene Setup", EditorStyles.boldLabel);
        GUILayout.Space(10);

        if (GUILayout.Button("Create Full Scene Setup", GUILayout.Height(40)))
        {
            CreatePlayer();
            CreateCamera();
            CreatePickupPoints();
            CreateDeliveryPoints();
            CreateGameManager();
            CreateDeliveryManager();
            CreateCanvas();
            EditorUtility.DisplayDialog("Done", "Scene setup complete! Check the Hierarchy.", "OK");
        }

        GUILayout.Space(10);
        GUILayout.Label("Individual Objects", EditorStyles.label);

        if (GUILayout.Button("Create Player Only")) CreatePlayer();
        if (GUILayout.Button("Create Camera Only")) CreateCamera();
        if (GUILayout.Button("Create Pickup Points Only")) CreatePickupPoints();
        if (GUILayout.Button("Create Delivery Points Only")) CreateDeliveryPoints();
        if (GUILayout.Button("Create UI Canvas Only")) CreateCanvas();
    }

    void CreatePlayer()
    {
        GameObject player = new GameObject("Player");
        player.tag = "Player";
        player.transform.position = Vector3.zero;

        // Collider
        var col = player.AddComponent<BoxCollider2D>();
        col.size = new Vector2(1f, 1f);

        // Rigidbody
        var rb = player.AddComponent<Rigidbody2D>();
        rb.gravityScale = 0f;
        rb.freezeRotation = true;

        // Scripts
        player.AddComponent<BikeController>();
        var pc = player.AddComponent<PlayerController>();
        player.AddComponent<WorldBounds>();

        // Visuals - Bike
        GameObject bikeVis = new GameObject("BikeVisuals");
        bikeVis.transform.SetParent(player.transform);
        bikeVis.transform.localPosition = Vector3.zero;
        var bikeSr = bikeVis.AddComponent<SpriteRenderer>();
        bikeSr.color = Color.red;
        // Create a simple placeholder sprite
        bikeSr.sprite = CreatePlaceholderSprite(Color.red, "BikePlaceholder");
        pc.bikeVisuals = bikeVis.transform;

        // Visuals - Character
        GameObject charVis = new GameObject("CharacterVisuals");
        charVis.transform.SetParent(player.transform);
        charVis.transform.localPosition = new Vector3(0f, 0.3f, 0f);
        var charSr = charVis.AddComponent<SpriteRenderer>();
        charSr.color = new Color(1f, 0.5f, 0f); // Orange
        charSr.sprite = CreatePlaceholderSprite(new Color(1f, 0.5f, 0f), "CharacterPlaceholder");
        pc.characterVisuals = charVis.transform;

        Selection.activeGameObject = player;
    }

    void CreateCamera()
    {
        Camera cam = Camera.main;
        if (cam == null)
        {
            GameObject camObj = new GameObject("MainCamera");
            cam = camObj.AddComponent<Camera>();
            cam.tag = "MainCamera";
        }
        cam.orthographic = true;
        cam.orthographicSize = 8f;
        cam.backgroundColor = new Color(0.53f, 0.81f, 0.92f); // Sky blue
        cam.transform.position = new Vector3(0f, 0f, -10f);

        var follow = cam.GetComponent<CameraFollow>();
        if (follow == null) follow = cam.gameObject.AddComponent<CameraFollow>();

        GameObject player = GameObject.FindWithTag("Player");
        if (player != null) follow.target = player.transform;
    }

    void CreatePickupPoints()
    {
        string[] names = { "BurgerPlace", "KebabKing", "MarketStore" };
        Color[] colors = { Color.yellow, new Color(0.8f, 0.2f, 0.2f), Color.green };

        for (int i = 0; i < names.Length; i++)
        {
            GameObject go = new GameObject(names[i]);
            go.transform.position = new Vector3(-8f + i * 8f, 5f, 0f);

            var loc = go.AddComponent<LocationName>();
            loc.displayName = names[i];

            var dp = go.AddComponent<DeliveryPoint>();
            dp.isPickupPoint = true;

            var col = go.AddComponent<BoxCollider2D>();
            col.isTrigger = true;
            col.size = new Vector2(2f, 2f);

            // Visual
            GameObject vis = new GameObject("Visual");
            vis.transform.SetParent(go.transform);
            vis.transform.localPosition = Vector3.zero;
            var sr = vis.AddComponent<SpriteRenderer>();
            sr.color = colors[i];
            sr.sprite = CreatePlaceholderSprite(colors[i], names[i] + "Sprite");
            sr.sortingOrder = -1;
        }
    }

    void CreateDeliveryPoints()
    {
        string[] names = { "HouseA", "HouseB", "HouseC", "HouseD", "HouseE" };

        for (int i = 0; i < names.Length; i++)
        {
            GameObject go = new GameObject(names[i]);
            go.transform.position = new Vector3(-10f + i * 5f, -5f, 0f);

            var loc = go.AddComponent<LocationName>();
            loc.displayName = names[i];

            var dp = go.AddComponent<DeliveryPoint>();
            dp.isPickupPoint = false;

            var col = go.AddComponent<BoxCollider2D>();
            col.isTrigger = true;
            col.size = new Vector2(1.5f, 1.5f);

            // Visual
            GameObject vis = new GameObject("Visual");
            vis.transform.SetParent(go.transform);
            vis.transform.localPosition = Vector3.zero;
            var sr = vis.AddComponent<SpriteRenderer>();
            sr.color = new Color(0.6f, 0.4f, 0.2f); // Brown house
            sr.sprite = CreatePlaceholderSprite(new Color(0.6f, 0.4f, 0.2f), names[i] + "Sprite");
            sr.sortingOrder = -1;
        }
    }

    void CreateGameManager()
    {
        GameObject go = GameObject.Find("GameManager");
        if (go == null) go = new GameObject("GameManager");

        var gm = go.GetComponent<GameManager>();
        if (gm == null) gm = go.AddComponent<GameManager>();

        GameObject player = GameObject.FindWithTag("Player");
        if (player != null) gm.player = player.GetComponent<PlayerController>();

        // Audio sources
        if (go.GetComponent<AudioSource>() == null)
        {
            var music = go.AddComponent<AudioSource>();
            music.loop = true;
            music.playOnAwake = false;
            gm.musicSource = music;
        }
        if (go.GetComponents<AudioSource>().Length < 2)
        {
            var sfx = go.AddComponent<AudioSource>();
            sfx.playOnAwake = false;
            gm.sfxSource = sfx;
        }
    }

    void CreateDeliveryManager()
    {
        GameObject go = GameObject.Find("DeliveryManager");
        if (go == null) go = new GameObject("DeliveryManager");

        var dm = go.GetComponent<DeliveryManager>();
        if (dm == null) dm = go.AddComponent<DeliveryManager>();

        // Auto-find points
        dm.pickupPoints.Clear();
        dm.deliveryPoints.Clear();

        foreach (var dp in Object.FindObjectsByType<DeliveryPoint>(FindObjectsSortMode.None))
        {
            if (dp.isPickupPoint)
                dm.pickupPoints.Add(dp.transform);
            else
                dm.deliveryPoints.Add(dp.transform);
        }
    }

    void CreateCanvas()
    {
        // Check for existing canvas
        Canvas canvas = Object.FindFirstObjectByType<Canvas>();
        if (canvas == null)
        {
            GameObject canvasObj = new GameObject("Canvas");
            canvas = canvasObj.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvasObj.AddComponent<CanvasScaler>();
            canvasObj.AddComponent<GraphicRaycaster>();
        }

        GameObject uiManagerObj = GameObject.Find("UIManager");
        if (uiManagerObj == null)
        {
            uiManagerObj = new GameObject("UIManager");
            uiManagerObj.transform.SetParent(canvas.transform);
        }
        var ui = uiManagerObj.GetComponent<UIManager>();
        if (ui == null) ui = uiManagerObj.AddComponent<UIManager>();

        // Create panels (simplified - expand as needed)
        if (ui.mainMenuPanel == null)
        {
            ui.mainMenuPanel = CreatePanel(canvas.transform, "MainMenuPanel", true);
            CreateText(ui.mainMenuPanel.transform, "FOOD RUSH DELIVERY", 48, Color.white);
        }
        if (ui.hudPanel == null)
        {
            ui.hudPanel = CreatePanel(canvas.transform, "HUDPanel", false);
        }
        if (ui.gameOverPanel == null)
        {
            ui.gameOverPanel = CreatePanel(canvas.transform, "GameOverPanel", false);
            CreateText(ui.gameOverPanel.transform, "GAME OVER", 48, Color.red);
        }
    }

    GameObject CreatePanel(Transform parent, string name, bool active)
    {
        GameObject panel = new GameObject(name);
        panel.transform.SetParent(parent);
        var rt = panel.AddComponent<RectTransform>();
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.offsetMin = Vector2.zero;
        rt.offsetMax = Vector2.zero;
        var img = panel.AddComponent<Image>();
        img.color = new Color(0f, 0f, 0f, 0.8f);
        panel.SetActive(active);
        return panel;
    }

    void CreateText(Transform parent, string text, int fontSize, Color color)
    {
        GameObject go = new GameObject("Text");
        go.transform.SetParent(parent);
        var rt = go.AddComponent<RectTransform>();
        rt.anchorMin = new Vector2(0.5f, 0.5f);
        rt.anchorMax = new Vector2(0.5f, 0.5f);
        rt.sizeDelta = new Vector2(600f, 100f);
        rt.anchoredPosition = Vector2.zero;
        var tmp = go.AddComponent<TextMeshProUGUI>();
        tmp.text = text;
        tmp.fontSize = fontSize;
        tmp.color = color;
        tmp.alignment = TextAlignmentOptions.Center;
    }

    Sprite CreatePlaceholderSprite(Color color, string name)
    {
        Texture2D tex = new Texture2D(64, 64);
        Color[] pixels = new Color[64 * 64];
        for (int i = 0; i < pixels.Length; i++) pixels[i] = color;
        tex.SetPixels(pixels);
        tex.Apply();
        tex.name = name;
        return Sprite.Create(tex, new Rect(0, 0, 64, 64), new Vector2(0.5f, 0.5f), 64f);
    }
}
#endif
