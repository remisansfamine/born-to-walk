using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private RagdollController ragdollController;

    private void Update()
    {
        if (Input.GetMouseButton(0))
            BoneSelection(100f);

        if (Input.GetMouseButton(1))
            BoneSelection(-100f);
    }

    private void BoneSelection(float torque)
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out RaycastHit hitInfo))
        {
            if (hitInfo.rigidbody)
            {
                if (hitInfo.rigidbody.TryGetComponent(out CharacterJoint characterJoint))
                {
                    hitInfo.rigidbody.AddRelativeTorque(characterJoint.axis * torque, ForceMode.Impulse);
                }
            }
        }

        
    }
}
