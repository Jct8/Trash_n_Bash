using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class MatchMongo
{
    public MatchMongo(int level_number, double score, DateTime date)
    {
        this.level_number = level_number;
        this.score = score;
        this.date = date;
    }
    public int level_number;
    public double score;
    public DateTime date;
}

[Serializable]
public class PlayerMongo
{
    public PlayerMongo(string id,int playerId, string firstName, string lastName, DateTime dob, string email, string nickname, bool optIn, List<MatchMongo> matches)
    {
        Id = id;
        player_id = playerId;
        first_name = firstName;
        last_name = lastName;
        date_of_birth = dob;
        this.email = email;
        this.nickname = nickname;
        opt_in = optIn;
        this.matches = matches;
    }
    public string Id;
    public int player_id;
    public string first_name;
    public string last_name;
    public DateTime date_of_birth;
    public string email;
    public string nickname;
    public bool opt_in;
    public List<MatchMongo> matches;
}

[Serializable]
public class MatchDuration
{
    public DateTime FromDate;
    public DateTime ToDate;
}

[Serializable]
public class TopTenMatch
{
    public string player_nickname;
    public int level_number;
    public double score;
    public DateTime date;
}

