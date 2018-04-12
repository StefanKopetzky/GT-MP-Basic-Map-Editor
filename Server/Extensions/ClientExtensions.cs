using GrandTheftMultiplayer.Server.Elements;

namespace GT_MP_Basic_Map_Editor.Server.Extensions
{
    public static class ClientExtensions
    {
        public static void CloseAllMenus(this Client player)
        {
            player.triggerEvent("CloseMenus");
        }
    }
}