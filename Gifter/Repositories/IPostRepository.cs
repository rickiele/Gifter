using Gifter.Models;
using System;
using System.Collections.Generic;

namespace Gifter.Repositories
{
    public interface IPostRepository
    {
        void Add(Post post);
        void Delete(int id);
        List<Post> GetAll();
        List<Post> GetAllWithComments();
        Post GetById(int id);
        List<Post> Search(string criterion, bool sortDescending);
        void Update(Post post);
        Post GetPostIdWithComments(int postId);
        List<Post> SearchforHottest(DateTime datetime);

    }
}