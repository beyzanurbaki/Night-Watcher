using UnityEngine;

public class EventManager : MonoBehaviour
{
    public static EventManager Instance;

    void Awake()
    {
        Instance = this;
    }

    public void SpawnRandomEvent()
    {
        int random = Random.Range(0, 3);

        switch (random)
        {
            case 0:
                Debug.Log("Mahallede gürültü duyuldu!");
                break;

            case 1:
                Debug.Log("Bir kedinin sesi duyuldu!");
                break;

            case 2:
                Debug.Log("Biri kapý çaldý!");
                break;
        }
    }
}