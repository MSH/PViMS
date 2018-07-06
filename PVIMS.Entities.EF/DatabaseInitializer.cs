using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PVIMS.Entities.EF.Migrations;

namespace PVIMS.Entities.EF
{
    public  class DatabaseInitializer : MigrateDatabaseToLatestVersion<PVIMSDbContext, Configuration>
    {
    }
}
