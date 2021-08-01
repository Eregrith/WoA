using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using WoA.Lib.Auctions;
using WoA.Lib.Blizzard;
using WoA.Lib.Business;
using WoA.Lib.TSM;

namespace WoA.Lib.Persistence.Files
{
    public class FilesRepository// : IGenericRepository
    {
        private string _databasePath;

        public FilesRepository(IStylizedConsole console, IConfiguration config)
        {
            string path = Path.GetDirectoryName(config.DatabasePath);
            _databasePath = path;
            if (!String.IsNullOrEmpty(path) && !Directory.Exists(path))
                Directory.CreateDirectory(path);
            try
            {
                CreateFileForClass<TsmItem>();
                CreateFileForClass<TsmRealmData>();
                CreateFileForClass<ItemBundle>();
                CreateFileForClass<Auction>();
                CreateFileForClass<WowItem>();
                CreateFileForClass<BlizzardRealmData>();
                CreateFileForClass<SoldAuction>();
                CreateFileForClass<Farmable>();
                CreateFileForClass<Recipe>();
                CreateFileForClass<Reagent>();
            }
            catch (Exception e)
            {
                throw;
            }
        }

        private void CreateFileForClass<T>() where T : IIdentifiable
        {
            string filePath = GetFilePathForClass<T>();
            if (!File.Exists(filePath))
                File.Create(filePath);
        }

        private string GetFilePathForClass<T>()
        {
            string typeName = typeof(T).Name;
            string filePath = Path.Combine(_databasePath, typeName + ".woadb");
            FileInfo fileInfo = new FileInfo(filePath);
            while (IsFileLocked(fileInfo))
            {
                Thread.Sleep(100);
            }
            return filePath;
        }

        public T Add<T>(T model)
        {
            string filePath = GetFilePathForClass<T>();
            using (StreamWriter sw = new StreamWriter(File.OpenWrite(filePath)))
            {
                string line = JsonConvert.SerializeObject(model, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Serialize });
                sw.WriteLine(line);
            }
            return model;
        }

        public void AddAll<T>(List<T> models)
        {
            models.ForEach(m => Add(m));
        }

        public bool Delete<T>(T model)
            where T : IIdentifiable
        {
            string filePath = GetFilePathForClass<T>();
            string[] lines = File.ReadAllLines(filePath);
            string[] linesToSave = lines.Where(l => JsonConvert.DeserializeObject<T>(l).Id != model.Id).ToArray();
            File.WriteAllLines(filePath, linesToSave);
            return true;
        }

        public void DeleteAll<T>(List<T> models)
             where T : IIdentifiable
        {
            string filePath = GetFilePathForClass<T>();
            string[] lines = File.ReadAllLines(filePath);
            string[] linesToSave = lines.Where(l => !models.Any(m => m.Id == JsonConvert.DeserializeObject<T>(l).Id)).ToArray();
            File.WriteAllLines(filePath, linesToSave);
        }

        public T[] GetAll<T>()
        {
            string filePath = GetFilePathForClass<T>();
            string[] lines = File.ReadAllLines(filePath);
            return lines.Select(l => JsonConvert.DeserializeObject<T>(l)).ToArray();
        }

        public T GetById<T>(string pk) where T : IIdentifiable, new()
        {
            string filePath = GetFilePathForClass<T>();
            string[] lines = File.ReadAllLines(filePath);
            return lines.Select(l => JsonConvert.DeserializeObject<T>(l)).FirstOrDefault(m => m.Id == pk);
        }

        public T GetById<T>(int id) where T : IIdentifiable, new()
        {
            string filePath = GetFilePathForClass<T>();
            string[] lines = File.ReadAllLines(filePath);
            return lines.Select(l => JsonConvert.DeserializeObject<T>(l)).FirstOrDefault(m => int.Parse(m.Id) == id);
        }

        public T Update<T>(T model)
            where T : IIdentifiable
        {
            string filePath = GetFilePathForClass<T>();
            string[] lines = File.ReadAllLines(filePath);
            lines = lines.Select(l =>
            {
                T obj = JsonConvert.DeserializeObject<T>(l);
                if (obj.Id == model.Id)
                    return JsonConvert.SerializeObject(model);
                return l;
            }).ToArray();
            File.WriteAllLines(filePath, lines);
            return model;
        }

        public void UpdateAll<T>(List<T> models)
            where T : IIdentifiable
        {
            string filePath = GetFilePathForClass<T>();
            string[] lines = File.ReadAllLines(filePath);
            lines = lines.Select(l =>
            {
                T obj = JsonConvert.DeserializeObject<T>(l);
                if (models.Any(m => m.Id == obj.Id))
                    return JsonConvert.SerializeObject(models.FirstOrDefault(m => m.Id == obj.Id));
                return l;
            }).ToArray();
            File.WriteAllLines(filePath, lines);
        }

        protected virtual bool IsFileLocked(FileInfo file)
        {
            try
            {
                using (FileStream stream = file.Open(FileMode.Open, FileAccess.Read, FileShare.None))
                {
                    stream.Close();
                }
            }
            catch (IOException)
            {
                //the file is unavailable because it is:
                //still being written to
                //or being processed by another thread
                //or does not exist (has already been processed)
                return true;
            }

            //file is not locked
            return false;
        }
    }
}
