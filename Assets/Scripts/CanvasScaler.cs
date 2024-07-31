using UnityEngine;

[RequireComponent(typeof(Canvas))]
public class CanvasScaler : MonoBehaviour
{
    private Canvas canvas;
    private Camera mainCamera;

    void Start()
    {
        canvas = GetComponent<Canvas>();
        mainCamera = canvas.worldCamera;

        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }

        UpdateCanvasRect();
    }

    void UpdateCanvasRect()
    {
        if (mainCamera != null)
        {
            Rect cameraRect = mainCamera.rect;
            RectTransform canvasRectTransform = canvas.GetComponent<RectTransform>();

            float screenWidth = Screen.width;
            float screenHeight = Screen.height;

            float rectX = cameraRect.x * screenWidth;
            float rectY = cameraRect.y * screenHeight;
            float rectWidth = cameraRect.width * screenWidth;
            float rectHeight = cameraRect.height * screenHeight;

            canvasRectTransform.offsetMin = new Vector2(rectX, rectY);
            canvasRectTransform.offsetMax = new Vector2(rectX + rectWidth, rectY + rectHeight);
        }
    }

    void Update()
    {
        UpdateCanvasRect();
    }
}