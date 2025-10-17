using UnityEngine;

public class CarrotCollect : MonoBehaviour
{
    [SerializeField] AudioSource carrotFX;

    void OnTriggerEnter(Collider other)
    {
        carrotFX.Play();
        GameInfo.carrotCount += 1;
        this.gameObject.SetActive(false);
    }
}
