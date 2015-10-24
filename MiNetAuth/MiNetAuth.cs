using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Security.Cryptography;

using log4net;

using Newtonsoft.Json;

using MiNET.Plugins;
using MiNET.Plugins.Attributes;

using MiNET;
using MiNET.Net;
using MiNET.Utils;
using MiNET.Blocks;
using MiNET.Worlds;
using MiNET.Security;
using System.Linq;

namespace MiNetAuth
{
    [Plugin(Author = "FKDPIBC", Description = "AuthPlugin", PluginName = "MiNetAuth", PluginVersion = "0.0.1")]
    public class MiNetAuth : Plugin
    {
        private string _basepath = AppDomain.CurrentDomain.BaseDirectory + "\\MiNetAuth\\RegisterPlayerList.json";
        private List<User> _registerlist;
        static ILog Log = LogManager.GetLogger(typeof(MiNetAuth));

        protected override void OnEnable()
        {
            if (!Getloginlist()) _registerlist = new List<User>();//Maybe this is invalidate line

            foreach (var level in Context.LevelManager.Levels)
            {
                level.BlockBreak += OnBreak;
                level.BlockPlace += OnPlace;
            }
            Log.Info("MiNetAuth Enable");
        }

        public override void OnDisable()
        {
            if (SaveloginLlist())
                Log.Info("MiNetAuth Disable");
        }

        [PacketHandler]
        public Package PlayerJoin(McpeLogin package, Player player)
        {
            if (!(_registerlist.Contains(player.User)))
            {
                player.AddPopup(new Popup()
                {
                    Message = "Type /reg [Password] to register",
                    MessageType = MessageType.Popup
                });
            }
            else if (!player.User.IsAuthenticated)
            {
                player.AddPopup(new Popup()
                {
                    Message = "Type /auth [Password] to Login",
                    MessageType = MessageType.Popup
                });
            }
            else
            {
                player.User = _registerlist.Find(t=>t.UserName==player.Username);
            }
            return package;
        }

        [PacketHandler]
        public Package PlayerDisconnect(McpeDisconnect package, Player player)
        {
            if (_registerlist.Contains(player.User))
            {
                player.User.IsAuthenticated = false;
            }
            return package;
        }

        [PacketHandler]
        public Package PlayerMove(McpeMovePlayer package, Player player)
        {
            if (!_registerlist.Contains(player.User) || !player.User.IsAuthenticated)
            {
                player.KnownPosition = player.Level.SpawnPoint;
            }
            return package;
        }

        [Command(Command = "reg")]
        public void Register(Player player, string password)
        {
            if (!_registerlist.Contains(player.User))
            {
                register(player, password);
            }
            else
            {
                player.SendMessage(ChatColors.Red + "You have already registered!");
            }
        }

        [Command(Command = "auth")]
        public void Auth(Player player, string password)
        {
            if (!_registerlist.Contains(player.User))
            {
                player.User.IsAuthenticated = player.User.PasswordHash == GetPasswordHash(password);
                player.ClearPopups();
                player.AddPopup(new Popup()
                {
                    Message = ChatColors.Green + "Welcome back,My friends.",
                    Duration = 20 * 10,
                    MessageType = MessageType.Popup
                });
            }
            else
            {
                player.SendMessage(ChatColors.Red + "You have not registered!!");
            }
        }

        private void OnBreak(object sender, BlockBreakEventArgs e)
        {
            //Block record = e.Block;
            //e.Level.SetBlock(record);
            e.Cancel = !_registerlist.Contains(e.Player.User) || !e.Player.User.IsAuthenticated;//Maybe properties is invalidate
        }

        private void OnPlace(object sender, BlockPlaceEventArgs e)
        {
            //Block record = e.Block;
            //e.Level.SetBlock(record);
            e.Cancel = !_registerlist.Contains(e.Player.User) || !e.Player.User.IsAuthenticated;//Maybe properties is invalidate
        }

        private string GetPasswordHash(string password)
        {
            SHA1 sha1 = SHA1.Create();
            return sha1.ComputeHash(Encoding.Unicode.GetBytes(password)).ToString();
        }

        private void register(Player player, string password)
        {
            player.User.UserName = player.Username;
            player.User.PasswordHash = GetPasswordHash(password);
            player.User.IsAuthenticated = true;
            _registerlist.Add(player.User);
            player.AddPopup(new Popup()
            {
                Message = "Registration Succeed",
                Duration = 20 * 10,
                MessageType = MessageType.Popup
            });
        }

        private bool SaveloginLlist()
        {
            if (!File.Exists(_basepath))
            {
                File.Create(_basepath);
                Log.Info("You Don't have RegisterPlayerList file,we will create it.");
                return SaveloginLlist();
            }
            else
            {
                string json = JsonConvert.SerializeObject(_registerlist.ToArray());
                File.WriteAllText(_basepath, json);
                return true;
            }
        }

        private bool Getloginlist()
        {
            if (!File.Exists(_basepath))
            {
                File.Create(_basepath);
                Log.Info("You Don't have RegisterPlayerList file,we will create it.");
                return Getloginlist();
            }
            else
            {
                string json = File.ReadAllText(_basepath);
                _registerlist.AddRange(JsonConvert.DeserializeObject<User[]>(json));
                return true;
            }
        }
    }
}
