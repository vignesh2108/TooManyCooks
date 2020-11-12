using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class PhysicsLink : NetworkBehaviour
{
    public Rigidbody rb;

    [SyncVar]//all the essental varibles of a rigidbody
    public Vector3 Velocity;
    [SyncVar]
    public Quaternion Rotation;
    [SyncVar]
    public Vector3 Position;
    [SyncVar]
    public Vector3 AngularVelocity;

    private NetworkIdentity networkIdentity;

    private void Start()
    {
        networkIdentity = GetComponent<NetworkIdentity>();
    }

    void FixedUpdate()
    {
        if (networkIdentity.isServer)//if we are the server update the varibles with our cubes rigidbody info
        {
            Position = rb.position;
            Rotation = rb.rotation;
            Velocity = rb.velocity;
            AngularVelocity = rb.angularVelocity;
            rb.position = Position;
            rb.rotation = Rotation;
            rb.velocity = Velocity;
            rb.angularVelocity = AngularVelocity;
        }
        if (networkIdentity.isClient)//if we are a client update our rigidbody with the servers rigidbody info
        {
            rb.position = Position+Velocity*(float)NetworkTime.rtt;//account for the lag and update our varibles
            rb.rotation = Rotation*Quaternion.Euler(AngularVelocity * (float)NetworkTime.rtt);
            rb.velocity = Velocity;
            rb.angularVelocity = AngularVelocity;
        }
    }
    [Command]//function that runs on server when called by a client
    public void CmdResetPose()
    {
        rb.position = new Vector3(0,1,0);
        rb.velocity = new Vector3();
    }
    public void ApplyForce(Vector3 force, ForceMode FMode)//apply force on the client-side to reduce the appearance of lag and then apply it on the server-side
    {
        //rb.AddForce(force, FMode);

        var velocity = rb.velocity;
        velocity = new Vector3(force.x, velocity.y, force.z);
        rb.velocity = velocity;
        CmdApplyForce(velocity, FMode);
        //CmdApplyForce(force, FMode);

    }
    [Command]//function that runs on server when called by a client
    public void CmdApplyForce(Vector3 force,ForceMode FMode)
    {
        //rb.AddForce(force, FMode);//apply the force on the server side
        rb.velocity = force;
    }
}
