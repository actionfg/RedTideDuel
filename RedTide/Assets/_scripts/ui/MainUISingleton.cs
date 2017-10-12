using UnityEngine;
using System.Collections;

public class MainUISingleton : MonoBehaviour
{

    public static GameObject MainUi;

    private void Awake()
    {
        if (MainUi == null)
        {
            MainUi = this.gameObject;
        }
        else if (MainUi != this.gameObject)
        {
            Destroy(this.gameObject);
        }
    }

    public static GameObject GetMainUI()
    {
        return MainUi;
    }
}
