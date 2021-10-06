using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

namespace UnityAR
{
    [RequireComponent(typeof(ARSessionOrigin))]
    [RequireComponent(typeof(ARPlaneManager))]
    [RequireComponent(typeof(ARRaycastManager))]
    [RequireComponent(typeof(PlayerInput))]

    public class ExMakeAppearOnPlane : MonoBehaviour
    {
        ARSessionOrigin sessionOrigin;
        ARPlaneManager planeManager;
        ARRaycastManager raycastManager;
        PlayerInput playerInput;
        [SerializeField] GameObject placementPrefab;
        GameObject instantiatedObject = null;

        float scale;
        public float Scale
        {
            get { return scale; }
            set
            {
                scale = value;
                if(sessionOrigin != null && instantiatedObject != null)
                {
                    sessionOrigin.transform.localScale = Vector3.one / scale;
                }
            }
        }

        Quaternion rotation;
        public Quaternion Rotation
        {
            get { return rotation; }
            set
            {
                rotation = value;
                if(sessionOrigin != null && instantiatedObject != null)
                {
                    sessionOrigin.MakeContentAppearAt(instantiatedObject.transform, instantiatedObject.transform.position, rotation);
                }
            }
        }

        public bool isAvailable { get; private set; }

        void Awake()
        {
            sessionOrigin = GetComponent<ARSessionOrigin>();
            planeManager = GetComponent<ARPlaneManager>();
            raycastManager = GetComponent<ARRaycastManager>();
            playerInput = GetComponent<PlayerInput>();

            if (sessionOrigin == null || sessionOrigin.camera == null
                || planeManager == null || planeManager.planePrefab == null
                || raycastManager == null
                || playerInput == null || playerInput.actions == null
                || placementPrefab == null)
            {
                isAvailable = false;
            }
            else
            {
                isAvailable = true;
            }

        }

        void OnTouch(InputValue touchInfo)
        {
            if (!isAvailable) { return; }

            var touchPosition = touchInfo.Get<Vector2>();
            var hits = new List<ARRaycastHit>();
            if(raycastManager.Raycast(touchPosition, hits, TrackableType.PlaneWithinPolygon))
            {
                var hitPose = hits[0].pose;
                if(instantiatedObject == null)
                {
                    instantiatedObject = Instantiate(placementPrefab, hitPose.position, hitPose.rotation);
                }
                else
                {
                    sessionOrigin.MakeContentAppearAt(instantiatedObject.transform, hitPose.position, rotation);
                }
            }
        }

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            if(instantiatedObject == null) { return; }

            foreach(var plane in planeManager.trackables)
            {
                plane.gameObject.SetActive(false);
            }
        }
    }
}