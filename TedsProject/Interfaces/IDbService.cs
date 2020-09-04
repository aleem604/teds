using Amazon.DynamoDBv2.DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TedsProject.Interfaces
{
  public  interface IDbService
    {
        Task Store<T>(T item) where T : new();
        Task UpdateItem<T>(T item) where T : class;
        Task DeleteItem<T>(T item);
        Task BatchStore<T>(IEnumerable<T> items) where T : class;
        Task<IEnumerable<T>> GetAll<T>(IEnumerable<ScanCondition> conditions = null) where T : class;
        Task<T> GetItem<T>(string key) where T : class;
        Task<IEnumerable<T>> GetItemsById<T>(string key) where T : class;
        Task<T> GetItemBySearchKey<T>(string key) where T : class;
    }
}
