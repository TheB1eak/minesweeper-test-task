using UnityEngine;

namespace Minesweeper.Presentation.Config
{
    [CreateAssetMenu(fileName = "CameraSettings", menuName = "Minesweeper/Camera Settings")]
    public sealed class CameraSettings : ScriptableObject
    {
        [Header("Movement Settings")]
        public float MoveSpeed = 10f;
        public float SmoothTime = 0.15f;

        [Header("Zoom Settings")]
        public float ZoomSpeed = 5f;
        public float MinZoom = 3f;
        public float MaxZoom = 15f;
        public float ZoomSmoothTime = 0.2f;

        [Header("Movement Bounds")]
        public Vector2 MinBounds = new Vector2(-10f, -10f);
        public Vector2 MaxBounds = new Vector2(10f, 10f);
    }
}
