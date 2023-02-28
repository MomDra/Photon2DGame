using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine.UI;

public class PlayerController : MonoBehaviourPunCallbacks, IPunObservable
{
    private Rigidbody2D rb;
    private Animator animator;
    private SpriteRenderer sr;
    private PhotonView pv;
    private TextMeshProUGUI nickNameText;
    private Image healthImage;

    private bool isGround;
    private Vector3 curPos;

    private float axis;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        sr = GetComponent<SpriteRenderer>();
        pv = GetComponent<PhotonView>();
        nickNameText = GetComponentInChildren<TextMeshProUGUI>();
        healthImage = GetComponentsInChildren<Image>()[1];

        nickNameText.text = pv.Owner.NickName;
        nickNameText.color = pv.IsMine ? Color.green : Color.red;
    }

    private void Update()
    {
        if (!pv.IsMine) return;

        axis = Input.GetAxisRaw("Horizontal");

        if (axis != 0)
        {
            animator.SetBool("walk", true);
            pv.RPC("FlipXRPC", RpcTarget.AllBuffered, axis);
        }
        else animator.SetBool("walk", false);
    }

    private void FixedUpdate()
    {
        if (!pv.IsMine) return;

        rb.velocity = new Vector2(4 * axis, rb.velocity.y);
    }

    [PunRPC]
    private void FlipXRPC(float axis)
    {
        sr.flipX = axis == -1;
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {

    }
}
