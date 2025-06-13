using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace BubbleRush.Scripts.GameUi
{
    public class InputController : MonoBehaviour
    {
        public Action OnShot;
        public Action<Vector2> OnUpdateDirection;
        
        private void Update()
        {
            if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject())
            {
                if (!RayCastToButton())
                {
                    Debug.Log("Shot");
                    OnShot?.Invoke();
                }
            }
            
            OnUpdateDirection?.Invoke(Input.mousePosition);
        }

        private bool RayCastToButton()
        {
            var ray = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            var hit = Physics2D.RaycastAll(ray, Vector2.zero);
            if (hit != null)
            {
                foreach (var hit2D in hit)
                {
                //    Debug.Log(hit2D.transform.name);
                
                    var ipress = hit2D.collider.GetComponent<IPress>();
                    if (ipress != null)
                    {
                        return ipress.OnPress();
                    }
                }
            }

            return false;
        }
    }
}