using UnityEngine;

public class DoorAnimation : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private string paramKey = "isOpen";
     
    public void ToggleAnimation(bool open)
    {
        animator.SetBool(paramKey, open);
    }
}
