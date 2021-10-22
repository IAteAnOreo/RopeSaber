using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.EventSystems;

namespace Com.MyCompany.MyGame
{
    public class PlayerManager : MonoBehaviourPunCallbacks
    {
        private new PhotonView photonView;
        public static GameObject LocalPlayerInstance;

        [SerializeField] private GameObject PlayerUiPrefab;
        [SerializeField] private Transform teammate;

        public float teamNumber;
        public float playerNumber;

        public int coins1;
        public int coins2;
        public int coins3;
        public int coins4;

        public static PlayerManager instance;
        [SerializeField] GameObject ropeManager;

        private void Awake()
        {
            photonView = GetComponent<PhotonView>();
            // #Important
            // used in GameManager.cs: we keep track of the localPlayer instance to prevent instantiation when levels are synchronized
            if (photonView.IsMine)
            {
                PlayerManager.LocalPlayerInstance = this.gameObject;
            }
            // #Critical
            // we flag as don't destroy on load so that instance survives level synchronization, thus giving a seamless experience when levels load.
            DontDestroyOnLoad(this.gameObject);
        }

#if UNITY_5_4_OR_NEWER
        void OnSceneLoaded(UnityEngine.SceneManagement.Scene scene, UnityEngine.SceneManagement.LoadSceneMode loadingMode)
        {
            this.CalledOnLevelWasLoaded(scene.buildIndex);
        }
#endif

        void Start()
        {
            CameraWork _cameraWork = this.gameObject.GetComponent<CameraWork>();

            if (_cameraWork != null)
            {
                if (photonView.IsMine)
                {
                    _cameraWork.OnStartFollowing();
                }
            }
            else
            {
                Debug.LogError("<Color=Red><a>Missing</a></Color> CameraWork Component on playerPrefab.", this);
            }

            if (PlayerUiPrefab != null)
            {
                GameObject _uiGo = Instantiate(PlayerUiPrefab);
                _uiGo.SendMessage("SetTarget", this, SendMessageOptions.RequireReceiver);
            }
            else
            {
                Debug.LogWarning("<Color=Red><a>Missing</a></Color> PlayerUiPrefab reference on player Prefab.", this);
            }

#if UNITY_5_4_OR_NEWER
            // Unity 5.4 has a new scene management. register a method to call CalledOnLevelWasLoaded.
            UnityEngine.SceneManagement.SceneManager.sceneLoaded += OnSceneLoaded;
#endif

            instance = this;

            playerNumber = PhotonNetwork.CurrentRoom.PlayerCount;
            teamNumber = Mathf.Ceil(playerNumber / 2);
            this.transform.SetParent(GameObject.Find("Team " + teamNumber).GetComponent<Transform>(), false);
            if (playerNumber % 2 == 0)
            {
                GameObject rope = PhotonNetwork.Instantiate("RopeManager", Vector3.zero, Quaternion.identity, 0);
                rope.transform.parent = transform.parent;
            }
        }

#if !UNITY_5_4_OR_NEWER
/// <summary>See CalledOnLevelWasLoaded. Outdated in Unity 5.4.</summary>
void OnLevelWasLoaded(int level)
{
    this.CalledOnLevelWasLoaded(level);
}
#endif


        void CalledOnLevelWasLoaded(int level)
        {
            // check if we are outside the Arena and if it's the case, spawn around the center of the arena in a safe zone
            if (!Physics.Raycast(transform.position, -Vector3.up, 5f))
            {
                transform.position = new Vector3(0f, 5f, 0f);
            }

            GameObject _uiGo = Instantiate(this.PlayerUiPrefab);
            _uiGo.SendMessage("SetTarget", this, SendMessageOptions.RequireReceiver);
        }

#if UNITY_5_4_OR_NEWER
        public override void OnDisable()
        {
            // Always call the base to remove callbacks
            base.OnDisable();
            UnityEngine.SceneManagement.SceneManager.sceneLoaded -= OnSceneLoaded;
        }
#endif

        private void Update()
        {
            if (Input.GetKey("d") && photonView.IsMine) transform.Translate(8f * Time.deltaTime, 0f, 0f);
            if (Input.GetKey("a") && photonView.IsMine) transform.Translate(-8f * Time.deltaTime, 0f, 0f);
            if (Input.GetKey("w") && photonView.IsMine) transform.Translate(0f, 8f * Time.deltaTime, 0f);
            if (Input.GetKey("s") && photonView.IsMine) transform.Translate(0f, -8f * Time.deltaTime, 0f);

            if (playerNumber % 2 == 0)
			{
                teammate = RopeManager.instance.playerOne;
			}
            else if (playerNumber % 2 != 0 && transform.parent.childCount >= 2)
			{
                teammate = RopeManager.instance.playerTwo;
			}

            if (teammate != null)
            {
                float dist = Vector3.Distance(transform.position, teammate.position);

                if (dist >= (((RopeManager.instance.ropeLength - 1) * 2.8) + 7.6))
                {
                    transform.position = Vector3.MoveTowards(transform.position, teammate.position, 7.1f * Time.deltaTime);
                }
            }

            if (photonView.IsMine == false && PhotonNetwork.IsConnected == true)
            {
                return;
            }

            if (photonView.IsMine)
            {
                transform.position += new Vector3(Input.GetAxis("Horizontal") * 0.1f, Input.GetAxis("Vertical") * 0.1f, 0);
            }
        }
    }
}