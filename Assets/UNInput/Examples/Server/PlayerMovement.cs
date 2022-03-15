using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniversalNetworkInput;

/// <summary>
/// Class that control the player's movement using joysticks.
/// </summary>
public class PlayerMovement : MonoBehaviour
{
    /// <summary>
    /// Vector with start position on the scene.
    /// </summary>
    [System.NonSerialized]
    public Vector3 startPosition = Vector3.zero;
    /// <summary>
    /// Current movement on X and Z.
    /// </summary>
    public Vector3 movement = Vector3.zero;
    /// <summary>
    /// current altitude on Y.
    /// </summary>
    [System.NonSerialized]
    public Vector3 gravity = Vector3.zero;
    /// <summary>
    /// Start rotation.
    /// </summary>
    [System.NonSerialized]
    public Quaternion startRotation;
    /// <summary>
    /// Max movement speed.
    /// </summary>
    public float speedMax = 10f;

    /// <summary>
    /// Speed rotation.
    /// </summary>
    [System.NonSerialized]
    public float speedRotation = 90f;
    /// <summary>
    /// Current movement speed.
    /// </summary>
    public float currentSpeed = 0f;

    /// <summary>
    /// Jump force.
    /// </summary>
    [System.NonSerialized]
    public float forceJump = 15f;

    /// <summary>
    /// Gravity force.
    /// </summary>
    [System.NonSerialized]
    public float forceGravity = 50f;

    /// <summary>
    /// If the player is moving.
    /// </summary>
    public bool moving = false;

    /// <summary>
    /// If the player is jumping for the second time.
    /// </summary>
    [System.NonSerialized]
    public bool doubleJumping = false;

    /// <summary>
    /// If the player can double jump.
    /// </summary>
    [System.NonSerialized]
    public bool canDoubleJump = false;

    /// <summary>
    /// If the player can move.
    /// </summary>
    [System.NonSerialized]
    public bool canMove = true;

    /// <summary>
    /// If the player can jump.
    /// </summary>
    public bool canJump = true;

    /// <summary>
    /// If using keyboard.
    /// </summary>
    public bool usingKeyboard = false;

    /// <summary>
    /// Current Clone Movement in X and Z.
    /// </summary>
    Vector3 movementClone = Vector3.zero;

    /// <summary>
    /// Character controller that is used to move.
    /// </summary>
    CharacterController controller;

    public int JoystickID = 0;
    void Start()
    {
        startPosition = transform.position; //Guardando posição inicial
        startRotation = transform.rotation; //Guardando rotação inicial

        controller = GetComponent<CharacterController>(); //Criando atalho para o componente de controle de personagem do jogador
        currentSpeed = speedMax; //Setando velocidade inicial
    }

    /// <summary>
    /// Move the player.
    /// </summary>
    /// <param name="i"> id of the joystic that is going to move this player. </param>
    public void Move(int i)
    {
        if (canMove)//Se o jogador pode se movimentar
        {
            if (usingKeyboard)
            {
                movement = Vector3.zero;
                if (Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.W))
                    movement += Vector3.forward;
                if (Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.S))
                    movement += Vector3.back;
                if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A))
                    movement += Vector3.left;
                if (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D))
                    movement += Vector3.right;
                //Setando movimentação
            }
            else
                movement = new Vector3(UNInput.GetAxis(i, AxisCode.LSH), 0f, UNInput.GetAxis(i, AxisCode.LSV)); //Setando movimentação

            if (movement.magnitude < 0.2f) //Se a movementação for pequena(correção do analógico)
                movement = Vector3.zero; //Zerando movimentação
            else
                transform.rotation = Quaternion.identity * Quaternion.LookRotation(movement, Vector3.up); //Atualizando a rotação

            if (controller.isGrounded) //Se estiver colidindo com o chão
            {
                doubleJumping = false; //Pode pular a segunda vez
                if (canJump)//Se pode pular
                    if (UNInput.GetButtonDown(i, ButtonCode.A) || (Input.GetKeyDown(KeyCode.KeypadEnter) && usingKeyboard)) //Se o jogador apertar o botão de pulo
                        Jump(); //Função de pulo
            }
            else //Se não estiver no chão
            {
                if (canDoubleJump)//Se puder pular uma segunda vez
                    if (!doubleJumping)//Se ainda não pulou o segundo pulo
                        if (UNInput.GetButtonDown(i, ButtonCode.A) || (Input.GetKeyDown(KeyCode.KeypadEnter) && usingKeyboard)) //Se o jogador apertar o botão de pulo
                        {
                            Jump(); //Função de pulo
                            doubleJumping = true; //pulou segundo pulo
                        }
            }

            gravity += Vector3.down * forceGravity * Time.deltaTime; //Adicionando a gravidade

            controller.Move(((transform.rotation * Vector3.forward * currentSpeed) * movement.magnitude * Time.deltaTime) + (gravity * Time.deltaTime)); //Movimentando
        }
        else
            controller.Move(gravity * Time.deltaTime); //Somente Adicionar a gravidade

        if (gravity.y < -forceGravity) //Se a gravidade for maior que a força gravitacional
            gravity = Vector3.down * forceGravity; //Setando o valor máximo da gravidade
    }

    /// <summary>
    /// Move the clone.
    /// </summary>
    /// <param name="i"> ID of the joystic that is going to move this player. </param>
    /// <param name="controller"> CharacterController of clone. </param>
    public void MoveClone(ref int i, GameObject clone)
    {
        if (clone != null)
        {
            CharacterController controllerClone = clone.GetComponent<CharacterController>();
            if (usingKeyboard)
                movementClone = new Vector3(Input.GetAxis("Horizontal"), 0f, Input.GetAxis("Vertical")); //Setando movimentação
            else
                movementClone = new Vector3(UNInput.GetAxis(i, "Right Stick Horizontal"), 0f, UNInput.GetAxis(i, "Right Stick Vertical")); //Setando movimentação

            if (movementClone.magnitude < 0.2f) //Se a movementação for pequena(correção do analógico)
                movementClone = Vector3.zero; //Zerando movimentação
            else
                clone.transform.rotation = Quaternion.identity * Quaternion.LookRotation(movementClone, Vector3.up); //Atualizando a rotação

            gravity += Vector3.down * forceGravity * Time.deltaTime; //Adicionando a gravidade

            controllerClone.Move(((clone.transform.rotation * Vector3.forward * currentSpeed) * movementClone.magnitude * Time.deltaTime) + (gravity * Time.deltaTime)); //Movimentando
            if (gravity.y < -forceGravity) //Se a gravidade for maior que a força gravitacional
                gravity = Vector3.down * forceGravity; //Setando o valor máximo da gravidade
        }
        //      else
        // GetComponent<PlayerPickup>().usingClone = false;
    }

    /// <summary>
    /// player's jump.
    /// </summary>
    public void Jump()
    {
        gravity = Vector3.up * forceJump; //Adicionando a força do pulo no eixo Y de movement
    }

    public void Update()
    {
        Move(JoystickID);
    }

    public void Spawn()
    {
        transform.position = startPosition;
        transform.rotation = startRotation;
    }
}
