using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BNG {

    /// <summary>
    /// Shows a ring at the grab point of a grabbable if within a certain distance
    /// </summary>
    public class GrabRingHelper : MonoBehaviour {

        [Tooltip("The Grabbable Object to Observe")]
        public Grabbable grabbable;

        [Tooltip("(Optional) If specified, the ring helper will only be valid if this Grabpoint is the nearest on the the grabbable object")]
        public GrabPoint Grabpoint;

        [Tooltip("Default Color of the ring")]
        public Color RingColor = Color.white;

        /// <summary>
        /// Color to use if selected by primary controller
        /// </summary>
        [Tooltip("Color to use if selected by primary controller")]
        public Color RingSelectedColor = Color.white;

        /// <summary>
        /// Color to use if selected by secondary controller
        /// </summary>
        [Tooltip("Color to use if selected by secondary controller")]
        public Color RingSecondarySelectedColor = Color.white;


        public Vector3 ringScaleInRange = Vector3.one;
        public Vector3 ringScaleGrabbable = new Vector3(1.1f, 1.1f, 1.1f);

        /// <summary>
        /// Don't show grab rings if left and right controllers / grabbers are  holding something
        /// </summary>
        [Tooltip("Don't show grab rings if left and right controllers / grabbers are  holding something")]
        public bool HideIfHandsAreFull = true;

        /// <summary>
        /// How fast to lerp the opacity if being hidden / shown
        /// </summary>
        [Tooltip("How fast to lerp the opacity if being hidden / shown")]
        public float RingFadeSpeed = 5;

        /// <summary>
        /// Used to determine if hands are full
        /// </summary>
        Grabber leftGrabber;
        Grabber rightGrabber;
        Grabber closestGrabber;

        bool handsFull = false;

        // Animate opacity
        private float _initalOpacity;
        private float _currentOpacity;

        public MeshRenderer meshRenderer;

        Transform mainCam;

        void Start() {
            AssignCamera();

            if (grabbable == null) {
                grabbable = transform.parent.GetComponent<Grabbable>();
            }

            //_initalOpacity = text.color.a;
            _currentOpacity = _initalOpacity;

            AssignGrabbers();
        }

        void Update() {

            // Double check for mainCam 
            AssignCamera();

            bool grabbersExist = leftGrabber != null && rightGrabber != null;

            // Holding Item
            handsFull = grabbersExist && leftGrabber.HoldingItem && rightGrabber.HoldingItem;

            // Not a valid Grab
            if (grabbersExist && grabbable.GrabButton == GrabButton.Grip && !leftGrabber.FreshGrip && !rightGrabber.FreshGrip) {
                handsFull = true;
            }

            // Recently dropped; keep hidden briefly
            if (!handsFull && Time.time - grabbable.LastDropTime < 0.1f) {
                meshRenderer.enabled = false;
                return;
            }

            bool showRings = handsFull;

            // If being held or not active, immediately hide the ring
            if (grabbable.BeingHeld || !grabbable.isActiveAndEnabled) {
                meshRenderer.enabled = false;
                return;
            }

            // Requires another Grabbable to be held. Can exit immediately
            if (grabbable.OtherGrabbableMustBeGrabbed != null && grabbable.OtherGrabbableMustBeGrabbed.BeingHeld == false) {
                meshRenderer.enabled = false;
                return;
            }

            // Show if within range
            float currentDistance = Vector3.Distance(transform.position, mainCam.position);
            if (!handsFull && currentDistance <= grabbable.RemoteGrabDistance) {
                showRings = true;
            } else {
                showRings = false;
            }

            // Animate ring opacity in / out
            if (showRings) {
                meshRenderer.enabled = true;
                transform.LookAt(mainCam);

                bool isClosest = grabbable.GetClosestGrabber() != null && grabbable.IsGrabbable();
                // Check if grabpoint was specified
                //if(Grabpoint != null) {

                //}

                // If a valid grabbable, increase size a bit
                if (isClosest) {
                    //scaler.dynamicPixelsPerUnit = ringScaleGrabbable;
                    transform.localScale = ringScaleGrabbable;

                    meshRenderer.material.SetColor("_Color",  getSelectedColor());
                    //text.color = getSelectedColor();
                } 
                else {
                    transform.localScale = ringScaleInRange;
                    // scaler.dynamicPixelsPerUnit = ringScaleInRange;
                    meshRenderer.material.SetColor("_Color", RingColor);
                }

                _currentOpacity += Time.deltaTime * RingFadeSpeed;
                if (_currentOpacity > _initalOpacity) {
                    _currentOpacity = _initalOpacity;
                }

                //Color colorCurrent = text.color;
                //colorCurrent.a = _currentOpacity;
                // renderer.material.SetColor("_Color", colorCurrent);

            } else {

                _currentOpacity -= Time.deltaTime * RingFadeSpeed;
                if (_currentOpacity <= 0) {
                    _currentOpacity = 0;
                    meshRenderer.enabled = false;
                } 
                else {
                    meshRenderer.enabled = true;
                    //Color colorCurrent = text.color;
                    //colorCurrent.a = _currentOpacity;
                    //text.color = colorCurrent;
                }
            }
        }

        public virtual void AssignCamera() {
            if (mainCam == null) {
                // Find By Tag instead of Camera.main as the camera could be disabled
                if (GameObject.FindGameObjectWithTag("MainCamera") != null) {
                    mainCam = GameObject.FindGameObjectWithTag("MainCamera").transform;
                }
            }
        }

        public virtual void AssignGrabbers() {
            // Assign left / right grabbers
            Grabber[] grabs;
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player) {
                grabs = player.GetComponentsInChildren<Grabber>();
            } else {
                // Fall back to all grabbers
                grabs = FindObjectsByType<Grabber>(FindObjectsSortMode.None);
            }

            // Check grabber assignment
            for (int x = 0; x < grabs.Length; x++) {
                Grabber g = grabs[x];
                if (g.HandSide == ControllerHand.Left) {
                    leftGrabber = g;
                } else if (g.HandSide == ControllerHand.Right) {
                    rightGrabber = g;
                }
            }
        }

        Color getSelectedColor() {

            // Use secondary color if closest grabber is on the left hand
            closestGrabber = grabbable.GetClosestGrabber();
            if (grabbable != null && closestGrabber != null) {
                if (closestGrabber.HandSide == ControllerHand.Left) {
                    return RingSecondarySelectedColor;
                }
            }

            return RingSelectedColor;
        }
    }
}

