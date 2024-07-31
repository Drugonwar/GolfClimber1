using UnityEngine.UI;
using UnityEngine;
using TMPro;
using System;

public class HeightScore : MonoBehaviour
{
    public Transform playerTransform;
    public TMP_Text heightText;
    public TMP_Text highestscoreText;

    public float height;
    public float highestPoint;
    void Start()
    {
        if (playerTransform != null)
        {
            highestPoint = playerTransform.position.y - transform.position.y;
        }
    }

    void Update()
    {

        if (playerTransform != null)
        {
            height = playerTransform.position.y - transform.position.y;

            
            if (height > highestPoint)
            {
                highestPoint = height;
                UpdateBestScoreUI();
            }
        }

        UpdateHeightScoreUI();
    }

    void UpdateBestScoreUI()
    {
        if (highestscoreText != null)
        {
            highestscoreText.text = "BEST: " + highestPoint.ToString("F2");
        }
    }

    void UpdateHeightScoreUI()
    {
        if (height >  0)
        {
            heightText.text = "HEIGHT: " + height.ToString("F2");
        }
        if (height == highestPoint)
        {
            heightText.color = Color.red;
        }
        else { heightText.color = Color.white; }
    }
}
