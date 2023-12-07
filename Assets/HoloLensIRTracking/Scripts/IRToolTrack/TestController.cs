using UnityEngine;
using IRToolTrack;

public class TestController : MonoBehaviour
{
    public IRToolController irToolController;
    private float amplitude = 1.0f;
    private float speed = 1.0f;
    private int markerId = 1;
    private float last_activate = 0f;
    private bool activate = true;

    private void Start()
    {
        last_activate = Time.time;
    }

    void Update()
    {
        // along y axis
        float xPosition = amplitude * Mathf.Sin(Time.time * speed);
        Vector3 position = new Vector3(xPosition, 0, 0);
        Quaternion rotation = Quaternion.identity;

        if (Time.time > last_activate + 1f)
        {
            activate = !activate;
            last_activate = Time.time;
        }
        if (activate) 
        {
            irToolController.ReceivePoseData(markerId, position, rotation);
            // irToolController.ReceivePoseData(0, new Vector3(0, 0, 0), rotation);
        }
        irToolController.ReceivePoseData(0, new Vector3(0, 0, 0), Quaternion.Euler(0, 0.2f * 90f * Time.time, 0));
    }
}

