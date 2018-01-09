using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movimiento : MonoBehaviour {

   

    //Variables movimiento
    public float velX = 0.03f;
    public float movX;
    public float inputX;
    public bool mirandoDerecha;
    public float posicionActual;
    

    //Variables de Salto
    public float fuerzaSalto = 100f;
    public Transform pie;
    public float radioPie = 0.05f;
    public LayerMask suelo;
    public bool enSuelo;

    //Variables Agachado
    public bool agachado;

    //Variable Mira pa arriba
    public bool miraArriba;

    //Variables Caida
    Rigidbody2D rb;
    public float caida;

    //Variables Derrape
    public int derrape;
    public int derecha;
    public int izquierda;

    //Variables correr
    public bool run;

    //Variables Turbo
    public bool turbo;
    public int contadorTurbo;

    //Variables Turbo Salto
    public bool turboSalto;

    //Animacion
    Animator animator;

    //Variables caparazon
    public float patada= 500;
    public Transform mano;
    public float radioMano = 0.07f;
    public LayerMask caparazon;
    public bool agarrarCaparazon;
    public GameObject Caparazon;
    public GameObject Mario;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
    }

    void Start () {



	}
	
	// Update is called once per frame
	void FixedUpdate () {

        //MOVIMIENTO HORIZONTAL
        float inputX = Input.GetAxis("Horizontal"); // Almacena el movimiento en el eje X
        movX = transform.position.x + (inputX * velX);//movX será igual a mi posicion en X + el movimiento en el eje X * velX
        posicionActual = movX;//almacena la posicion actual
        if (!agachado && !miraArriba)
        {//Si no estoy agachado y no estoy mirando arriba

            if (inputX > 0)
            {//Si la velocidad en el eje X es mayor que 0
                transform.position = new Vector3(movX, transform.position.y, 0);//mi posicion = movX, la posicion que tenga en y, 0
                transform.localScale = new Vector3(1, 1, 1);//la escala original si me muevo a la derecha
                mirandoDerecha = true;//establece que estoy mirando a la derecha
                animator.SetBool("derrape", false);
            }

            if (inputX < 0)
            {//Si la velocidad en el eje X es menor que 0
                transform.position = new Vector3(movX, transform.position.y, 0);//mi posicion = movX, la posicion que tenga en y, 0
                transform.localScale = new Vector3(-1, 1, 1);//la escala inversa si me muevo a la izquierda
                mirandoDerecha = false;//Establece que no estoy mirando a la derecha
                animator.SetBool("derrape", false);
            }
        }

        if (inputX != 0)
        {
            animator.SetFloat("velX", 1);// Si me muevo le digo al animador que la velocidad en X es 1
        }
        else
        {
            animator.SetFloat("velX", 0);// Si no me muevo le digo al animador que la velocidad en X es 0
        }

        //Salto
        enSuelo = Physics2D.OverlapCircle(pie.position, radioPie, suelo);// Si estoy tocando el suelo enSuelo será true

        if (enSuelo)
        {//Si estoy en el suelo
            animator.SetBool("enSuelo", true);//Le digo al animador que estoy en el suelo
            animator.SetBool("turboSalto", false);
            if (Input.GetKey(KeyCode.C) && !agachado && !miraArriba)
            {//Si pulso la tecla C y no estoy agachado
                GetComponent<Rigidbody2D>().AddForce(new Vector2(0, fuerzaSalto));//Accedo a la velocidad del Rigidbody2D y le añado la fuerza vertical establecida en fuerzaSalto
                animator.SetBool("enSuelo", false);//Le digo al animador que no estoy en el suelo
            }

        }
        else
        {
            animator.SetBool("enSuelo", false);//Le digo al animador que no estoy en el suelo
        }

        //Agacharse
        if (enSuelo&&Input.GetKey(KeyCode.DownArrow))
        {
            animator.SetBool("agachado",true);
            agachado = true;
        }
        else
        {
            animator.SetBool("agachado", false);
            agachado = false;
        }

        //Mira pa arriba 
        if (inputX == 0)
        {
            if (enSuelo && Input.GetKey(KeyCode.UpArrow))
            {
                animator.SetBool("miraArriba", true);
                miraArriba = true;               
            }
            else
            {
                animator.SetBool("miraArriba", false);
                miraArriba = false;
            }
        }


        //Caida
        caida = rb.velocity.y;
        if (caida!=0 || caida==0)
        {
            animator.SetFloat("velY", caida);
        }

        //DERRAPE

        if (inputX == 0)
        {// Si no me muevo o estoy cambiando de direccion
            StartCoroutine(TiempoEspera());//Llamo a la corutina TiempoEspera()
        }
        //DERRAPE DERECHA-->IZQUIERDA

        if (inputX > 0.5f)
        {//Si la velocidad en X es mayor de 0.5
            izquierda = 0;
            derecha = 1;//Le doy a derecha un valor de 1
            derrape = 1;//Le doy a derrape un valor de 1
        }


        if (derrape == 1 && Input.GetKey(KeyCode.LeftArrow))
        {//Si derrape es 1 y pulso izquierda
            Debug.Log("ENTRO");
            if (!Input.GetKey(KeyCode.RightArrow))
            {
                animator.SetBool("derrape", true);//Le digo al animador que estoy derrapando
                animator.SetBool("turbo", false);
                StartCoroutine(TiempoEspera());//Llamo a la corutina TiempoEspera()
                StopCoroutine(Turbo());
            }          
        }

        //DERRAPE IZQUIERDA --> DERECHA

        if (inputX < 0)
        {//Si la velocidad en X es menor que 0            
            izquierda = 1;//Le doy a izquierda un valor de 1
            derrape = -1;//Le doy a derrape un valor de -1
        }


        if (derrape == -1 && Input.GetKey(KeyCode.RightArrow))
        {//Si derrape es 1 y pulso derecha
            Debug.Log("ENTRO EN LA IZQUIERDA");
            if (!Input.GetKey(KeyCode.LeftArrow))
            {
                animator.SetBool("derrape", true);//Le digo al animador que estoy derrapando
                animator.SetBool("turbo", false);
                StartCoroutine(TiempoEspera());//Llamo a la corutina TiempoEspera()
                StopCoroutine(Turbo());

            }
         
        }



        //CORRER

        if (inputX!=0)
        {
            if (Input.GetKey(KeyCode.X) && enSuelo)
            {
                run = true;
                velX = 0.06f;
                animator.SetBool("run", true);                
            }
            else
            {
                velX = 0.03f;
                animator.SetBool("run", false);
                run = false;
                contadorTurbo = 0;
            }
        }else
        {
            run = false;
            animator.SetBool("run", false);
            animator.SetBool("derrape", false);
            contadorTurbo = 0;
        }

        //TURBO

        if (Input.GetKey(KeyCode.X) && run==true && enSuelo)
        {
            StartCoroutine(Turbo());
        }
        else
        {
            turbo = false;
            animator.SetBool("turbo", false);
            StopAllCoroutines();
        }

        //TURBOSALTO

        if (inputX > 0 || inputX < 0)
        {//Si me estoy moviendo
            if (turbo && Input.GetKeyDown(KeyCode.C))
            {//Si turbo está activado y pulso C
                animator.SetBool("turboSalto", true);//Le digo al animador que active turbosalto
            }
        }

        //CAPARAZON

        agarrarCaparazon = Physics2D.OverlapCircle(mano.position, radioMano, caparazon);
        if(agarrarCaparazon && mirandoDerecha)
        {
            if (Input.GetKey(KeyCode.Z))
            {
                Caparazon.transform.parent = Mario.transform;
                Caparazon.GetComponent<Rigidbody2D>().gravityScale = 0;
                Caparazon.GetComponent<Rigidbody2D>().isKinematic = true;
            }else
            {
                Caparazon.GetComponent<Rigidbody2D>().AddForce(new Vector2(patada, 0));
                Caparazon.transform.parent = null;
                Caparazon.GetComponent<Rigidbody2D>().gravityScale = 2;
                Caparazon.GetComponent<Rigidbody2D>().isKinematic = false;
            }
        }

        if (agarrarCaparazon && !mirandoDerecha)
        {
            if (Input.GetKey(KeyCode.Z))
            {
                Caparazon.transform.parent = Mario.transform;
                Caparazon.GetComponent<Rigidbody2D>().gravityScale = 0;
                Caparazon.GetComponent<Rigidbody2D>().isKinematic = true;
            }
            else
            {
                Caparazon.GetComponent<Rigidbody2D>().AddForce(new Vector2(patada*(-1), 0));
                Caparazon.transform.parent = null;
                Caparazon.GetComponent<Rigidbody2D>().gravityScale = 2;
                Caparazon.GetComponent<Rigidbody2D>().isKinematic = false;
            }
        }


    }
    //Fin FIXUPDATE


    public IEnumerator TiempoEspera()
    {//Funcion para que haya un tiempo de espera desde que cambio de dirección hasta que reestablezcan los valores
        yield return new WaitForSeconds(0.3f);//Espero 0.3 "segundos"
        derrape = 0;//Reestablezco derrape
        derecha = 0;//Restablezco derecha
        izquierda = 0;//Reestablezco izquierda
        animator.SetBool("derrape", false);//Le digo al animador que ya no estoy derrapando
        StopAllCoroutines();//Detengo la corrutina 
    }

    public IEnumerator Turbo()
    {//Funcion para activar el turbo
        while (contadorTurbo <= 15)
        {//Mientras contador turbo sea menor que 20
            yield return new WaitForSeconds(1.5f);//espera 1.5 "segundos"
            contadorTurbo++;//Sumo 1 al contador turbo
        }
        turbo = true;//Activo turbo
        animator.SetBool("turbo", true);//Le digo al animador que estoy en turbo
        velX = 0.12f;//Establezco la velocidad en X a 0.12f
        StopAllCoroutines();//Detengo la corrutina 
    }
}
