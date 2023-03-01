using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class Bullet : MonoBehaviourPunCallbacks
{
    private PhotonView pv;
    private int dir;

    private void Awake()
    {
        pv = GetComponent<PhotonView>();
    }

    private void Start()
    {
        Destroy(gameObject, 3.5f);
    }

    private void Update()
    {
        transform.Translate(Vector3.right * 7 * Time.deltaTime * dir);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Ground") DestroyRPC();
        // pv.RPC("DestroyRPC", RpcTarget.AllBuffered);

        if (!pv.IsMine && other.tag == "Player" && other.GetComponent<PhotonView>().IsMine)
        {
            other.GetComponent<PlayerController>().Hit();
            pv.RPC("DestroyRPC", RpcTarget.AllBuffered);
        }
    }

    [PunRPC]
    private void DirRPC(int dir)
    {
        this.dir = dir;
    }

    [PunRPC]
    private void DestroyRPC()
    {
        Destroy(gameObject);
    }
}
