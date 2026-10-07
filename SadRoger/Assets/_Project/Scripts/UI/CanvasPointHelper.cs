using UnityEngine;

namespace UI
{
    internal static class CanvasPointHelper
    {
        public static Vector2 WorldToContainerPoint(
            Vector3 worldPosition,
            Camera worldCamera,
            Canvas canvas,
            RectTransform container)
        {
            Vector2 screenPoint = worldCamera.WorldToScreenPoint(worldPosition);

            Camera uiCamera = canvas.renderMode == RenderMode.ScreenSpaceOverlay
                ? null
                : canvas.worldCamera;

            RectTransformUtility.ScreenPointToLocalPointInRectangle(container, screenPoint, uiCamera, out Vector2 localPoint);

            return localPoint;
        }
    }
}
