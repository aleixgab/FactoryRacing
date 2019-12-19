﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerController : NetworkBehaviour
{
    private Animator animator;
    private TextMesh nameLabel;

    private CustomNetworkManager networkManager;

    const float RUNNING_SPEED = 10.0f;
    const float ROTATION_SPEED = 180.0f;

    public Transform finishTransform;

    // Name sync /////////////////////////////////////
    [SyncVar(hook = "SyncNameChanged")]
    string playerName = "Player";

    [Command]
    void CmdChangeName(string name) { playerName = name; }

    void SyncNameChanged(string name) { nameLabel.text = name; }


    // Prefab sync /////////////////////////////////////
    [Command]
    void CmdChangePlayerPrefab(int prefabIndex)
    {
        networkManager.ChangePlayerPrefab(this, prefabIndex);
    }
    [Command]
    void CmdChangePlayerPrefab(int prefabIndex, Transform parent)
    {
        networkManager.ChangePlayerPrefab(this, prefabIndex, parent);
    }

    // Pumpking sync /////////////////////////////////////
    [Command]
    void CmdAddPumpkin(int prefabIndex)
    {
        networkManager.AddObject(5, this.transform);
    }
        
    // OnGUI /////////////////////////////////////////
    private void OnGUI()
    {
        if (isLocalPlayer)
        {
            GUILayout.BeginArea(new Rect(Screen.width - 260, 10, 250, Screen.height - 20));

            string prevPlayerName = playerName;
            playerName = GUILayout.TextField(playerName);
            if (playerName != prevPlayerName)
            {
                if (nameLabel != null)
                {
                    CmdChangeName(playerName);
                }
            }

            GUILayout.EndArea();
        }
    }

    // Finish sync ////////////////////////////////
    [SyncVar(hook = "SyncFinishLap")]
    bool isFinished = false;

    [Command]
    void CmdFinishLap() { isFinished = true; }
    void SyncFinishLap() 
    {
        finishTransform.gameObject.SetActive(true);
        CmdChangePlayerPrefab(networkManager.playerPrefabIndex + 3);
    }


    // Animation sync ////////////////////////////////
    [SyncVar(hook = "OnSetAnimation")]
    string animationName;

    void SetAnimation(string animName)
    {
        OnSetAnimation(animName);
        CmdSetAnimation(animName);
    }

    [Command]
    void CmdSetAnimation(string animName) { animationName = animName; }


    void OnSetAnimation(string animName)
    {
        if (animationName == animName) return;
        animationName = animName;

        animator.SetBool("Idling", false);
        animator.SetBool("Running", false);
        animator.SetBool("Running backwards", false);
        animator.ResetTrigger("Jumping");
        animator.ResetTrigger("Kicking");

        if (animationName == "Idling") animator.SetBool("Idling", true);
        else if (animationName == "Running") animator.SetBool("Running", true);
        else if (animationName == "Running backwards") animator.SetBool("Running backwards", true);
        else if (animationName == "Jumping") animator.SetTrigger("Jumping");
        else if (animationName == "Kicking") animator.SetTrigger("Kicking");
    }


    // Lifecycle methods ////////////////////////////

    // Use this for initialization
    void Start ()
    {
        //animator = GetComponent<Animator>();
        //nameLabel = transform.Find("Label").gameObject.GetComponent<TextMesh>();

        networkManager = NetworkManager.singleton.GetComponent<CustomNetworkManager>();
        if (networkManager == null)
            Debug.Log(":(");
        else
            Debug.Log(":)");
    }

    // Update is called once per frame
    void Update()
    {
        if (isLocalPlayer)
        {
            Vector3 translation = new Vector3();
            float angle = 0.0f;

            float horizontalAxis = Input.GetAxis("Horizontal");
            float verticalAxis = Input.GetAxis("Vertical");

            if (verticalAxis > 0.0)
            {
                SetAnimation("Running");
                translation += new Vector3(0.0f, 0.0f, verticalAxis * RUNNING_SPEED * Time.deltaTime);
                transform.Translate(translation);
            }
            else if (verticalAxis < 0.0)
            {
                SetAnimation("Running backwards");
                translation += new Vector3(0.0f, 0.0f, verticalAxis * RUNNING_SPEED * Time.deltaTime * 0.5f);
                transform.Translate(translation);
            }
            else
            {
                SetAnimation("Idling");
            }

            if (horizontalAxis > 0.0f)
            {
                angle = horizontalAxis * Time.deltaTime * ROTATION_SPEED;
                transform.Rotate(new Vector3(0.0f, 1.0f, 0.0f), angle);
            }
            else if (horizontalAxis < 0.0f)
            {
                angle = horizontalAxis * Time.deltaTime * ROTATION_SPEED;
                transform.Rotate(new Vector3(0.0f, 1.0f, 0.0f), angle);
            }

            if (Input.GetButtonDown("Jump"))
            {
                SetAnimation("Jumping");
            }

            if (Input.GetButtonDown("Fire1"))
            {
                SetAnimation("Kicking");
            }

            if (nameLabel)
            {
                nameLabel.transform.rotation = Quaternion.identity;
            }
        }
    }
    private void OnDestroy()
    {
    }
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log(other.tag.ToString());
        if(other.CompareTag("Finish"))
        {
            CmdFinishLap();
        }
    }
}
