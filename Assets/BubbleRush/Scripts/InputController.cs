using System;
using UnityEngine;

namespace BubbleRush.Scripts
{
    public class InputController : MonoBehaviour
    {
        public Action OnShot;
        public Action<Vector2> OnUpdateDiraection;
        
        private void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                OnShot?.Invoke();
            }
            
            OnUpdateDiraection?.Invoke(Input.mousePosition);
        }
    }
}