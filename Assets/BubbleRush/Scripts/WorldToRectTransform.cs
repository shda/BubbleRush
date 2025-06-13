using UnityEngine;

namespace BubbleRush.Scripts.Lua
{
    public class WorldToRectTransform : MonoBehaviour
    {
        [SerializeField] private RectTransform uiElement; // Ваш UI-элемент
        [SerializeField] private Transform worldTarget;   // Объект в мировых координатах
        [SerializeField] private Camera mainCamera;      // Камера, через которую рендерится UI
        [SerializeField] private Canvas parentCanvas;
        
        private void Update()
        {
            if (mainCamera == null || worldTarget == null) return;
    
            // Преобразуем мировые координаты в экранные
            Vector2 screenPoint = mainCamera.WorldToScreenPoint(worldTarget.position);
    
            // Преобразуем экранные координаты в локальные координаты Canvas
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                parentCanvas.GetComponent<RectTransform>(),
                screenPoint,
                parentCanvas.worldCamera,
                out Vector2 localPoint
            );
    
            // Применяем позицию к RectTransform
            uiElement.localPosition = localPoint;
        }
    }
}