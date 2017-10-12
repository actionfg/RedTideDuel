using System;
using UnityEngine;
using UnityEngine.UI;

public class ImageAlphaControl : MonoBehaviour
{
    public delegate void OnAlphaChangeDelegate (float alpha);

    public event OnAlphaChangeDelegate OnAlphaChange;

    public float Speed;
    private bool isEmerge = false; // True则alpha 从 0 到 1, 反之从 1 到 0

    private Image _image;
    private float _alpha = 0;

    public void Start()
    {
        _image = GetComponent<Image>();

    }

    public void Update()
    {
        if (isEmerge)
        {
            _alpha = Math.Min(_alpha + Speed * Time.unscaledDeltaTime, 1f);
        }
        else
        {
            _alpha = Math.Max(0, _alpha - Speed * Time.unscaledDeltaTime);
        }
        ChangeImageAlpha(_image, _alpha);
        if (OnAlphaChange != null && (_alpha >= 1f || _alpha <= 0f))
        {
            OnAlphaChange(_alpha);
        }
    }

    public void SetEmerge(bool emerge, float speed)
    {
        isEmerge = emerge;
        if (emerge)
        {
            _alpha = 0;
        }
        else
        {
            _alpha = 1;
        }
        Speed = speed;
    }

    private void ChangeImageAlpha(Image image, float alpha)
    {
        if (image.material)
        {
            image.material.SetColor("_Color", new Color(1f, 1f, 1f, alpha));
        }
        else
        {
            image.color = new Color(1f, 1f, 1f, alpha);
        }
    }

}