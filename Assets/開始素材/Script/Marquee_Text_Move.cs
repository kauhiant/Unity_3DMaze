using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class Marquee_Text_Move : MonoBehaviour {
    public float Text_Move_Speed;       //跑馬燈速度
    

    void Update () {
        this.transform.localPosition -= new Vector3(1 * Text_Move_Speed * Time.deltaTime, 0, 0);    //文字移動
        if (this.transform.localPosition.x <= -445) {      //重新設定位置
            this.transform.localPosition = new Vector3(445, this.transform.localPosition.y, this.transform.localPosition.z);
            this.SendMessageUpwards("set_Marquee_text");    //呼叫  設定新的文字
        }
	}
}
