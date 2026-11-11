using UnityEngine;

public class IdleBehaviour : StateMachineBehaviour
{
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (PlayerController.instance.inputReceived)
        {
            animator.SetTrigger("AttackOne");
            PlayerController.instance.inputReceived = false;
            PlayerController.instance.canReceiveInput = false;
        }
    }

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // Mở khóa input khi nhân vật quay về đứng yên
        PlayerController.instance.canReceiveInput = true;
        PlayerController.instance.inputReceived = false;

        // Cực kỳ quan trọng: Reset các trigger cũ để tránh kẹt logic
        animator.ResetTrigger("AttackOne");
        animator.ResetTrigger("AttackTwo");
        animator.ResetTrigger("AttackThree");
    }
}