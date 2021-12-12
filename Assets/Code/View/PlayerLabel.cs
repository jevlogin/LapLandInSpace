using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.Networking;

namespace WORLDGAMEDEVELOPMENT
{
    public sealed class PlayerLabel : MonoBehaviour
    {
        public void DrawLabel(Camera camera)
        {
            if (camera == null)
            {
                return;
            }

            var style = new GUIStyle();
            style.alignment = TextAnchor.MiddleCenter;
            style.normal.textColor = Color.blue;
            style.normal.background = Texture2D.redTexture;
            var objects = ClientScene.objects;
            for(int i = 0; i < objects.Count; i++)
            {
                var obj = objects.ElementAt(i).Value;
                var position = camera.WorldToScreenPoint(obj.transform.position);
                var collider = obj.GetComponent<Collider>();
                if (collider != null && camera.Visible(collider) && obj.transform != transform)
                {
                    GUI.Label(new Rect(new Vector2(position.x, Screen.height - position.y), new Vector2(10.0f, name.Length * 10.5f)), obj.name, style);
                }
            }
        }
    } 
}
