using GrandTheftMultiplayer.Server.Elements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace GT_MP_Basic_Map_Editor.Server.Models
{
    public class Menu
    {
        public string Title = "Title";
        public string SubTitle = "";
        public List<MenuItem> Items = new List<MenuItem>();

        public Menu() { }
        public Menu(string title)
        {
            Title = title;
        }
        public Menu(string title, string subtitle)
        {
            Title = title;
            SubTitle = subtitle;
        }

        public void Show(Client player)
        {
            player.triggerEvent("OpenEditorMenu", JsonConvert.SerializeObject(this));
        }

        public void AddItem(MenuItem item)
        {
            if (Items.Contains(item))
                return;
            Items.Add(item);
        }
    }

    public class MenuItem
    {
        public string Title = "Item Name";
        public string Description = "";
        public string RightLabel = "";
        public bool Enabled = true;
        public BadgeStyle LeftBadge = BadgeStyle.None;
        public BadgeStyle RightBadg = BadgeStyle.None;
        public string SelectEvent = "";
        public string SelectEventArg = "";
        public bool UserInput = false;
        public string DefaultUserInput = "";

        public MenuItem() { }
        public MenuItem(string title, string description = "", string rightlabel = "")
        {
            Title = title;
            Description = description;
            RightLabel = rightlabel;
        }
    }

    public enum BadgeStyle
    {
        None,
        BronzeMedal,
        GoldMedal,
        SilverMedal,
        Alert,
        Crown,
        Ammo,
        Armour,
        Barber,
        Clothes,
        Franklin,
        Bike,
        Car,
        Gun,
        Heart,
        Makeup,
        Mask,
        Michael,
        Star,
        Tatoo,
        Trevor,
        Lock,
        Tick,
    }
}
