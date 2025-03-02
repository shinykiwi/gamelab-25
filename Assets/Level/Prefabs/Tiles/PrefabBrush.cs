using UnityEditor.Tilemaps;
using UnityEngine;

[CreateAssetMenu(fileName = "New Brush", menuName = "Brushes/GameObject Brush")]
[CustomGridBrush(false, true, false, "Prefab Brush")]

public class PrefabBrush : GameObjectBrush
{

}
