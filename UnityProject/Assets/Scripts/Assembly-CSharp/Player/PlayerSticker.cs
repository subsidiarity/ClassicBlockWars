using UnityEngine;
using System.Collections;

/*
 * This file handles the logic of players sticking to moving cars. As for why it's called
 * "PlayerSticker" sticker is just fun to type and I don't normally get to type it too often.
 *
 * Players stick to anything the "Feet" layer collides with. Players can only stick to one object
 * at a time. Players cannot stick to objects with a high rotation because this is just meant to
 * be used on cars and players should fall off sideways cars.
 */
public class PlayerSticker : MonoBehaviour
{
    /* The player that is sticking. */
    public PlayerBehavior playerScript;

    public BoxCollider collider;

    /* The object the player is bound to. */
    private Rigidbody boundObject;

    /* If the player is currently on the bound object. */
    private bool grounded;

    /* The desired point of the player relative to the bound object. */
    private Vector3 DesiredPoint;

    /* The velocity inherited by the player at the last touch of "boundObject". */
    private Vector3 lastVelocity;

    // TODO: Try and inlined the functions that this calls.
    private void FixedUpdate()
    {
        Debug.Log("Update");

        if (boundObject == null)
        {
            Debug.Log("Not bound.");
            return;
        }

        // TODO: Comment
        RaycastHit hit;
        Ray ray = new Ray (
            playerScript.gameObject.transform.position,
            boundObject.transform.position - playerScript.gameObject.transform.position
        );

        bool was_hit = collider.Raycast (
            ray,
            out hit,
            CompilationSettings.PlayerStickMaxDeltaDistance
        );

        Debug.Log(was_hit);

        if (Mathf.Abs(boundObject.transform.rotation.x) > CompilationSettings.PlayerStickMaxRotation
        || Mathf.Abs(boundObject.transform.rotation.z) > CompilationSettings.PlayerStickMaxRotation
        || !was_hit)
        {
            DetachSticker();
            return;
        }

        if (grounded)
        {
            UpdateGroundedSticker();
        }
        else
        {
            UpdateFlyingSticker();
        }
    }

    /*
     * This updates a sticker that is bound to a player that is standing on that object. Only called
     * when the bound object is not null.
     */
    private void UpdateGroundedSticker()
    {
        Debug.Log("Updating grounded sticker.");

        // TODO: Attach to point
        // playerScript.transform.position += boundObject.velocity;

        DesiredPoint = boundObject.transform.position - playerScript.transform.position;
    }

    /* This updates a sticker that is bound to a player that is in the air. */
    private void UpdateFlyingSticker()
    {
        // TODO: Check if too far away.
    }

    private void DetachSticker()
    {
        Debug.Log("Detaching");
        Rigidbody player = playerScript.gameObject.GetComponent<Rigidbody>();
        player.velocity += lastVelocity;
        boundObject = null;
    }

    /* This cannot be called if currently sticking to an object. */
    private void AttachSticker(Rigidbody to_bind)
    {
        Debug.Log("Sticked");
        boundObject = to_bind;
        // TODO: Move the player to the desired point and rotation
    }

    private void OnTriggerExit()
    {
        Debug.Log("Ground ended");
        grounded = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Collision");
        if (boundObject == null)
        {
            AttachSticker(other.GetComponent<Rigidbody>());
        }
        else if (boundObject != other.GetComponent<Rigidbody>())
        {
            Debug.Log("Player dual car collide, detaching.");
            DetachSticker();
        }
        else
        {
            Debug.Log("Grounded");
            grounded = true;
        }
    }

    public void Detach()
    {
        if (boundObject == null)
        {
            return;
        }
        // TODO: The rest of this stuff.
    }
}