namespace DbaLogger
{
    using MySql.Data.Entity;
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity;
    using System.Linq;

    /*
     * Please note that version 8 of MySql.Data does not work
     * MySql.Data and MySQL.Data.Entity version 6.9.12 work
     * https://bugs.mysql.com/bug.php?id=91030
     */

    [DbConfigurationType(typeof(MySqlEFConfiguration))]
    public class DbaLogModel : DbContext
    {
        public DbaLogModel()
            : base("name=DbaLogModel")
        {
        }

        public virtual DbSet<DbaLog> DbaLogs { get; set; }
    }

    public class DbaLog
    {
        [Key]
        public DateTime Id { get; set; }
        public int Dba { get; set; }
    }
}