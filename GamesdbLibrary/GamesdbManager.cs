using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
namespace GamesdbLibrary
{
    public class GamesdbManager
    {
        private string _connectionstring;
        public GamesdbManager(string _ConnectionString)
        {
            _connectionstring = _ConnectionString;
        }
        public IEnumerable<Esubscribtion> GetEsubscribtions()
        {
            SqlConnection Con = new SqlConnection(_connectionstring);
            SqlCommand cmd = Con.CreateCommand();
            cmd.CommandText =@"select * from Enotification";
            Con.Open();
            List<Esubscribtion> subs = new List<Esubscribtion>();
            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                subs.Add(new Esubscribtion
                {
                    Id = (int)reader["Id"],
                    FirstName = (string)reader["FirstName"],
                    LastName = (string)reader["LastName"],
                    Email = (string)reader["Email"],
                });
            }
            Con.Close();
            Con.Dispose();
            return subs;
        }
        public int AddEsubscribtion(Esubscribtion sub)
        {

            SqlConnection Con = new SqlConnection(_connectionstring);
            SqlCommand cmd = Con.CreateCommand();
            cmd.CommandText = @"insert into Enotification
                                Values(@FN,@LN,@email)
                                select SCOPE_IDENTITY()";
            cmd.Parameters.AddWithValue("@Fn", sub.FirstName);
            cmd.Parameters.AddWithValue("@LN", sub.LastName);
            cmd.Parameters.AddWithValue("@Email", sub.Email);
            Con.Open();
            int SI = (int)(decimal)cmd.ExecuteScalar();
            sendconfirmation(sub);
            return SI;
        }
        public Game GetNextGame()
        {
            SqlConnection conn = new SqlConnection(_connectionstring);
            SqlCommand cmd = conn.CreateCommand();
            cmd.CommandText = @"SELECT g.Id,g.Date,g.MaxPlayers,COUNT(Players.Gameid) AS NumberOfPlayers, MIN(g.Date) FROM Game g
                                JOIN Players ON Players.Gameid = g.Id
                                where g.Date > GETDATE()
                                GROUP BY g.Id, g.Date, g.MaxPlayers;";
            conn.Open();
            SqlDataReader reader = cmd.ExecuteReader();
            Game game = new Game();
            while (reader.Read())
            {
                game.Id = (int)reader["Id"];
                game.datetime = (DateTime)reader["Date"];
                game.MaxPlayers = (int)reader["MaxPlayers"];
                game.PLayerCnt = (int)reader["NumberOfPlayers"];
            }
            conn.Close();
            conn.Dispose();
            return game;
        }
        public IEnumerable<Game> GetGames(bool WithPlayers)
        {
            SqlConnection conn = new SqlConnection(_connectionstring);
            SqlCommand cmd = conn.CreateCommand();
            cmd.CommandText = @"SELECT g.Id,g.Date,g.MaxPlayers,COUNT(Players.Gameid) AS NumberOfPlayers FROM Game g
                                JOIN Players ON Players.Gameid = g.Id
                                GROUP BY g.Id, g.Date, g.MaxPlayers;";
            conn.Open();
            SqlDataReader reader = cmd.ExecuteReader();
            List<Game> games = new List<Game>();
            while(reader.Read())
            {
                Game g = new Game {
                    Id=(int)reader["Id"],
                    datetime=(DateTime)reader["Date"],
                    MaxPlayers=(int)reader["MaxPlayers"],
                    PLayerCnt=(int)reader["NumberOfPlayers"]
                };
                if(WithPlayers)
                {
                    g.Players = GetPlayers(g.Id);
                }
                games.Add(g);
            }
            conn.Close();
            conn.Dispose();
            return games;
        }
        public IEnumerable<Player> GetPlayers(int? id)
        {
            SqlConnection Con = new SqlConnection(_connectionstring);
            SqlCommand cmd = Con.CreateCommand();
            cmd.CommandText = @"Select * from Players";
            if (id != null)
            {
                cmd.CommandText += " where Gameid = @id";
                cmd.Parameters.AddWithValue("@id", id);
            }
            Con.Open();
            List<Player> players = new List<Player>();
            List<int> dids = new List<int>();
            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                Player p = new Player {
                    Id = (int)reader["Id"],
                    FirstName=(string)reader["FirstName"],
                    LastName=(string)reader["LastName"],
                    Email=(string)reader["Email"],
                    Gameid=(int)reader["Gameid"]
                };
                if(players.Any(l =>(l.FirstName.ToLower() == p.FirstName.ToLower() && l.LastName.ToLower() == p.LastName.ToLower() && l.Email.ToLower() == p.Email.ToLower() && l.Gameid == p.Gameid)))
                {
                    dids.Add(p.Id);
                }
                players.Add(p);
            }
            Con.Close();
            Con.Dispose();
            deleteduplicates(dids);
            return players;
        }
        public int AddGame(Game game)
        {
            SqlConnection Con = new SqlConnection(_connectionstring);
            SqlCommand cmd = Con.CreateCommand();
            cmd.CommandText = @"insert into game
                                Values(@Date,@Max)
                                select SCOPE_IDENTITY()";
            cmd.Parameters.AddWithValue("@Date", game.datetime);
            cmd.Parameters.AddWithValue("@Max", game.MaxPlayers);
            Con.Open();
            int SI = (int)(decimal)cmd.ExecuteScalar();
            sendnotifications(game.datetime);
            return SI;
        }
        public int AddPlayer(Player player)
        {
            SqlConnection Con = new SqlConnection(_connectionstring);
            SqlCommand cmd = Con.CreateCommand();
            cmd.CommandText = @"insert into Players
                                Values(@FN,@LN,@email,@Gid)
                                select SCOPE_IDENTITY()";
            cmd.Parameters.AddWithValue("@Fn",player.FirstName);
            cmd.Parameters.AddWithValue("@LN",player.LastName);
            cmd.Parameters.AddWithValue("@Email", player.Email);
            cmd.Parameters.AddWithValue("@Gid",player.Gameid);
            Con.Open();
            int SI = (int)(decimal)cmd.ExecuteScalar();
            return SI;
        }
        public void DeleteGame(int id)
        {
            SqlConnection Con = new SqlConnection(_connectionstring);
            SqlCommand cmd = Con.CreateCommand();
            cmd.CommandText = @"delete from players where Gameid = @id
                                delete from Game where Id = @id";
            cmd.Parameters.AddWithValue("@id", id);
            Con.Open();
            cmd.ExecuteNonQuery();
        }
        public void DeletePlayer(int id)
        {
            SqlConnection Con = new SqlConnection(_connectionstring);
            SqlCommand cmd = Con.CreateCommand();
            cmd.CommandText = @"delete from players where id = @id";
            cmd.Parameters.AddWithValue("@id", id);
            Con.Open();
            cmd.ExecuteNonQuery();
        }

        private void sendnotifications(DateTime gTime)
        {
            List<Esubscribtion> subsc =(List<Esubscribtion>) GetEsubscribtions();
            List<string> subs = subsc.Select(s => s.Email).ToList(); 
            EmailContent cnt = new EmailContent();
            cnt.Subject = $"New game is up {DateTime.Now}!";
            cnt.To = "no@one.com";
            cnt.message = $"The admin put a new game up! Quick! before you're out again! Game's Date-{gTime.ToLongDateString()} At:{gTime.ToLongTimeString()}";
            EmailManager mngr = new EmailManager();
            mngr.SendNotificationToMultiple(cnt, subs);
        }
        private void sendconfirmation(Esubscribtion sub)
        {
            EmailContent cnt = new EmailContent();
            cnt.Subject = $"Welcome {sub.FirstName} {sub.LastName}";
            cnt.To = sub.Email;
            cnt.message = $"Welcome {sub.FirstName} {sub.LastName}! You have successfully subscribed! You will recieve a notification every time the admin posts a new game. Sorry, no way to unsubscribe yet..:P";
            EmailManager mngr = new EmailManager();
            mngr.SendNotification(cnt);
        }
        private void deleteduplicates(List<int> Duplicateids)
        {
            if(Duplicateids.Count < 1)
            {
                return;
            }
            SqlConnection Con = new SqlConnection(_connectionstring);
            SqlCommand cmd = Con.CreateCommand();
            foreach (int n in Duplicateids)
            {
                cmd.CommandText += @"delete from players where id = @id";
                cmd.Parameters.AddWithValue("@id", n);
            }
            Con.Open();
            cmd.ExecuteNonQuery();
        }
    }
}
