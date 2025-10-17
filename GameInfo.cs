using UnityEngine;

public class GameInfo : MonoBehaviour
{
    public static int carrotCount = 0;
    [SerializeField] GameObject carrotDisplay;

    // Update is called once per frame
    void Update()
    {
        carrotDisplay.GetComponent<TMPro.TMP_Text>().text = "CARROTS:  " + carrotCount;
    }
}
