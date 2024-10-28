using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Oscillator : MonoBehaviour
{
    [SerializeField] Vector3 movementVector; // (8, 0 ,0)
    float movementFactor; // 0.5
    [SerializeField] float periyod = 2f; // bir periyot için geçirilen süre

    Vector3 startingPosition; // bu scriptin bağlı olduğu nesnenin sahnedeki ilk konumu

    // Start is called before the first frame update
    void Start()
    {
        // bu scriptin bağlı olduğu nesnenin sahnedeki ilk/başlangıç konumu
        startingPosition = transform.position; // (0.00, 2.10, 0.00)
    }

    // Update is called once per frame
    void Update()
    {
        if (periyod <= Mathf.Epsilon) // periyod tam olarak sıfıra eşit olmayabilir.
        {
            return;
        }

        float cycles = Time.time / periyod; // toplam döngü sayısı
        const float tau = Mathf.PI * 2; // bir periyot: 2*pi
        float rawSinWave = Mathf.Sin(cycles * tau); // sinüs dalgası (sin0=0, sin(pi/2)=1, sin(pi)=0, sin(3*pi/2)=-1, sin(2*pi)=0)
        // rawSinWave: 0 -> 1 -> 0 -> -1 -> 0 | 0 -> 1 -> 0 -> -1 -> 0 | ...

        // nesnenin (0.00, 2.10, 0.00) ile (8, 2.10, 0) arasında gidip gelmesini istiyorum
        movementFactor = (rawSinWave + 1f) / 2; // 0 ile 1 arasında olmalı

        // (8, 0, 0) * 0.5 = (4, 0, 0)
        Vector3 offset = movementVector * movementFactor;
        // (0.00, 2.10, 0.00) + (4, 0, 0) = (4, 2.10, 0) x ekseninde pozitif yönde 4 br ilerledi
        transform.position = startingPosition + offset;
    }
}

// movementFactor değeri 0 ile 1 arasındadır. bu değeri değiştirdikçe
// nesne (0.00, 2.10, 0.00) ile (8, 2.10, 0) konumları arasında yer değiştirir.

// bu nesne sinüs dalgalarını kullanarak A noktasında B noktasına doğru ilerler.
// -1 ile +1 arasında değerler üretir
// sinüs dalgası, periyodik bir dalga formudur
// periyot, kendini tekrar etmeden önce geçen süredir.

