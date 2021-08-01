using System;
using SQLite;
using System.Linq;
using WoA.Lib.TSM;
using System.Collections.Generic;
using System.IO;
using WoA.Lib.Auctions;
using WoA.Lib.Blizzard;
using WoA.Lib.Business;

namespace WoA.Lib.Persistence.SQLite
{
    public class GenericRepository : IGenericRepository, IDisposable
    {
        protected readonly SQLiteConnection _context;

        public GenericRepository(IStylizedConsole console, IConfiguration config)
        {
            string path = Path.GetDirectoryName(config.DatabasePath);
            if (!String.IsNullOrEmpty(path) && !Directory.Exists(path))
                Directory.CreateDirectory(path);
            try
            {
                _context = new SQLiteConnection(config.DatabasePath);
                _context.CreateTable<TsmItem>();
                _context.CreateTable<TsmRealmData>();
                _context.CreateTable<ItemBundle>();
                _context.CreateTable<Auction>();
                _context.CreateTable<WowItem>();
                _context.CreateTable<BlizzardRealmData>();
                _context.CreateTable<SoldAuction>();
                _context.CreateTable<Farmable>();
                _context.CreateTable<Recipe>();
                _context.CreateTable<Reagent>();
            }
            catch (Exception e)
            {
                throw;
            }
        }

        public T Add<T>(T model)
        {
            _context.Insert(model);
            return model;
        }

        public void AddAll<T>(List<T> models)
        {
            _context.InsertAll(models);
        }

        public T Update<T>(T model)
            where T : IIdentifiable
        {
            _context.Update(model);
            return model;
        }

        public void UpdateAll<T>(List<T> models)
            where T : IIdentifiable
        {
            _context.UpdateAll(models);
        }

        public bool Delete<T>(T model)
            where T: IIdentifiable
        {
            int iRes = _context.Delete(model);
            return iRes.Equals(1);
        }

        public void DeleteAll<T>(List<T> models)
            where T : IIdentifiable
        {
            models.ForEach(m => _context.Delete(m));
        }

        public T GetById<T>(string pk) where T : IIdentifiable, new()
        {
            var map = _context.GetMapping(typeof(T));
            return _context.Query<T>(map.GetByPrimaryKeySql, pk).FirstOrDefault();
        }

        public T GetById<T>(int id) where T : IIdentifiable, new()
        {
            var map = _context.GetMapping(typeof(T));
            return _context.Query<T>(map.GetByPrimaryKeySql, id).FirstOrDefault();
        }

        public T[] GetAll<T>()
        {
            return new TableQuery<T>(_context).ToArray();
        }

        private bool disposed = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed && disposing)
                _context.Dispose();
            this.disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
