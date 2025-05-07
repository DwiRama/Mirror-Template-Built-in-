using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace BNG {

    /// <summary>
    /// This script will toggle a GameObject whenever the provided InputAction is executed
    /// </summary>
    public class ToggleActiveOnInputAction : MonoBehaviour {

        public InputActionReference InputAction = default;
        public GameObject ToggleObject = default;

        public ControllerBinding controllerBinding = ControllerBinding.None;

        void Update() {
            if (controllerBinding != ControllerBinding.None) {
                if(InputBridge.Instance.GetControllerBindingValue(controllerBinding)) {
                    ToggleObject.SetActive(!ToggleObject.activeSelf);
                }
            }
        }

        private void OnEnable() {
            if(InputAction) {
                InputAction.action.performed += ToggleActive;

                InputAction.action.Enable();
            }
        }

        private void OnDisable() {
            if (InputAction) {
                InputAction.action.performed -= ToggleActive;
                InputAction.action.Disable();
            }
        }

        public void ToggleActive(InputAction.CallbackContext context) {
            Debug.Log("Toggellll");
            
            if(ToggleObject) {
                ToggleObject.SetActive(!ToggleObject.activeSelf);
            }
        }
    }
}

