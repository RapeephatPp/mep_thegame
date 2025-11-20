using UnityEngine;

public class FlashAutoDestroy : MonoBehaviour
{
    public float time = 0.12f;
    void Start()
    {
        Destroy(gameObject, time);
    }
}