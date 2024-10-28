using UnityEngine;
using UnityEngine.SceneManagement; // sahneyi yani seviyeyi tekrar yüklemek için gerekli

public class CollisionHandler : MonoBehaviour
{
    [SerializeField] private float levelLoadDelay = 1.5f;
    [SerializeField] private AudioClip success;
    [SerializeField] private AudioClip crash;
    [SerializeField] private ParticleSystem successParticles;
    [SerializeField] private ParticleSystem crashParticles;

    private AudioSource audioSource;

    private bool collisionDisabled = false; // collider başlangıçta devre dışı değil
    private bool isTransitioning = false; // çarpışma sesini bir kez çalmak için
    // sadece ilk çarpışmada çalmak için

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    private void Update()
    {
        RespondToDebugKeys();
    }

    private void RespondToDebugKeys()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            // Bir sonraki seviyeyi yüklemek için L tuşuna basılır.
            LoadNextLevel();
        }
        else if (Input.GetKeyDown(KeyCode.C))
        {
            // Kullanıcı C tuşuna bastığında çarpışmaları devre dışı bırakmaya çalışıyoruz. 
            // Böylece duvarlara çarptığımızda ölmeden oynayabiliriz.

            // çarpışmayı engellemek için collider'ı devre dışı bırakıyorum.
            // veya C tuşuna tekrar bastığında aktifleştiriyorum.
            collisionDisabled = !collisionDisabled;
        }
    }

    private void OnCollisionEnter(Collision other)
    {

        if (isTransitioning || collisionDisabled)
        {
            // collisionDisable = true ise bulunduğu seviye için çarpışmalar görmezden gelinir
            // isTransitioning = true ise ilk çarpışma dışında diğer çarpışmalar görmezden gelinir
            return;
        }

        // bu script dosyasına sahip nesnenin(roketin) çarpıştığı nesne other'dır.
        switch (other.gameObject.tag)
        {
            case "Friendly": // Launch Pad (fırlatma rampası)
                Debug.Log("Friendly");
                break;
            case "Finish": // Landing Pad (iniş rampası)
                StartSuccessSequence();
                break;
            default:
                StartCrashSequence();
                break;
        }

    }

    private void StartSuccessSequence()
    {
        isTransitioning = true;
        audioSource.Stop(); // Başarı sesini çalmadan önce, roketle ilgili tüm sesleri durdur.
        // seviyeyi geçtiğimde SFX(ses) (Landing Pad(iniş rampası)'e çarptığında)
        audioSource.PlayOneShot(success);
        // seviyeyi geçtiğimde particle effect oynatılacak
        successParticles.Play();

        // seviyeyi geçtiğimde roketin hareket etmesini sağlayan Movement.cs bileşenini false yapacağım
        GetComponent<Movement>().enabled = false;
        // metodu 1 saniye geçtikten sonra çağırır.
        Invoke("LoadNextLevel", levelLoadDelay);
    }

    private void StartCrashSequence()
    {
        isTransitioning = true;
        audioSource.Stop();
        // engele çarptığında SFX(ses) eklenecek
        audioSource.PlayOneShot(crash);
        // engele çarptığında particle effect oynatılacak
        crashParticles.Play();

        // engele çarptığımda roketin hareket etmesini sağlayan Movement.cs bileşenini false yapacağım
        GetComponent<Movement>().enabled = false;
        // metodu 1 saniye geçtikten sonra çağırır.
        Invoke("ReloadLevel", levelLoadDelay);
    }

    private void LoadNextLevel()
    {
        // şu anda bulunduğu sahnenin/seviyenin indeksini geri döndür.
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        // bir sonraki seviye/sahne
        int nextSceneIndex = currentSceneIndex + 1;
        // toplam sahne/seviye sayısı
        int totalScene = SceneManager.sceneCountInBuildSettings;

        if (nextSceneIndex == totalScene)
        {
            // tüm seviyeleri bitirmişse ilk seviyeye geri dönsün
            nextSceneIndex = 0;
        }

        // son seviyeyi bitirmemişse bir sonraki seviyeye geçsin
        SceneManager.LoadScene(nextSceneIndex);
    }

    private void ReloadLevel()
    {
        // şu anda bulunduğu sahnenin/seviyenin indeksini geri döndür.
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        // parantez içerisindeki indekse sahip sahne yüklenir.
        SceneManager.LoadScene(currentSceneIndex);
    }
}

// Invoke kullanarak sahneyi/seviyeyi gecikmeli yükleyeceğiz
// Invoke("MethodName", delayInSeconds);
// metodu belirlenen saniye geçtikten sonra çağırır.

/*
Friendly ve Finish etiketli nesnelere çarpmakta bir sakınca yok.
roket diğer nesnelere çarparsa yok edilir.
roket Finish etiketli nesneye çarparsa seviye tamamlanır.

Launch Pad (fırlatma rampası) = Friendly
Landing Pad (iniş rampası) = Finish

bu etiketler dışındaki nesnelere çarparsa sahne baştan yüklenecek, roket yeniden canlandırılacak
*/