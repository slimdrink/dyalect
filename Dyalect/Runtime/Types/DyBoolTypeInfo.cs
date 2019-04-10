﻿using System;

namespace Dyalect.Runtime.Types
{
    internal sealed class DyBoolTypeInfo : DyTypeInfo
    {
        public static readonly DyBoolTypeInfo Instance = new DyBoolTypeInfo();

        private DyBoolTypeInfo() : base(StandardType.Bool)
        {

        }

        public override string TypeName => StandardType.BoolName;

        public override DyObject Create(ExecutionContext ctx, params DyObject[] args) =>
            args.TakeOne(DyBool.False).AsBool() ? DyBool.True : DyBool.False;

        public override bool CanConvertFrom(Type type) => type == CliType.Boolean;

        public override DyObject ConvertFrom(object obj, Type type, ExecutionContext ctx) => (bool)obj ? DyBool.True : DyBool.False;

        public override bool CanConvertTo(Type type) => type == CliType.Boolean;

        public override object ConvertTo(DyObject obj, Type type, ExecutionContext ctx) => obj.AsBool();
    }
}