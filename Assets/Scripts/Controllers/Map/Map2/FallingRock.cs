
using UnityEngine;

public class FallingRock : MonoBehaviour
{
    public ButtonMechanism buttonMechanism;

    private bool hasPressedButton;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (hasPressedButton)
            return;

        if (collision.collider.CompareTag("Button"))
        {
            hasPressedButton = true;

            Debug.Log("Đá đã chạm Button!");

            buttonMechanism.ActivateWood();
        }
    }
}
