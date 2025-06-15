using System;
using UnityEngine;

namespace BubbleRush.Scripts.GameUi
{
    public class ButtonUi : MonoBehaviour , IPress
    {
        public Action<ButtonUi> OnPressButton { get; set; }
        
        public bool OnPress()
        {
            OnPressButton?.Invoke(this);
            return true;
        }
    }
}