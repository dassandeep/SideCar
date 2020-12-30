using SideCarCore.DomainEntity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace SideCarCore.MongoLogRepository
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IMongoRepository<TDocument> where TDocument : IDocument
    {
        void InsertOne(TDocument document);
        Task InsertOneAsync(TDocument document);
    }
}
