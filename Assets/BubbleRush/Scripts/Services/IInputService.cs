using System;
using UnityEngine;

namespace BubbleRush.Scripts.Services
{
    public interface IInputService
    {
        Action<Vector2> OnPointToScreen { get; set; }
        Action OnShotEvent { get; set; }
    }
}