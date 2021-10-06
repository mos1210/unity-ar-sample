using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

namespace UnityAR
{
    [RequireComponent(typeof(ARPlaneManager))]
    [RequireComponent(typeof(ARRaycastManager))]
    [RequireComponent(typeof(PlayerInput))]

    public class ExPlaneDetection : MonoBehaviour
    {
        [SerializeField] Text message;
        [SerializeField] GameObject placementPrefab;
        ARPlaneManager planeManager;
        ARRaycastManager raycastManager;
        PlayerInput playerInput;

        bool isReady;

        void ShowMessage(string text) { message.text = $"{text}\r\n"; }
        void AddMessage(string text) { message.text += $"{text}\r\n"; }

        void Awake()
        {
            if (message == null) {
                Application.Quit();
            }
            planeManager = GetComponent<ARPlaneManager>();
            playerInput = GetComponent<PlayerInput>();
            raycastManager = GetComponent<ARRaycastManager>();

            if (placementPrefab == null ||
                planeManager == null || planeManager.planePrefab == null
                || raycastManager == null
                || playerInput == null || playerInput.actions == null) {
                isReady = false;
                ShowMessage("Error: error settgins of SerializeField");
            }
            else
            {
                isReady = true;
                ShowMessage("平面検出");
                AddMessage("Point the camera at the floor so that you can detect the plane. Touch the screen, and a chair appears.");
            }
        }


        //// Start is called before the first frame update
        //void Start()
        //{

        //}

        //// Update is called once per frame
        //void Update()
        //{

        //}

        GameObject instantiatedObject = null;

        /// <summary>
        /// This is a EventHandler.
        /// This method is called by ActionMap.
        /// </summary>
        /// <param name="touchInfo"></param>
        void OnTouch(InputValue touchInfo)
        {
            if (!isReady) { Application.Quit(); }

            var touchPosition = touchInfo.Get<Vector2>();
            var hits = new List<ARRaycastHit>();
            if (raycastManager.Raycast(touchPosition, hits, TrackableType.PlaneWithinPolygon))
            {
                var hitPose = hits[0].pose;
                if (instantiatedObject == null)
                {
                    instantiatedObject = Instantiate(placementPrefab, hitPose.position, hitPose.rotation);
                }
                else
                {
                    instantiatedObject.transform.position = hitPose.position;
                }
            }
        }
    }
}