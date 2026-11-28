
namespace NihongoHub.Init
{
    public class DatabaseContext : IDisposable
    {
        private readonly LiteDB.LiteDatabase _database;

        public DatabaseContext(string databasePath)
        {
            _database = new LiteDB.LiteDatabase(databasePath);
        }

        public LiteDB.ILiteCollection<Models.Entities.User> Users
        {
            get
            {
                return _database.GetCollection<Models.Entities.User>("users");
            }
        }

        public LiteDB.ILiteCollection<Models.Entities.VocabularyNotebook> VocabularyNotebooks
        {
            get
            {
                return _database.GetCollection<Models.Entities.VocabularyNotebook>("tanngocyou");
            }
        }

        public LiteDB.ILiteCollection<Models.Entities.Word> Words
        {
            get
            {
                return _database.GetCollection<Models.Entities.Word>("tanngo");
            }
        }

        public void Dispose()
        {
            _database.Dispose();
        }
    }

    public static class DatabaseInit
    {
        private static DatabaseContext? _databaseContext;

        public static DatabaseContext Current
        {
            get
            {
                if(_databaseContext == null){
                    Logging.LogReporter.Fatal("数据库未能初始化",Logging.LogCategory.Database);
                }
                return _databaseContext;
            }
        }


        public static void Init()
        {
            _databaseContext = new DatabaseContext(Path.Combine(AppContext.BaseDirectory,"Config", ConfigInit.Current.Database.FileName));

            ///建立唯一索引
            /// EnsureIndex(?,true)的true表示唯一索引
            _databaseContext.Users.EnsureIndex( x => x.username,true);
        }
    }
}
