
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class WheelGenerator : MonoBehaviour
{

    private static readonly Color[] FixedColors =
{
    new Color32(244, 67, 54, 255),   // Red
    new Color32(255, 152, 0, 255),   // Orange
    new Color32(255, 235, 59, 255),  // Yellow
    new Color32(76, 175, 80, 255),   // Green
    new Color32(33, 150, 243, 255),  // Blue
    new Color32(156, 39, 176, 255)  // Purple
};

    [Header("Wheel Settings")]
    public float wheelSize = 400f;
    public Font numberFont;

    [Header("Segments")]
    public List<string> segments =
        new List<string>()
        {
            "1",
            "2",
            "3",
            "4",
            "5",
            "6"
        };

    [Header("Text Settings")]
    [Range(0.1f, 0.9f)]
    public float textRadius = 0.32f;

    public bool rotateText = true;

    [Header("Generated Wheel")]
    public RectTransform wheel;

    private Sprite circleSprite;

    void Awake()
    {
        GenerateWheel();
    }

    public void GenerateWheel()
    {
        if (wheel != null)
        {
            Destroy(wheel.gameObject);
            wheel = null;
        }

        if (segments == null || segments.Count == 0)
        {
            Debug.LogError("No wheel segments defined!");
            return;
        }

        GameObject wheelObject = new GameObject(
            "Wheel", typeof(RectTransform));

        wheelObject.transform.SetParent(transform, false);

        wheel = wheelObject.GetComponent<RectTransform>();

        wheel.anchorMin = new Vector2(0.5f, 0.5f);
        wheel.anchorMax = new Vector2(0.5f, 0.5f);
        wheel.pivot = new Vector2(0.5f, 0.5f);
        wheel.sizeDelta = new Vector2(wheelSize, wheelSize);
        wheel.anchoredPosition = Vector2.zero;

        circleSprite = CreateCircleSprite();

        // Create all segments first
        for (int i = 0; i < segments.Count; i++)
        {
            CreateSegment(i);
        }

        // Create all labels above the segments
        for (int i = 0; i < segments.Count; i++)
        {
            CreateText(i);
        }

        CreatePointer();
    }

    Sprite CreateCircleSprite()
    {
        const int size = 256;

        Texture2D texture = new Texture2D(
            size, size, TextureFormat.RGBA32, false);

        texture.filterMode = FilterMode.Bilinear;

        float center = (size - 1) / 2f;
        float radius = center;

        Color[] pixels = new Color[size * size];

        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                float distance = Vector2.Distance(
                    new Vector2(x, y),
                    new Vector2(center, center));

                pixels[y * size + x] =
                    distance <= radius
                    ? Color.white
                    : Color.clear;
            }
        }

        texture.SetPixels(pixels);
        texture.Apply();

        return Sprite.Create(
            texture,
            new Rect(0, 0, size, size),
            new Vector2(0.5f, 0.5f));
    }


    void CreateSegment(int index)
    {
        float segmentAngle = 360f / segments.Count;

        GameObject obj = new GameObject(
            "Segment " + (index + 1),
            typeof(RectTransform),
            typeof(Image));

        obj.transform.SetParent(wheel, false);

        RectTransform rect =
            obj.GetComponent<RectTransform>();

        rect.anchorMin = new Vector2(0.5f, 0.5f);
        rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.sizeDelta = new Vector2(wheelSize, wheelSize);
        rect.anchoredPosition = Vector2.zero;

        Image image = obj.GetComponent<Image>();

        image.sprite = circleSprite;
        image.type = Image.Type.Filled;
        image.fillMethod = Image.FillMethod.Radial360;
        image.fillOrigin = (int)Image.Origin360.Top;

        // Fill clockwise from the top
        image.fillClockwise = true;
        image.fillAmount = 1f / segments.Count;

        image.color = FixedColors[index % FixedColors.Length];
        image.raycastTarget = false;

        // Center segment 1 at 12 o'clock.
        // Subsequent segments proceed clockwise.
        float rotation =
            segmentAngle / 2f - index * segmentAngle;

        rect.localRotation =
            Quaternion.Euler(0, 0, rotation);
    }



    void CreateText(int index)
    {
        float segmentAngle = 360f / segments.Count;

        // Clockwise ordering from the top
        float angle = 90f - index * segmentAngle;

        float radians = angle * Mathf.Deg2Rad;

        float radius = wheelSize * textRadius;

        GameObject obj = new GameObject(
            "Text " + (index + 1),
            typeof(RectTransform),
            typeof(Text));

        obj.transform.SetParent(wheel, false);

        RectTransform rect =
            obj.GetComponent<RectTransform>();

        rect.anchorMin = new Vector2(0.5f, 0.5f);
        rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.pivot = new Vector2(0.5f, 0.5f);

        rect.sizeDelta = new Vector2(
            wheelSize * 0.35f,
            wheelSize * 0.15f);

        rect.anchoredPosition = new Vector2(
            Mathf.Cos(radians) * radius,
            Mathf.Sin(radians) * radius);

        if (rotateText)
        {
            // Rotate text with the segment
            rect.localRotation = Quaternion.Euler(
                0, 0, -index * segmentAngle);
        }

        Text text = obj.GetComponent<Text>();

        text.text = segments[index];
        text.font = numberFont;
        text.fontSize = 36;
        text.fontStyle = FontStyle.Bold;
        text.alignment = TextAnchor.MiddleCenter;
        text.color = Color.black;
        text.raycastTarget = false;
    }


    void CreatePointer()
    {
        GameObject obj = new GameObject(
            "Pointer",
            typeof(RectTransform),
            typeof(Text));

        obj.transform.SetParent(transform, false);

        RectTransform rect =
            obj.GetComponent<RectTransform>();

        rect.anchorMin = new Vector2(0.5f, 0.5f);
        rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.sizeDelta = new Vector2(60, 60);
        rect.anchoredPosition =
            new Vector2(0, wheelSize / 2f + 20f);

        Text text = obj.GetComponent<Text>();
        text.text = "▼";
        text.font = numberFont;
        text.fontSize = 48;
        text.alignment = TextAnchor.MiddleCenter;
        text.color = Color.black;
        text.raycastTarget = false;
    }

    public string GetSegmentText(int index)
    {
        if (index < 0 || index >= segments.Count)
            return "";

        return segments[index];
    }
}
