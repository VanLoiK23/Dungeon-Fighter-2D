using UnityEngine;

public class TransitionTwoBehaviour : StateMachineBehaviour
{
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        PlayerController.instance.canReceiveInput = true;
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (PlayerController.instance.inputReceived)
        {
            animator.SetTrigger("AttackThree"); // Kích hoạt đòn thứ 3
            PlayerController.instance.inputReceived = false;
            PlayerController.instance.canReceiveInput = false;
        }
    }
}