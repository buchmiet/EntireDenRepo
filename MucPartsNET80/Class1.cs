using System.Data.SQLite;
using System.Reflection;
using System.Text;
using Dapper;

namespace MucPartsNET80;

public static class MyDatabase
{


    public class family
    {
        public int family_id;
        public string family_name;
    }

    public class part_type
    {
        public int part_type_id;
        public string name;
    }

    public class part
    {
        public int part_id;
        public string sku;
        public string mpn;
        public string picname;
        public int part_type_id;
    }

    public class watch_part_relation
    {
        public int watch_id;
        public int part_id;
    }

    public static watch getwatch(int watch, SQLiteConnection db)
    {
        //    List<family> families = con.Query<family>(
        //        "SELECT * FROM family").ToList();
        var oj = @"select * from watch where watch_id=" + watch;
        return db.QueryFirst<watch>(oj);
    }

    public static async Task<watch[]> getAllWatches()
    {
        var oj = @"select * from watch";
        var _db = getConnection();
        var ret = await _db.QueryAsync<watch>(oj).ConfigureAwait(false);
        return ret.ToArray();
    }


    public static async Task<part[]> getAllParts()
    {
        var oj = @"select * from part";
        var _db = getConnection();
        var ret = await _db.QueryAsync<part>(oj).ConfigureAwait(false);
        return ret.ToArray();
    }

    public static async Task<family[]> getAllFamilies()
    {
        var oj = @"select * from family";
        var _db = getConnection();
        var ret = await _db.QueryAsync<family>(oj).ConfigureAwait(false);
        return ret.ToArray();
    }

    public static async Task<part[]> getAllPartsForWatch(int id)
    {
        var oj = @"select * from watch_part_relation where watch_id=" + id;
        var _db = getConnection();
        var ret00 = await _db.QueryAsync<watch_part_relation>(oj).ConfigureAwait(false);
        var ret0 = ret00.ToArray();
        StringBuilder str = new StringBuilder("select * from part where part_id in ");
        int i = 0;
        int cal = ret0.Count();
        StringBuilder stro = new StringBuilder("(");
        foreach (watch_part_relation ide in ret0)
        {
            stro.Append(ide.part_id);
            if (i < cal - 1)
                stro.Append(",");
            ++i;
        }
        stro.Append(')');
        str.Append((object)stro);
        var ret = await _db.QueryAsync<part>(str.ToString()).ConfigureAwait(false);
        return ret.ToArray();
    }

    public static async Task<part_type[]> getAllPartTypes()
    {
        var oj = @"select * from part_types";
        var _db = getConnection();
        var ret = await _db.QueryAsync<part_type>(oj).ConfigureAwait(false);
        return ret.ToArray();
    }

    public static async Task<watch_part_relation[]> getAllWatchPartRelations()
    {
        var oj = @"select * from watch_part_relation";
        var _db = getConnection();
        var ret = await _db.QueryAsync<watch_part_relation>(oj).ConfigureAwait(false);
        return ret.ToArray();
    }

    private static SQLiteConnection db;

    public static void setConnection(string path)
    {
        db = new SQLiteConnection(path);
        db.Open();
    }

    public static SQLiteConnection getConnection()
    {
        string cs = @"URI=file:" + Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), @"c:\amazon\zegareczkidb.db");
        if (db != null)
        {
            //  if (db.)
            return db;
        }
        db = new SQLiteConnection(cs);
        db.Open();

        return db;
    }




}