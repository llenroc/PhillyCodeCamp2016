﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using Akka.Actor;
using Consumer.Data;
using Consumer.Domain.Actor;
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
            var mongo = new MongoConnection();
            var rethink = new RethinkConnection();
            var eventStore = new EventConnection();

            var system = ActorSystem.Create("baseballStats");
            var topLevel = system.ActorOf(GameEventCoordinator.Create(), "gameCoordinator");

            var handler = new PlayHandler(topLevel);


            ReqlExpr dbTable = RethinkDB.R.Db("baseball").Table("plays");

            var feed = dbTable.Changes()
                        .RunChanges<Play>(RethinkConnection.Connection/*, new { include_initial = true }*/);

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
