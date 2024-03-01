using System.Collections;
using UnityEngine;

public class Movement : MonoBehaviour
{
    [Header("Movement")]
    public GameObject leftLeg;
    public GameObject rightLeg;
    Rigidbody2D leftLegRB;
    Rigidbody2D rightLegRB;


    Animator anim;
    [SerializeField] float speed = 2f;
    [SerializeField] float jumpHeight = 2f;

    [SerializeField] float legWait = .5f;

    private Vector2 _moveInput;
    
    // Start is called before the first frame update
    void Start()
    {
        leftLegRB = leftLeg.GetComponent<Rigidbody2D>();
        rightLegRB = rightLeg.GetComponent<Rigidbody2D>();
        
        anim = GetComponent<Animator>();
        
        InputManager.Instance.onMovementInput.AddListener(UpdateMoveInput);
    }

    private void UpdateMoveInput(Vector2 moveInput)
    {
        _moveInput = moveInput;
    }

    // Update is called once per frame
    void Update()
    {

        if (_moveInput.x != 0)
        {
            if (_moveInput.x > 0)
            {
                anim.Play("WalkLeft");
                StartCoroutine(MoveRight(legWait));
            }
            else
            {
                anim.Play("WalkRight");
                StartCoroutine(MoveLeft(legWait));

            }

        }
        else
        {
            anim.Play("idle");
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            leftLegRB.AddForce(Vector2.up * (jumpHeight * 1000));
            rightLegRB.AddForce(Vector2.up * (jumpHeight * 1000));
        }

    }


    IEnumerator MoveRight(float seconds)
    {
        leftLegRB.AddForce(Vector2.right * (speed * 1000) * Time.deltaTime);
        yield return new WaitForSeconds(seconds);
        rightLegRB.AddForce(Vector2.right * (speed * 1000) * Time.deltaTime);
    }

    IEnumerator MoveLeft(float seconds)
    {
        rightLegRB.AddForce(Vector2.left * (speed * 1000) * Time.deltaTime);
        yield return new WaitForSeconds(seconds);
        leftLegRB.AddForce(Vector2.left * (speed * 1000) * Time.deltaTime);
    }
}