﻿using Dyalect.Parser.Model;
using System;
using System.Collections.Generic;
using System.Text;
using static Dyalect.Compiler.Hints;
using Dyalect.Linker;
using Dyalect.Runtime.Types;
using Dyalect.Parser;
using System.Linq;

namespace Dyalect.Compiler
{
    partial class Builder
    {
        private void Build(DNode node, Hints hints, CompilerContext ctx)
        {
            switch (node.NodeType)
            {
                case NodeType.Assignment:
                    Build((DAssignment)node, hints, ctx);
                    break;
                case NodeType.Binary:
                    Build((DBinaryOperation)node, hints, ctx);
                    break;
                case NodeType.Binding:
                    Build((DBinding)node, hints, ctx);
                    break;
                case NodeType.Block:
                    Build((DBlock)node, hints, ctx);
                    break;
                case NodeType.Boolean:
                    Build((DBooleanLiteral)node, hints, ctx);
                    break;
                case NodeType.Float:
                    Build((DFloatLiteral)node, hints, ctx);
                    break;
                case NodeType.If:
                    Build((DIf)node, hints, ctx);
                    break;
                case NodeType.Integer:
                    Build((DIntegerLiteral)node, hints, ctx);
                    break;
                case NodeType.Name:
                    Build((DName)node, hints, ctx);
                    break;
                case NodeType.String:
                    Build((DStringLiteral)node, hints, ctx);
                    break;
                case NodeType.Nil:
                    Build((DNilLiteral)node, hints, ctx);
                    break;
                case NodeType.Unary:
                    Build((DUnaryOperation)node, hints, ctx);
                    break;
                case NodeType.Application:
                    Build((DApplication)node, hints, ctx);
                    break;
                case NodeType.While:
                    Build((DWhile)node, hints, ctx);
                    break;
                case NodeType.Break:
                    Build((DBreak)node, hints, ctx);
                    break;
                case NodeType.Return:
                    Build((DReturn)node, hints, ctx);
                    break;
                case NodeType.Continue:
                    Build((DContinue)node, hints, ctx);
                    break;
                case NodeType.Function:
                    Build((DFunctionDeclaration)node, hints, ctx);
                    break;
                case NodeType.Import:
                    Build((DImport)node, hints, ctx);
                    break;
            }
        }

        private void Build(DImport node, Hints hints, CompilerContext ctx)
        {
            var r = new Reference(node.ModuleName, node.Dll, node.Location, unit.FileName);
            var res = linker.Link(r);

            if (res.Success)
            {
                var referencedUnit = new UnitInfo(unit.ModuleHandles.Count, res.Value);
                unit.References.Add(res.Value);
                referencedUnits.Add(node.Alias ?? node.ModuleName, referencedUnit);

                foreach (var n in res.Value.ExportList)
                {
                    var imp = new ImportedName(node.ModuleName, unit.ModuleHandles.Count, n);

                    if (imports.ContainsKey(n.Name))
                        imports[n.Name] = imp;
                    else
                        imports.Add(n.Name, imp);
                }

                for (var i = 0; i < res.Value.TypeNames.Count; i++)
                {
                    var name = res.Value.TypeNames[i];
                    var ti = new TypeInfo(res.Value.TypeHandles[i], referencedUnit);

                    if (types.ContainsKey(name))
                        types[name] = ti;
                    else
                        types.Add(name, ti);
                }

                cw.RunMod(unit.ModuleHandles.Count);
                unit.ModuleHandles.Add(-1); //Реальные хэндлы добавляются линкером

                var addr = AddVariable(node.Alias ?? node.ModuleName, node, VarFlags.Module);
                cw.PopVar(addr);
            }
        }

        private void Build(DBreak node, Hints hints, CompilerContext ctx)
        {
            if (ctx.BlockBreakExit.IsEmpty())
                AddError(CompilerError.NoEnclosingLoop, node.Location);

            if (node.Expression != null)
                Build(node.Expression, hints, ctx);
            else
                cw.PushNil();

            AddLinePragma(node);
            cw.Br(ctx.BlockBreakExit);
        }

        private void Build(DReturn node, Hints hints, CompilerContext ctx)
        {
            if (ctx.FunctionExit.IsEmpty())
                AddError(CompilerError.ReturnNotAllowed, node.Location);

            if (node.Expression != null)
                Build(node.Expression, hints, ctx);
            else
                cw.PushNil();

            AddLinePragma(node);
            cw.Br(ctx.FunctionExit);
        }

        private void Build(DContinue node, Hints hints, CompilerContext ctx)
        {
            if (ctx.BlockSkip.IsEmpty())
                AddError(CompilerError.NoEnclosingLoop, node.Location);

            AddLinePragma(node);
            cw.Br(ctx.BlockSkip);
        }

        private void Build(DWhile node, Hints hints, CompilerContext ctx)
        {
            ctx = new CompilerContext(ctx)
            {
                BlockSkip = cw.DefineLabel(),
                BlockExit = cw.DefineLabel(),
                BlockBreakExit = cw.DefineLabel()
            };
            var iter = cw.DefineLabel();

            cw.MarkLabel(iter);
            Build(node.Condition, hints.Append(Push), ctx);
            cw.Brfalse(ctx.BlockExit);

            Build(node.Body, hints.Remove(Push), ctx);

            cw.MarkLabel(ctx.BlockSkip);
            cw.Br(iter);

            cw.MarkLabel(ctx.BlockExit);
            PushIf(hints);
            AddLinePragma(node);
            cw.MarkLabel(ctx.BlockBreakExit);
            cw.Nop();
        }

        private void Build(DApplication node, Hints hints, CompilerContext ctx)
        {
            var argCount = node.Arguments.Count;
            string mxName = null;
            var sys = AddVariable();

            if (node.Target.NodeType == NodeType.Mixin)
            {
                var fld = (DMixin)node.Target;
                Build(fld.Target, hints.Append(Push), ctx);
                cw.Dup();
                cw.Type();
                cw.PopVar(sys);
                argCount++;
                mxName = fld.Field;
            }

            foreach (var a in node.Arguments)
                Build(a, hints.Append(Push), ctx);

            if (node.Target.NodeType == NodeType.Mixin)
            {
                cw.PushVar(new ScopeVar(sys));
                cw.GetMx(mxName);
            }
            else
                Build(node.Target, hints.Append(Push), ctx);

            AddLinePragma(node);

            cw.Call(argCount);
            PopIf(hints);
        }

        private void Build(DIf node, Hints hints, CompilerContext ctx)
        {
            var falseLabel = cw.DefineLabel();
            var skipLabel = cw.DefineLabel();
            var push = hints.Append(Push);

            Build(node.Condition, push, ctx);
            AddLinePragma(node);
            cw.Brfalse(falseLabel);
            Build(node.True, push, ctx);
            PopIf(hints);
            AddLinePragma(node);
            cw.Br(skipLabel);
            cw.MarkLabel(falseLabel);

            if (node.False != null)
            {
                Build(node.False, push, ctx);
                PopIf(hints);
            }
            else
                PushIf(hints);

            cw.MarkLabel(skipLabel);
            cw.Nop();
        }

        private void Build(DStringLiteral node, Hints hints, CompilerContext ctx)
        {
            AddLinePragma(node);
            cw.Push(node.Value);
            PopIf(hints);
        }

        private void Build(DFloatLiteral node, Hints hints, CompilerContext ctx)
        {
            AddLinePragma(node);
            cw.Push(node.Value);
            PopIf(hints);
        }

        private void Build(DName node, Hints hints, CompilerContext ctx)
        {
            var sv = GetVariable(node.Value, node.Location);
            AddLinePragma(node);

            if (hints.Has(Push))
            {
                if ((sv.Data & VarFlags.This) == VarFlags.This)
                    cw.This();
                else
                    cw.PushVar(sv);

                PopIf(hints);
            }
            else
            {
                if ((sv.Data & VarFlags.Const) == VarFlags.Const)
                    AddError(CompilerError.UnableAssignConstant, node.Location, node.Value);

                cw.PopVar(sv.Address);
            }
        }

        private void Build(DIntegerLiteral node, Hints hints, CompilerContext ctx)
        {
            AddLinePragma(node);
            cw.Push(node.Value);
            PopIf(hints);
        }

        private void Build(DBooleanLiteral node, Hints hints, CompilerContext ctx)
        {
            AddLinePragma(node);
            cw.Push(node.Value);
            PopIf(hints);
        }

        private void Build(DNilLiteral node, Hints hints, CompilerContext ctx)
        {
            AddLinePragma(node);
            cw.PushNil();
            PopIf(hints);
        }

        private void Build(DBlock node, Hints hints, CompilerContext ctx)
        {
            //Начинаем лексический скоуп времени компиляции
            StartScope(fun: false, loc: node.Location);

            for (var i = 0; i < node.Nodes.Count; i++)
            {
                var n = node.Nodes[i];
                var push = i == node.Nodes.Count - 1 ? hints.Append(Push) : hints.Remove(Push);
                Build(n, push, ctx);
            }

            EndScope();
            PopIf(hints);
        }

        private void Build(DAssignment node, Hints hints, CompilerContext ctx)
        {
            if (node.AutoAssign != null)
                Build(node.Target, hints.Append(Push), ctx);

            Build(node.Value, hints.Append(Push), ctx);

            if (node.AutoAssign != null)
                EmitBinaryOp(node.AutoAssign.Value);

            if (node.Target.NodeType != NodeType.Name)// && node.Target.NodeType != DysNodeType.Field)
                AddError(CompilerError.UnableAssignExpression, node.Target.Location, node.Target);

            if (hints.Has(Push))
                cw.Dup();

            Build(node.Target, hints.Remove(Push), ctx);
        }

        private void Build(DBinding node, Hints hints, CompilerContext ctx)
        {
            if (node.Init != null)
                Build(node.Init, hints.Append(Push), ctx);
            else
                cw.PushNil();

            var flags = currentScope.IsGlobal ? VarFlags.Exported :  VarFlags.None;
            var a = AddVariable(node.Name, node, node.Constant ? flags | VarFlags.Const : flags);
            cw.PopVar(a);
            PushIf(hints);
        }

        private void Build(DUnaryOperation node, Hints hints, CompilerContext ctx)
        {
            Build(node.Node, hints.Append(Push), ctx);
            AddLinePragma(node);

            if (node.Operator == UnaryOperator.Neg)
                cw.Neg();
            else if (node.Operator == UnaryOperator.Not)
                cw.Not();
            else if (node.Operator == UnaryOperator.BitwiseNot)
                cw.BitNot();
            else if (node.Operator == UnaryOperator.Length)
                cw.Len();

            PopIf(hints);
        }

        private void Build(DBinaryOperation node, Hints hints, CompilerContext ctx)
        {
            var termLab = default(Label);
            var exitLab = default(Label);

            switch (node.Operator)
            {
                case BinaryOperator.And:
                    Build(node.Left, hints.Append(Push), ctx);
                    termLab = cw.DefineLabel();
                    exitLab = cw.DefineLabel();
                    cw.Brfalse(termLab);
                    Build(node.Right, hints.Append(Push), ctx);
                    AddLinePragma(node);
                    cw.Br(exitLab);
                    cw.MarkLabel(termLab);
                    AddLinePragma(node);
                    cw.Push(false);
                    cw.MarkLabel(exitLab);
                    cw.Nop();
                    break;
                case BinaryOperator.Or:
                    Build(node.Left, hints.Append(Push), ctx);
                    termLab = cw.DefineLabel();
                    exitLab = cw.DefineLabel();
                    cw.Brtrue(termLab);
                    Build(node.Right, hints.Append(Push), ctx);
                    AddLinePragma(node);
                    cw.Br(exitLab);
                    cw.MarkLabel(termLab);
                    AddLinePragma(node);
                    cw.Push(true);
                    cw.MarkLabel(exitLab);
                    cw.Nop();
                    break;
                default:
                    Build(node.Left, hints.Append(Push), ctx);
                    Build(node.Right, hints.Append(Push), ctx);
                    AddLinePragma(node);
                    EmitBinaryOp(node.Operator);
                    break;
            }

            PopIf(hints);
        }

        private void EmitBinaryOp(BinaryOperator op)
        {
            switch (op)
            {
                case BinaryOperator.Add: cw.Add(); break;
                case BinaryOperator.Sub: cw.Sub(); break;
                case BinaryOperator.Mul: cw.Mul(); break;
                case BinaryOperator.Div: cw.Div(); break;
                case BinaryOperator.Rem: cw.Rem(); break;
                case BinaryOperator.Eq: cw.Eq(); break;
                case BinaryOperator.NotEq: cw.NotEq(); break;
                case BinaryOperator.Gt: cw.Gt(); break;
                case BinaryOperator.Lt: cw.Lt(); break;
                case BinaryOperator.GtEq: cw.GtEq(); break;
                case BinaryOperator.LtEq: cw.LtEq(); break;
                case BinaryOperator.ShiftLeft: cw.Shl(); break;
                case BinaryOperator.ShiftRight: cw.Shr(); break;
                case BinaryOperator.BitwiseAnd: cw.And(); break;
                case BinaryOperator.BitwiseOr: cw.Or(); break;
                case BinaryOperator.Xor: cw.Xor(); break;
                default:
                    throw Ice();
            }
        }

        private void Build(DFunctionDeclaration node, Hints hints, CompilerContext ctx)
        {
            if (node.Name != null)
            {
                var flags = VarFlags.Const | VarFlags.Function;
                var addr = 0;

                if (!node.IsMemberFunction)
                    addr = AddVariable(node.Name, node, flags | VarFlags.Exported);

                BuildFunctionBody(node, hints, ctx);

                if (hints.Has(Push))
                    cw.Dup();

                if (node.IsMemberFunction)
                {
                    cw.Push(node.Name);
                    var code = GetTypeHandle(node.TypeName, node.Location);
                    cw.SetMx(code);
                }

                AddLinePragma(node);

                if (node.IsMemberFunction)
                    cw.Nop();
                else
                    cw.PopVar(addr);
            }
            else
            {
                BuildFunctionBody(node, hints, ctx);
                AddLinePragma(node);
                cw.Nop();
                PopIf(hints);
            }
        }

        private void BuildFunctionBody(DFunctionDeclaration node, Hints hints, CompilerContext ctx)
        {
            //Начинаем новый фрейм
            var args = node.Parameters.ToArray();
            var argCount = args.Length + (node.TypeName != null ? 1 : 0);
            StartFun(node.Name, args, argCount);

            var startLabel = cw.DefineLabel();
            var funEndLabel = cw.DefineLabel();

            //Функции мы всегда компилируем по месту проживания, т.е. как наткнулись на функцию,
            //тут её и компилируем. Поэтому, чтобы общий код фрейма исполнялся без сюрпризов,
            //вставляем тут goto, который просто перепрыгнет через код функции.
            //Зачем так делается? Ну функция у нас обычное значение, как строка или целое число.
            var funSkipLabel = cw.DefineLabel();
            cw.Br(funSkipLabel);

            ctx = new CompilerContext { FunctionExit = funEndLabel };
            hints = Function | Push;

            //Actual start of a function
            cw.MarkLabel(startLabel);

            //Начинаем реальный (а не времени компиляции) лексический скоуп для функции.
            StartScope(fun: true, loc: node.Location);

            //Вот здесь реальный лексический скоуп и начинается
            StartSection();

            AddLinePragma(node);
            var address = cw.Offset;

            AddVariable("this", node, data: VarFlags.This | VarFlags.Const);

            //Это функция, расширяющая тип. У неё есть неявный первый параметр - self.
            if (node.IsMemberFunction)
            {
                var a = AddVariable("self", node, data: VarFlags.Argument | VarFlags.Const);
                cw.PopVar(a);
            }

            //Инициализационная логика параметров
            for (var i = 0; i < args.Length; i++)
            {
                var arg = args[i];
                var a = AddVariable(arg, node, data: VarFlags.Argument);
                cw.PopVar(a);
            }

            //Теперь компилируем тело функции
            Build(node.Body, hints, ctx);

            //Возвращаемся из функции. Кстати, любое исполнение функции доходит до сюда,
            //т.е. нельзя выйти раньше. Преждевременный return всё равно прыгает сюда, и здесь
            //уже исполняется реальный return (Ret). Т.е. это эпилог функции.
            cw.MarkLabel(funEndLabel);
            cw.Ret();
            cw.MarkLabel(funSkipLabel);

            AddLinePragma(node);

            //Завершаем потихоньку, закрываем скоупы.
            var funHandle = unit.Layouts.Count;
            var ss = EndFun(funHandle);
            unit.Layouts.Add(new MemoryLayout(currentCounter, ss, address));
            EndScope();
            EndSection();

            //А здесь мы уже создаём функцию как значение (эмитится Newfun).
            cw.Push(node.Variadic ? argCount - 1 : argCount);

            if (node.Variadic)
                cw.NewFunV(funHandle);
            else
                cw.NewFun(funHandle);
        }

        private int GetTypeHandle(Qualident name, Location loc)
        {
            var stdCode = -1;

            if (name.Parent == null)
                stdCode = StandardType.GetTypeCodeByName(name.Local);

            if (stdCode >= 0)
                return stdCode;

            if (name.Parent == null)
            {
                if (!types.TryGetValue(name.Local, out var ti))
                {
                    AddError(CompilerError.UndefinedType, loc, name.Local);
                    return 0;
                }
                else
                    return ti.Unit.Handle | ti.Handle << 8;
            }
            else
            {
                if (!referencedUnits.TryGetValue(name.Parent, out var ui))
                {
                    AddError(CompilerError.UndefinedModule, loc, name.Parent);
                    return 0;
                }
                else
                {
                    var ti = -1;

                    for (var i = 0; i < ui.Unit.TypeHandles.Count; ti++)
                    {
                        if (ui.Unit.TypeNames[i] == name.Local)
                        {
                            ti = ui.Unit.TypeHandles[i];
                            break;
                        }
                    }

                    if (ti == -1)
                        AddError(CompilerError.UndefinedType, loc, name);

                    return ui.Handle | ti << 8;
                }
            }
        }
    }
}