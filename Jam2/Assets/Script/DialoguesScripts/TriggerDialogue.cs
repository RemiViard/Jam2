using UnityEngine;
using UnityEngine.Events;
using Yarn.Unity;

public class TriggerDialogue : MonoBehaviour
{
    public string _name;
    public bool _playOnlyOnce;
    [SerializeField] DialogueRunner _dialogueRunner;

    void Start()
    {
        if (_dialogueRunner == null)
            Debug.Log("No dialogue runner linked to this trigger : " + this.gameObject.name);
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.CompareTag("Player"))
        {
            RunDialogue();
        }
    }
    private void RunDialogue()
    {
        _dialogueRunner.StartDialogue(_name);
        if (_playOnlyOnce)
            Destroy(this.gameObject);
    }

}
