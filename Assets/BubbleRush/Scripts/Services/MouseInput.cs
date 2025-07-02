using System;
using BubbleRush.Scripts.GameUi;
using UnityEngine;
using UnityEngine.EventSystems;
using Zenject;

namespace BubbleRush.Scripts.Services
{
    public class InputService : IInputService , ITickable
    {
        public Action<Vector2> OnPointToScreen { get; set; }
        public Action OnShotEvent { get; set; }

        void ITickable.Tick()
        {
            if (!EventSystem.current.IsPointerOverGameObject())
            {
                if (Input.touchCount > 0)
                {
                    if (Input.GetTouch(0).phase == TouchPhase.Ended)
                    {
                        if (!RayCastToButton())
                        {
                            OnShotEvent?.Invoke();
                        }
                    }
                }
                else if (Input.GetMouseButtonDown(0))
                {
                    if (!RayCastToButton())
                    {
                        OnShotEvent?.Invoke();
                    }
                }
            }
            
            
            
            OnPointToScreen?.Invoke(Input.mousePosition);
        }

        private bool RayCastToButton()
        {
            var ray = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            var hit = Physics2D.RaycastAll(ray, Vector2.zero);
            if (hit != null)
            {
                foreach (var hit2D in hit)
                {
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