using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class MatchSQL
{
    public MatchSQL(int match_id, int player_id, int level_number, double score, DateTime date)
    {
        this.match_id = match_id;
        this.player_id = player_id;
        this.level_number = level_number;
        this.score = score;
        this.date = date;
    }

    public int match_id;
    public int player_id;
    public int level_number;
    public double score;
    public DateTime date;
}

[Serializable]
public class PlayerSQL
{
    public PlayerSQL(int playerId, string firstName, string lastName, DateTime dob, string email,string nickname,bool optIn)
    {
        player_id = playerId;
        first_name = firstName;
        last_name = lastName;
        date_of_birth = dob;
        this.email = email;
        this.nickname = nickname;
        opt_in = optIn;
    }

    public int player_id;
    public string first_name;
    public string last_name;
    public DateTime date_of_birth;
    public string email;
    public string nickname;
    public bool opt_in;
}
