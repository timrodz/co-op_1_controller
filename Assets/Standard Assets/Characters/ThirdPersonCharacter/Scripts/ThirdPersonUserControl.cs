using System;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

// DOTween
using DG.Tweening;

namespace UnityStandardAssets.Characters.ThirdPerson
{
    [RequireComponent(typeof (ThirdPersonCharacter))]
    public class ThirdPersonUserControl : MonoBehaviour
    {
        
        public bool isPlayerToTheRight = true;
        private float originalX, currentRX, currentLX;
        private bool canLeftPlayerMoveHorizontally = true, canRightPlayerMoveHorizontally = true;
        
        private ThirdPersonCharacter m_Character; // A reference to the ThirdPersonCharacter on the object
        private Transform m_Cam;                  // A reference to the main camera in the scenes transform
        private Vector3 m_CamForward;             // The current forward direction of the camera
        private Vector3 m_Move;
        private bool m_Jump;                      // the world-relative desired move direction, calculated from the camForward and user input.

        
        private void Start()
        {
            
            originalX = transform.position.x;
            currentRX = originalX;
            
            // get the transform of the main camera
            if (Camera.main != null)
            {
                m_Cam = Camera.main.transform;
            }
            else
            {
                Debug.LogWarning(
                    "Warning: no main camera found. Third person character needs a Camera tagged \"MainCamera\", for camera-relative controls.", gameObject);
                // we use self-relative controls in this case, which probably isn't what the user wants, but hey, we warned them!
            }

            // get the third person character ( this should never be null due to require component )
            m_Character = GetComponent<ThirdPersonCharacter>();
        }


        private void Update()
        {
            if (!m_Jump)
            {
                m_Jump = CrossPlatformInputManager.GetButtonDown("Jump");
            }
        }


        // Fixed update is called in sync with physics
        private void FixedUpdate()
        {
            
            // read inputs
            float hL = CrossPlatformInputManager.GetAxisRaw("HorizontalL");
            float vL = CrossPlatformInputManager.GetAxis("VerticalL");
            
            float hR = CrossPlatformInputManager.GetAxisRaw("HorizontalR");
            float vR = CrossPlatformInputManager.GetAxisRaw("HorizontalR");
            
            if (!isPlayerToTheRight && hL != 0 && canLeftPlayerMoveHorizontally) {
                
                print("HR");
                
                canRightPlayerMoveHorizontally = false;
                
                currentLX += hL;
                currentLX = Mathf.Clamp(currentLX, originalX - 1, originalX + 1);
                
                transform.DOMoveX(currentLX, 0.4f).OnComplete(ActivateLeftPlayerMovement);
                
            }
            
            if (isPlayerToTheRight && hR != 0 && canRightPlayerMoveHorizontally) {
                
                print("HL");
                
                canLeftPlayerMoveHorizontally = false;
                
                currentRX += hR;
                currentRX = Mathf.Clamp(currentRX, originalX - 1, originalX + 1);
                
                transform.DOMoveX(currentRX, 0.4f).OnComplete(ActivateRightPlayerMovement);
                
            }

            // calculate move direction to pass to character
            if (m_Cam != null)
            {
                // calculate camera relative direction to move:
                m_CamForward = Vector3.Scale(m_Cam.forward, new Vector3(1, 0, 1)).normalized;
                
                // Always move forward
                m_Move = m_CamForward + 0*m_Cam.right;
            }
            else
            {
                // we use world-relative directions in the case of no main camera
                m_Move = vL*Vector3.forward + hL*Vector3.right;
            }
#if !MOBILE_INPUT
			// walk speed multiplier
	        if (Input.GetKey(KeyCode.LeftShift)) m_Move *= 0.5f;
#endif

            // pass all parameters to the character control script
            m_Character.Move(m_Move, false, m_Jump);
            m_Jump = false;
        }
        
        void ActivateLeftPlayerMovement() {
            canLeftPlayerMoveHorizontally = true;
        }
        
        void ActivateRightPlayerMovement() {
            canRightPlayerMoveHorizontally = true;
        }
        
    }
}
