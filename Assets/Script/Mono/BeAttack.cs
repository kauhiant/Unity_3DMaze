using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Maze
{
    // 明滅閃爍.
    class BeAttack : MonoBehaviour
    {
        private Color originColor;

        public void Flash(float time, float pwm, Color originColor)
        {
            this.originColor = originColor;

            float hideWidth = time * (1f - pwm);

            hide();
            Invoke("show", hideWidth);
        }

        private void show()
        {
            this.gameObject.GetComponent<SpriteRenderer>().color = originColor;
        }

        private void hide()
        {
            this.gameObject.GetComponent<SpriteRenderer>().color = Color.clear;
        }
    }
}
