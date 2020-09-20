using Amazon;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TedsProject.Interfaces;
using TedsProject.Models;

namespace TedsProject.Services
{
    public class DbService : IDbService
    {
        public readonly DynamoDBContext _context;
        private readonly AmazonDynamoDBClient client;

        public DbService(IAmazonDynamoDB client)
        {
            this.client = new AmazonDynamoDBClient("AKIAR4FECMCZJLBIDAYK", "J/9gWSv4I4cg+snBLRfmVwHI8ndNx03l/WL8d4Zk", RegionEndpoint.CACentral1);

            _context = new DynamoDBContext(this.client, new DynamoDBContextConfig
            {

                ConsistentRead = true,
                SkipVersionCheck = true
            });
            _context.FromQueryAsync<CrossingsModel>(new Amazon.DynamoDBv2.DocumentModel.QueryOperationConfig { });
        }

        /// <summary>
        /// The Store method allows you to save a POCO to DynamoDb
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="item"></param>
        public async Task Store<T>(T item) where T : new()
        {
            await _context.SaveAsync(item);
        }

        /// <summary>
        /// The BatchStore Method allows you to store a list of items of type T to dynamoDb
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="items"></param>
        public async Task BatchStore<T>(IEnumerable<T> items) where T : class
        {
            var itemBatch = _context.CreateBatchWrite<T>();

            foreach (var item in items)
            {
                itemBatch.AddPutItem(item);
            }

            await itemBatch.ExecuteAsync();
        }
        /// <summary>
        /// Uses the scan operator to retrieve all items in a table
        /// <remarks>[CAUTION] This operation can be very expensive if your table is large</remarks>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public async Task<IEnumerable<T>> GetAll<T>(IEnumerable<ScanCondition> conditions = null) where T : class
        {
            conditions ??= new List<ScanCondition>();
            IEnumerable<T> items = await _context.ScanAsync<T>(conditions).GetRemainingAsync();
            return items;
        }

        /// <summary>
        /// Retrieves an item based on a search key
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public async Task<T> GetItem<T>(string key) where T : class
        {
            var r = await _context.QueryAsync<T>(key).GetRemainingAsync();
            if (r is null)
            {
                return null;
            }
            else
            {
                return r.FirstOrDefault();
            }
        }

        /// <summary>
        /// Retrieves an item based on a search key
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public async Task<T> GetItemBySearchKey<T>(string key) where T : class
        {
            return await _context.LoadAsync<T>(key);
        }

        public async Task<IEnumerable<T>> GetItemsById<T>(string key) where T : class
        {
            return await _context.QueryAsync<T>(key).GetRemainingAsync();
        }

        /// <summary>
        /// Method Updates and existing item in the table
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="item"></param>
        public async Task UpdateItem<T>(T item) where T : class
        {
            T savedItem = await _context.LoadAsync(item);

            if (savedItem == null)
            {
                throw new AmazonDynamoDBException("The item does not exist in the Table");
            }

            await _context.SaveAsync(item);
        }

        /// <summary>
        /// Deletes an Item from the table.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="item"></param>
        public async Task DeleteItem<T>(T item)
        {
            var savedItem = _context.LoadAsync(item);

            if (savedItem == null)
            {
                throw new AmazonDynamoDBException("The item does not exist in the Table");
            }

            await _context.DeleteAsync(item);
        }
    }
}