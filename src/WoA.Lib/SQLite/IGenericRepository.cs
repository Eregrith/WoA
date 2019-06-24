using System;
using System.Collections.Generic;
using System.Text;
using WoA.Lib.TSM;

namespace WoA.Lib.SQLite
{
    public interface IGenericRepository : IDisposable
    {
        T Add<T>(T model);
        void AddAll<T>(List<T> models);
        T Update<T>(T model);
        void UpdateAll<T>(List<T> models);
        bool Delete<T>(T model);
        void DeleteAll<T>(List<T> models);
        T GetById<T>(string pk) where T : new();
        T[] GetAll<T>() where T : new();
    }
}
