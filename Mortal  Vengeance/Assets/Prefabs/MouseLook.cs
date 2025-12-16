using UnityEngine;

public class MouseLook : MonoBehaviour
{
    // Sensibilitatea mouse-ului: Cât de repede se mi?c? camera la o mi?care mic? a mouse-ului.
    [Header("Setari Sensibilitate")]
    [SerializeField] private float mouseSensitivity = 100f;

    // Referin?a la transform-ul corpului juc?torului (parintele camerei)
    // Acesta va fi rotit pe axa Y.
    [Header("Referinta Player Body")]
    public Transform playerBody;

    // Stoc?m rota?ia curent? pe axa X (sus/jos)
    private float xRotation = 0f;

    void Start()
    {
        // Bloc?m cursorul în centrul ecranului ?i îl facem invizibil
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        // 1. Citirea intr?rii de la mouse

        // Citirea mi?c?rii orizontale ?i verticale a mouse-ului
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        // 2. Rota?ia Vertical? (Sus/Jos) - Axa X

        // Sc?dem mi?carea vertical?, deoarece mi?carea mouse-ului în sus
        // ar trebui s? roteasc? camera în jos (inversare axa Y)
        xRotation -= mouseY;

        // Limit?m rota?ia pe axa X (Pitch) pentru a preveni r?sucirea capului (flip-ul camerei)
        // (De exemplu, între -90 grade - privit în sus, ?i 90 grade - privit în jos)
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        // Aplic?m rota?ia pe axa X (Pitch) la transform-ul Camerei
        // Camera se rote?te doar pe axa local? X.
        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);

        // 3. Rota?ia Orizontal? (Stânga/Dreapta) - Axa Y

        // Rotim întregul corp al juc?torului (playerBody) pe axa Y.
        // Acest lucru asigur? c? mi?carea de forward a juc?torului se aliniaz? cu privirea.
        playerBody.Rotate(Vector3.up * mouseX);
    }

    // Asigur?-te c? cursorul este eliberat la ie?irea din joc
    void OnApplicationQuit()
    {
        Cursor.lockState = CursorLockMode.None;
    }
}