using UnityEngine;

[RequireComponent(typeof(MeshRenderer))]
public class LevelTrigger : MonoBehaviour
{
    private MeshRenderer meshRenderer;
    private bool canUse = false;

    private float portalOpenScale = 3.0f;
    private float portalScaleSpeed = 2.0f;
    private float portalClosedScale = 0.05f;
  
    void Start()
    {
        meshRenderer = GetComponent<MeshRenderer>();

        // Hide the object at first
        Hide();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!canUse) return;
        canUse = false;
        LevelManager.Instance.LoadNextLevel();
    }

    private void Update()
    {
        if(canUse)
        {
            ShowAnim();
        }
        else {
            HideAnim();
        }
    }

    public void Show()
    {
        canUse = true;
    }

    public void Hide()
    {
        canUse = false;
    }

    private void ShowAnim()
    {

        meshRenderer.enabled = true;
        // Open portal slowly
        if (meshRenderer.gameObject.transform.localScale.x < portalOpenScale)
        {
            meshRenderer.gameObject.transform.localScale +=
                meshRenderer.gameObject.transform.localScale.x * portalScaleSpeed * Time.deltaTime * Vector3.one;
            if (meshRenderer.gameObject.transform.localScale.x > portalOpenScale)
            {
                meshRenderer.gameObject.transform.localScale = portalOpenScale * Vector3.one;
            }
        }

    }

    private void HideAnim()
    {

        if (meshRenderer.gameObject.transform.localScale.x > portalClosedScale)
        {
            meshRenderer.gameObject.transform.localScale -=
                meshRenderer.gameObject.transform.localScale.x * portalScaleSpeed * Time.deltaTime * Vector3.one;
            if (meshRenderer.gameObject.transform.localScale.x < portalClosedScale)
            {
                meshRenderer.gameObject.transform.localScale = portalClosedScale * Vector3.one;
                meshRenderer.enabled = false;
            }
        }
        meshRenderer.enabled = false;
    }
    
    
}
