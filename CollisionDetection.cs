using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class CollisionDetection : MonoBehaviour
{
    [Header("Object References")]
    [SerializeField] private GameObject thePlayer;
    [SerializeField] private GameObject playerAnimator;
    [SerializeField] private GameObject fadeOut;
    
    [Header("Controller References")]
    [SerializeField] private SaveController saveController;

    private bool hasCollided = false;

    void Start()
    {
        if (saveController == null)
        {
            saveController = FindAnyObjectByType<SaveController>();
        }

        if (saveController == null)
        {
            Debug.LogError("CollisionDetection Error: SaveController not found in the scene. Scene transition will not work.");
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (hasCollided) return;
        
        if (other.CompareTag("Player"))
        {
            hasCollided = true;
            StartCoroutine(CollisionEndSequence());
        }
    }

    IEnumerator CollisionEndSequence()
    {
        thePlayer.GetComponent<FirstPersonController>().enabled = false;
        playerAnimator.GetComponent<Animator>().Play("Stumble Backwards");
        yield return new WaitForSeconds(0.5f);
        fadeOut.SetActive(true);
        yield return new WaitForSeconds(1.5f);

        if (saveController != null)
        {
            saveController.ResetAndGoToMenu();
        }
    }
}

