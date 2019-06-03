﻿using System;
using SQLite;
using System.Linq;
using WoA.Lib.TSM;
using System.Collections.Generic;
using System.IO;
using WoA.Lib.Auctions;

namespace WoA.Lib.SQLite
{
    public class GenericRepository : IGenericRepository, IDisposable
    {
        protected readonly SQLiteConnection _context;

        public GenericRepository(IStylizedConsole console, IConfiguration config)
        {
            string path = Path.GetDirectoryName(config.DatabasePath);
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
            _context = new SQLiteConnection(config.DatabasePath);
            _context.CreateTable<TsmItem>();
            _context.CreateTable<TsmRealmData>();
            _context.CreateTable<ItemBundle>();
        }

        public T Add<T>(T model)
        {
            int iRes = _context.Insert(model);
            return model;
        }

        public void AddAll<T>(List<T> models)
        {
            _context.InsertAll(models);
        }

        public T Update<T>(T model)
        {
            int iRes = _context.Update(model);
            return model;
        }

        public bool Delete<T>(T model)
        {
            int iRes = _context.Delete(model);
            return iRes.Equals(1);
        }

        public T GetById<T>(string pk) where T : new()
        {
            var map = _context.GetMapping(typeof(T));
            return _context.Query<T>(map.GetByPrimaryKeySql, pk).FirstOrDefault();
        }

        public T[] GetAll<T>() where T : new()
        {
            return new TableQuery<T>(_context).ToArray();
        }

        private bool disposed = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    _context.Dispose();
                }
            }
            this.disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
