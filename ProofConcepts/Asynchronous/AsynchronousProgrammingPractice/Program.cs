// See https://aka.ms/new-console-template for more information
using AsynchronousProgrammingPractice;
using System.Collections;
using System.Diagnostics;

bool async = false;

if (async)
{
    AsynchronousCaller asyncCaller = new();
    asyncCaller.startCall();
}
else
{
    SynchronousCaller syncCaller = new();
    syncCaller.startCall();
}