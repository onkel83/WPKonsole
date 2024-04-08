using Newtonsoft.Json;
using WPInterfaces;

namespace WPRepo
{
    public sealed class JsonRepository<T> where T : IData, new()
    {
        private static readonly Lazy<JsonRepository<T>> _instance = new(() => new JsonRepository<T>());
        private readonly object _lock = new();
        private string _filePath = "";
        private List<T> _dataCache = [];
        private Timer? _cacheTimer = null;

        public static JsonRepository<T> Instance => _instance.Value;

        public string FilePath
        {
            get => _filePath;
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    throw new ArgumentNullException(nameof(value));
                }
                _filePath = value;
            }
        }

        private JsonRepository()
        {
            _filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data", $"{typeof(T).Name.ToLower()}.json");
            _dataCache = LoadData();
            StartCacheTimer();
        }

        private void StartCacheTimer()
        {
            _cacheTimer = new Timer(CacheCallback, null, TimeSpan.Zero, TimeSpan.FromMinutes(1)); // Adjust cache interval as needed
        }

        private void CacheCallback(object? state)
        {
            lock (_lock)
            {
                SaveDataToFile();
                _dataCache = LoadDataFromFile();
            }
        }

        private List<T> LoadData()
        {
            lock (_lock)
            {
                return LoadDataFromFile();
            }
        }

        private List<T> LoadDataFromFile()
        {
            try
            {
                return File.Exists(_filePath) ? JsonConvert.DeserializeObject<List<T>>(File.ReadAllText(_filePath))?? [] : [];
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading data from JSON file: {ex.Message}");
                return [];
            }
        }

        private void SaveDataToFile()
        {
            try
            {
                string directoryPath = Path.GetDirectoryName(_filePath)?? Path.Combine(AppDomain.CurrentDomain.BaseDirectory, Path.Combine("Data", $"{typeof(T).Name.ToLower()}.json"));
                if (!Directory.Exists(directoryPath))
                {
                    Directory.CreateDirectory(directoryPath);
                }
                File.WriteAllText(_filePath, JsonConvert.SerializeObject(_dataCache, Formatting.Indented));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving data to JSON file: {ex.Message}");
            }
        }

        public List<T> GetData()
        {
            lock (_lock)
            {
                return new List<T>(_dataCache);
            }
        }

        public void AddData(T data)
        {
            lock (_lock)
            {
                _dataCache.Add(data);
            }
        }

        public void Dispose()
        {
            _cacheTimer?.Dispose();
            SaveDataToFile();
        }
    }
}
