using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Com.MyCompany.MyGame
{
    public class PlayerUI : MonoBehaviour
    {
        public Text playerNameText;
        [SerializeField] private Text coinCounter;
        [SerializeField] private Text playerDisplay;
        [SerializeField] private Text teamDisplay;

        [Tooltip("The Player's UI GameObject Prefab")]
        [SerializeField]
        public GameObject PlayerUiPrefab;

        private int coins;

        private PlayerManager target;

        public static PlayerUI instance;

        readonly float characterHeight = 0f;
        Transform targetTransform;
        Renderer targetRenderer;
        CanvasGroup _canvasGroup;
        Vector3 targetPosition;

        [SerializeField] float yOffset = 80f;

        //[SerializeField] TMPro.TextMeshProUGUI coins1;
        //[SerializeField] TMPro.TextMeshProUGUI coins2;
        //[SerializeField] TMPro.TextMeshProUGUI coins3;
        //[SerializeField] TMPro.TextMeshProUGUI coins4;

        private void Awake()
        {
            _canvasGroup = this.GetComponent<CanvasGroup>();
            this.transform.SetParent(GameObject.Find("Canvas").GetComponent<Transform>(), false);

            instance = this;
        }

        void Update()
        {
            if (target == null)
            {
                Destroy(this.gameObject);
                return;
            }

            if (PlayerManager.instance.photonView.IsMine)
            {
                switch (PlayerManager.instance.teamNumber)
                {
                    case 1:
                        coins = PlayerManager.instance.coins1;
                        break;
                    case 2:
                        coins = PlayerManager.instance.coins2;
                        break;
                    case 3:
                        coins = PlayerManager.instance.coins3;
                        break;
                    case 4:
                        coins = PlayerManager.instance.coins4;
                        break;
                }

                coinCounter.text = "" + coins;
                teamDisplay.text = "" + PlayerManager.instance.teamNumber;
                playerDisplay.text = "" + PlayerManager.instance.playerNumber;
            }
        }

        public void SetTarget(PlayerManager _target)
        {
            if (_target == null)
            {
                Debug.LogError("<Color=Red><a>Missing</a></Color> PlayMakerManager target for PlayerUI.SetTarget.", this);
                return;
            }
            // Cache references for efficiency
            target = _target;
            if (playerNameText != null)
            {
                playerNameText.text = target.photonView.Owner.NickName;
            }
        }

        void Start()
        {
            this.transform.SetParent(GameObject.Find("Canvas").GetComponent<Transform>(), false);

            if (PlayerUiPrefab != null)
            {
                GameObject _uiGo = Instantiate(PlayerUiPrefab);
                _uiGo.SendMessage("SetTarget", this, SendMessageOptions.RequireReceiver);
            }
            else
            {
                Debug.LogWarning("<Color=Red><a>Missing</a></Color> PlayerUiPrefab reference on player Prefab.", this.gameObject);
            }
        }

        void LateUpdate()
        {
            // Do not show the UI if we are not visible to the camera, thus avoid potential bugs with seeing the UI, but not the player itself.
            if (targetRenderer != null)
            {
                this._canvasGroup.alpha = targetRenderer.isVisible ? 1f : 0f;
            }

            if (target == null)
            {
                Destroy(this.gameObject);
                return;
            }

            // #Critical
            // Follow the Target GameObject on screen.
            if (targetTransform != null)
            {
                targetPosition = targetTransform.position;
                targetPosition.y += characterHeight;
                this.transform.position = Camera.main.WorldToScreenPoint(targetPosition) + new Vector3(0f, yOffset, 0f);
            }
        }
    }
}
