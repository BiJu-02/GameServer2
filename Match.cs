using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer2
{
    internal class Match
    {
        public enum MatchState
        {
            Queued,
            MatchFound,
            Start,
            End
        }

        public string Id;
        public Client[] Player = new Client[2];

        public Match(Client _player1, Client _player2) 
        {
            Player[0] = _player1;
            Player[1] = _player2;
            Player[0].SelfIndex = 0;
            Player[0].OppIndex = 1;
            Player[1].SelfIndex = 1;
            Player[1].OppIndex = 0;
            Id = Player[0].Id + Player[1].Id;
            _player1.MatchId = Id;
            _player2.MatchId = Id;
        }

    }
}
