using UnityEngine;
using System.Collections;
using System.Linq;
using UnityEngine.UI;

public class MobHpControl : MonoBehaviour {

    public GameUnit gameUnit { get; set; }

    [HideInInspector]
    public GameObject target;

    [HideInInspector] public Transform TargetTransform;

    public Transform panelTransform;
    public Image baseImage;
    public Image indicatorImage;

    private Camera _camera;

    void Start()
    {
        _camera = Camera.main;
    }
    // Update is called once per frame
    void Update ()
    {
        if (gameUnit != null)
        {
            float currentHp = gameUnit.CurrentHp;
            float maxHp = gameUnit.MaxHp;

            if (currentHp >= maxHp || currentHp <= 0 || maxHp <= 0f)
            {
                baseImage.gameObject.SetActive(false);
                indicatorImage.gameObject.SetActive(false);
            }
            else
            {
                baseImage.gameObject.SetActive(true);
                indicatorImage.gameObject.SetActive(true);
                indicatorImage.fillAmount = currentHp / maxHp;

                var worldPos = target.transform.position + new Vector3(0, 2f, 0);
                if (TargetTransform != null)
                {
                    worldPos = TargetTransform.position;
                }
                Vector3 point = _camera.WorldToScreenPoint(worldPos);
                panelTransform.position = point + new Vector3(0f, 10f, 0f);
            }
        }
        else {
            Destroy(gameObject);
        }

    }
}
