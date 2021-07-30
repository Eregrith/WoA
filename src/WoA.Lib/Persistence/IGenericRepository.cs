using System;
using System.Collections.Generic;
using System.Text;
using WoA.Lib.TSM;

namespace WoA.Lib.Persistence
{
    public interface IGenericRepository : IDisposable
    {
        T Add<T>(T model);
        void AddAll<T>(List<T> models);
        T Update<T>(T model) where T : IIdentifiable;
        void UpdateAll<T>(List<T> models) where T : IIdentifiable;
        bool Delete<T>(T model) where T : IIdentifiable;
        void DeleteAll<T>(List<T> models) where T : IIdentifiable;
        T GetById<T>(string pk) where T : IIdentifiable, new();
        T GetById<T>(int id) where T : IIdentifiable, new();
        T[] GetAll<T>() where T : IIdentifiable, new();
    }
}
