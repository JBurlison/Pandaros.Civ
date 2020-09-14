using Pipliz;

using Chatting;
using NetworkUI;
using NetworkUI.Items;
using Science;
using System;
using Shared;

using System.Collections.Generic;
using System.Linq;
using Pipliz.JSON;
using System.IO;
using UnityEngine;
using Pandaros.API.localization;
using Pandaros.API.Models;
using Pandaros.API.Items;
using Pandaros.API.Questing;

namespace Pandaros.Civ.QuestBook
{
    public class QuestBookType : CSType
    {
        public static string NAME = GameSetup.GetNamespace("QuestBook");
        public override string name { get; set; } = NAME;
        public override StaticItems.StaticItem StaticItemSettings => new StaticItems.StaticItem() { Name = NAME };
        public override string icon => GameSetup.Textures.GetPath(TextureType.icon, "QuestBook.png");
        public override bool? isPlaceable => false;
        public override List<string> categories => new List<string>()
            {
                CommonCategories.Essential,
                "AAA",
                GameSetup.NAMESPACE
            };
        public override int? maxStackSize => 1;
    }
    [ModLoader.ModManager]
    public class UIManageing
    {
        [ModLoader.ModCallback(ModLoader.EModCallbackType.OnPlayerClicked, GameSetup.NAMESPACE + ".CommandTool.OnPlayerClick")]
        public static void UIManagement(Players.Player player, PlayerClickedData data)
        {
            if (data.TypeSelected == ItemTypes.GetType(QuestBookType.NAME).ItemIndex && data.ClickType == PlayerClickedData.EClickType.Right && player.ActiveColony != null)
            {
                QuestingSystem.OnPlayerPushedNetworkUIButton(data);  
            }
        }
    }
}
