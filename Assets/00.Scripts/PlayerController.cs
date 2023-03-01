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

        isGround = Physics2D.OverlapCircle((Vector2)transform.position + new Vector2(0, -0.5f), 0.07f, 1 << LayerMask.NameToLayer("Ground"));
        animator.SetBool("jump", !isGround);
        if (Input.GetKeyDown(KeyCode.W) && isGround) pv.RPC("JumpRPC", RpcTarget.All);

        if (Input.GetKeyDown(KeyCode.Space))
        {
            PhotonNetwork.Instantiate("Bullet", transform.position + new Vector3(sr.flipX ? -0.4f : 0.4f, -0.11f, 0), Quaternion.identity).GetComponent<PhotonView>().RPC("DirRPC", RpcTarget.All, sr.flipX ? -1 : 1);
            animator.SetTrigger("shot");
        }
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

    [PunRPC]
    private void JumpRPC()
    {
        rb.velocity = Vector2.zero;
        rb.AddForce(Vector2.up * 700);
    }

    [PunRPC]
    private void DestroyRPC()
    {
        Destroy(gameObject);
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(healthImage.fillAmount);
        }
        else
        {
            healthImage.fillAmount = (float)stream.ReceiveNext();
            Debug.Log("Receive");
        }
    }

    public void Hit()
    {
        healthImage.fillAmount -= 0.1f;
        if (healthImage.fillAmount <= 0)
        {
            GameObject.Find("Canvas").transform.Find("RespawnPanel").gameObject.SetActive(true);
            pv.RPC("DestroyRPC", RpcTarget.AllBuffered);
        }
    }
}
