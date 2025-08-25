using UnityEngine;

public class DayNigthCycleManager : MonoBehaviour
{
    public enum DayNightCycleState
    {
        Day,
        Night
    }
    static public DayNightCycleState state;
    public float dayDurationSec = 600f; // Duration of the day in seconds
    public float nightDurationSec = 600f; // Duration of the night in seconds
    float timer = 0f;
    private void Start()
    {
        state = DayNightCycleState.Day;
    }
    private void Update()
    {
        timer += Time.deltaTime;
        if (state == DayNightCycleState.Day && timer >= dayDurationSec)
        {
            state = DayNightCycleState.Night;
            timer = 0f;
            Debug.Log("It's now Night");
        }
        else if (state == DayNightCycleState.Night && timer >= nightDurationSec)
        {
            state = DayNightCycleState.Day;
            timer = 0f;
            Debug.Log("It's now Day");
        }
    }

}

