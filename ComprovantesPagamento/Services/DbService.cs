using ComprovantesPagamento.Contracts;
using ComprovantesPagamento.Domain.Models;
using MongoDB.Driver;
using System;

namespace ComprovantesPagamento.Services
{
    public class DbService: IDbService
    {
        private DatabaseConfig _dbConfig;

        public DbService(DatabaseConfig dbConfig)
        {
            _dbConfig = dbConfig;
        }

        public IMongoDatabase OpenConnection()
        {
            try
            {
                var client = new MongoClient(_dbConfig.Url);
                var db = client.GetDatabase(_dbConfig.Database);
                return db;
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
