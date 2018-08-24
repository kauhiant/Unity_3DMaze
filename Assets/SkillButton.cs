using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Assets
{
    public class SkillButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        public HintBox hintBox;
        public Sprite sprite;
        public new String name;
        public String detail;
        public Maze.Animal.Command skillCommand;
        

        // 設定Image.
        void Start()
        {
            Image image = gameObject.GetComponent<Image>();
            if (image == null)
                image = gameObject.AddComponent<Image>();
            
            image.sprite = this.sprite;
        }

        // 當滑鼠"移到"物件上面.
        // 顯示此技能的提示.
        public void OnPointerEnter(PointerEventData eventData)
        {
            hintBox.ShowMessage(this.detail, this.name);
        }

        // 當滑鼠"離開"物件上面.
        // 關閉提示欄.
        public void OnPointerExit(PointerEventData eventData)
        {
            hintBox.Hide();
        }

        // 當SkillBar按下對應熱鍵時觸發.
        public void ChooseSkill()
        {
            if (GlobalAsset.player == null)
                Debug.Log("player not defined!");
            else
                GlobalAsset.player.command = this.skillCommand;
        }
    }
}
