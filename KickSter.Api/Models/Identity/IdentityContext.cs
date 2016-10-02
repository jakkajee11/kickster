using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MongoDB.Driver;
using MongoDB.Driver.Core;

namespace KickSter.Models.Identity
{
    public class IdentityContext<T>
    {
        
        public IMongoCollection<T> Users { get; private set; }

        public IdentityContext(IMongoCollection<T> users)
        {
            Users = users;
            EnsureUniqueIndexOnUserName(users);
        }

        private void EnsureUniqueIndexOnUserName(IMongoCollection<T> users)
        {
            var builder = new IndexKeysDefinitionBuilder<T>().Ascending("UserName");
            //var builder = new IndexOptionDefaults().
            //var userName = new IndexKeysBuilder().Ascending("UserName");
            //var unique = new IndexOptionsBuilder().SetUnique(true);
            //users.EnsureIndex(userName, unique);
            //users.EnSureIndex
        }
    }
}