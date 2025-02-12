using UnityEngine;
using UnityEngine.InputSystem;

public class TestScript : MonoBehaviour
{
    [SerializeField]
    GameObject obj;

    private void Start()
    {
        var t = FindFirstObjectByType<PlayerInputManager>();
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            Vector3 randomSpawnPoint = transform.position + 15.0f * new Vector3(Random.value, 0.0f, Random.value);
            Instantiate(obj, randomSpawnPoint, Quaternion.identity);
        }
    }
}
