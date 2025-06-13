using System;
using UnityEngine;

namespace BubbleRush.Scripts.GameUi
{
    public class ButtonUi : MonoBehaviour , IPress
    {
        public Action<ButtonUi> OnPressButton { get; set; }
        
        public bool OnPress()
        {
            Debug.Log(gameObject.name + " pressed");
            OnPressButton?.Invoke(this);
            return true;
        }
    }
}