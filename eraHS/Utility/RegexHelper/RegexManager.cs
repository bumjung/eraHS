using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace eraHS.Utility.RegexHelper
{
    public static class RegexManager
    {
        // D 02:55:54.0639010 GameState.DebugPrintPower() - FULL_ENTITY - Creating ID=64 CardID=HERO_09
        public static readonly Regex heroIdRegex = new Regex(@"^D .*GameState.DebugPrintPower\(\) - FULL_ENTITY - Creating ID=([0-9]*) CardID=HERO_([0-9]*)");

        // D 03:01:02.7734420 GameState.DebugPrintPower() - TAG_CHANGE Entity=Jacob Kim tag=PLAYER_ID value=1
        public static readonly Regex playerIdRegex = new Regex(@"^D .*GameState.DebugPrintPower\(\) - TAG_CHANGE Entity=(.*) tag=PLAYER_ID value=([0-9]*)");

        // D 02:57:40.7757570 GameState.DebugPrintPower() - TAG_CHANGE Entity=bums tag=HERO_ENTITY value=66
        public static readonly Regex playerEntityRegex = new Regex(@"^D .*GameState.DebugPrintPower\(\) - TAG_CHANGE Entity=(.*) tag=HERO_ENTITY value=([0-9]*)");

        public static readonly Regex controllerRegex = new Regex(@"^D .*GameState.DebugPrintPower\(\) - TAG_CHANGE Entity=(?<name>.*) tag=CONTROLLER value=(?<value>\d+)");
        
        // D 02:56:53.9372700 GameState.DebugPrintPower() - TAG_CHANGE Entity=bums tag=PLAYSTATE value=LOST
        public static readonly Regex gameResultRegex = new Regex(@"^D .*GameState.DebugPrintPower\(\) - TAG_CHANGE Entity=(.*) tag=PLAYSTATE value=(LOST|WON|TIED)");

        // D 02:55:54.0534740 GameState.DebugPrintPower() - CREATE_GAME
        public static readonly Regex createGameRegex = new Regex(@"^D (.*) GameState.DebugPrintPower\(\) - CREATE_GAME");

        //D 02:56:58.7317290 PowerTaskList.DebugPrintPower() - TAG_CHANGE Entity=bums tag=GOLD_REWARD_STATE value=2
        public static readonly Regex goldRewardRegex = new Regex(@"^D .*PowerTaskList.DebugPrintPower\(\) -.*TAG_CHANGE Entity=.*tag=GOLD_REWARD_STATE.*value=[0-9]*");

        //D 02:55:54.0564330 GameState.DebugPrintPower() - FULL_ENTITY - Creating ID=4 CardID=FP1_009
        public static readonly Regex cardIdCreationRegex = new Regex(@"^D .*GameState.DebugPrintPower\(\) - FULL_ENTITY - Creating ID=(?<id>(\d+)) CardID=(?<cardId>(\w*))");
    }
}
