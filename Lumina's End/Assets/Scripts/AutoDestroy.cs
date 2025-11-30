using UnityEngine;

public class AutoDestroy : MonoBehaviour
{
    public float time = 0.12f;
    void Start()
    {
        Destroy(gameObject, time);
    }
}