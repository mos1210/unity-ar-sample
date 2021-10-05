using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;

namespace UnityAR
{
    public class ExCheckSupport : MonoBehaviour
    {
        [SerializeField] Text message;
        [SerializeField] ARSession session;
        bool isReady;

        void ShowMessage(string text) { message.text = $"{text}\r\n"; }
        void AddMessage(string text) { message.text += $"{text}\r\n"; }

        void Awake()
        {
            if (message == null)
            {
                Application.Quit();
            }
            if (session == null)
            {
                isReady = false;
                ShowMessage("エラー");
            }
            else
            {
                isReady = true;
                ShowMessage("ARのサポート調査");
            }
        }

        IEnumerator checkSupport()
        {
            yield return ARSession.CheckAvailability();
            if (ARSession.state == ARSessionState.NeedsInstall)
            {
                AddMessage("ARサービスのソフトウェアの更新が必要です。");
                yield return ARSession.Install();
            }
            if (ARSession.state == ARSessionState.NeedsInstall || ARSession.state == ARSessionState.Installing)
            {
                AddMessage("ソフトウェアの更新に失敗");
                AddMessage($"State:{ARSession.state}");
                yield break;
            }
            if (ARSession.state == ARSessionState.Unsupported)
            {
                AddMessage("このデバイスはサポートしていません。");
                AddMessage($"State:{ARSession.state}");
                yield break;
            }

            AddMessage("このデバイスはARをサポートしています。");
            AddMessage("ARセッションの初期化");

            session.enabled = true;
            const float Interval = 0.5f;
            var timer = Interval;
            while(ARSession.state == ARSessionState.Ready || ARSession.state == ARSessionState.SessionInitializing && timer > 0)
            {
                var waitTime = 0.5f;
                timer -= waitTime;
                yield return new WaitForSeconds(waitTime);
            }
            if(timer <= 0)
            {
                AddMessage("初期化タイムオーバー");
                AddMessage($"State:{ARSession.state}");
                yield break;
            }
            AddMessage("初期化完了");
            AddMessage($"State:{ARSession.state}");
        }

        void OnEnable()
        {
            if(!isReady) { return; }

            StartCoroutine(checkSupport());
        }

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

    }
}
