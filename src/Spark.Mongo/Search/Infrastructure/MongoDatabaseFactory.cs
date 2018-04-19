using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Authentication;
using System.Text;
using System.Threading.Tasks;

namespace Spark.Store.Mongo
{
    public static class MongoDatabaseFactory
    {
        private static Dictionary<string, MongoDatabase> instances;

        public static MongoDatabase GetMongoDatabase(string url)
        {
            MongoDatabase result;
        
            if (instances == null) //instances dictionary is not at all initialized
            {
                instances = new Dictionary<string,MongoDatabase>();
            }
            if (instances.Where(i => i.Key == url).Count() == 0) //there is no instance for this url yet
            {
                result = CreateMongoDatabase(url);
                instances.Add(url, result);
            }; 
            return instances.First(i => i.Key == url).Value; //now there must be one.
        }

        private static MongoDatabase CreateMongoDatabase(string url)
        {
            var mongourl = new MongoUrl(url);

            //Switch to Tls12 only to be compatible with CosmosDB
            var settings = MongoClientSettings.FromUrl(mongourl);
            settings.SslSettings = new SslSettings();
            settings.SslSettings.EnabledSslProtocols = SslProtocols.Tls12;
            var client = new MongoClient(settings);

            return client.GetServer().GetDatabase(mongourl.DatabaseName);
        }
    }
}
