﻿using Dyalect.Compiler;
using System.Collections.Generic;

namespace Dyalect.Runtime.Types
{
    public abstract class DyTypeInfo : DyObject
    {
        private readonly Dictionary<string, DyFunction> traits = new Dictionary<string, DyFunction>();

        public override object ToObject() => this;

        public override string ToString() => TypeName.PutInBrackets();

        public abstract string TypeName { get; }

        public int TypeCode { get; internal set; }

        internal DyTypeInfo(int typeCode) : base(StandardType.Type)
        {
            TypeCode = typeCode;
        }

        #region Binary Operations
        //x + y
        private DyFunction add;
        protected virtual DyObject AddOp(DyObject left, DyObject right, ExecutionContext ctx)
        {
            if (right.TypeId == StandardType.String)
                return new DyString(left.ToString(ctx).Value + right.GetString());

            return Err.OperationNotSupported(Traits.AddName, left.TypeName(ctx), right.TypeName(ctx)).Set(ctx);
        }
        internal DyObject Add(DyObject left, DyObject right, ExecutionContext ctx)
        {
            if (add != null)
                return add.Call2(left, right, ctx);
            return AddOp(left, right, ctx);
        }

        //x - y
        private DyFunction sub;
        protected virtual DyObject SubOp(DyObject left, DyObject right, ExecutionContext ctx) => 
            Err.OperationNotSupported(Traits.SubName, left.TypeName(ctx), right.TypeName(ctx)).Set(ctx);
        internal DyObject Sub(DyObject left, DyObject right, ExecutionContext ctx)
        {
            if (sub != null)
                return sub.Call2(left, right, ctx);
            return SubOp(left, right, ctx);
        }

        //x * y
        private DyFunction mul;
        protected virtual DyObject MulOp(DyObject left, DyObject right, ExecutionContext ctx) =>
            Err.OperationNotSupported(Traits.MulName, left.TypeName(ctx), right.TypeName(ctx)).Set(ctx);
        internal DyObject Mul(DyObject left, DyObject right, ExecutionContext ctx)
        {
            if (mul != null)
                return mul.Call2(left, right, ctx);
            return MulOp(left, right, ctx);
        }

        //x / y
        private DyFunction div;
        protected virtual DyObject DivOp(DyObject left, DyObject right, ExecutionContext ctx) => 
            Err.OperationNotSupported(Traits.DivName, left.TypeName(ctx), right.TypeName(ctx)).Set(ctx);
        internal DyObject Div(DyObject left, DyObject right, ExecutionContext ctx)
        {
            if (div != null)
                return div.Call2(left, right, ctx);
            return DivOp(left, right, ctx);
        }

        //x % y
        private DyFunction rem;
        protected virtual DyObject RemOp(DyObject left, DyObject right, ExecutionContext ctx) => 
            Err.OperationNotSupported(Traits.RemName, left.TypeName(ctx), right.TypeName(ctx)).Set(ctx);
        internal DyObject Rem(DyObject left, DyObject right, ExecutionContext ctx)
        {
            if (rem != null)
                return rem.Call2(left, right, ctx);
            return RemOp(left, right, ctx);
        }

        //x << y
        private DyFunction shl;
        protected virtual DyObject ShiftLeftOp(DyObject left, DyObject right, ExecutionContext ctx) =>
            Err.OperationNotSupported(Traits.ShlName, left.TypeName(ctx), right.TypeName(ctx)).Set(ctx);
        internal DyObject ShiftLeft(DyObject left, DyObject right, ExecutionContext ctx)
        {
            if (shl != null)
                return shl.Call2(left, right, ctx);
            return ShiftLeftOp(left, right, ctx);
        }

        //x >> y
        private DyFunction shr;
        protected virtual DyObject ShiftRightOp(DyObject left, DyObject right, ExecutionContext ctx) =>
            Err.OperationNotSupported(Traits.ShrName, left.TypeName(ctx), right.TypeName(ctx)).Set(ctx);
        internal DyObject ShiftRight(DyObject left, DyObject right, ExecutionContext ctx)
        {
            if (shr != null)
                return shr.Call2(left, right, ctx);
            return ShiftRightOp(left, right, ctx);
        }

        //x & y
        private DyFunction and;
        protected virtual DyObject AndOp(DyObject left, DyObject right, ExecutionContext ctx) =>
            Err.OperationNotSupported(Traits.AndName, left.TypeName(ctx), right.TypeName(ctx)).Set(ctx);
        internal DyObject And(DyObject left, DyObject right, ExecutionContext ctx)
        {
            if (and != null)
                return and.Call2(left, right, ctx);
            return AndOp(left, right, ctx);
        }

        //x | y
        private DyFunction or;
        protected virtual DyObject OrOp(DyObject left, DyObject right, ExecutionContext ctx) =>
            Err.OperationNotSupported(Traits.OrName, left.TypeName(ctx), right.TypeName(ctx)).Set(ctx);
        internal DyObject Or(DyObject left, DyObject right, ExecutionContext ctx)
        {
            if (or != null)
                return or.Call2(left, right, ctx);
            return OrOp(left, right, ctx);
        }

        //x ^ y
        private DyFunction xor;
        protected virtual DyObject XorOp(DyObject left, DyObject right, ExecutionContext ctx) =>
            Err.OperationNotSupported(Traits.XorName, left.TypeName(ctx), right.TypeName(ctx)).Set(ctx);
        internal DyObject Xor(DyObject left, DyObject right, ExecutionContext ctx)
        {
            if (xor != null)
                return xor.Call2(left, right, ctx);
            return XorOp(left, right, ctx);
        }

        //x == y
        private DyFunction eq;
        protected virtual DyObject EqOp(DyObject left, DyObject right, ExecutionContext ctx) =>
            ReferenceEquals(left, right) ? DyBool.True : DyBool.False;
        internal DyObject Eq(DyObject left, DyObject right, ExecutionContext ctx)
        {
            if (eq != null)
                return eq.Call2(left, right, ctx);
            return EqOp(left, right, ctx);
        }

        //x != y
        private DyFunction neq;
        protected virtual DyObject NeqOp(DyObject left, DyObject right, ExecutionContext ctx) =>
            Eq(left, right, ctx) == DyBool.True ? DyBool.False : DyBool.True;
        internal DyObject Neq(DyObject left, DyObject right, ExecutionContext ctx)
        {
            if (neq != null)
                return eq.Call2(left, right, ctx);
            return NeqOp(left, right, ctx);
        }

        //x > y
        private DyFunction gt;
        protected virtual DyObject GtOp(DyObject left, DyObject right, ExecutionContext ctx) =>
            Err.OperationNotSupported(Traits.GtName, left.TypeName(ctx), right.TypeName(ctx)).Set(ctx);
        internal DyObject Gt(DyObject left, DyObject right, ExecutionContext ctx)
        {
            if (gt != null)
                return gt.Call2(left, right, ctx);
            return GtOp(left, right, ctx);
        }

        //x < y
        private DyFunction lt;
        protected virtual DyObject LtOp(DyObject left, DyObject right, ExecutionContext ctx) =>
            Err.OperationNotSupported(Traits.LtName, left.TypeName(ctx), right.TypeName(ctx)).Set(ctx);
        internal DyObject Lt(DyObject left, DyObject right, ExecutionContext ctx)
        {
            if (lt != null)
                return lt.Call2(left, right, ctx);
            return LtOp(left, right, ctx);
        }

        //x >= y
        private DyFunction gte;
        protected virtual DyObject GteOp(DyObject left, DyObject right, ExecutionContext ctx)
        {
            var ret = Gt(left, right, ctx) == DyBool.True || Eq(left, right, ctx) == DyBool.True;
            return ret ? DyBool.True : DyBool.False;
        }
        internal DyObject Gte(DyObject left, DyObject right, ExecutionContext ctx)
        {
            if (gte != null)
                return gte.Call2(left, right, ctx);
            return GteOp(left, right, ctx);
        }

        //x <= y
        private DyFunction lte;
        protected virtual DyObject LteOp(DyObject left, DyObject right, ExecutionContext ctx)
        {
            var ret = Lt(left, right, ctx) == DyBool.True || Eq(left, right, ctx) == DyBool.True;
            return ret ? DyBool.True : DyBool.False;
        }
        internal DyObject Lte(DyObject left, DyObject right, ExecutionContext ctx)
        {
            if (lte != null)
                return lte.Call2(left, right, ctx);
            return LteOp(left, right, ctx);
        }
        #endregion

        #region Unary Operations
        //-x
        private DyFunction neg;
        protected virtual DyObject NegOp(DyObject arg, ExecutionContext ctx) =>
            Err.OperationNotSupported(Traits.NegName, arg.TypeName(ctx)).Set(ctx);
        internal DyObject Neg(DyObject arg, ExecutionContext ctx)
        {
            if (neg != null)
                return neg.Call1(arg, ctx);
            return NegOp(arg, ctx);
        }

        //!x
        private DyFunction not;
        protected virtual DyObject NotOp(DyObject arg, ExecutionContext ctx) =>
            arg.GetBool() ? DyBool.False : DyBool.True;
        internal DyObject Not(DyObject arg, ExecutionContext ctx)
        {
            if (not != null)
                return not.Call1(arg, ctx);
            return NotOp(arg, ctx);
        }

        //~x
        private DyFunction bitnot;
        protected virtual DyObject BitwiseNotOp(DyObject arg, ExecutionContext ctx) =>
            Err.OperationNotSupported(Traits.BitName, arg.TypeName(ctx)).Set(ctx);
        internal DyObject BitwiseNot(DyObject arg, ExecutionContext ctx)
        {
            if (bitnot != null)
                return bitnot.Call1(arg, ctx);
            return BitwiseNotOp(arg, ctx);
        }

        //#x
        private DyFunction len;
        protected virtual DyObject LengthOp(DyObject arg, ExecutionContext ctx) =>
            Err.OperationNotSupported(Traits.LenName, arg.TypeName(ctx)).Set(ctx);
        internal DyObject Length(DyObject arg, ExecutionContext ctx)
        {
            if (len != null)
                return len.Call1(arg, ctx);
            return LengthOp(arg, ctx);
        }

        //x.toString
        private DyFunction tos;
        protected virtual DyString ToStringOp(DyObject arg, ExecutionContext ctx) => new DyString(arg.ToString());
        internal DyString ToString(DyObject arg, ExecutionContext ctx)
        {
            if (tos != null)
            {
                var retval = tos.Call1(arg, ctx);
                return retval.TypeId == StandardType.String ? (DyString)retval : DyString.Empty;
            }

            return ToStringOp(arg, ctx);
        }
        #endregion

        #region Other Operations
        internal DyObject GetTraitOp(DyObject self, string name, ExecutionContext ctx)
        {
            if (!traits.TryGetValue(name, out var value))
            {
                value = InternalGetTrait(name, ctx);

                if (value != null)
                    traits.Add(name, value);
            }

            if (value != null)
                return value.Clone(self);

            return Err.OperationNotSupported(name, TypeName).Set(ctx);
        }

        internal void SetTraitOp(string name, DyObject value, ExecutionContext ctx)
        {
            var func = (DyFunction)value;

            switch (name)
            {
                case Traits.AddName: add = func; break;
                case Traits.SubName: sub = func; break;
                case Traits.MulName: mul = func; break;
                case Traits.DivName: div = func; break;
                case Traits.RemName: rem = func; break;
                case Traits.ShlName: shl = func; break;
                case Traits.ShrName: shr = func; break;
                case Traits.AndName: and = func; break;
                case Traits.OrName: or = func; break;
                case Traits.XorName: xor = func; break;
                case Traits.EqName: eq = func; break;
                case Traits.NeqName: neq = func; break;
                case Traits.GtName: gt = func; break;
                case Traits.LtName: lt = func; break;
                case Traits.GteName: gte = func; break;
                case Traits.LteName: lte = func; break;
                case Traits.NegName: neg = func; break;
                case Traits.NotName: not = func; break;
                case Traits.BitName: bitnot = func; break;
                case Traits.LenName: len = func; break;
                case Traits.TosName: tos = func; break;
            }

            traits.Remove(name);
            traits.Add(name, func);
        }

        private DyFunction InternalGetTrait(string name, ExecutionContext ctx)
        {
            if (name == "toString")
                return DyMemberFunction.Create(ToStringOp, name);

            if (name == "iterator")
                return DyMemberFunction.Create(GetIterator, name);

            return GetTrait(name, ctx);
        }

        private DyObject GetIterator(DyObject arg, ExecutionContext ctx)
        {
            if (arg is IEnumerable<DyObject> en)
                return new DyIterator(en.GetEnumerator());
            else
                return Err.OperationNotSupported("iterator", TypeName).Set(ctx);
        }

        protected virtual DyFunction GetTrait(string name, ExecutionContext ctx) => null;

        internal protected virtual DyObject Get(DyObject obj, DyObject index, ExecutionContext ctx) => obj.GetItem(index, ctx);

        internal protected virtual void Set(DyObject obj, DyObject index, DyObject val, ExecutionContext ctx) => obj.SetItem(index, val, ctx);
        #endregion
    }

    internal sealed class DyTypeTypeInfo : DyTypeInfo
    {
        public static readonly DyTypeTypeInfo Instance = new DyTypeTypeInfo();

        private DyTypeTypeInfo() : base(StandardType.Type)
        {

        }

        public override string TypeName => StandardType.TypeName;

        protected override DyString ToStringOp(DyObject arg, ExecutionContext ctx) =>
            new DyString(("typeInfo " + TypeName).PutInBrackets());
    }
}