using GLib.Common;
using MongoDB.Bson;
using MongoDB.Driver;

namespace KickSter.Api.Services
{
    public class ServiceHelpers
    {
        public static FilterDefinition<T> BuildKeyFilter<T>(FilterDefinitionBuilder<T> builder, ObjectId objectId)
        {
            return builder.Eq("_id", objectId);
        }

        public static FilterDefinition<T> BuildKeyFilter<T>(FilterDefinitionBuilder<T> builder, string objectId)
        {
            return builder.Eq("_id", ObjectId.Parse(objectId));
        }

        public static FilterDefinition<T> BuildStringKeyFilter<T>(FilterDefinitionBuilder<T> builder, string objectId)
        {
            return builder.Eq("_id", objectId);
        }

        //public static FilterDefinition<T> BuildKeyFilter<T>(FilterDefinitionBuilder<T> builder, int objectId)
        //{
        //    return builder.Eq("_id", objectId);
        //}

        public static FindOptions<T> BuildFindOptions<T>(Paging paging)
        {
            if (paging == null)
            {
                paging = new Paging
                {
                    Page = 1,
                    Size = AppConfig.DEFAULT_PAGE_SIZE,
                    Sort = "_id",
                    Desc = false
                };
            }

            if (string.IsNullOrEmpty(paging.Sort)) paging.Sort = "_id";
            var sortDef = Builders<T>.Sort
                        .Ascending(paging.Sort);
            if (paging.Desc)
            {
                sortDef = sortDef.Descending(paging.Sort);
            }
            var findOptions = new FindOptions<T>
            {   
                Sort = sortDef
            };

            if (paging.Page > 0)
            {
                findOptions.Limit = paging.Size;
                findOptions.Skip = paging.Page - 1;
            }

            return findOptions;
        }

        public static FindOneAndUpdateOptions<T> BuildFindOneAndUpdateOptions<T>(bool returnNew, bool isUpsert = false)
        {
            return new FindOneAndUpdateOptions<T>
            {
                ReturnDocument = returnNew ? ReturnDocument.After : ReturnDocument.Before,
                IsUpsert = isUpsert
            };
        }
    }
}