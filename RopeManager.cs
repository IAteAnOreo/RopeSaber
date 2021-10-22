using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

namespace Com.MyCompany.MyGame
{
    public class RopeManager : MonoBehaviourPunCallbacks
    {
        [SerializeField] Transform parentObject;

        [SerializeField] GameObject middle;
        [SerializeField] GameObject end;

        public int ropeLength;

        public Transform playerOne;
        public Transform playerTwo;

        HingeJoint2D[] joints = new HingeJoint2D[2];

        readonly List<GameObject> segments = new List<GameObject>();
        readonly List<float> spawnX = new List<float>();

        public static RopeManager instance;
        private int coins;

        void Start()
        {
            instance = this;
            parentObject = transform.parent;

            playerOne = parentObject.GetChild(0);
            playerTwo = parentObject.GetChild(1);

            Creation();
        }

        private void Update()
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

            playerOne = parentObject.GetChild(0);
            playerTwo = parentObject.GetChild(1);

            if (Input.GetKeyDown(KeyCode.Space) && ropeLength < 8 && coins >= 5) Upgrade();
        }

        private void Creation()
        {
            spawnX.Clear();
            spawnX.Add(-4.2f);
            spawnX.Add(-1.4f);
            spawnX.Add(1.4f);
            spawnX.Add(4.2f);

            ropeLength = 8;

            GameObject startSeg = PhotonNetwork.Instantiate("end", new Vector2(-6.8f, 0f), Quaternion.identity);
            startSeg.transform.parent = this.transform.parent;
            startSeg.name = "1";
            startSeg.transform.eulerAngles = new Vector3(0f, 180f, 0f);
            segments.Add(startSeg);


            for (int i = 0; i < ropeLength; i++)
            {

                GameObject seg = PhotonNetwork.Instantiate("mid", new Vector2(spawnX[i], 0f), Quaternion.identity);
                seg.transform.parent = this.transform.parent;
                seg.name = "" + (i + 2);
                segments.Add(seg);

                if (i == ropeLength - 1)
                {
                    Debug.Log("Yay");

                    GameObject endSeg = PhotonNetwork.Instantiate("end", new Vector2(6.8f, 0f), Quaternion.identity);
                    endSeg.transform.parent = this.transform.parent;
                    endSeg.name = "" + (ropeLength + 2);
                    segments.Add(endSeg);

                    Anchors();
                }
            }
        }

        private void Anchors()
        {
            for (int i = 0; i < segments.Count; i++)
            {
                Debug.Log(i);
                joints = segments[i].GetComponents<HingeJoint2D>();
                if (i == 0)
                {
                    joints[0].connectedBody = playerOne.GetComponent<Rigidbody2D>();
                    joints[0].anchor = new Vector2(-0.15f, 0f);
                    joints[0].connectedAnchor = new Vector2(0f, 0f);

                    joints[1].connectedBody = segments[i + 1].GetComponent<Rigidbody2D>();
                    joints[1].anchor = new Vector2(0.15f, 0f);
                    joints[1].connectedAnchor = new Vector2(-0.175f, 0f);
                }
                else if (i == 1)
                {
                    joints[0].connectedBody = segments[i - 1].GetComponent<Rigidbody2D>();
                    joints[0].anchor = new Vector2(-0.175f, 0f);
                    joints[0].connectedAnchor = new Vector2(0.15f, 0f);

                    joints[1].connectedBody = segments[i + 1].GetComponent<Rigidbody2D>();
                    joints[1].anchor = new Vector2(0.175f, 0f);
                    joints[1].connectedAnchor = new Vector2(-0.175f, 0f);
                }
                else if (i == segments.Count - 2)
                {
                    joints[0].connectedBody = segments[i - 1].GetComponent<Rigidbody2D>();
                    joints[0].anchor = new Vector2(-0.175f, 0f);
                    joints[0].connectedAnchor = new Vector2(0.175f, 0f);

                    joints[1].connectedBody = segments[i + 1].GetComponent<Rigidbody2D>();
                    joints[1].anchor = new Vector2(0.175f, 0f);
                    joints[1].connectedAnchor = new Vector2(-0.15f, 0f);
                }
                else if (i == segments.Count - 1)
                {
                    joints[0].connectedBody = segments[i - 1].GetComponent<Rigidbody2D>();
                    joints[0].anchor = new Vector2(-0.15f, 0f);
                    joints[0].connectedAnchor = new Vector2(0.175f, 0f);

                    joints[1].connectedBody = playerTwo.GetComponent<Rigidbody2D>();
                    joints[1].anchor = new Vector2(0.15f, 0f);
                    joints[1].connectedAnchor = new Vector2(0f, 0f);
                }
                else
                {
                    joints[0].connectedBody = segments[i - 1].GetComponent<Rigidbody2D>();
                    joints[0].anchor = new Vector2(-0.175f, 0f);
                    joints[0].connectedAnchor = new Vector2(0.175f, 0f);

                    joints[1].connectedBody = segments[i + 1].GetComponent<Rigidbody2D>();
                    joints[1].anchor = new Vector2(0.175f, 0f);
                    joints[1].connectedAnchor = new Vector2(-0.175f, 0f);
                }
            }
        }

        void Upgrade()
        {
            //ask how to shorten this line
            Vector3 insertedPosition = new Vector3
                                       (Mathf.Rad2Deg * Mathf.Sin((90 - Mathf.Abs(segments[segments.Count - 2].transform.eulerAngles.z)) * 3.5f),
                                        Mathf.Rad2Deg * Mathf.Sin(segments[segments.Count - 2].transform.eulerAngles.z * 3.5f), 0f);

            GameObject newSeg = PhotonNetwork.Instantiate("mid", insertedPosition, Quaternion.identity);
            newSeg.transform.parent = this.transform.parent;
            newSeg.transform.eulerAngles = new Vector3(0f, 0f, segments[segments.Count - 1].transform.eulerAngles.z);
            segments.Add(newSeg);

            ropeLength++;
            Anchors();
        }
    }
}