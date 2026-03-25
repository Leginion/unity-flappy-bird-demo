using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MedalController : MonoBehaviour
{
    public Sprite[] medalSprites;
    public Image image;

    public int medalId = -1;
    private int _medalId = -1;

    private void Awake()
    {
        image = GetComponent<Image>();
    }

    private void Update()
    {
        if (_medalId != medalId)
        {
            if (medalId == 0)
            {
                image.color = new Color(0, 0, 0, 0);
            }
            else
            {
                image.color = new Color(1, 1, 1, 1);
                Sprite sp = medalSprites[medalId - 1];
                image.sprite = sp;
            }
        }
    }

    // 0 透明 1-4 级别
    public void SetMedalId(int id)
    {
        medalId = id;
    }
}
