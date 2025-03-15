using UnityEngine;

[CreateAssetMenu(fileName = "ZoneColorSettings", menuName = "Scriptable Objects/ZoneColorSettings")]
public class ZoneColorSettings : ScriptableObject
{
    public Material zoneA;
    public Material zoneB;
    public Material zoneC;
    public Material zoneD;
    public Material zoneE;
    public Material zoneF;

    public Material GetZone(ZoneType type)
    {
        switch (type)
        {
            case ZoneType.A:
                return zoneA;
            case ZoneType.B:
                return zoneB;
            case ZoneType.C:
                return zoneC;
            case ZoneType.D:
                return zoneD;
            case ZoneType.E:
                return zoneE;
            case ZoneType.F:
                return zoneF;
            default: return null;
        }

    }
}
