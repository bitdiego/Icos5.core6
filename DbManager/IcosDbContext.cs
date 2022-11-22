using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
//using System.Data.Entity.ModelConfiguration.Conventions;
//using System.Data.Entity.ModelConfiguration;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using IcosClassLibrary.Models;
using Icos5.core6.Models.General;
using Icos5.core6.Models.Variables;
using Icos5.core6.Models.Profile;
//using IcosModelLib.DTO.PIAreaModels;
//using IcosClassLibrary.Models;
//using IdentitySample.Models;
//using IcosModelLib.DTO;

namespace Icos5.core6.DbManager
{
    public class IcosDbContext : DbContext
    {
        public IcosDbContext(DbContextOptions<IcosDbContext> options) : base(options)
        {
        }

        public DbSet<BADMList> BADMList { get; set; }
        public DbSet<Site> Sites { get; set; }
        public DbSet<SubmittedFile> SubmittedFiles { get; set; }
        public DbSet<FileType> FileType { get; set; }
        public DbSet<Station> Stations { get; set; }
        public DbSet<GRP_TEAM> GRP_TEAM { get; set; }
        public DbSet<GRP_UTC_OFFSET> GRP_UTC_OFFSET { get; set; }
        public DbSet<GRP_CLIM_AVG> GRP_CLIM_AVG { get; set; }
        public DbSet<GRP_DM> GRP_DM { get; set; }
        public DbSet<GRP_PLOT> GRP_PLOT { get; set; }
        public DbSet<GRP_INST> GRP_INST { get; set; }
        public DbSet<GRP_LOGGER> GRP_LOGGER { get; set; }
        public DbSet<GRP_FILE> GRP_FILE { get; set; }
        public DbSet<GRP_EC> GRP_EC { get; set; }
        public DbSet<GRP_ECSYS> GRP_ECSYS { get; set; }
        public DbSet<GRP_ECWEXCL> GRP_ECWEXCL { get; set; }
        public DbSet<GRP_BM> GRP_BM { get; set; }
        public DbSet<GRP_LOCATION> GRP_LOCATION { get; set; }
        public DbSet<GRP_TOWER> GRP_TOWER { get; set; }
        public DbSet<GRP_LAND_OWNERSHIP> GRP_LAND_OWNERSHIP { get; set; }
        public DbSet<GRP_DHP> GRP_DHP { get; set; }
        public DbSet<GRP_CEPT> GRP_CEPT { get; set; }
        public DbSet<GRP_TREE> GRP_TREE { get; set; }
        public DbSet<GRP_FLSM> GRP_FLSM { get; set; }
        public DbSet<GRP_SOSM> GRP_SOSM { get; set; }
        public DbSet<GRP_SPPS> GRP_SPPS { get; set; }
        public DbSet<GRP_AGB> GRP_AGB { get; set; }
        public DbSet<GRP_LITTERPNT> GRP_LITTERPNT { get; set; }
        public DbSet<GRP_WTDPNT> GRP_WTDPNT { get; set; }
        public DbSet<GRP_D_SNOW> GRP_D_SNOW { get; set; }
        public DbSet<GRP_HEADER> GRP_HEADER { get; set; }
        public DbSet<GRP_BULKH> GRP_BULKH { get; set; }
        public DbSet<GRP_GAI> GRP_GAI { get; set; }
        public DbSet<GRP_STO> GRP_STO { get; set; }
        public DbSet<GRP_INSTMAN> GRP_INSTMAN { get; set; }
        public DbSet<GRP_INSTPAIR> GRP_INSTPAIR { get; set; }
        public DbSet<GRP_FUNDING> GRP_FUNDING { get; set; }
        //public DbSet<IcosSitePI> IcosSitePIS { get; set; }
        public DbSet<WGUsers> WGUsers { get; set; }
        public DbSet<UsersToWG> UsersToWG { get; set; }
        public DbSet<WorkGroups> WorkGroups { get; set; }
        public DbSet<Fieldbook> Fieldbooks { get; set; }
        public DbSet<DataStorageSync> DataStorageSync { get; set; }
        
        public DbSet<GRP_IGBP> GRP_IGBP { get; set; }
        /********************************************/
        //GENERAL AND UTILITY SECTION
        public DbSet<OutOfRange> OutOfRange { get; set; }
        public DbSet<ICOSLabellingDate> ICOSLabellingDate { get; set; }
        public DbSet<AggregationRules2021> AggregationRules2021 { get; set; }
        public DbSet<CalcAggr> CalcAggr { get; set; }
        public DbSet<Var_Renamed> Var_Renamed { get; set; }
        public DbSet<BaseVarRenamed> CalcAndRenamed { get; set; }
        //public DbSet<VarProfileExt> AggrAndRenamed { get; set; }
        public DbSet<AggrAndRenamed> AggrAndRenamed { get; set; }

        ///Views <summary>
        public DbSet<GRP_EC> EcOperationsByDate { get; set; } //MIND XDATE!!!
        public DbSet<GRP_BM> BmOperationsByDate { get; set; } //MIND XDATE!!!
        public DbSet<GRP_STO> StoOperationsByDate { get; set; } //MIND XDATE!!!

    }
}
