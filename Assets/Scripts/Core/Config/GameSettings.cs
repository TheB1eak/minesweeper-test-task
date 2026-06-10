using UnityEngine;

namespace Minesweeper.Core.Config
{
    [CreateAssetMenu(fileName = "GameSettings", menuName = "Minesweeper/Game Settings")]
    public sealed class GameSettings : ScriptableObject
    {
        [Header("Grid Settings")]
        public int Width = 10;
        public int Height = 10;

        [Header("Mine Generation")]
        [Tooltip("Number of mines. If 0, mines will be auto-calculated as width * height / 10.")]
        public int CustomMineNumber;

        [Tooltip("Place mines on the first click. The clicked cell and its neighbors stay mine-free.")]
        public bool SafeFirstClick = true;

        [Header("Prefabs & Sprites")]
        public GameObject CellPrefab;
        public Sprite ClosedCellSprite;
        public Sprite EmptyCellSprite;
        public Sprite FlagCellSprite;
        public Sprite MineCellSprite;
        public Sprite[] NumberSprites;

        public int ResolveMineCount()
        {
            return CustomMineNumber == 0 ? Width * Height / 10 : CustomMineNumber;
        }
    }
}
