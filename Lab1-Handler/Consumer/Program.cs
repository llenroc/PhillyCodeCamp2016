﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using Consumer.Handler;
using RethinkDb.Driver;
using RethinkDb.Driver.Ast;
using RethinkDb.Driver.Model;
using Shared;

namespace Consumer
{
    class Program
    {
        static void Main(string[] args)
        {
            var handler = new PlayHandler();

            var conn = RethinkDB.R
                        .Connection()
                        .Hostname("localhost")
                        .Port(RethinkDBConstants.DefaultPort)
                        .Timeout(30)
                        .Connect();
            ReqlExpr dbTable = RethinkDB.R.Db("baseball").Table("plays");

            var feed = dbTable.Changes()
                        .RunChanges<Play>(conn, new { include_initial = true });

            var observe = feed.ToObservable();
            observe.SubscribeOn(CurrentThreadScheduler.Instance)
                .Subscribe((Change<Play> p) =>
                {
                    handler.Handle(p.NewValue);
                });



            Console.WriteLine("Hit enter to exit...");
            Console.ReadLine();


        }
    }
}
