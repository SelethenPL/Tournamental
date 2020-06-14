using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Tournamental.Models
{
    public partial class Match
    {
        public Match(int tournamentid, int teamid1, int teamid2, int roundnumber, int winner)
        {
            this.tournamentid = tournamentid;
            this.teamid1 = teamid1;
            this.teamid2 = teamid2;
            this.roundnumber = roundnumber;
            this.winner = winner;
        }
    }
}