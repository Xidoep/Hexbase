using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class UI_FuncionsGeneriques
{
    public static void SetCapa_UIPeces(this GameObject gameObject)
    {
        if (gameObject.layer != 7)
            gameObject.layer = 10;
    }
    public static void SetCapa_UIPeces(this Transform transform)
    {
        if (transform.gameObject.layer != 7)
            transform.gameObject.layer = 10;
    }
}
