// See https://aka.ms/new-console-template for more information
using DatabaseProofOfConcept;

Database databaseInstance = new Database();
databaseInstance.CreateTable("exampleTable", ["primaryID", "textHolder"], ["INTEGER PRIMARY KEY", "TEXT"]);
databaseInstance.InsertValuesExplicit("exampleTable", ["primaryID", "textHolder"], [$"{databaseInstance.IDCurrent}", "hi im text"]);
string queryResult = databaseInstance.BasicQuery("exampleTable", ["*"]);
Console.WriteLine(queryResult);