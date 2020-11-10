using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum RampDirection
{
    NONE,
    UP,
    DOWN
}


public class OpossumBehaviour : MonoBehaviour
{
    public float runForce;
    public Rigidbody2D rigidbody;
    public Transform lookInFrontPoint;
    public Transform lookAheadPoint;
    public LayerMask collisionWallLayer;
    public LayerMask collisionGroundLayer;
    public bool isGroundAhead;
    public bool onRamp;
    public RampDirection rampDirection;

    // Start is called before the first frame update
    void Start()
    {
        rigidbody = GetComponent<Rigidbody2D>();
        rampDirection = RampDirection.NONE;       
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        _LookInFront();
        _LookAhead();
        _Move();
    }


    private void _LookInFront()
    {
        var wallHit = Physics2D.Linecast(transform.position, lookInFrontPoint.position, collisionWallLayer);

        if (wallHit)
        {
            if(!wallHit.collider.CompareTag("Ramps") && transform.rotation.z == 0.0f)
            {
                if (!onRamp)
                {
                    _FlipX();
                }

                rampDirection = RampDirection.DOWN;
            }
            else
            {
                rampDirection = RampDirection.UP;
            }
            
        }

        Debug.DrawLine(transform.position, lookInFrontPoint.position, Color.red);
    }

    private void _LookAhead()
    {
        var groundHit = Physics2D.Linecast(transform.position, lookAheadPoint.position, collisionGroundLayer);

        if (groundHit)
        {
            if (groundHit.collider.CompareTag("Ramps"))
            {
                onRamp = true;
            }

            if (groundHit.collider.CompareTag("Platforms"))
            {
                onRamp = false; ;
            }

            isGroundAhead = true;
        }
        else
        {
            isGroundAhead = false;
        }

        Debug.DrawLine(transform.position, lookAheadPoint.position, Color.green);
    }

    private void _Move()
    {
        if(isGroundAhead)
        {
            rigidbody.AddForce(Vector2.left * runForce * Time.deltaTime * transform.localScale.x);

            if(onRamp)
            {
                if(rampDirection == RampDirection.UP)
                {
                    rigidbody.AddForce(Vector2.up * runForce * 0.5f * Time.deltaTime);
                }
                else
                {
                    rigidbody.AddForce(Vector2.down * runForce * 0.25f * Time.deltaTime);
                }

                //transform.rotation = Quaternion.Euler(0.0f, 0.0f, -26.0f);
                StartCoroutine(Rotate());
            }
            else
            {
                //transform.rotation = Quaternion.Euler(0.0f, 0.0f, 0.0f);
                StartCoroutine(Normalize());
            }
            

            rigidbody.velocity *= 0.90f;
        }
        else
        {
            _FlipX();
        }        
    }
    
    IEnumerator Rotate()
    {
        yield return new WaitForSeconds(0.05f);
        transform.rotation = Quaternion.Euler(0.0f, 0.0f, -26.0f);
    }

    IEnumerator Normalize()
    {
        yield return new WaitForSeconds(0.05f);
        transform.rotation = Quaternion.Euler(0.0f, 0.0f, 0.0f);
    }

    private void _FlipX()
    {
        transform.localScale = new Vector3(transform.localScale.x * -1.0f, transform.localScale.y, transform.localScale.z);
    }
}
