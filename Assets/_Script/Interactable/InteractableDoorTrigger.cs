using Mirror;
using UnityEngine;

public class InteractableDoorTrigger : Interactable
{
    [SerializeField] private float durationToClose = 2;
    [SerializeField] private Animator animator;
    [SerializeField] private string paramKey = "isOpen";
    [SerializeField, SyncVar(hook = nameof(OnDoorOpenChanged))] private bool doorOpen = true;

    private float timer;

    public override void OnStartServer()
    {
        base.OnStartServer();
        timer = durationToClose;
        ToggleDoor(doorOpen);
    }

    private void Update()
    {
        if (doorOpen)
        {
            if (timer > 0)
            {
                timer -= Time.deltaTime;
            }
            else
            {
                // Close door
                doorOpen = false;
                ToggleDoor(doorOpen);
            }
        }
    }

    public override void TriggerInteraction()
    {
        // Open Door
        doorOpen = true;
        ToggleDoor(doorOpen); 

        base.TriggerInteraction(); // Call the base interaction event

        // Ensure the object remains interactable
        base.TriggerInteractionComplete();
    }

    private void OnDoorOpenChanged(bool oldValue, bool newValue)
    {
        doorOpen = newValue;

        ToggleDoor(doorOpen);

        Debug.Log($"DoorOpen: {doorOpen}");
    }

    // Must disable needUserInput
    private void OnTriggerEnter(Collider other)
    {
        if (needUserInput)
            return;

        if (other.gameObject.CompareTag("Player"))
        {
            TriggerInteraction();
        }
    }

    public void ToggleDoor(bool open)
    {
        animator.SetBool(paramKey, open);
        timer = durationToClose;
    }
}
