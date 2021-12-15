using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using FaceAI.Classes;
using FaceAI.Exceptions;
using Microsoft.Data.SqlClient;

namespace FaceAI.Azure.Database
{
    class Database
    {
        SqlConnectionStringBuilder builder;

        public Database()
        {
            builder = new SqlConnectionStringBuilder();
            builder.DataSource = ConfigurationManager.AppSettings.Get("DBS_SOURCE");
            builder.UserID = ConfigurationManager.AppSettings.Get("USERID");
            builder.Password = ConfigurationManager.AppSettings.Get("PASSWORD");
            builder.InitialCatalog = ConfigurationManager.AppSettings.Get("INITIAL");
        }

        public void Insert(string query)
        {
            using (SqlConnection con = new SqlConnection(builder.ConnectionString))
            {
                InputRecord(query, con);
            }
        }

        public User GetUser(string username, string password)
        {
            String query = String.Format("SELECT username, first_name, surname FROM registered WHERE username = '{0}' AND password = '{1}'", username, password);
            User user = null;

            using (SqlConnection con = new SqlConnection(builder.ConnectionString))
            {
                using (SqlCommand command = new SqlCommand(query, con))
                {
                    con.Open();

                    using (SqlDataReader reader = command.ExecuteReader())
                    {

                        while (reader.Read())
                        {
                            user = new User(reader.GetString(0).Trim(), null, reader.GetString(1).Trim(), reader.GetString(2).Trim());
                        }
                        reader.Close();
                    }
                    con.Close();
                }

                if (user == null)
                {
                    throw new UserExistsException("Username or password is incorrect.");
                }
            }

            return user;
        }

        public List<string> GetImageFiles(string username)
        {
            String query = String.Format("SELECT faceID FROM faceLinks WHERE username = '{0}'", username);
            List<string> faces = new List<string>();

            using (SqlConnection con = new SqlConnection(builder.ConnectionString))
            {
                using (SqlCommand command = new SqlCommand(query, con))
                {
                    con.Open();

                    using (SqlDataReader reader = command.ExecuteReader())
                    {

                        while (reader.Read())
                        {
                            faces.Add(reader.GetString(0));
                        }
                        reader.Close();
                    }
                    con.Close();
                }
            }

            return faces;
        }

        public void NewUser(User newUser, string file_name)
        {
            // Ensure that all fields are occupied.
            if (newUser.Username.Equals("") || newUser.Password.Equals("") || newUser.First_name.Equals("") || newUser.Surname.Equals(""))
            {
                throw new DatabaseInsertException("Ensure all fields are filled!");
            }

            // The query to check that the 
            String query = "SELECT username FROM registered WHERE username = '" + newUser.Username +"'";

            using (SqlConnection con = new SqlConnection(builder.ConnectionString))
            {
                // We are checking that the username does not already exist. This will prevent a password collision in the future. 
                String existingUser = null;
                using (SqlCommand command = new SqlCommand(query, con))
                {
                    con.Open();
                    SqlDataReader reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        IDataRecord record = (IDataRecord)reader;
                        existingUser = record[0].ToString();

                        if(existingUser != null) // We only need one match so we don't need to check the entire return
                        {
                            break;
                        }
                    }
                    // Close the reader and connection.
                    reader.Close();

                    con.Close();
                }

                // Throw and error if the user already exists.
                if(existingUser != null)
                {
                    throw new UserExistsException(String.Format("User: {0} already exists", newUser.Username));
                }

                // Save a user
                query = String.Format("INSERT INTO registered (username, password, first_name, surname) VALUES ('{0}','{1}','{2}','{3}')", newUser.Username, newUser.Password, newUser.First_name, newUser.Surname);

                InputRecord(query, con);

                // This is to be able to link a face to a user so all faces relate to a user
                query = String.Format("INSERT INTO faceLinks (faceID, username) VALUES ('{0}','{1}')", file_name, newUser.Username);

                InputRecord(query, con);
            }

        }

        public List<Profiles> GetUserProfiles(string username)
        {
            List<Profiles> profiles = new List<Profiles>();

            String query = String.Format("SELECT site, site_username, link FROM siteLinks WHERE username = '{0}'", username);

            using (SqlConnection con = new SqlConnection(builder.ConnectionString))
            {
                using (SqlCommand command = new SqlCommand(query, con))
                {
                    con.Open();

                    using (SqlDataReader reader = command.ExecuteReader())
                    {

                        while (reader.Read())
                        {
                            profiles.Add(new Profiles(reader.GetString(0).Trim(), reader.GetString(1).Trim(), reader.GetString(2).Trim()));
                        }
                        reader.Close();
                    }
                    con.Close();
                }
            }

            return profiles;
        }

        private void InputRecord(string query, SqlConnection con)
        {
            using (SqlCommand command = new SqlCommand(query, con))
            {
                // Open the connection, save the data, close the connection
                con.Open();
                SqlDataReader reader = command.ExecuteReader();
                reader.Close();
                con.Close();
            }
        }

        public User FindUser(string filename)
        {
            User thisUser = null;

            string query = String.Format("SELECT registered.username, first_name, surname, faceID FROM facelinks INNER JOIN registered ON facelinks.username = registered.username WHERE faceLinks.faceID = '{0}'", filename);

            using (SqlConnection con = new SqlConnection(builder.ConnectionString))
            {
                using (SqlCommand command = new SqlCommand(query, con))
                {
                    con.Open();

                    using (SqlDataReader reader = command.ExecuteReader())
                    {

                        while (reader.Read())
                        {
                            List<string> image = new List<string>();
                            image.Add(filename);
                            thisUser = new User(reader.GetString(0).Trim(), null, reader.GetString(1).Trim(), reader.GetString(2).Trim(), image, null);
                            break;
                        }
                        reader.Close();
                    }
                    con.Close();
                }
            }
            return thisUser;
        }
    }
}

