using MongoDB.Driver;

namespace ComprovantesPagamento.Contracts
{
    public interface IDbService
    {
        IMongoDatabase OpenConnection();
    }
}
