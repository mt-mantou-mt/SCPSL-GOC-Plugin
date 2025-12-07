using CommandSystem;
using Exiled.API.Enums;
using Exiled.API.Extensions;
using Exiled.API.Features;
using Exiled.API.Interfaces;
using Exiled.Events.EventArgs.Interfaces;
using MapGeneration;
using MEC;
using Mirror;
using PlayerRoles;
using PlayerRoles.PlayableScps.Scp079.Map;
using Respawning;
using Respawning.Graphics;
using System;
using System.Collections.Generic;
using System.Resources;
using System.Runtime.InteropServices;
using System.Security.Policy;
using System.Text;
using UnityEngine;
using YamlDotNet.Core.Tokens;
namespace GOC
{
    public class plugin : Plugin<Config>
    {
        public ushort times = 2;
        public override string Author => "_mt馒头mt_";
        public override string Name => "UN-GOC";
        public override Version Version => new Version(1,0,0);
        public override string Prefix => "goc";
        private bool isrun=false;
        public override void OnEnabled()
        {
            Exiled.Events.Handlers.Server.RoundStarted += start;
            Exiled.Events.Handlers.Player.Hurting+=hurt;
            Exiled.Events.Handlers.Player.FlippingCoin += flipcoin;
            Exiled.Events.Handlers.Warhead.Stopping += stopwarhead;
            Exiled.Events.Handlers.Player.Left += left;
            Exiled.Events.Handlers.Player.Died += die;
            base.OnEnabled();
        }
        public override void OnDisabled()
        {
            Exiled.Events.Handlers.Server.RoundStarted -= start;
            Exiled.Events.Handlers.Player.Hurting -= hurt;
            Exiled.Events.Handlers.Player.FlippingCoin -= flipcoin;
            Exiled.Events.Handlers.Warhead.Stopping -= stopwarhead;
            Exiled.Events.Handlers.Player.Left -= left;
            Exiled.Events.Handlers.Player.Died -= die;
            base.OnDisabled();
        }
        public override void OnReloaded()
        {
            Exiled.Events.Handlers.Server.RoundStarted -= start;
            Exiled.Events.Handlers.Player.Hurting -= hurt;
            Exiled.Events.Handlers.Player.FlippingCoin -= flipcoin;
            Exiled.Events.Handlers.Warhead.Stopping -= stopwarhead;
            Exiled.Events.Handlers.Player.Died -= die;
            Exiled.Events.Handlers.Player.Left -= left;
            Exiled.Events.Handlers.Player.Left += left;
            Exiled.Events.Handlers.Player.Died += die;
            Exiled.Events.Handlers.Server.RoundStarted += start;
            Exiled.Events.Handlers.Player.Hurting += hurt;
            Exiled.Events.Handlers.Player.FlippingCoin += flipcoin;
            Exiled.Events.Handlers.Warhead.Stopping += stopwarhead;
            base.OnReloaded();
        }
        public void stopwarhead(Exiled.Events.EventArgs.Warhead.StoppingEventArgs ev)
        {
            Timing.KillCoroutines("warhead");
            isrun = false;
        }
        public void flipcoin(Exiled.Events.EventArgs.Player.FlippingCoinEventArgs ev)
        {
            if (HasTag(ev.Player, "GOC")&&ev.Player.CurrentRoom.RoomName==RoomName.HczWarhead&&!isrun)
            {
                Timing.RunCoroutine(warhead(), "warhead");
            }
        }
        public void die(Exiled.Events.EventArgs.Player.DiedEventArgs ev)
        {
            RemoveTag(ev.Player, "GOC");
        }
        public void left(Exiled.Events.EventArgs.Player.LeftEventArgs ev)
        {
            RemoveTag(ev.Player, "GOC");
        }
        public IEnumerator<float> warhead()
        {
            isrun = true;
            Warhead.Detonate(90f);
            Cassie.MessageTranslated("Attention . Omega warhead detonation sequence has been initiated .  THE_CONSONANT facility will be destroyed in 90 seconds", "<color=red>注意,Omega核弹引爆程序已启动,设施将在90秒后被摧毁</color>", true, false, true);
            yield return Timing.WaitForSeconds(90f);
            foreach(Player player in Player.List)
            {
                player.Hurt(float.MaxValue, DamageType.Warhead);
            }
        }
        public void hurt(Exiled.Events.EventArgs.Player.HurtingEventArgs ev)
        {
            Player player = ev.Player;
            Player attacker = ev.Attacker;
            if (HasTag(player, "GOC")&&HasTag(attacker, "GOC")&&player!=attacker)
            {
                ev.IsAllowed = false;
                attacker.ShowHint("友军!", 1);
                attacker.ShowHitMarker(0);
            }
        }
        public IEnumerator<float> wait()
        {
            for (ushort i = 0; i < times; i++)
            {
                yield return Timing.WaitForSeconds(1200f);
                ushort person = 0;
                foreach (Player player in Player.List)
                {
                    if (player.Role == RoleTypeId.Spectator)
                    {
                        person++;
                        if (person == 1)
                        {
                            setgoc(0, player);
                        }
                        else if (person == 2 || person == 3)
                        {
                            setgoc(1, player);
                        }
                        else
                        {
                            setgoc(2, player);
                        }
                        
                    }
                    if (person == 8)
                    {
                        break;
                    }
                }
                
                
            }
        }
        public void setgoc(int i, Player player)
        {
            player.Role.Set(RoleTypeId.Tutorial);
            switch (i)
            {
                case 0:
                    player.AddItem(ItemType.GunFRMG0);
                    player.AddItem(ItemType.GrenadeHE);
                    player.AddItem(ItemType.KeycardO5);
                    player.AddItem(ItemType.MicroHID);
                    player.MaxHealth = 150;
                    player.AddAhp(120, 120, 0, 0.75f, 0, false);
                    player.Health = 150;
                    player.AddItem(ItemType.ArmorHeavy);
                    player.EnableEffect(EffectType.Scp1853, 1, 0, false);
                    player.AddItem(ItemType.Coin);
                    player.Broadcast(5, "\n<color=blue>你是全球超自然联盟攻击小组</color><color=white>████</color><color=blue>-指挥官,消灭设施内的异常及武装人员</color>");
                    break;
                case 1:
                    player.AddItem(ItemType.KeycardChaosInsurgency);
                    player.AddItem(ItemType.GunLogicer);
                    player.AddItem(ItemType.ParticleDisruptor);
                    player.MaxHealth = 200;
                    player.AddAhp(200, 200, 0, 0.85f, 0, false);
                    player.Health = 200;
                    player.EnableEffect(EffectType.Slowness, 30, 0, false);
                    player.AddItem(ItemType.ArmorHeavy);
                    player.AddItem(ItemType.Coin);
                    player.Broadcast(5, "\n<color=blue>你是全球超自然联盟攻击小组</color><color=white>████</color><color=blue>-重装兵,消灭设施内的异常及武装人员</color>");
                    break;
                case 2:
                    player.AddItem(ItemType.KeycardChaosInsurgency);
                    player.AddItem(ItemType.GunE11SR);
                    player.AddItem(ItemType.Jailbird);
                    player.MaxHealth = 120;
                    player.AddAhp(100, 100, 0, 0.7f, 0, false);
                    player.Health = 120;
                    player.EnableEffect(EffectType.MovementBoost, 35, 0, false);
                    player.EnableEffect(EffectType.Scp1853, 1, 0, false);
                    player.AddItem(ItemType.ArmorCombat);
                    player.AddItem(ItemType.Coin);
                    player.Broadcast(5, "\n<color=blue>你是全球超自然联盟攻击小组</color><color=white>████</color><color=blue>-士兵,消灭设施内的异常及武装人员</color>");
                    break;

            }
            AddTag(player, "GOC");
            Room nukeRoom = Room.Get(RoomType.EzGateA);
            player.AddItem(ItemType.SCP500);
            player.AddItem(ItemType.Adrenaline);
            player.Teleport(nukeRoom.Position + Vector3.up);

        }
        public static void AddTag(Player player, string tag)
        {
            if (!player.SessionVariables.ContainsKey("tags"))
            {
                player.SessionVariables["tags"] = new HashSet<string>();
            }

            var tags = player.SessionVariables["tags"] as HashSet<string>;
            tags.Add(tag);

            Log.Debug($"已为玩家 {player.Nickname} 添加标签: {tag}");
        }

        public static bool HasTag(Player player, string tag)
        {
            if (player.SessionVariables.ContainsKey("tags"))
            {
                var tags = player.SessionVariables["tags"] as HashSet<string>;
                return tags.Contains(tag);
            }
            return false;
        }
        public static void RemoveTag(Player player, string tag)
        {
            if (player.SessionVariables.ContainsKey("tags"))
            {
                var tags = player.SessionVariables["tags"] as HashSet<string>;
                tags.Remove(tag);

                Log.Debug($"已从玩家 {player.Nickname} 移除标签: {tag}");
            }
        }
        public void start()
        {
            Timing.RunCoroutine(wait());
        }

        
    }
    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    public class AdminStatsCommand : ICommand
    {
        public static void AddTag(Player player, string tag)
        {
            if (!player.SessionVariables.ContainsKey("tags"))
            {
                player.SessionVariables["tags"] = new HashSet<string>();
            }

            var tags = player.SessionVariables["tags"] as HashSet<string>;
            tags.Add(tag);

            Log.Debug($"已为玩家 {player.Nickname} 添加标签: {tag}");
        }
        public static bool HasTag(Player player, string tag)
        {
            if (player.SessionVariables.ContainsKey("tags"))
            {
                var tags = player.SessionVariables["tags"] as HashSet<string>;
                return tags.Contains(tag);
            }
            return false;
        }

        public static void RemoveTag(Player player, string tag)
        {
            if (player.SessionVariables.ContainsKey("tags"))
            {
                var tags = player.SessionVariables["tags"] as HashSet<string>;
                tags.Remove(tag);

                Log.Debug($"已从玩家 {player.Nickname} 移除标签: {tag}");
            }
        }
        public string Command => "setgoc";
        public string[] Aliases => new[] { "gocset", "goc" };
        public string Description => "将玩家设置为GOC";

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            string args1 = arguments.At(0);
            string args2 = arguments.At(1);
            foreach (Player player in Player.List)
            {
                if (player.Id == int.Parse(args1))
                {
                    set(int.Parse(args2), player);
                }
            }
            
            response = " ";
            return true;
        }
        public void set(int i, Player player)
        {
            player.Role.Set(RoleTypeId.Tutorial);
            switch (i)
            {
                case 0:
                    player.AddItem(ItemType.GunFRMG0);
                    player.AddItem(ItemType.GrenadeHE);
                    player.AddItem(ItemType.KeycardO5);
                    player.AddItem(ItemType.MicroHID);
                    player.MaxHealth = 150;
                    player.AddAhp(120, 120, 0, 0.75f, 0, false);
                    player.Health = 150;
                    player.AddItem(ItemType.ArmorHeavy);
                    player.EnableEffect(EffectType.Scp1853, 1, 0, false);
                    player.AddItem(ItemType.Coin);
                    player.Broadcast(5, "\n<color=blue>你是全球超自然联盟攻击小组</color><color=white>████</color><color=blue>-指挥官,消灭设施内的异常及武装人员</color>");
                    break;
                case 1:
                    player.AddItem(ItemType.KeycardChaosInsurgency);
                    player.AddItem(ItemType.GunLogicer);
                    player.AddItem(ItemType.ParticleDisruptor);
                    player.MaxHealth = 200;
                    player.AddAhp(200, 200, 0, 0.85f, 0, false);
                    player.Health = 200;
                    player.EnableEffect(EffectType.Slowness, 30, 0, false);
                    player.AddItem(ItemType.ArmorHeavy);
                    player.AddItem(ItemType.Coin);
                    player.Broadcast(5, "\n<color=blue>你是全球超自然联盟攻击小组</color><color=white>████</color><color=blue>-重装兵,消灭设施内的异常及武装人员</color>");
                    break;
                case 2:
                    player.AddItem(ItemType.KeycardChaosInsurgency);
                    player.AddItem(ItemType.GunE11SR);
                    player.AddItem(ItemType.Jailbird);
                    player.MaxHealth = 120;
                    player.AddAhp(100, 100, 0, 0.7f, 0, false);
                    player.Health = 120;
                    player.EnableEffect(EffectType.MovementBoost, 35, 0, false);
                    player.EnableEffect(EffectType.Scp1853, 1, 0, false);
                    player.AddItem(ItemType.ArmorCombat);
                    player.AddItem(ItemType.Coin);
                    player.Broadcast(5, "\n<color=blue>你是全球超自然联盟攻击小组</color><color=white>████</color><color=blue>-士兵,消灭设施内的异常及武装人员</color>");
                    break;

            }
            AddTag(player, "GOC");
            Room nukeRoom = Room.Get(RoomType.EzGateA);
            player.AddItem(ItemType.SCP500);
            player.AddItem(ItemType.Adrenaline);
            player.Teleport(nukeRoom.Position + Vector3.up);

        }

    }


    public class Config : IConfig
    {
        public bool IsEnabled { get; set; } = true;
        public bool Debug { get; set; } = true;
    }
    


}
