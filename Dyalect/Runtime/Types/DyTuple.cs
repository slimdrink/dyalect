﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Dyalect.Runtime.Types
{
    public abstract class DyTuple : DyObject
    {
        public static DyTuple Create(DyObject arg1, DyObject arg2) =>
            new DyTuplePair(null, arg1, null, arg2 );
        public static DyTuple Create(string key1, DyObject arg1, string key2, DyObject arg2) =>
            new DyTuplePair(key1, arg1, key2, arg2);
        public static DyTuple Create(string[] keys, DyObject[] args) =>
            new DyTupleVariadic(keys, args);
        public static DyTuple Create(DyObject[] args) =>
            new DyTupleVariadic(new string[args.Length], args);

        protected DyTuple() : base(StandardType.Tuple)
        {

        }

        public override object ToObject() => ToDictionary();

        public abstract IDictionary<string, object> ToDictionary();

        internal protected override DyObject GetItem(DyObject index, ExecutionContext ctx)
        {
            if (index.TypeId == StandardType.Integer)
                return GetItem((int)index.GetInteger()) ?? Err.IndexOutOfRange(this.TypeName(ctx), index).Set(ctx);
            else if (index.TypeId == StandardType.String)
                return GetItem(index.GetString()) ?? Err.IndexOutOfRange(this.TypeName(ctx), index).Set(ctx);
            else
                return Err.IndexInvalidType(this.TypeName(ctx), index.TypeName(ctx)).Set(ctx);
        }

        protected internal abstract int GetOrdinal(string name);

        protected internal abstract DyObject GetItem(int index);

        protected internal abstract string GetKey(int index);

        private DyObject GetItem(string index) => GetItem(GetOrdinal(index));

        protected string DefaultKey() => Guid.NewGuid().ToString();

        public abstract int Count { get; }
    }

    internal sealed class DyTuplePair : DyTuple
    {
        private readonly string key1;
        private readonly string key2;
        private readonly DyObject value1;
        private readonly DyObject value2;

        public override int Count => 2;

        public DyTuplePair(string key1, DyObject value1, string key2, DyObject value2)
        {
            this.key1 = key1;
            this.key2 = key2;
            this.value1 = value1;
            this.value2 = value2;
        }

        public override IDictionary<string, object> ToDictionary()
        {
            var dict = new Dictionary<string, object>();
            dict.Add(key1 ?? DefaultKey(), value1);
            dict.Add(key2 ?? DefaultKey(), value2);
            return dict;
        }

        protected internal override DyObject GetItem(int index)
        {
            if (index == 0)
                return value1;

            if (index == 1)
                return value2;

            return null;
        }

        protected internal override int GetOrdinal(string name)
        {
            if (name == key1)
                return 0;

            if (name == key2)
                return 1;

            return -1;
        }

        protected internal override string GetKey(int index)
        {
            if (index == 0)
                return key1;

            return key2;
        }
    }

    internal sealed class DyTupleVariadic : DyTuple
    {
        private readonly DyObject[] values;
        private readonly string[] keys;

        public override int Count => keys.Length;

        internal DyTupleVariadic(string[] keys, DyObject[] values)
        {
            this.keys = keys;
            this.values = values;
        }

        public override IDictionary<string, object> ToDictionary()
        {
            var dict = new Dictionary<string, object>();

            for (var i = 0; i < keys.Length; i++)
            {
                var k = keys[i] ?? Guid.NewGuid().ToString();

                try
                {
                    dict.Add(k, values[i].ToObject());
                }
                catch
                {
                    dict.Add(Guid.NewGuid().ToString(), values[i].ToObject());
                }
            }

            return dict;
        }

        protected internal override int GetOrdinal(string name) => Array.IndexOf(keys, name);

        protected internal override DyObject GetItem(int index)
        {
            if (index < 0 || index >= values.Length)
                return null;
            return values[index];
        }

        protected internal override string GetKey(int index) => keys[index];
    }

    internal sealed class DyTupleTypeInfo : DyTypeInfo
    {
        public static readonly DyTupleTypeInfo Instance = new DyTupleTypeInfo();

        private DyTupleTypeInfo() : base(StandardType.Tuple)
        {

        }

        public override string TypeName => StandardType.TupleName;

        protected override DyObject LengthOp(DyObject arg, ExecutionContext ctx)
        {
            var len = ((DyTuple)arg).Count;
            return len == 1 ? DyInteger.One
                : len == 2 ? DyInteger.Two
                : len == 3 ? DyInteger.Three
                : new DyInteger(len);
        }

        protected override DyString ToStringOp(DyObject arg, ExecutionContext ctx)
        {
            var tup = (DyTuple)arg;
            var sb = new StringBuilder();
            sb.Append('(');

            for (var i = 0; i < tup.Count; i++)
            {
                if (i > 0)
                    sb.Append(", ");

                var k = tup.GetKey(i);
                var val = tup.GetItem(i).ToString(ctx);

                if (ctx.Error != null)
                    return DyString.Empty;

                if (k != null)
                {
                    sb.Append(k);
                    sb.Append(": ");
                }

                sb.Append(val.GetString());
            }

            sb.Append(')');
            return new DyString(sb.ToString());
        }
    }
}
