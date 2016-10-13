﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MESACCA.Utilities;
using MESACCA.Models;
using S = System.Data.SqlClient;
using T = System.Threading;

namespace MESACCA.DataBaseManagers
{
    public static class SQLManager
    {
        ///This method returns an ADO.NET connection string. 
        public static string GetSqlConnectionString()
        {
            return Common.GetSqlConnectionString();
        }

        //This method attempts to connect to the SQL database and returns a User object
        public static Users sqlConnection(String username, String password)
        {
            String _username = username;
            String _password = password;
            Users foundUser = null;
            int totalNumberOfTimesToTry = 3;
            int retryIntervalSeconds = 1;

            for (int tries = 1; tries <= totalNumberOfTimesToTry; tries++)
            {
                try
                {
                    if (tries > 1)
                    {
                        System.Threading.Thread.Sleep(1000 * retryIntervalSeconds);
                        retryIntervalSeconds = Convert.ToInt32(retryIntervalSeconds * 1.5);
                    }
                    foundUser = accessDatabase(_username, _password);
                    //Break if an account from the SQL database was found 
                    if (foundUser.AccountType != null)
                    {
                        break;
                    }
                }
                //Break if there is an exception
                catch (Exception Exc)
                {
                    break;
                }
            }
            return foundUser;
        }
       
        //This method connects to the database, reads the database and finding an entry with the same information
        //as the provided username and password and returns a User object with some information 
        public static Users accessDatabase(string username, string password)
        {
            String _username = username;
            String _password = password;
            Users foundUser = null;
            using (var sqlConnection = new S.SqlConnection(GetSqlConnectionString()))
            {
                using (var dbCommand = sqlConnection.CreateCommand())
                {
                    //Opening SQL connection
                    sqlConnection.Open();
                    //Creating SQL query
                    dbCommand.CommandText = @"SELECT * FROM Users WHERE Username = @username AND Password = @password";
                    dbCommand.Parameters.AddWithValue("@username", SecurityUtility.ParseSQL(_username));
                    dbCommand.Parameters.AddWithValue("@password", SecurityUtility.ParseSQL(_password));
                    //Building data reader
                    var dataReader = dbCommand.ExecuteReader();
                    //Advancing to the next record which is the first and only record in this case
                    dataReader.Read();
                    //Storing information from found sql entry into a User object and returning it
                    //I trim all of the found User data because the SQL server seems to add spaces.

                    foundUser = new Users();
                    foundUser.ID = dataReader.GetInt32(0);
                    foundUser.FirstName = dataReader.GetString(1).TrimEnd(' ');
                    foundUser.LastName = dataReader.GetString(2).TrimEnd(' ');
                    foundUser.AccountType = dataReader.GetString(3).TrimEnd(' ');
                    foundUser.Center = dataReader.GetString(4).TrimEnd(' ');
                    foundUser.Email = dataReader.GetString(5).TrimEnd();
                    foundUser.Username = dataReader.GetString(7).TrimEnd(' ');
                    foundUser.Password = dataReader.GetString(8).TrimEnd();
                    //Closing SQL connectioon
                    sqlConnection.Close();
                }
                return foundUser;
            }
        }
        
        public static List<NewsArticle> getNewsPosts()
        {
            List<NewsArticle> returnValue = new List<NewsArticle>();
            try
            {
                using (var sqlConnection = new S.SqlConnection(Common.GetSqlConnectionString()))
                {
                    using (var dbCommand = sqlConnection.CreateCommand())
                    {

                        //Opening SQL connection
                        sqlConnection.Open();
                        //Creating SQL query that updates the SQL table entry and returns the updated table entry
                        dbCommand.CommandText = @"SELECT n.ArticleTitle, n.ArticleBody, n.DateOfArticle, u.FirstName, u.LastName FROM (SELECT top 100 * FROM NewsArticles order by DateOfArticle desc) as n INNER JOIN Users as u on u.ID = n.CreatedByUser";
                        var dataReader = dbCommand.ExecuteReader();
                        var iterator = dataReader.GetEnumerator();
                        while (iterator.MoveNext())
                        {
                            NewsArticleExtention article = new NewsArticleExtention();
                            //Getting the SQL entry information 
                            //I trim all of the found User data because the SQL server seems to add spaces.
                            article.ArticleTitle = dataReader.GetString(0).TrimEnd(' ');
                            article.ArticleBody = dataReader.GetString(1).TrimEnd(' ');
                            article.DateOfArticle = dataReader.GetDateTime(2);
                            article.AuthorName = dataReader.GetString(3).TrimEnd(' ') + " " + dataReader.GetString(4).TrimEnd(' ');
                            returnValue.Add(article);
                        }
                        //Closing SQL connection
                        sqlConnection.Close();
                    }
                }
            }
            catch (Exception ex)
            {

            }

            return returnValue;

        }

        //This method invokes "accessDatabaseForUser" to attempt to connect to the SQL database and returns a User object
        public static User sqlConnectionForUser(int ID)
        {
            User foundUser = new User();
            int totalNumberOfTimesToTry = 3;
            int retryIntervalSeconds = 1;

            for (int tries = 1; tries <= totalNumberOfTimesToTry; tries++)
            {
                try
                {
                    if (tries > 1)
                    {
                        T.Thread.Sleep(1000 * retryIntervalSeconds);
                        retryIntervalSeconds = Convert.ToInt32(retryIntervalSeconds * 1.5);
                    }
                    foundUser = accessDatabaseForUser(ID);
                    //Break if an account from the SQL database was found 
                    if (foundUser.AccountType != null)
                    {
                        break;
                    }
                }
                //Break if there is an exception
                catch (Exception Exc)
                {
                    break;
                }
            }
            return foundUser;
        }

        //This method connects to the database, reads the database and finding an entry with the same information
        //as the provided username and password and returns a User object with all information of the User
        public static User accessDatabaseForUser(int ID)
        {
            User foundUser = new User();
            using (var sqlConnection = new S.SqlConnection(GetSqlConnectionString()))
            {
                using (var dbCommand = sqlConnection.CreateCommand())
                {
                    //Opening SQL connection
                    sqlConnection.Open();
                    //Creating SQL query
                    dbCommand.CommandText = @"SELECT * FROM Users WHERE ID = @ID";
                    dbCommand.Parameters.AddWithValue("@ID", ID);
                    //Building data reader
                    var dataReader = dbCommand.ExecuteReader();
                    //Advancing to the next record which is the first and only record in this case
                    dataReader.Read();
                    //Storing information from found sql entry into a User object and returning it
                    foundUser.ID = dataReader.GetInt32(0);
                    foundUser.FirstName = dataReader.GetString(1).TrimEnd(' ');
                    foundUser.LastName = dataReader.GetString(2).TrimEnd(' ');
                    foundUser.AccountType = dataReader.GetString(3).TrimEnd(' ');
                    foundUser.Center = dataReader.GetString(4).TrimEnd(' ');
                    foundUser.Email = dataReader.GetString(5).TrimEnd(' ');
                    foundUser.PhoneNumber = dataReader.GetString(6).TrimEnd(' ');
                    foundUser.Username = dataReader.GetString(7).TrimEnd(' ');
                    foundUser.Password = dataReader.GetString(8).TrimEnd(' ');
                    foundUser.Home = dataReader.GetString(9).TrimEnd(' ');
                    foundUser.About_Us = dataReader.GetString(10).TrimEnd(' ');
                    foundUser.Vision_Mission_Values = dataReader.GetString(11).TrimEnd(' ');
                    foundUser.MESA_Schools_Program = dataReader.GetString(12).TrimEnd(' ');
                    foundUser.MESA_Community_College_Program = dataReader.GetString(13).TrimEnd(' ');
                    foundUser.MESA_Engineering_Program = dataReader.GetString(14).TrimEnd(' ');
                    foundUser.News = dataReader.GetString(15).TrimEnd(' ');
                    foundUser.Donate = dataReader.GetString(16).TrimEnd(' ');
                    //Closing SQL connection
                    sqlConnection.Close();
                }
                return foundUser;
            }
        }
        
        //This method invokes "updateUserDatabase" to attempt to connect to the SQL database and returns a Boolean value regarding update confirmation
        public static Boolean sqlConnectionUpdateUser(int ID, User updatedUser)
        {
            Boolean success = false;
            int totalNumberOfTimesToTry = 3;
            int retryIntervalSeconds = 1;

            for (int tries = 1; tries <= totalNumberOfTimesToTry; tries++)
            {
                try
                {
                    if (tries > 1)
                    {
                        T.Thread.Sleep(1000 * retryIntervalSeconds);
                        retryIntervalSeconds = Convert.ToInt32(retryIntervalSeconds * 1.5);
                    }
                    success = updateUserDatabase(ID, updatedUser);
                    //Break if an account from the SQL database was found 
                    if (success == true)
                    {
                        break;
                    }
                }
                //Break if there is an exception
                catch (Exception Exc)
                {
                    break;
                }
            }
            return success;
        }

        //This method connects to the database, updates the specified SQL entry by the User's ID, collects the 
        //SQL entry for comparison and return a Boolean value based on the comparisons performed.
        public static Boolean updateUserDatabase(int ID, User updatedUser)
        {
            Boolean success = false;
            User foundUser = new User();
            using (var sqlConnection = new S.SqlConnection(GetSqlConnectionString()))
            {
                using (var dbCommand = sqlConnection.CreateCommand())
                {
                    //Opening SQL connection
                    sqlConnection.Open();
                    //Creating SQL query that updates the SQL table entry and returns the updated table entry
                    dbCommand.CommandText = @"UPDATE Users 
                                              SET FirstName = @FirstName, LastName = @LastName, AccountType = @AccountType, Center = @Center, Email = @Email,
                                                  PhoneNumber = @PhoneNumber, Username = @Username, Password = @Password,
                                                  Home = @Home, About_Us = @About_Us, Vision_Mission_Values = @Vision_Mission_Values,
                                                  MESA_Schools_Program = @MESA_Schools_Program,
                                                  MESA_Community_College_Program = @MESA_Community_College_Program, 
                                                  MESA_Engineering_Program = @MESA_Engineering_Program, 
                                                  News = @News, Donate = @Donate
                                              WHERE ID = @ID
                                              SELECT * FROM Users WHERE ID = @ID";
                    //Updating User information based on comparison with current and new User information
                    //I trim the end of all fields to remove empty spaces
                    dbCommand.Parameters.AddWithValue("@FirstName", updatedUser.FirstName.TrimEnd(' '));
                    dbCommand.Parameters.AddWithValue("@LastName", updatedUser.LastName.TrimEnd(' '));
                    dbCommand.Parameters.AddWithValue("@AccountType", updatedUser.AccountType.TrimEnd(' '));
                    dbCommand.Parameters.AddWithValue("@Center", updatedUser.Center.TrimEnd(' '));
                    dbCommand.Parameters.AddWithValue("@Email", updatedUser.Email.TrimEnd(' '));
                    dbCommand.Parameters.AddWithValue("@PhoneNumber", updatedUser.PhoneNumber.TrimEnd(' '));
                    dbCommand.Parameters.AddWithValue("@UserName", updatedUser.Username.TrimEnd(' '));
                    dbCommand.Parameters.AddWithValue("@Password", updatedUser.Password.TrimEnd(' '));
                    dbCommand.Parameters.AddWithValue("@Home", updatedUser.Home.TrimEnd(' '));
                    dbCommand.Parameters.AddWithValue("@About_Us", updatedUser.About_Us.TrimEnd(' '));
                    dbCommand.Parameters.AddWithValue("@Vision_Mission_Values", updatedUser.Vision_Mission_Values.TrimEnd(' '));
                    dbCommand.Parameters.AddWithValue("@MESA_Schools_Program", updatedUser.MESA_Schools_Program.TrimEnd(' '));
                    dbCommand.Parameters.AddWithValue("@MESA_Community_College_Program", updatedUser.MESA_Community_College_Program.TrimEnd(' '));
                    dbCommand.Parameters.AddWithValue("@MESA_Engineering_Program", updatedUser.MESA_Engineering_Program.TrimEnd(' '));
                    dbCommand.Parameters.AddWithValue("@News", updatedUser.News.TrimEnd(' '));
                    dbCommand.Parameters.AddWithValue("@Donate", updatedUser.Donate.TrimEnd(' '));
                    //Specifing update by ID number to ensure correct User's information is updated
                    dbCommand.Parameters.AddWithValue("@ID", ID);
                    //Building data reader
                    var dataReader = dbCommand.ExecuteReader();
                    dataReader.Read();
                    //Getting the updated SQL entry information for comparison testing to verify the update was successful
                    //I trim all of the found User data because the SQL server seems to add spaces.
                    foundUser.FirstName = dataReader.GetString(1).TrimEnd(' ');
                    foundUser.LastName = dataReader.GetString(2).TrimEnd(' ');
                    foundUser.AccountType = dataReader.GetString(3).TrimEnd(' ');
                    foundUser.Center = dataReader.GetString(4).TrimEnd(' ');
                    foundUser.Email = dataReader.GetString(5).TrimEnd(' ');
                    foundUser.PhoneNumber = dataReader.GetString(6).TrimEnd(' ');
                    foundUser.Username = dataReader.GetString(7).TrimEnd(' ');
                    foundUser.Password = dataReader.GetString(8).TrimEnd(' ');
                    foundUser.Home = dataReader.GetString(9).TrimEnd(' ');
                    foundUser.About_Us = dataReader.GetString(10).TrimEnd(' ');
                    foundUser.Vision_Mission_Values = dataReader.GetString(11).TrimEnd(' ');
                    foundUser.MESA_Schools_Program = dataReader.GetString(12).TrimEnd(' ');
                    foundUser.MESA_Community_College_Program = dataReader.GetString(13).TrimEnd(' ');
                    foundUser.MESA_Engineering_Program = dataReader.GetString(14).TrimEnd(' ');
                    foundUser.News = dataReader.GetString(15).TrimEnd(' ');
                    foundUser.Donate = dataReader.GetString(16).TrimEnd(' ');
                    //Determining if the update was successfully executed by checking if an entry is returned and comparing
                    //all of the returned entry's information with the updated information provided by the user.
                    if (dataReader.HasRows == true && updatedUser.FirstName.Equals(foundUser.FirstName) &&
                        updatedUser.LastName.Equals(foundUser.LastName) &&
                        updatedUser.AccountType.Equals(foundUser.AccountType) &&
                        updatedUser.Center.Equals(foundUser.Center) &&
                        updatedUser.Email.Equals(foundUser.Email) &&
                        updatedUser.PhoneNumber.Equals(foundUser.PhoneNumber) &&
                        updatedUser.Username.Equals(foundUser.Username) &&
                        updatedUser.Password.Equals(foundUser.Password) &&
                        updatedUser.Home.Equals(foundUser.Home) &&
                        updatedUser.About_Us.Equals(foundUser.About_Us) &&
                        updatedUser.Vision_Mission_Values.Equals(foundUser.Vision_Mission_Values) &&
                        updatedUser.MESA_Schools_Program.Equals(foundUser.MESA_Schools_Program) &&
                        updatedUser.MESA_Community_College_Program.Equals(foundUser.MESA_Community_College_Program) &&
                        updatedUser.MESA_Engineering_Program.Equals(foundUser.MESA_Engineering_Program) &&
                        updatedUser.News.Equals(foundUser.News) &&
                        updatedUser.Donate.Equals(foundUser.Donate))
                    {
                        success = true;
                    }
                    //Closing SQL connection
                    sqlConnection.Close();
                }
                return success;
            }
        }
        
        //This method invokes "accessDatabaseForUsers" to attempt to connect to the SQL database and returns a List object containing all Users
        public static List<User> sqlConnectionForUsersList()
        {
            //SortedList<String, User> userList = new SortedList<String, User>();
            List<User> userList = new List<User>();
            int totalNumberOfTimesToTry = 3;
            int retryIntervalSeconds = 1;

            for (int tries = 1; tries <= totalNumberOfTimesToTry; tries++)
            {
                try
                {
                    if (tries > 1)
                    {
                        T.Thread.Sleep(1000 * retryIntervalSeconds);
                        retryIntervalSeconds = Convert.ToInt32(retryIntervalSeconds * 1.5);
                    }
                    userList = accessDatabaseForUsers();
                    //Break if the List object is not empty
                    if (userList.Count > 0)
                    {
                        break;
                    }
                }
                //Break if there is an exception
                catch (Exception Exc)
                {
                    break;
                }
            }
            return userList;
        }

        //This method connects to the database, collects all the entries in the Users table into a list
        //based on Users' account type and returns the list.
        public static List<User> accessDatabaseForUsers()
        {
            List<User> userList = new List<User>();
            using (var sqlConnection = new S.SqlConnection(GetSqlConnectionString()))
            {
                using (var dbCommand = sqlConnection.CreateCommand())
                {
                    //Opening SQL connection
                    sqlConnection.Open();
                    //Creating SQL query that updates the SQL table entry and returns the updated table entry
                    dbCommand.CommandText = @"SELECT * FROM Users";
                    var dataReader = dbCommand.ExecuteReader();
                    var iterator = dataReader.GetEnumerator();
                    while (iterator.MoveNext())
                    {
                        User foundUser = new User();
                        //Getting the SQL entry information 
                        //I trim all of the found User data because the SQL server seems to add spaces.
                        foundUser.ID = dataReader.GetInt32(0);
                        foundUser.FirstName = dataReader.GetString(1).TrimEnd(' ');
                        foundUser.LastName = dataReader.GetString(2).TrimEnd(' ');
                        foundUser.AccountType = dataReader.GetString(3).TrimEnd(' ');
                        foundUser.Center = dataReader.GetString(4).TrimEnd(' ');
                        foundUser.Email = dataReader.GetString(5).TrimEnd(' ');
                        foundUser.PhoneNumber = dataReader.GetString(6).TrimEnd(' ');
                        foundUser.Username = dataReader.GetString(7).TrimEnd(' ');
                        foundUser.Password = dataReader.GetString(8).TrimEnd(' ');
                        userList.Add(foundUser);
                    }
                    //Closing SQL connection
                    sqlConnection.Close();
                }
                return userList;
            }
        }

        //This method invokes "accessDatabaseToAddUser" to attempt to connect to the SQL database and a Boolean value regarding account creation confirmation
        public static Boolean sqlConnectionAddUser(int newID, User newUser)
        {
            Boolean success = false;
            int totalNumberOfTimesToTry = 3;
            int retryIntervalSeconds = 1;

            for (int tries = 1; tries <= totalNumberOfTimesToTry; tries++)
            {
                try
                {
                    if (tries > 1)
                    {
                        T.Thread.Sleep(1000 * retryIntervalSeconds);
                        retryIntervalSeconds = Convert.ToInt32(retryIntervalSeconds * 1.5);
                    }
                    success = accessDatabaseToAddUser(newID, newUser);
                    //Break if an account added to the SQL database 
                    if (success == true)
                    {
                        break;
                    }
                }
                //Break if there is an exception
                catch (Exception Exc)
                {
                    break;
                }
            }
            return success;
        }
        
        //This method connects to the database, adds to the Users table, collects Users from the table for comparison,
        //and returns Boolean value regarding success
        public static Boolean accessDatabaseToAddUser(int newID, User newUser)
        {
            Boolean success = false;
            User foundUser = new User();
            using (var sqlConnection = new S.SqlConnection(GetSqlConnectionString()))
            {
                using (var dbCommand = sqlConnection.CreateCommand())
                {
                    //Opening SQL connection
                    sqlConnection.Open();
                    //Creating SQL query
                    dbCommand.CommandText = @"INSERT INTO Users (ID, FirstName, LastName, AccountType, Center, Email, PhoneNumber, Username, Password,
                                                                 Home, About_Us, Vision_Mission_Values, MESA_Schools_Program, MESA_Community_College_Program, 
                                                                 MESA_Engineering_Program, News, Donate)
                                              Values (@ID, @FirstName, @LastName, @AccountType, @Center, @Email, @PhoneNumber, @Username, @Password,
                                                      @Home, @About_Us, @Vision_Mission_Values, @MESA_Schools_Program, @MESA_Community_College_Program, 
                                                      @MESA_Engineering_Program, @News, @Donate)
                                              Select * FROM Users WHERE ID = @ID";
                    dbCommand.Parameters.AddWithValue("@ID", newID);
                    //I trim the ends of empty spaces
                    dbCommand.Parameters.AddWithValue("@FirstName", newUser.FirstName.TrimEnd(' '));
                    dbCommand.Parameters.AddWithValue("@LastName", newUser.LastName.TrimEnd(' '));
                    dbCommand.Parameters.AddWithValue("@AccountType", newUser.AccountType.TrimEnd(' '));
                    dbCommand.Parameters.AddWithValue("@Center", newUser.Center.TrimEnd(' '));
                    dbCommand.Parameters.AddWithValue("@Email", newUser.Email.TrimEnd(' '));
                    dbCommand.Parameters.AddWithValue("@PhoneNumber", newUser.PhoneNumber.TrimEnd(' '));
                    dbCommand.Parameters.AddWithValue("@Username", newUser.Username.TrimEnd(' '));
                    dbCommand.Parameters.AddWithValue("@Password", newUser.Password.TrimEnd(' '));
                    dbCommand.Parameters.AddWithValue("@Home", newUser.Home.TrimEnd(' '));
                    dbCommand.Parameters.AddWithValue("@About_Us", newUser.About_Us.TrimEnd(' '));
                    dbCommand.Parameters.AddWithValue("@Vision_Mission_Values", newUser.Vision_Mission_Values.TrimEnd(' '));
                    dbCommand.Parameters.AddWithValue("@MESA_Schools_Program", newUser.MESA_Schools_Program.TrimEnd(' '));
                    dbCommand.Parameters.AddWithValue("@MESA_Community_College_Program", newUser.MESA_Community_College_Program.TrimEnd(' '));
                    dbCommand.Parameters.AddWithValue("@MESA_Engineering_Program", newUser.MESA_Engineering_Program.TrimEnd(' '));
                    dbCommand.Parameters.AddWithValue("@News", newUser.News.TrimEnd(' '));
                    dbCommand.Parameters.AddWithValue("@Donate", newUser.Donate.TrimEnd(' '));
                    //Building data reader
                    var dataReader = dbCommand.ExecuteReader();
                    //Advancing to the next record which is the first and only record in this case
                    dataReader.Read();
                    //Storing information from found sql entry into a User object 
                    //I trim all of the found User data because the SQL server seems to add spaces.
                    foundUser.ID = dataReader.GetInt32(0);
                    foundUser.FirstName = dataReader.GetString(1).TrimEnd(' ');
                    foundUser.LastName = dataReader.GetString(2).TrimEnd(' ');
                    foundUser.AccountType = dataReader.GetString(3).TrimEnd(' ');
                    foundUser.Center = dataReader.GetString(4).TrimEnd(' ');
                    foundUser.Email = dataReader.GetString(5).TrimEnd(' ');
                    foundUser.PhoneNumber = dataReader.GetString(6).TrimEnd(' ');
                    foundUser.Username = dataReader.GetString(7).TrimEnd(' ');
                    foundUser.Password = dataReader.GetString(8).TrimEnd(' ');
                    //Determining if the table entry was successfully executed by checking if an entry is returned and comparing
                    //all of the returned entry's information with the new User's information.
                    if (dataReader.HasRows == true && newUser.FirstName.Equals(foundUser.FirstName) &&
                        newUser.LastName.Equals(foundUser.LastName) &&
                        newUser.Center.Equals(foundUser.Center) &&
                        newUser.Email.Equals(foundUser.Email) &&
                        newUser.PhoneNumber.Equals(foundUser.PhoneNumber) &&
                        newUser.Username.Equals(foundUser.Username) &&
                        newUser.Password.Equals(foundUser.Password))
                    {
                        success = true;
                    }
                    //Closing SQL connection
                    sqlConnection.Close();
                }
                return success;
            }
        }
        
        //This method invokes "accessDatabaseToDeleteUser" to attempt to connect to the SQL database and returns a Boolean value regarding deletion confirmation
        public static Boolean sqlConnectionDeleteUser(int ID)
        {
            Boolean success = false;
            int totalNumberOfTimesToTry = 3;
            int retryIntervalSeconds = 1;

            for (int tries = 1; tries <= totalNumberOfTimesToTry; tries++)
            {
                try
                {
                    if (tries > 1)
                    {
                        T.Thread.Sleep(1000 * retryIntervalSeconds);
                        retryIntervalSeconds = Convert.ToInt32(retryIntervalSeconds * 1.5);
                    }
                    success = accessDatabaseToDeleteUser(ID);
                    //Break if an account from the SQL database was found to be gone
                    if (success == true)
                    {
                        break;
                    }
                }
                //Break if there is an exception
                catch (Exception Exc)
                {
                    break;
                }
            }
            return success;
        }
        
        //This method connects to the database, delete the entry with the given ID, connects with the database again
        //to check if the entry is gone and returns the Boolean result of the check.
        public static Boolean accessDatabaseToDeleteUser(int ID)
        {
            Boolean success = false;
            User foundUser = new User();
            using (var sqlConnection = new S.SqlConnection(GetSqlConnectionString()))
            {
                using (var dbCommand = sqlConnection.CreateCommand())
                {
                    //Opening SQL connection
                    sqlConnection.Open();
                    //Creating SQL query
                    dbCommand.CommandText = @" DELETE FROM Users WHERE ID = @ID
                                               SELECT * FROM Users WHERE ID = @ID";
                    dbCommand.Parameters.AddWithValue("@ID", ID);
                    //Building data reader
                    var dataReader = dbCommand.ExecuteReader();
                    dataReader.Read();
                    //If the User can't be found, then the User was successfully deleted 
                    if (dataReader.HasRows == false)
                    {
                        success = true;
                    }
                    //Closing SQL connection
                    sqlConnection.Close();
                }
                return success;
            }
        }
        
    }
}