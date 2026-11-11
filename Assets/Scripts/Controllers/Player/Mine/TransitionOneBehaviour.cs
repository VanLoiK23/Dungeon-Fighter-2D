using UnityEngine;

public class TransitionOneBehaviour : StateMachineBehaviour
{
    // Bắt buộc phải có hàm này để mở khóa nhận phím
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        PlayerController.instance.canReceiveInput = true;
        // Xóa sạch input cũ rớt lại (nếu có) để tránh lặp đòn
        PlayerController.instance.inputReceived = false; 
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (PlayerController.instance.inputReceived)
        {
            animator.SetTrigger("AttackTwo"); 
            PlayerController.instance.inputReceived = false;
            PlayerController.instance.canReceiveInput = false;
        }
    }
}