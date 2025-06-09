using UnityEngine;

namespace BubbleRush.Scripts
{
    public class Cannon : MonoBehaviour
    {
        [SerializeField] private Vector2 minMaxAngle;
        [SerializeField] private Transform rootRotate;
        
        private void Update()
        {
            // Получаем позицию мыши в мировых координатах
            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePosition.z = 0; // Обнуляем Z, чтобы не было проблем с 2D
            
            Vector2 direction = mousePosition - rootRotate.position;
        
            // Вычисляем угол в радианах и переводим в градусы
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        
            angle = Mathf.Clamp(angle, minMaxAngle.x, minMaxAngle.y);


            if (minMaxAngle.x < angle && angle < minMaxAngle.y)
            {
                rootRotate.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
            }
        }
    }
}