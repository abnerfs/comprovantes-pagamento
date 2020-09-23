using ComprovantesPagamento.Contracts;
using ComprovantesPagamento.Domain.Models;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ComprovantesPagamento.Repositories
{
    public abstract class BaseRepository<T>
    {
        private IDbService _dbService;
        private string _collectionName;


        private IMongoDatabase _db;


        protected FilterDefinitionBuilder<T> Filter => Builders<T>.Filter;

        protected FilterDefinition<T> FilterId(string id)
        {
            return Filter.Eq("_id", ObjectId.Parse(id));
        }


        protected UpdateDefinitionBuilder<T> UpdateDefinition => Builders<T>.Update;


        public T Get(string id)
        {
            try
            {
                return Collection.Find(FilterId(id))
                    .FirstOrDefault();
            }
            catch (Exception)
            {

                throw;
            }
        }



        public void Update(string id, T obj)
        {
            try
            {
                Collection.ReplaceOne(FilterId(id), obj);
            }
            catch (Exception)
            {

                throw;
            }
        }

        public void Insert(T document)
        {
            try
            {
                Collection.InsertOne(document);
            }
            catch (Exception)
            {

                throw;
            }
        }

        public void Delete(string id)
        {
            try
            {
                Collection.DeleteOne(FilterId(id));
            }
            catch (Exception)
            {

                throw;
            }
        }

        public IEnumerable<T> List()
        {
            try
            {
                return Collection.Find(_ => true)
                    .ToList();
            }
            catch (Exception)
            {

                throw;
            }
        }


        protected IMongoCollection<T> Collection
        {
            get
            {
                if (_db == null)
                    _db = _dbService.OpenConnection();

                return _db.GetCollection<T>(_collectionName);
            }
        }

        protected void CloseDatabase()
        {

        }

        public BaseRepository(IDbService dbService, string collectionName)
        {
            _dbService = dbService;
            _collectionName = collectionName;
        }
    }
}
