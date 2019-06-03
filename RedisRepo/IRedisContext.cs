﻿using System;
using System.Collections.Generic;
using PubComp.RedisRepo.Payoneer.Labs.Throttling.Common.Redis;
using StackExchange.Redis;

namespace PubComp.RedisRepo
{
    public interface IRedisContext
    {
        bool AtomicExchange(string key, bool value);
        bool? AtomicExchange(string key, bool? value);
        double AtomicExchange(string key, double value);
        double? AtomicExchange(string key, double? value);
        int AtomicExchange(string key, int value);
        int? AtomicExchange(string key, int? value);
        long AtomicExchange(string key, long value);
        long? AtomicExchange(string key, long? value);
        string AtomicExchange(string key, string value);
        double Decrement(string key, double value);
        long Decrement(string key, long value);
        void Delete(params string[] keys);
        void Delete(string key);
        IEnumerable<string> GetKeys(string pattern = null);
        TimeSpan? GetTimeToLive(string key);
        double Increment(string key, double value);
        long Increment(string key, long value);
        void Set(string key, bool value, TimeSpan? expiry = null);
        bool Set(string key, string value, When when, TimeSpan? expiry = null);
        void Set(string key, bool? value, TimeSpan? expiry = null);
        void Set(string key, double value, TimeSpan? expiry = null);
        void Set(string key, double? value, TimeSpan? expiry = null);
        void Set(string key, int value, TimeSpan? expiry = null);
        void Set(string key, int? value, TimeSpan? expiry = null);
        void Set(string key, long value, TimeSpan? expiry = null);
        void Set(string key, long? value, TimeSpan? expiry = null);
        void Set(string key, string value, TimeSpan? expiry = null);
        void SetOrAppend(string key, string value);
        void SetTimeToLive(string key, TimeSpan? expiry);
        bool TryGet(string key, out bool value);
        bool TryGet(string key, out bool? value);
        bool TryGet(string key, out double value);
        bool TryGet(string key, out double? value);
        bool TryGet(string key, out int value);
        bool TryGet(string key, out int? value);
        bool TryGet(string key, out long value);
        bool TryGet(string key, out long? value);
        bool TryGet(string key, out string value);

        #region Redis Lists

        /// <summary>
        /// Adds <paramref name="value"/> to the end of a list that is at <paramref name="key"/>.
        /// If the list doesn't exist then it is created.
        /// </summary>
        /// <returns>The length of the list after the addition</returns>
        long AddToList(string key, string value);

        /// <summary>
        /// Adds <paramref name="values"/> to the end of a list that is at <paramref name="key"/>.
        /// If the list doesn't exist then it is created.
        /// </summary>
        /// <returns>The length of the list after the addition</returns>
        long AddRangeToList(string key, string[] values);

        /// <summary>
        /// Returns the list that is at <paramref name="key"/>.
        /// If <paramref name="start"/> and <paramref name="stop"/> are not given then the whole list will be returned.
        /// Else, a sub-list is returned that starts at the index <paramref name="start"/> and stops at the index <paramref name="stop"/>.
        /// Please note that the list is zero-based indexed (so 0 is is the first element).
        /// If <paramref name="start"/> or <paramref name="stop"/> is negative then it means it's counted from the end of the list (-1 is the last element, -2 is the element before the last element and so on).
        /// If the index is out-of-bounds then instead of throwing an exception the index is initialized to the nearest boundary (start or end of the list), and only then the operation will be done.
        /// </summary>
        string[] GetList(string key, long start = 0, long stop = -1);

        #endregion
        
        #region Redis Sets

        void AddToSet(string key, string[] values);

        long CountSetMembers(string key);

        string[] GetSetMembers(string key);

        /// <summary>
        /// Get the diff between the set at index 0 of <paramref name="keys"/> and all other sets in <paramref name="keys"/>
        /// </summary>
        string[] GetSetsDifference(string[] keys);

        /// <summary>
        /// Union sets at keys <paramref name="setKeys"/>
        /// </summary>
        string[] UnionSets(string[] keys);

        /// <summary>
        /// Intersect sets at keys <paramref name="keys"/>
        /// </summary>
        string[] IntersectSets(string[] keys);

        /// <summary>
        /// Get the diff between the set at index 0 of <paramref name="keys"/> and all other sets in <paramref name="keys"/>
        /// store the result at <param name="destinationKey"></param>
        /// </summary>
        void StoreSetsDifference(string destinationKey, string[] keys);

        /// <summary>
        /// Union sets at keys <paramref name="keys"/>
        /// store the result at <param name="destinationKey"></param>
        /// </summary>
        void UnionSetsAndStore(string destinationKey, string[] keys);

        /// <summary>
        /// Intersect sets at keys <paramref name="keys"/>
        /// store the result at <param name="destinationKey"></param>
        /// </summary>
        void IntersectSetsAndStore(string destinationKey, string[] keys);

        bool SetContains(string key, string member);

        bool TryGetDistributedLock(string lockObjectName, string lockerName, TimeSpan lockTtl);

        #endregion

        #region Redis Sorted Sets

        long AddToSortedSet(string key, (double score, string element)[] values, When when = When.Always);

        long CountSortedSetMembers(string key, double min = double.NegativeInfinity, double max = double.PositiveInfinity, Exclude exclude = Exclude.None);

        long RemoveFromSortedSet(string key, string[] values);

        long RemoveRangeFromSortedSetByScore(string key, double start, double end, Exclude exclude = Exclude.None);

        long RemoveRangeFromSortedSetByRank(string key, long start, long end);

        string[] GetSortedSetMembersByScore(string key, double start = double.NegativeInfinity, double end = double.PositiveInfinity, Order order = Order.Ascending);

        string[] GetSortedSetMembersByRank(string key, long start = 0, long end = -1, Order order = Order.Ascending);

        List<(double score, string element)> GetSortedSetMembersByScoreWithScores(string key, double start = double.NegativeInfinity, double end = double.PositiveInfinity, Order order = Order.Ascending);

        List<(double score, string element)> GetSortedSetMembersByRankWithScores(string key, long start = 0, long end = -1, Order order = Order.Ascending);

        #endregion

        #region Lua Scripting

        RedisScriptKeysAndArguments CreateScriptKeyAndArguments();

        /// <summary>
        /// Run a lua script against the connected redis instance
        /// </summary>
        /// <param name="script">the script to run. Keys should be @Key1, @Key2 ... @Key10. Int arguments: @IntArg1 .. @IntArg20. String arguments: @StringArg1 .. @StringArg20</param>
        /// <param name="keysAndParameters">an instance of RedisScriptKeysAndArguments</param>
        void RunScript(string script, RedisScriptKeysAndArguments keysAndParameters);

        /// <summary>
        /// Run a lua script against the connected redis instance
        /// </summary>
        /// <param name="script">the script to run. Keys should be @Key1, @Key2 ... @Key10. Int arguments: @IntArg1 .. @IntArg20. String arguments: @StringArg1 .. @StringArg20</param>
        /// <param name="keysAndParameters">an instance of RedisScriptKeysAndArguments</param>
        /// <returns>result as string</returns>
        string RunScriptString(string script, RedisScriptKeysAndArguments keysAndParameters);

        /// <summary>
        /// Run a lua script against the connected redis instance
        /// </summary>
        /// <param name="script">the script to run. Keys should be @Key1, @Key2 ... @Key10. Int arguments: @IntArg1 .. @IntArg20. String arguments: @StringArg1 .. @StringArg20</param>
        /// <param name="keysAndParameters">an instance of RedisScriptKeysAndArguments</param>
        /// <returns>result as int</returns>
        int RunScriptInt(string script, RedisScriptKeysAndArguments keysAndParameters);

        /// <summary>
        /// Run a lua script against the connected redis instance
        /// </summary>
        /// <param name="script">the script to run. Keys should be @Key1, @Key2 ... @Key10. Int arguments: @IntArg1 .. @IntArg20. String arguments: @StringArg1 .. @StringArg20</param>
        /// <param name="keysAndParameters">an instance of RedisScriptKeysAndArguments</param>
        /// <returns>result as string</returns>
        long RunScriptLong(string script, RedisScriptKeysAndArguments keysAndParameters);

        /// <summary>
        /// Run a lua script against the connected redis instance
        /// </summary>
        /// <param name="script">the script to run. Keys should be @Key1, @Key2 ... @Key10. Int arguments: @IntArg1 .. @IntArg20. String arguments: @StringArg1 .. @StringArg20</param>
        /// <param name="keysAndParameters">an instance of RedisScriptKeysAndArguments</param>
        /// <returns>result as double</returns>
        double RunScriptDouble(string script, RedisScriptKeysAndArguments keysAndParameters);

        /// <summary>
        /// Run a lua script against the connected redis instance
        /// </summary>
        /// <param name="script">the script to run. Keys should be @Key1, @Key2 ... @Key10. Int arguments: @IntArg1 .. @IntArg20. String arguments: @StringArg1 .. @StringArg20</param>
        /// <param name="keysAndParameters">an instance of RedisScriptKeysAndArguments</param>
        /// <returns>result as byte array</returns>
        byte[] RunScriptByteArray(string script, RedisScriptKeysAndArguments keysAndParameters);

        /// <summary>
        /// Run a lua script against the connected redis instance
        /// </summary>
        /// <param name="script">the script to run. Keys should be @Key1, @Key2 ... @Key10. Int arguments: @IntArg1 .. @IntArg20. String arguments: @StringArg1 .. @StringArg20</param>
        /// <param name="keysAndParameters">an instance of RedisScriptKeysAndArguments</param>
        /// <returns>result as string[]</returns>
        string[] RunScriptStringArray(string script, RedisScriptKeysAndArguments keysAndParameters);

        #endregion
    }
}