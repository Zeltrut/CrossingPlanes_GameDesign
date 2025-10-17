using UnityEngine;

public class NewMonoBehaviourScript : MonoBehaviour
{
    [SerializeField] float rotateSpeed = 0.5f;
    void Update()
    {
        transform.Rotate(0, rotateSpeed, 0, Space.World);
    }
}
