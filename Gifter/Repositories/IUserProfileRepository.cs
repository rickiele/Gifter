﻿using Gifter.Models;
using System.Collections.Generic;

namespace Gifter.Repositories
{
    public interface IUserProfileRepository
    {
        void Add(UserProfile user);
        void Delete(int id);
        List<UserProfile> GetAll();
        UserProfile GetById(int id);
        void Update(UserProfile user);
        UserProfile GetUserIdWithComments(int userId);
        UserProfileWithPostsNComments GetUserWithPostsAndComments(int userId);
    }
}