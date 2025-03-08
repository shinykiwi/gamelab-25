using Code.Scripts;
using Unity.Cinemachine;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    private Transform player1;
    private Transform player2;

    private float baseDistance;

    private float initialDistance;
    
    private CinemachinePositionComposer positionComposer;

    private void Start()
    {
        positionComposer = GetComponent<CinemachinePositionComposer>();
        baseDistance = positionComposer.CameraDistance;
        initialDistance = CalculateDistance();
        
        Player[] players = FindObjectsByType<Player>(FindObjectsSortMode.None);
        player1 = players[0].transform;
        player2 = players[1].transform;

        if (player1 == null)
        {
            Debug.Log("CANT FIND PLAYER");
        }
    }
    
    private void Zoom()
    {
        // Decide whether we need to zoom or not
        // If the distanced has increased since the initial distance
        if (CalculateDistance() > initialDistance)
        {
            positionComposer.CameraDistance = baseDistance + (CalculateDistance() - initialDistance);
        }
        else
        {
            positionComposer.CameraDistance = baseDistance;
        }
    }

    private float CalculateDistance()
    {
        return Vector3.Distance(player1.position, player2.position);
    }

    private Vector3 CalculateMidpoint()
    {
        Vector3 a = player1.position;
        Vector3 b = player2.position;
        
        return new Vector3((a.x + b.x) / 2, (a.y + b.y) / 2, (a.z + b.z) / 2);
    }
    
    // Update is called once per frame
    void Update()
    {
        transform.position = CalculateMidpoint();
        Zoom();
    }
}
