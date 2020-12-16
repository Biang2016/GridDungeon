using BiangStudio.GameDataFormat.Grid;
using UnityEngine;

namespace Client
{
    [ExecuteInEditMode]
    public class RectGridSnapper : MonoBehaviour
    {
        void LateUpdate()
        {
            float gridSize = 20f;
            Vector3 position = ((RectTransform) transform).anchoredPosition;
            ((RectTransform) transform).anchoredPosition = new Vector3(Mathf.RoundToInt(position.x / gridSize) * gridSize, Mathf.RoundToInt(position.y / gridSize) * gridSize, 0);
        }
    }
}