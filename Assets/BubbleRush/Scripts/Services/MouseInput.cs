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
            if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject())
            {
                if (!RayCastToButton())
                {
                    OnShotEvent?.Invoke();
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