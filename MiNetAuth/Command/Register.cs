using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MiNET;
using MiNET.Plugins.Attributes;
using MiNET.Security;
using MiNET.Utils;

namespace MiNetAuth.Command
{
    public class Register : MiNETAuth
    {
        [Command(Command = "reg")]
        public void RegisterCommand(Player player, string password)
        {

            var userManager = Context.Server.UserManager;

            var user = userManager.FindByNameAsync(player.Username).Result;

            if (user == null)
            {
                if (password == null && password.IndexOf(' ') > -1)
                {
                    player.SendMessage($"Do not add any space in your password!");
                    return;
                }
                user = new User(player.Username);
                Context.Server.UserManager.CreateAsync(user, password);
                user.IsAuthenticated = true;
                userManager.UpdateAsync(user);
            }
            else if (user.IsAuthenticated)
            {
                player.SendMessage($"{ChatColors.Red}You has been Login!");
            }
            else if (Context.Server.UserManager.HasPasswordAsync(user.Id).Result)
            {
                player.SendMessage($"You has been register,Plz Type your password to login");
            }
        }
    }
}
