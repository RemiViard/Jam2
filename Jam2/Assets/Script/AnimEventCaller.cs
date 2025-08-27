using UnityEngine;
using UnityEngine.Events;

public class AnimEventCaller : MonoBehaviour
{
    public UnityEvent OnPunch = new UnityEvent();
    public void Punch()
        {
        // This function is called by an animation event
        // You can add your punch logic here
        Debug.Log("Punch event triggered");
        OnPunch.Invoke();
    }
}
