using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts
{
    public static class Utils
    {
        public static void SetLayerRecursively(this GameObject obj, int newLayer)
        {
            obj.layer = newLayer;

            for (int i = 0; i < obj.transform.childCount; i++)
                obj.transform.GetChild(i).gameObject.SetLayerRecursively(newLayer);
        }

    }
}
