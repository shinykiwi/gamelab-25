using Code.Scripts;
using Unity.Cinemachine;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    
    [SerializeField] private GameObject cameraTarget;
    [SerializeField] private float zoomFactor = 0.5f;
    [SerializeField] private float maxDistanceThreshold = 10f;
    
    
    // The players transforms
    private Transform player1;
    private Transform player2;

    // Base camera distance threshold not to pass
    private float baseDistance;

    // The initial distance between the camera and the players
    private float initialDistance;
    
    private CinemachinePositionComposer positionComposer;

    private CinemachineCamera cam;
    
    private void Start()
    {
        Player[] players = FindObjectsByType<Player>(FindObjectsSortMode.None);
        player1 = players[0].transform;
        player2 = players[1].transform;
        
        positionComposer = GetComponent<CinemachinePositionComposer>();
        cam = GetComponent<CinemachineCamera>();
        
        baseDistance = positionComposer.CameraDistance;
        initialDistance = CalculateDistance();
        
        Debug.Log(positionComposer);
        Debug.Log(baseDistance);
        Debug.Log(initialDistance);

        cameraTarget = Instantiate(cameraTarget);
        cam.Follow = cameraTarget.transform;

    }
    
    private void Zoom()
    {
        // Decide whether we need to zoom or not
        // If the distanced has increased since the initial distance
        if (CalculateDistance() > initialDistance)
        {
            float difference = CalculateDistance() - initialDistance;
            if (difference > maxDistanceThreshold)
            {
                positionComposer.CameraDistance = baseDistance + ((difference) * zoomFactor);
            }
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
        cameraTarget.transform.position = CalculateMidpoint();
        Zoom();
    }
}
