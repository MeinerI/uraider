﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Combat : StateBase<PlayerController>
{
    private Transform target;
    private Weapon leftPistol;
    private Weapon rightPistol;

    public override void OnEnter(PlayerController player)
    {
        player.ForceWaistRotation = true;
        player.Anim.SetBool("isCombat", true);
        //player.Anim.applyRootMotion = false;
        player.Stats.ShowCanvas();
        player.pistolLHand.SetActive(true);
        player.pistolRHand.SetActive(true);
        player.pistolLLeg.SetActive(false);
        player.pistolRLeg.SetActive(false);
        leftPistol = player.pistolLHand.GetComponent<Weapon>();
        rightPistol = player.pistolRHand.GetComponent<Weapon>();
    }

    public override void OnExit(PlayerController player)
    {
        player.camController.State = CameraState.Grounded;
        player.WaistTarget = null;
        player.ForceWaistRotation = false;
        player.Anim.SetBool("isCombat", false);
        player.Stats.HideCanvas();
        player.Anim.SetBool("isTargetting", false);
        player.pistolLHand.SetActive(false);
        player.pistolRHand.SetActive(false);
        player.pistolLLeg.SetActive(true);
        player.pistolRLeg.SetActive(true);
    }

    public override void Update(PlayerController player)
    {
        if (!Input.GetButton("Draw Weapon"))
        {
            player.StateMachine.GoToState<Locomotion>();
            return;
        }

        CheckForTargets(player);

        float moveSpeed = Input.GetButton("Walk") ? player.walkSpeed
            : player.runSpeed;

        player.Anim.SetFloat("Right", Input.GetAxis("Horizontal"));
        player.Anim.SetFloat("Forward", Input.GetAxis("Vertical"));

        player.MoveGrounded(moveSpeed);

        if (Input.GetButtonDown("Jump"))
            player.StateMachine.GoToState<Jumping>();

        if (target != null)
        {
            player.RotateToTarget(target.position);
            player.WaistRotation = Quaternion.LookRotation(
                (target.position - player.transform.position).normalized, Vector3.up);
            player.camController.State = CameraState.Combat;
            player.Anim.SetBool("isTargetting", true);
        }
        else
        {
            player.WaistRotation = player.transform.rotation;
            player.camController.State = CameraState.Grounded;
            player.RotateToVelocityGround();
            player.Anim.SetBool("isTargetting", false);
        }

        player.Anim.SetBool("isFiring", Input.GetMouseButton(0));
    }

    private void CheckForTargets(PlayerController player)
    {
        Collider[] hitColliders = Physics.OverlapSphere(player.transform.position, 10f);
        foreach (Collider c in hitColliders)
        {
            if (c.gameObject.CompareTag("Enemy"))
            {
                target = c.gameObject.transform;
                break;
            }
            else
            {
                target = null;
            }
        }
    }
}
