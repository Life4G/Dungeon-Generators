using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Room
{

    internal class RoomStyleManager : MonoBehaviour
    {
        [SerializeField]
        private List<RoomStyle> roomStyles;

        public RoomStyle GetRoomStyle(string name)
        {
            return roomStyles.Find(style => style.name == name);
        }
    }
}
