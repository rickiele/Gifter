using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Gifter.Models
{
    public class UserProfileWithPostsNComments
    {
        public UserProfile UserProfile { get; set; }
        public List<Post> Posts { get; set; }
        public List<Comment> Comments { get; set; }
    }
}
