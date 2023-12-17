namespace ODBCConnectionTest
{
    class Program
    {
        static List<Query> queries = new List<Query>();

        const int MAX_THREADS = 20; //1200;

        const string DSN = "oscarvm14";

        static string[] sqlqueries =
        {
            "SELECT COUNT(*) FROM systables",
            "SELECT COUNT(*) FROM syscolumns",
            "SELECT COUNT(*) FROM sysindices",
            "SELECT COUNT(*) FROM systabauth",
            "SELECT COUNT(*) FROM syscolauth",
            "SELECT COUNT(*) FROM sysviews",
            "SELECT COUNT(*) FROM sysusers",
            "SELECT COUNT(*) FROM sysdepend",
            "SELECT COUNT(*) FROM syssynonyms",
            "SELECT COUNT(*) FROM syssyntable",
            "SELECT COUNT(*) FROM sysconstraints",
            "SELECT COUNT(*) FROM sysreferences",
            "SELECT COUNT(*) FROM syschecks",
            "SELECT COUNT(*) FROM sysdefaults",
            "SELECT COUNT(*) FROM syscoldepend",
            "SELECT COUNT(*) FROM sysprocedures",
            "SELECT COUNT(*) FROM sysprocbody",
            "SELECT COUNT(*) FROM sysprocplan",
            "SELECT COUNT(*) FROM sysprocauth",
            "SELECT COUNT(*) FROM sysblobs",
            "SELECT COUNT(*) FROM sysopclstr",
            "SELECT COUNT(*) FROM systriggers",
            "SELECT COUNT(*) FROM systrigbody",
            "SELECT COUNT(*) FROM sysdistrib",
            "SELECT COUNT(*) FROM sysfragments",
            "SELECT COUNT(*) FROM sysobjstate",
            "SELECT COUNT(*) FROM sysviolations",
            "SELECT COUNT(*) FROM sysfragauth",
            "SELECT COUNT(*) FROM sysroleauth",
            "SELECT COUNT(*) FROM sysxtdtypes",
            "SELECT COUNT(*) FROM sysattrtypes",
            "SELECT COUNT(*) FROM sysxtddesc",
            "SELECT COUNT(*) FROM sysinherits",
            "SELECT COUNT(*) FROM syscolattribs",
            "SELECT COUNT(*) FROM syslogmap",
            "SELECT COUNT(*) FROM syscasts",
            "SELECT COUNT(*) FROM sysxtdtypeauth",
            "SELECT COUNT(*) FROM sysroutinelangs",
            "SELECT COUNT(*) FROM syslangauth",
            "SELECT COUNT(*) FROM sysams",
            "SELECT COUNT(*) FROM systabamdata",
            "SELECT COUNT(*) FROM sysopclasses",
            "SELECT COUNT(*) FROM syserrors",
            "SELECT COUNT(*) FROM systraceclasses",
            "SELECT COUNT(*) FROM systracemsgs",
            "SELECT COUNT(*) FROM sysaggregates",
            "SELECT COUNT(*) FROM syssequences",
            "SELECT COUNT(*) FROM sysdirectives",
            "SELECT COUNT(*) FROM sysxasourcetypes",
            "SELECT COUNT(*) FROM sysxadatasources",
            "SELECT COUNT(*) FROM sysseclabelcomponents",
            "SELECT COUNT(*) FROM sysseclabelcomponentelements",
            "SELECT COUNT(*) FROM syssecpolicies",
            "SELECT COUNT(*) FROM syssecpolicycomponents",
            "SELECT COUNT(*) FROM syssecpolicyexemptions",
            "SELECT COUNT(*) FROM sysseclabels",
            "SELECT COUNT(*) FROM sysseclabelnames",
            "SELECT COUNT(*) FROM sysseclabelauth",
            "SELECT COUNT(*) FROM syssurrogateauth",
            "SELECT COUNT(*) FROM sysproccolumns",
            "SELECT COUNT(*) FROM sysexternal",
            "SELECT COUNT(*) FROM sysextdfiles",
            "SELECT COUNT(*) FROM sysextcols",
            "SELECT COUNT(*) FROM sysdomains",
            "SELECT COUNT(*) FROM sysindexes"
        };

        static void Main(string[] args)
        {
            Random randomizer = new Random();

            string processName    = System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName;
            string writerFileName = Path.ChangeExtension(processName, ".txt");
            FileWriter fw = new FileWriter(writerFileName);
            Console.Out.WriteLine("Logging to: {0}", writerFileName);

            string connectionString = string.Format("DSN={0}", DSN);

            Console.Out.WriteLine("Creating {0} query threads", MAX_THREADS);
            for (int i = 0; i < MAX_THREADS; ++i)
            {
                int index = randomizer.Next(sqlqueries.Length);

                string sql = sqlqueries[index];

                queries.Add(new Query(connectionString, sql, fw));
            }

            Console.Out.WriteLine("Starting...");
            foreach (Query query in queries)
            {
                query.Go();
            }

            Console.Out.Write("Press any key to stop...");
            Console.ReadKey();
            Console.Out.WriteLine();

            Console.Out.WriteLine("Stopping...");
            foreach (Query query in queries)
            {
                query.Stop();
            }

            Console.Out.WriteLine("Waiting for all to finish...");
            int runningCount = 0;
            do
            {
                if (runningCount > 0)
                {
                    Console.Out.WriteLine("Waiting for {0} threads...", runningCount);
                    Thread.Sleep(TimeSpan.FromSeconds(1));
                }

                runningCount = 0;
                foreach (Query query in queries)
                {
                    runningCount += query.IsStopped ? 0 : 1;
                }
            } while (runningCount > 0);

            Console.Out.WriteLine("All stopped");
        }
    }
}
