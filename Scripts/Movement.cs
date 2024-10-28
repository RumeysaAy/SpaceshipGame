using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    // Roketin havada ne kadar hızlı ilerleyeceğini ayarlamamızı sağlar
    [SerializeField] private float mainThrust = 3f; // yukarıya doğru itme kuvveti: (0, 1, 0) * mainThrust = (0, mainThrust, 0)
    [SerializeField] private float rotationThrust = 3f; // dönme kuvveti

    [SerializeField] private AudioClip mainEngine;
    [SerializeField] private ParticleSystem mainEngineParticles;
    [SerializeField] private ParticleSystem leftThrusterParticles;
    [SerializeField] private ParticleSystem rightThrusterParticles;

    private Rigidbody rigidbody;
    private AudioSource audioSource;

    // Start is called before the first frame update
    void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        ProcessThrust(); // yukarıya doğru ilerlemek için
        ProcessRotation(); // sağa ya da sola dönmek için
    }

    private void ProcessThrust()
    {
        if (Input.GetKey(KeyCode.Space))
        {
            // space tuşuna basılı tutulursa roket yukarı doğru ilerlemeye devam eder
            StartThrusting();
        }
        else
        {
            StopThrusting();
        }
    }

    private void ProcessRotation()
    {
        if (Input.GetKey(KeyCode.A))
        {
            // A tuşuna basılı tutulursa roket sola dönmeye devam eder
            RotateLeft();
        }
        else if (Input.GetKey(KeyCode.D))
        {
            // D tuşuna basılı tutulursa roket sağa dönmeye devam eder
            RotateRight();
        }
        else
        {
            StopRotating();
        }
    }

    private void StartThrusting()
    {
        // Roketin yukarı doğru ilerlemesi için rigidbody aracılığıyla rokete kuvvet uygulayacağız
        rigidbody.AddRelativeForce(Vector3.up * mainThrust * Time.deltaTime); // Vector3.up = (x, y, z) = (0, 1, 0)
                                                                              // oyuncuya y ekseninde pozitif yönde 1 birimlik kuvvet uygulanır: rigidbody.AddRelativeForce(Vector3.up);
        if (!audioSource.isPlaying)
        {
            audioSource.PlayOneShot(mainEngine);
        }

        if (!mainEngineParticles.isPlaying)
        {
            mainEngineParticles.Play();
        }
    }

    private void StopThrusting()
    {
        audioSource.Stop();
        mainEngineParticles.Stop();
    }

    private void RotateLeft()
    {
        ApplyRotation(rotationThrust);

        if (!leftThrusterParticles.isPlaying)
        {
            leftThrusterParticles.Play();
        }
    }

    private void RotateRight()
    {
        ApplyRotation(-rotationThrust);

        if (!rightThrusterParticles.isPlaying)
        {
            rightThrusterParticles.Play();
        }
    }

    private void StopRotating()
    {
        leftThrusterParticles.Stop();
        rightThrusterParticles.Stop();
    }

    private void ApplyRotation(float rotationThisFrame)
    {
        // Uçarken, herhangi bir şeye çarparsam, kontrolümü bozar, böylece artık mantıklı bir şekilde dönemem.
        rigidbody.freezeRotation = true; // bu yüzden dönmeyi durduruyorum
        transform.Rotate(Vector3.forward * rotationThisFrame * Time.deltaTime);
        // Vector3.forward: Vector3(0, 0, 1)
        rigidbody.freezeRotation = false; // dönmeye devam et
    }
}

// GetKey: tuşa basılı tutuyorsa true değerini döndürür.
// GetKeyDown: tuşa bastığı anda true değerini döndürür.
// GetKeyUp: tuştan elini çektiği anda true değerini döndürür.

// Roketin uçabilmesi için rigidbody eklemem gerekiyor. Rigidbody nesnelere fizik uygulanmasını sağlar.

// rigidbody constraints: 
// position z
// rotation x, y

// drag(sürtünme) = 0 ise itiş gücünü durdurduğumuzda sabit hızda ilerler

// itiş gücünü durdurduğumuzda yavaşlamak için
// rigidbody drag(sürtünme) = 0.25

// Edit > Project Settings > Physics > Gravity: y = -4

/*
Ses efektini çalmak için şu adımları izleyebilirsiniz:
- Ses dinleyicisi (sesi duymak için) (Audio Listener, Main Camera üzerinde bulunur.)
- Ses kaynağı (sesi çalmak için) (Audio Source, roket nesnesinin bir bileşenidir çünkü roketten ses çıkıyor.)
- Ses dosyası (Audio File) (Audio Source > Audio Clip)
- Sadece space tuşuna bastığımda ses çalsın.
*/


