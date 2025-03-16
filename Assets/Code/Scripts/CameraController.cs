using Code.Scripts;
using Unity.Cinemachine;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    
    [SerializeField] private GameObject cameraTarget;
    [SerializeField] private float zoomFactor = 0.5f;
    [SerializeField] private float maxDistanceThreshold = 10f;

    [Range(0f,1f)]
    [SerializeField] private float enemyWeight = 0.25f;

    private Vector3 enemyPosition = Vector3.zero;
    
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
        
        // Debug.Log(positionComposer);
        // Debug.Log(baseDistance);
        // Debug.Log(initialDistance);

        cameraTarget = Instantiate(cameraTarget);
        cam.Follow = cameraTarget.transform;

        FindClosestEnemy();

    }
    
    private void Zoom()
    {
        // Decide whether we need to zoom or not
        // If the distanced has increased since the initial distance

        float distance = CalculateDistance();
        if (distance > initialDistance)
        {
            float difference = distance - initialDistance;
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
        Vector3 c = Vector3.zero;

        Vector3 playersMidpoint = new Vector3((a.x + b.x) / 2, (a.y + b.y) / 2, (a.z + b.z) / 2);

        float factor = 1 + enemyWeight;
        // if (playersMidpoint.z > enemyPosition.z)
        // {
        //     factor *= -1;
        // }
        if (enemyPosition != Vector3.zero)
        {
            c = enemyPosition;
            return new Vector3((a.x + b.x + c.x) / 3, (a.y + b.y + c.y) / 3, (a.z + b.z + c.z ) / 3);
        }

        return playersMidpoint;

    }
    
    // Update is called once per frame
    void Update()
    {
        cameraTarget.transform.position = CalculateMidpoint();
        Zoom();

        if (Input.GetKeyDown(KeyCode.B))
        {
            SetClosestEnemy();
        }
    }

    private Vector3 FindClosestEnemy()
    {
        Enemy[] enemies = FindObjectsByType<Enemy>(FindObjectsSortMode.InstanceID);

        float dToBeat = 0;
        int index = -1;
        for (int i = 0; i < enemies.Length; i++)
        {
            // If the enemy is not alive, then skip this one
            if (!enemies[i].IsAlive) continue;
            
            // The distance between the enemy and the players D((Enemy), (Player1 + Player2))
            float distance = Vector3.Distance(enemies[i].transform.position, CalculateMidpoint());
            
            if (distance < dToBeat || dToBeat == 0)
            {
                dToBeat = distance;
                index = i;
            }
        }
        
        Debug.Log("Index is " + index);

        //Debug.Log("There are " + enemies.Length);
        return enemies[index].transform.position;
    }

    public void SetClosestEnemy()
    {
        enemyPosition = FindClosestEnemy();
        Debug.Log("finding closest enemy");
    }

    public void ClearClosestEnemy()
    {
        enemyPosition = Vector3.zero;
    }
}
