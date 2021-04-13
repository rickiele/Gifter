using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using Gifter.Models;
using Gifter.Utils;
using Microsoft.Data.SqlClient;

namespace Gifter.Repositories
{
    public class UserProfileRepository : BaseRepository, IUserProfileRepository
    {
        public UserProfileRepository(IConfiguration configuration) : base(configuration) { }

        public List<UserProfile> GetAll()
        {
            using (var conn = Connection)
            {
                conn.Open();
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                    SELECT  up.Id AS UserProfileId, up.Name, up.Email, up.ImageUrl AS UserProfileImageUrl, up.Bio,
                            up.DateCreated AS UserProfileDateCreated
                    FROM    UserProfile up
                ORDER BY    up.DateCreated";

                    using (var reader = cmd.ExecuteReader())
                    {
                        var userProfiles = new List<UserProfile>();
                        while (reader.Read())
                        {
                            userProfiles.Add(new UserProfile()
                            {
                                Id = DbUtils.GetInt(reader, "UserProfileId"),
                                Name = DbUtils.GetString(reader, "Name"),
                                Email = DbUtils.GetString(reader, "Email"),
                                ImageUrl = DbUtils.GetString(reader, "UserProfileImageUrl"),
                                Bio = DbUtils.GetString(reader, "Bio"),
                                DateCreated = DbUtils.GetDateTime(reader, "UserProfileDateCreated")
                            });
                        }

                        reader.Close();

                        return userProfiles;
                    }
                }
            }
        }

        public UserProfile GetById(int id)
        {
            using (var conn = Connection)
            {
                conn.Open();
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                    SELECT  up.Id AS UserProfileId, up.Name, up.Email, up.ImageUrl AS UserProfileImageUrl, up.Bio
                            up.DateCreated AS UserProfileDateCreated, 

                    FROM    UserProfile up
                    WHERE   up.Id = @Id";

                    DbUtils.AddParameter(cmd, "@Id", id);

                    var reader = cmd.ExecuteReader();

                    UserProfile userProfile = null;
                    while (reader.Read())
                    {
                        userProfile = new UserProfile()
                        {
                            Id = DbUtils.GetInt(reader, "UserProfileId"),
                            Name = DbUtils.GetString(reader, "UserName"),
                            Email = DbUtils.GetString(reader, "Email"),
                            DateCreated = DbUtils.GetDateTime(reader, "UserProfileDateCreated"),
                            ImageUrl = DbUtils.GetString(reader, "ImageUrl"),
                        };
                    }

                    reader.Close();

                    return userProfile;
                }
            }
        }

        public UserProfileWithPostsNComments GetUserWithPostsAndComments(int userId)
        {
            using (SqlConnection sqlConn = Connection)
            {
                sqlConn.Open();
                using (SqlCommand sqlCmd = sqlConn.CreateCommand())
                {
                    sqlCmd.CommandText = @"
                    SELECT  up.Id AS UserProfileId, up.Name AS UserName, up.Bio, up.Email, up.DateCreated AS ProfileDateCreated,
                            up.ImageUrl AS UserProfileImageUrl, 

                            c.Id AS CommentId, c.Message, c.UserProfileId AS CommentUserId, c.PostId AS CommentPostId,

                            p.Id AS PostId, p.Title, p.Caption, p.DateCreated AS PostDateCreated, p.ImageUrl AS PostImageUrl

                    FROM    UserProfile up
               LEFT JOIN    Comment c on c.UserProfileId = up.Id
               LEFT JOIN    Post p on p.UserProfileId = p.Id

                   WHERE    up.Id = @Id";

                    DbUtils.AddParameter(sqlCmd, "@Id", userId);

                    var reader = sqlCmd.ExecuteReader();

                    UserProfileWithPostsNComments userProfileWithPostsNComments = null;

                    while (reader.Read())
                    {
                        var userProfileId = DbUtils.GetInt(reader, "UserProfileId");

                        if (userProfileWithPostsNComments == null)
                        {
                            userProfileWithPostsNComments = new UserProfileWithPostsNComments()
                            {
                                UserProfile = new UserProfile()
                                {
                                    Id = userProfileId,
                                    Name = DbUtils.GetString(reader, "UserName"),
                                    Bio = DbUtils.GetString(reader, "Bio"),
                                    Email = DbUtils.GetString(reader, "Email"),
                                    DateCreated = DbUtils.GetDateTime(reader, "ProfileDateCreated")
                                },

                                Posts = new List<Post>(),
                                Comments = new List<Comment>()
                            };
                        }

                        if (DbUtils.IsNotDbNull(reader, "PostId"))
                        {
                            userProfileWithPostsNComments.Posts.Add(new Post()
                            {
                                Id = DbUtils.GetInt(reader, "PostId"),
                                Title = DbUtils.GetString(reader, "Title"),
                                Caption = DbUtils.GetString(reader, "Caption"),
                                DateCreated = DbUtils.GetDateTime(reader, "PostDateCreated"),
                                ImageUrl = DbUtils.GetString(reader, "PostImageUrl"),
                                UserProfileId = userProfileId
                            });
                        }

                        if (DbUtils.IsNotDbNull(reader, "CommentId"))
                        {
                            userProfileWithPostsNComments.Comments.Add(new Comment()
                            {
                                Id = DbUtils.GetInt(reader, "CommentId"),
                                Message = DbUtils.GetString(reader, "Message"),
                                UserProfileId = userId,
                                PostId = DbUtils.GetInt(reader, "CommentPostId")
                            });

                        }
                    }

                    reader.Close();

                    return userProfileWithPostsNComments;

                }
            }

        }

        public UserProfile GetUserIdWithComments(int userId)
        {
            using (var conn = Connection)
            {
                conn.Open();
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                      SELECT      up.Id AS UserProfileId, up.Name AS UserName, up.Bio, up.Email, up.DateCreated AS UserProfileDateCreated,
                                  up.ImageUrl AS UserProfileImageUrl,

                                  c.Id AS CommentId, c.Message, c.UserProfileId AS CommentUserProfileId
                           FROM UserProfile up
                                LEFT JOIN Comment c on c.UserProfileId = up.id
                           WHERE up.Id = @Id";

                    DbUtils.AddParameter(cmd, "@Id", userId);

                    var reader = cmd.ExecuteReader();

                    UserProfile user = null;
                    while (reader.Read())
                    {
                        if (user == null)
                        {
                            user = new UserProfile()
                            {
                                Id = userId,
                                Name = DbUtils.GetString(reader, "UserName"),
                                Email = DbUtils.GetString(reader, "Email"),
                                DateCreated = DbUtils.GetDateTime(reader, "UserProfileDateCreated"),
                                ImageUrl = DbUtils.GetString(reader, "UserProfileImageUrl"),

                                Comments = new List<Comment>()
                            };
                        }

                        if (DbUtils.IsNotDbNull(reader, "CommentId"))
                        {
                            user.Comments.Add(new Comment()
                            {
                                Id = DbUtils.GetInt(reader, "CommentId"),
                                Message = DbUtils.GetString(reader, "Message"),
                                UserProfileId = userId
                            });

                        }

                    }

                    reader.Close();

                    return user;
                }
            }
        }


        public void Add(UserProfile user)
        {
            using (var conn = Connection)
            {
                conn.Open();
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                        INSERT INTO UserProfile (Name, Email, ImageUrl, Bio, DateCreated)
                        OUTPUT INSERTED.ID
                        VALUES (@Name, @Email, @ImageUrl, @Bio, @DateCreated)";

                    DbUtils.AddParameter(cmd, "@Name", user.Name);
                    DbUtils.AddParameter(cmd, "@Email", user.Email);
                    DbUtils.AddParameter(cmd, "@ImageUrl", user.ImageUrl);
                    DbUtils.AddParameter(cmd, "@Bio", user.Bio);
                    DbUtils.AddParameter(cmd, "@DateCreated", user.DateCreated);

                    user.Id = (int)cmd.ExecuteScalar();
                }
            }
        }

        public void Update(UserProfile user)
        {
            using (var conn = Connection)
            {
                conn.Open();
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                        UPDATE UserProfile
                           SET Name = @Name,
                               Email = @Email,
                               ImageUrl = @ImageUrl,
                               Bio = @Bio,
                               DateCreated = @DateCreated
                         WHERE Id = @Id";

                    DbUtils.AddParameter(cmd, "@Name", user.Name);
                    DbUtils.AddParameter(cmd, "@Email", user.Email);
                    DbUtils.AddParameter(cmd, "@ImageUrl", user.ImageUrl);
                    DbUtils.AddParameter(cmd, "@Bio", user.Bio);
                    DbUtils.AddParameter(cmd, "@DateCreated", user.DateCreated);
                    DbUtils.AddParameter(cmd, "@Id", user.Id);

                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void Delete(int id)
        {
            using (var conn = Connection)
            {
                conn.Open();
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "DELETE FROM UserProfile WHERE Id = @Id";
                    DbUtils.AddParameter(cmd, "@id", id);
                    cmd.ExecuteNonQuery();
                }
            }
        }

    }
}