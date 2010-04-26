using System;
using System.Collections.Generic;

using NUnit.Framework;
using MongoDB.Driver.Bson;

namespace MongoDB.Driver
{
    [TestFixture]
    public class TestCollectionMetaData : MongoTestBase
    {
        Database adminDb;

        String adminuser = "adminuser";
        String adminpass = "admin1234";
        
        public override string TestCollections {
            get {
                return "indextests,rename,renamed";
            }
        }
        
        public override void OnInit (){
            IMongoCollection its = DB["indextests"];
            its.Insert(createDoc("S","A","Anderson","OH"));
            its.Insert(createDoc("T","B","Delhi","OH"));
            its.Insert(createDoc("F","B","Cincinnati","OH"));
            its.Insert(createDoc("U","D","Newtown","OH"));
            its.Insert(createDoc("J","E","Newport","KY"));

            adminDb = DB.GetSisterDatabase("admin");
            //adminDb.MetaData.AddUser(adminuser, adminpass);
        }

        public override void OnDispose (){
            //adminDb.MetaData.RemoveUser(adminuser);
        }

        [Test]
        public void TestGetOptions(){
            CollectionMetaData cmd = DB["reads"].MetaData;
            Document options = cmd.Options;
            Assert.IsNotNull(options);
        }

        [Test]
        public void Should_ForEach_Indexes_Without_Needing_Local_Variable()
        {
            var cmd = DB["indextests"].MetaData;

            foreach (var key in cmd.Indexes.Keys)
            {
                System.Console.WriteLine(String.Format("Key: {0} Value: {1}", key, cmd.Indexes[key]));
            }
        }

        [Test]
        public void TestGetIndexes(){
            CollectionMetaData cmd = DB["indextests"].MetaData;
            Dictionary<string, Document> indexes = cmd.Indexes;

            Assert.IsNotNull(indexes);
            Assert.IsTrue(indexes.Count > 0, "Should have been at least one index found.");
            foreach(string key in indexes.Keys){
                System.Console.WriteLine(String.Format("Key: {0} Value: {1}", key, indexes[key]));
            }
        }

        [Test]
        public void TestCreateIndex(){
            CollectionMetaData cmd = DB["indextests"].MetaData;
            cmd.CreateIndex("lastnames", new Document().Append("lname", IndexOrder.Ascending), false);
            Dictionary<string, Document> indexes = cmd.Indexes;
            Assert.IsNotNull(indexes["lastnames"]);
        }

        [Test]
        public void TestCreateIndexNoNames(){
            CollectionMetaData cmd = DB["indextests"].MetaData;
            cmd.CreateIndex(new Document().Append("lname", IndexOrder.Ascending).Append("fname",IndexOrder.Ascending), true);
            Dictionary<string, Document> indexes = cmd.Indexes;
            Assert.IsNotNull(indexes["_lname_fname_unique_"]);
        }

        [Test]
        public void TestDropIndex(){
            CollectionMetaData cmd = DB["indextests"].MetaData;
            cmd.CreateIndex("firstnames", new Document().Append("fname", IndexOrder.Ascending), false);
            Dictionary<string, Document> indexes = cmd.Indexes;
            Assert.IsNotNull(indexes["firstnames"]);
            cmd.DropIndex("firstnames");
            Assert.IsFalse(cmd.Indexes.ContainsKey("firstnames"));
        }

        [Test]
        public void TestRename(){
            DB["rename"].Insert(new Document(){{"test", "rename"}});
            Assert.AreEqual(1, DB["rename"].Count());
            CollectionMetaData cmd = DB["rename"].MetaData;
            cmd.Rename("renamed");
            Assert.IsFalse(DB.GetCollectionNames().Contains(DB.Name + ".rename"), "Shouldn't have found collection");
            Assert.IsTrue(DB.GetCollectionNames().Contains(DB.Name + ".renamed"),"Should have found collection");
            Assert.AreEqual(1, DB["renamed"].Count());
        }

        protected Document createDoc(string fname, string lname, string city, string state){
            Document doc = new Document();
            doc["fname"] = fname;
            doc["lname"] = lname;
            doc["city"] = city;
            doc["state"] = state;
            return doc;
        }

    }
}
