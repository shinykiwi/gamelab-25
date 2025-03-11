using UnityEngine;
#if UNITY_EDITOR
using UnityEditor.Tilemaps;

[CreateAssetMenu(fileName = "New Brush", menuName = "Brushes/GameObject Brush")]
[CustomGridBrush(false, true, false, "Prefab Brush")]
public class PrefabBrush : GameObjectBrush
{

}
#endif
