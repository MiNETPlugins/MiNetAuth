using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;

using MiNET;
using MiNET.Net;
using MiNET.Utils;
using MiNET.Plugins;
using MiNET.Security;
using MiNET.Plugins.Attributes;

using log4net;
using System.Net;

namespace MiNetAuth.PackageHandler
{
    public class HandlePackage : MiNETAuth
    {
        protected override void OnEnable()
        {
            _log.Info("PackageHandler has been Loaded...");
        }
        public override void OnDisable()
        {
            _log.Info("PackageHandler has been Disable...");
        }

        [PacketHandler]
        public Package OnPlayerMove(McpeMovePlayer packet, Player player)
        {
            var userManager = Context.Server.UserManager;
            var user = userManager.FindByNameAsync(player.Username).Result;
            if (user != null)
            {
                if (!user.IsAuthenticated)
                {
                    player.SetPosition(player.SpawnPosition);
                    player.ClearPopups();
                    player.AddPopup(new Popup()
                    {
                        Message = $"{ChatColors.Green}Type your password to login!",
                        Duration = 10,
                        Priority = 1000
                    });
                }
            }
            else
            {
                player.SetPosition(player.SpawnPosition);
                player.ClearPopups();
                player.AddPopup(new Popup()
                {
                    Message = $"{ChatColors.Green}Type /reg password to register!",
                    Duration = 10,
                    Priority = 1000
                });
            }
            return packet;
        }

        [PacketHandler]
        public Package OnPlayerSendMessage(McpeText packet, Player player)
        {
            var userManager = Context.Server.UserManager;
            var user = userManager.FindByNameAsync(player.Username).Result;
            if (user != null)
            {
                if (user.IsAuthenticated)
                {
                    if (userManager.CheckPasswordAsync(user, packet.message).Result)
                    {
                        user.IsAuthenticated = true;
                        userManager.UpdateAsync(user);
                        player.SendMessage("you has been Logged");
                    }
                    else
                    {
                        player.SendMessage($"{ChatColors.Green}Check your password again,that have some error!");
                    }
                    packet.message = null;
                    return packet;
                }
                else
                {
                    return packet;
                }

            }
            else if (packet.message.StartsWith("/reg"))
            {
                return packet;
            }
            else
            {
                player.SendMessage($"{ChatColors.Green}Plz Register frist!");
                packet.message = null;
                return packet;
            }
        }
    }
}
