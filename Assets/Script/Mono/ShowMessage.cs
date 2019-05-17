using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Maze
{
    class ShowMessage : MonoBehaviour
    {
        static private HintBox HintBox { get { return GlobalAsset.hintBox; } }

        public MazeObject refer;

       

        private void OnMouseEnter()
        {
            HintBox.ShowMessage(refer.Info(), refer.Name());
        }

        private void OnMouseExit()
        {
            HintBox.Hide();
        }

    }
}
