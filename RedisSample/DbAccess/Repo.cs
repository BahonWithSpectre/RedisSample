using Dapper;
using Microsoft.Data.SqlClient;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace RedisSample.DbAccess
{
    public class Repo
    {
        public async Task<IEnumerable<Search>> GetSearchAsync()
        {
            IEnumerable<Search> search = null;
            using (IDbConnection conn = new SqlConnection("Data Source=SQL5059.site4now.net;Initial Catalog=db_a43a43_geoid;User Id=db_a43a43_geoid_admin;Password=1q2w3e4r5"))
            {
                conn.Open();
                string sqlP = "SELECT * FROM Searches";
                search = await conn.QueryAsync<Search>(sqlP);
            }
            return search;
        }
    }


    public class Search
    {
        public string Name { get; set; }
        public string Otvet { get; set; }
    }
}
