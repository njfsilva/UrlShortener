using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using System.Linq.Expressions;
using URLShortener.CrossCutting;

namespace URLShortener.Data
{
    public class GenericDbContext<T> where T : class
    {
        private readonly IMongoClient _mongoClient;
        private readonly IOptions<MongoDbSettings> _settings;
        private readonly IMongoDatabase _database;
        private readonly IMongoCollection<T> _collection;

        public GenericDbContext(IMongoClient mongoClient, IOptions<MongoDbSettings> settings)
        {
            _mongoClient = mongoClient;
            _settings = settings;
            _database = _mongoClient.GetDatabase(_settings.Value.DatabaseName);
            _collection = _database.GetCollection<T>(typeof(T).Name);
        }

        public async Task AddAsync(T entity)
        {
           await _collection.InsertOneAsync(entity);
        }

        public async Task<bool> AnyAsync(Expression<Func<T, bool>> whereCondition)
        {
            return await _collection.CountDocumentsAsync(whereCondition) > 0;
        }

        public async Task<T> GetSingleAsync(Expression<Func<T, bool>> whereCondition)
        {
            return await _collection.Find(whereCondition).FirstOrDefaultAsync();
        }

        public async Task UpdateAsync(Expression<Func<T, bool>> whereCondition, T entity)
        {
            await _collection.ReplaceOneAsync(whereCondition, entity);
        }

        public async Task<IList<T>> GetAllAsync()
        {
            return await _collection.Find(new BsonDocumentFilterDefinition<T>(new BsonDocument())).ToListAsync();
        }
    }
}
