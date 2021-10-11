using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.AR;

namespace UnityAR
{

    public class ExInteraction : MonoBehaviour
    {
        [SerializeField] Text message;
        [SerializeField] ARPlaneManager planeManager;
        [SerializeField] ARPlacementInteractable placementInteractable;
        [SerializeField] ARGestureInteractor gestureInteractor;

        bool isReady;

        void ShowMessage(string text) { message.text = $"{text}\r\n"; }
        void AddMessage(string text) { message.text += $"{text}\r\n"; }

        void Awake()
        {
            if(message == null) { Application.Quit(); }

            if(planeManager == null || planeManager.planePrefab == null
                || placementInteractable == null
                || placementInteractable.placementPrefab == null
                || gestureInteractor == null)
            {
                isReady = false;
                ShowMessage("Error: SelializeField");
            }
            else
            {
                isReady = true;
                ShowMessage("Take picture");
            }
        }

        void OnEnable()
        {
            placementInteractable.onObjectPlaced.AddListener(OnObjectPlaced);
            gestureInteractor.onHoverEntered.AddListener(OnHoverEntered);
            gestureInteractor.onHoverExited.AddListener(OnHoverExited);
        }

        bool hasPlaced = false;

        void OnObjectPlaced(ARPlacementInteractable arg0, GameObject placedObject)
        {
            if (hasPlaced)
            {
                Destroy(placedObject);
                return;
            }

            var selectInteractable = placedObject.GetComponent<ARSelectionInteractable>();
            if(selectInteractable != null)
            {
                selectInteractable.onSelectEntered.AddListener(OnSelectEntered);
                selectInteractable.onSelectExited.AddListener(OnSelectExited);
                hasPlaced = true;
            }
            else
            {
                isReady = false;
                ShowMessage("Error: ARSelectionInteractable");
            }
        }

        string hoverStatus = "";
        string selectStatus = "";

        void OnHoverEntered(XRBaseInteractable arg0)
        {
            hoverStatus = $"talkable:{arg0.gameObject.name}";
        }

        void OnHoverExited(XRBaseInteractable arg0)
        {
            hoverStatus = "sattus: cannot talkable";
        }

        void OnSelectEntered(XRBaseInteractor arg0)
        {
            selectStatus = $"selecting: {arg0.selectTarget.gameObject.name}";
        }

        void OnSelectExited(XRBaseInteractor arg0)
        {
            selectStatus = "選択解除";
        }

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            if (!isReady || !hasPlaced) { return; }

            foreach(var plane in planeManager.trackables)
            {
                plane.gameObject.SetActive(false);
            }

            ShowMessage("インタラクション");
            AddMessage(hoverStatus);
            AddMessage(selectStatus);
        }
    }
}