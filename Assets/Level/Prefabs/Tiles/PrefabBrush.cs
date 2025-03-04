using UnityEditor.Tilemaps;
using UnityEngine;

#if UNITY_EDITOR
[CreateAssetMenu(fileName = "New Brush", menuName = "Brushes/GameObject Brush")]
[CustomGridBrush(false, true, false, "Prefab Brush")]
#endif
public class PrefabBrush : GameObjectBrush
{

}
