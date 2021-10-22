using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

namespace Com.MyCompany.MyGame
{
	public class RopeSegment : MonoBehaviourPunCallbacks
	{
		private void OnTriggerEnter2D(Collider2D collision)
		{
			if (collision.name.StartsWith("coin"))
			{
				switch (PlayerManager.instance.teamNumber)
				{
					case 1:
						PlayerManager.instance.coins1++;
						break;
					case 2:
						PlayerManager.instance.coins2++;
						break;
					case 3:
						PlayerManager.instance.coins3++;
						break;
					case 4:
						PlayerManager.instance.coins4++;
						break;
				}

				Destroy(collision.gameObject);

				Vector3 pos = new Vector3(Random.Range(-36f, 36f), Random.Range(36f, 36f), 5f);
				GameObject createCoin = PhotonNetwork.Instantiate("coin", pos, Quaternion.identity);
				createCoin.transform.SetParent(GameObject.Find("Coins").GetComponent<Transform>());
			}

			for (int i = 0; i < 11; i++)
			{
				if (collision.name == "" + i)
				{
					int cName = int.Parse(collision.gameObject.name);
					int tName = int.Parse(gameObject.name);

					if (tName - cName > RopeManager.instance.ropeLength)
					{
						int mName = (cName + tName) / 2;

						Vector2 cPos = collision.transform.position;
						Vector2 tPos = gameObject.transform.position;
						Transform mPos = transform.parent.GetChild(mName);

						Vector2 center = new Vector2((cPos.x + tPos.x + mPos.position.x) / 3f, (cPos.y + tPos.y + mPos.position.y) / 3f);
						float radius = Mathf.Sqrt(((tPos.x - center.x) * (tPos.x - center.x)) + ((tPos.y - center.y) * (tPos.y - center.y)));

						Collider2D[] circle = Physics2D.OverlapCircleAll(center, radius);
						foreach (Collider2D c in circle)
						{
							if (c.gameObject.CompareTag("Player") && !photonView.IsMine && c.transform.parent != this.transform.parent)
							{
								c.gameObject.transform.parent.gameObject.SetActive(false);
							}
						}
					}
				}
			}
		}
	}
}
